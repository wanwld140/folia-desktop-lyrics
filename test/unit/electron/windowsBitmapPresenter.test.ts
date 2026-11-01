import { createRequire } from 'node:module';
import { describe, expect, it, vi } from 'vitest';

// Verify complete-buffer painting, GDI synchronization and callback ownership without a desktop.
const { createWindowsBitmapPresenter } = createRequire(import.meta.url)('../../../electron/wallpaper/windowsBitmapPresenter.cjs');
function fixture() {
    const bitmap = 0xffffffff85000001n, memoryDC = 0xffffffff86000002n, original = 0xffffffff87000003n;
    const pointer = 0x12340000n, callbackAddress = 0x12345000n;
    const events: any[] = [], positions: any[] = [];
    let valid = true, procedure: any = 0x9000, callback: (...args: any[]) => any;
    const functions: Record<string, any> = {
        FindWindowW: () => 10,
        FindWindowExW: (root: number, _: any, cls: string) => root === 10 ? (cls === 'SHELLDLL_DefView' ? 11 : cls === 'WorkerW' ? 12 : 0) : 0,
        IsWindow: () => Number(valid), GetAncestor: () => 10,
        CreateWindowExW: () => 100,
        DestroyWindow: () => { events.push('destroy'); valid = false; return 1; },
        SetWindowLongPtrW: (_h: any, _index: any, address: any) => { events.push(['proc', address]); const old = procedure; procedure = address; return old; },
        CallWindowProcW: vi.fn(() => 7),
        BeginPaint: () => { events.push('begin'); return 99; },
        EndPaint: () => { events.push('end'); return 1; },
        SendMessageTimeoutW: vi.fn(), SetLayeredWindowAttributes: () => 1,
        SetWindowPos: (...args: any[]) => { positions.push(args); return 1; },
        ScreenToClient: () => 1, InvalidateRect: () => 1,
        UpdateWindow: () => { callback(100, 0x0f, 0, 0); return 1; },
        GetDC: () => 20, ReleaseDC: () => 1,
        CreateDIBSection: vi.fn((_dc: any, _header: any, _usage: any, output: any[]) => { output[0] = pointer; return bitmap; }),
        DeleteObject: (handle: any) => { events.push(['delete', handle]); return 1; },
        CreateCompatibleDC: () => memoryDC,
        SelectObject: (_dc: any, handle: any) => { events.push(['select', handle]); return original; },
        DeleteDC: (handle: any) => { events.push(['deleteDC', handle]); return 1; },
        BitBlt: vi.fn(() => { events.push('blit'); return 1; }),
        GdiFlush: vi.fn(() => { events.push('flush'); return 1; }),
    };
    const encode = vi.fn((_pointer: any, _type: any, _bytes: any, _length: any) => { events.push('write'); });
    const k = { load: () => ({ func: (_cc: any, name: string) => functions[name] }),
        proto: () => 'paintProc', out: (v: any) => v, pointer: (v: any) => v, encode,
        register: (fn: any) => { callback = fn; return callbackAddress; }, address: (v: any) => v,
        unregister: () => { events.push('unregister'); } };
    const presenter = createWindowsBitmapPresenter({ bounds: { x: 0, y: 0, width: 2, height: 1 }, loadKoffi: () => k });
    const frame = { getSize: () => ({ width: 2, height: 1 }), toBitmap: () => Buffer.alloc(8, 255) };
    return { presenter, frame, bitmap, memoryDC, original, pointer, encode, events, positions, functions,
        message: (message: number) => callback(100, message, 0, 0) };
}
describe('Windows bitmap desktop presentation', () => {
    it('paints complete frames and flushes GDI reads before the next pixel write', () => {
        const f = fixture(); f.presenter.present(f.frame); f.presenter.present(f.frame);
        expect(f.functions.CreateDIBSection).toHaveBeenCalledOnce();
        expect(f.functions.BitBlt.mock.calls[0][5]).toBe(f.memoryDC);
        expect(f.encode.mock.calls[0][0]).toBe(f.pointer);
        expect(f.events.slice(-10)).toEqual(['write', 'begin', 'blit', 'flush', 'end', 'write', 'begin', 'blit', 'flush', 'end']);
        expect(f.positions[0][1]).toBe(11);
        f.presenter.close();
    });
    it('suppresses background erasure and reuses the retained frame on exposure paints', () => {
        const f = fixture(); f.presenter.present(f.frame);
        expect(f.message(0x14)).toBe(1);
        f.message(0x0f);
        expect(f.functions.BitBlt).toHaveBeenCalledTimes(2);
        expect(f.encode).toHaveBeenCalledOnce();
        expect(f.message(0x10)).toBe(7);
        expect(f.functions.CallWindowProcW).toHaveBeenCalledWith(0x9000, 100, 0x10, 0, 0);
        f.presenter.close();
    });
    it('restores the procedure before destroying the HWND and releases exact 64-bit handles once', () => {
        const f = fixture(); f.presenter.present(f.frame); f.presenter.close();
        expect(f.events.slice(-6)).toEqual([['proc', 0x9000], 'destroy', 'unregister', ['select', f.original], ['delete', f.bitmap], ['deleteDC', f.memoryDC]]);
        const count = f.events.length; f.presenter.close(); expect(f.events).toHaveLength(count);
    });
    it('keeps geometry unchanged during periodic z-order checks', () => {
        const f = fixture(); f.presenter.present(f.frame); f.presenter.reassert();
        expect(f.positions.at(-1)).toEqual([100, 11, 0, 0, 0, 0, 0x13]);
        f.presenter.close();
    });
    it('reports a GDI synchronization failure for normal-window recovery', () => {
        const f = fixture(); f.functions.GdiFlush.mockReturnValue(0);
        expect(() => f.presenter.present(f.frame)).toThrow('synchronized');
        expect(f.events).toContain('end');
        f.presenter.close();
    });
    it('rejects mismatched pixel sizes before allocating a frame buffer', () => {
        const f = fixture();
        expect(() => f.presenter.present({ ...f.frame, toBitmap: () => Buffer.alloc(4) })).toThrow('dimensions');
        expect(f.functions.CreateDIBSection).not.toHaveBeenCalled();
        f.presenter.close();
    });
});
