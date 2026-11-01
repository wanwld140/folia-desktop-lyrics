// Presents complete Chromium frames on a separate layered surface below the desktop icons.
let paintProcType;
function createWindowsBitmapPresenter({ bounds, loadKoffi = () => require('koffi') }) {
    const k = loadKoffi();
    const u = k.load('user32.dll'), g = k.load('gdi32.dll');
    const fn = (lib, name, result, args) => lib.func('__stdcall', name, result, args);
    const find = fn(u, 'FindWindowW', 'uintptr_t', ['str16', 'str16']);
    const child = fn(u, 'FindWindowExW', 'uintptr_t', ['uintptr_t', 'uintptr_t', 'str16', 'str16']);
    const valid = fn(u, 'IsWindow', 'int', ['uintptr_t']);
    const parent = fn(u, 'GetAncestor', 'uintptr_t', ['uintptr_t', 'uint32_t']);
    const create = fn(u, 'CreateWindowExW', 'uintptr_t', ['uint32_t', 'str16', 'str16', 'uint32_t', 'int', 'int', 'int', 'int', 'uintptr_t', 'uintptr_t', 'uintptr_t', 'uintptr_t']);
    const destroy = fn(u, 'DestroyWindow', 'int', ['uintptr_t']);
    const show = fn(u, 'ShowWindow', 'int', ['uintptr_t', 'int']);
    const timeout = fn(u, 'SendMessageTimeoutW', 'intptr_t', ['uintptr_t', 'uint32_t', 'uintptr_t', 'intptr_t', 'uint32_t', 'uint32_t', 'void *']);
    const alpha = fn(u, 'SetLayeredWindowAttributes', 'int', ['uintptr_t', 'uint32_t', 'uint8_t', 'uint32_t']);
    const setProc = fn(u, 'SetWindowLongPtrW', 'intptr_t', ['uintptr_t', 'int', 'intptr_t']);
    const callProc = fn(u, 'CallWindowProcW', 'intptr_t', ['intptr_t', 'uintptr_t', 'uint32_t', 'uintptr_t', 'intptr_t']);
    const beginPaint = fn(u, 'BeginPaint', 'uintptr_t', ['uintptr_t', 'void *']);
    const endPaint = fn(u, 'EndPaint', 'int', ['uintptr_t', 'void *']);
    const invalidate = fn(u, 'InvalidateRect', 'int', ['uintptr_t', 'void *', 'int']);
    const update = fn(u, 'UpdateWindow', 'int', ['uintptr_t']);
    const position = fn(u, 'SetWindowPos', 'int', ['uintptr_t', 'uintptr_t', 'int', 'int', 'int', 'int', 'uint32_t']);
    const toClient = fn(u, 'ScreenToClient', 'int', ['uintptr_t', 'void *']);
    const dc = fn(u, 'GetDC', 'uintptr_t', ['uintptr_t']);
    const releaseDC = fn(u, 'ReleaseDC', 'int', ['uintptr_t', 'uintptr_t']);
    const dib = fn(g, 'CreateDIBSection', 'uintptr_t', ['uintptr_t', 'void *', 'uint32_t', k.out(k.pointer('void *')), 'uintptr_t', 'uint32_t']);
    const remove = fn(g, 'DeleteObject', 'int', ['uintptr_t']);
    const compatibleDC = fn(g, 'CreateCompatibleDC', 'uintptr_t', ['uintptr_t']);
    const select = fn(g, 'SelectObject', 'uintptr_t', ['uintptr_t', 'uintptr_t']);
    const deleteDC = fn(g, 'DeleteDC', 'int', ['uintptr_t']);
    const blit = fn(g, 'BitBlt', 'int', ['uintptr_t', 'int', 'int', 'int', 'int', 'uintptr_t', 'int', 'int', 'uint32_t']);
    const flush = fn(g, 'GdiFlush', 'int', []);
    const { x, y, width, height } = bounds;
    if (![x, y, width, height].every(Number.isSafeInteger) || width <= 0 || height <= 0
        || width > 16384 || height > 16384 || width * height > 67108864) throw new Error('Invalid wallpaper dimensions.');
    const progman = find('Progman', null);
    const locate = () => {
        const icons = child(progman, 0, 'SHELLDLL_DefView', null);
        if (icons && child(progman, 0, 'WorkerW', null)) return { root: progman, after: icons, mode: 'raised' };
        let window = 0;
        while ((window = child(0, window, null, null))) {
            if (!child(window, 0, 'SHELLDLL_DefView', null)) continue;
            const worker = child(0, window, 'WorkerW', null);
            if (worker) return { root: worker, after: 0, mode: 'classic' };
        }
        return null;
    };
    let desktop = locate();
    if (!desktop && progman) {
        // Only a missing desktop layer needs the shell message; never rebuild an existing one.
        timeout(progman, 0x052c, 0x0d, 1, 2, 1000, null);
        desktop = locate();
    }
    if (!desktop) throw new Error('The Windows desktop wallpaper layer is unavailable.');
    const origin = Buffer.alloc(8); origin.writeInt32LE(x, 0); origin.writeInt32LE(y, 4);
    if (!toClient(desktop.root, origin)) throw new Error('The desktop coordinate mapping failed.');
    const px = origin.readInt32LE(0), py = origin.readInt32LE(4);
    // Progman on 24H2 has NOREDIRECTIONBITMAP. Our own LAYERED child supplies a surface.
    let hwnd = Number(create(0x08080080, 'STATIC', 'Folia animated wallpaper', 0x46000000,
        px, py, width, height, desktop.root, 0, 0, 0));
    let bitmap = 0, bits = null, memoryDC = 0, originalBitmap = 0, shown = false;
    let callback = null, originalProc = 0, paintError = null;
    const paintState = Buffer.alloc(72); // PAINTSTRUCT, aligned for the x64 Windows build.
    const close = () => {
        if (hwnd && valid(hwnd)) {
            const restored = originalProc ? Boolean(setProc(hwnd, -4, originalProc)) : true;
            if (!destroy(hwnd) && valid(hwnd)) {
                show(hwnd, 0);
                // A callback still reachable from a live HWND must keep its DIB/DC alive.
                if (!restored) {
                    console.error('[WallpaperWin] paint callback retained because its hidden window could not be destroyed');
                    return false;
                }
            }
        }
        hwnd = 0;
        if (callback) k.unregister(callback);
        callback = null; originalProc = 0;
        if (memoryDC && originalBitmap) select(memoryDC, originalBitmap);
        if (bitmap) remove(bitmap);
        if (memoryDC) deleteDC(memoryDC);
        memoryDC = 0; originalBitmap = 0;
        bitmap = 0; bits = null;
    };
    if (!hwnd || !alpha(hwnd, 0, 255, 2)) { close(); throw new Error('The layered wallpaper surface could not be created.'); }
    try {
        paintProcType ??= k.proto('intptr_t __stdcall FoliaWallpaperPaintProc(uintptr_t, uint32_t, uintptr_t, intptr_t)');
        callback = k.register((window, message, wparam, lparam) => {
            // Suppress the STATIC background fill; each paint copies one complete retained frame.
            if (message === 0x14) return 1;
            if (message === 0x0f) {
                let targetDC = 0;
                try {
                    targetDC = beginPaint(window, paintState);
                    if (memoryDC && (!targetDC || !blit(targetDC, 0, 0, width, height, memoryDC, 0, 0, 0x00cc0020))) {
                        paintError = new Error('The wallpaper frame could not be painted.');
                    }
                    // Complete GDI reads before the next frame writes directly into DIB memory.
                    if (memoryDC && !flush()) paintError = new Error('The wallpaper frame could not be synchronized.');
                } catch (error) { paintError = error; }
                finally { if (targetDC && !endPaint(window, paintState)) paintError = new Error('The wallpaper paint could not be completed.'); }
                return 0;
            }
            try { return callProc(originalProc, window, message, wparam, lparam); }
            catch (error) { paintError = error; return 0; }
        }, k.pointer(paintProcType));
        originalProc = setProc(hwnd, -4, k.address(callback));
        if (!originalProc) throw new Error('The wallpaper paint handler could not be installed.');
    } catch (error) { close(); throw error; }
    const isAlive = () => Boolean(hwnd && valid(hwnd) && valid(desktop.root)
        && Number(parent(hwnd, 1)) === Number(desktop.root) && (!desktop.after || valid(desktop.after)));
    return {
        mode: desktop.mode,
        isAlive,
        close,
        reassert() {
            if (paintError) throw paintError;
            // Reassert only sibling order. Resizing a live surface can expose an erase frame.
            if (!isAlive() || !position(hwnd, desktop.after, 0, 0, 0, 0, 0x13)) {
                throw new Error('The Windows wallpaper layer was lost.');
            }
        },
        present(image) {
            if (!isAlive()) throw new Error('The Windows wallpaper surface was destroyed.');
            const size = image.getSize();
            const bytes = image.toBitmap();
            if (size.width !== width || size.height !== height || bytes.length !== width * height * 4) {
                throw new Error('The rendered wallpaper frame does not match the display dimensions.');
            }
            if (!bitmap) {
                const header = Buffer.alloc(44);
                header.writeUInt32LE(40, 0); header.writeInt32LE(width, 4); header.writeInt32LE(-height, 8);
                header.writeUInt16LE(1, 12); header.writeUInt16LE(32, 14);
                const output = [null], screenDC = dc(0);
                // GDI handles can be sign-extended 64-bit values; Number would discard bits.
                try { bitmap = dib(screenDC, header, 0, output, 0, 0); } finally { releaseDC(0, screenDC); }
                bits = output[0];
                if (!bitmap || !bits) throw new Error('The wallpaper frame buffer could not be allocated.');
                memoryDC = compatibleDC(0);
                if (!memoryDC) throw new Error('The wallpaper drawing context could not be created.');
                const previous = select(memoryDC, bitmap);
                if (!previous || previous === 0xffffffffffffffffn) throw new Error('The wallpaper frame buffer could not be selected.');
                originalBitmap = previous;
            }
            k.encode(bits, 'uint8_t', bytes, width * height * 4);
            invalidate(hwnd, null, 0); update(hwnd);
            if (paintError) throw paintError;
            if (!shown) {
                shown = true;
                if (!position(hwnd, desktop.after, px, py, width, height, 0x50)) throw new Error('The wallpaper surface could not be shown.');
            }
        },
    };
}

module.exports = { createWindowsBitmapPresenter };
