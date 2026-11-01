import { createRequire } from 'node:module';
import { EventEmitter } from 'node:events';
import { afterEach, describe, expect, it, vi } from 'vitest';

// Exercise exit and recovery independently of the Chromium renderer and Explorer lifecycle.
const { createWindowsBitmapWallpaper, prepareWallpaperHandoff } = createRequire(import.meta.url)('../../../electron/wallpaper/windowsBitmapWallpaper.cjs');
function fixture() {
    vi.useFakeTimers();
    const contents = Object.assign(new EventEmitter(), { isOffscreen: () => true, setFrameRate: vi.fn(), invalidate: vi.fn() });
    const win = Object.assign(new EventEmitter(), { isDestroyed: () => false, webContents: contents });
    const controls: any[] = [];
    class Control extends EventEmitter {
        dead = false;
        webContents = { setWindowOpenHandler: vi.fn(), on: vi.fn() };
        constructor() { super(); controls.push(this); }
        isDestroyed() { return this.dead; }
        setMenu() {}
        loadURL() { return Promise.resolve(); }
        show() {}
        focus() {}
        destroy() { this.dead = true; this.emit('closed'); }
    }
    const presenter = { present: vi.fn(), reassert: vi.fn(), close: vi.fn() };
    const createPresenter = vi.fn(() => presenter);
    const store = { set: vi.fn() }, onExit = vi.fn(), onDegrade = vi.fn();
    const globalShortcut = { register: vi.fn((_name: string, _callback: () => void) => true), unregister: vi.fn() };
    const controller = createWindowsBitmapWallpaper({ BrowserWindow: Control, globalShortcut,
        screen: { dipToScreenRect: (_: any, b: any) => b }, store, getWindow: () => win,
        getBounds: () => ({ x: 0, y: 0, width: 2560, height: 1440 }), language: () => 'zh-CN',
        createPresenter, onExit, onDegrade, log: { info() {}, error() {} } });
    const image = { isEmpty: () => false };
    return { controller, contents, win, controls, presenter, createPresenter, store, onExit, onDegrade, globalShortcut, image };
}
afterEach(() => { vi.clearAllTimers(); vi.useRealTimers(); });
describe('Windows full-player bitmap wallpaper', () => {
    it('opens the player when entering from the library and preserves playback', () => {
        const original = { ui: { currentView: 'lattice', playerChromeHidden: true }, activePlayback: { time: 42 } };
        const next = prepareWallpaperHandoff(original, true);
        expect(next.ui).toEqual({ currentView: 'player', playerChromeHidden: true });
        expect(next.activePlayback).toBe(original.activePlayback);
        expect(original.ui.currentView).toBe('lattice');
        expect(prepareWallpaperHandoff(original, false)).toBe(original);
    });
    it('creates the native surface only when a frame is ready and cleans up on detach', () => {
        const f = fixture();
        expect(f.controller.attach()).toBe('attached');
        expect(f.createPresenter).not.toHaveBeenCalled();
        f.contents.emit('paint', {}, {}, f.image);
        expect(f.presenter.present).toHaveBeenCalledWith(f.image);
        expect(f.controller.detach()).toBe(true);
        expect(f.presenter.close).toHaveBeenCalledOnce();
        expect(f.contents.listenerCount('paint')).toBe(0);
        expect(f.onExit).not.toHaveBeenCalled();
        vi.advanceTimersByTime(20000);
        expect(f.onDegrade).not.toHaveBeenCalled();
    });
    it('does not create a persistent control window and exits through the main-process action', () => {
        const f = fixture(); f.controller.attach();
        expect(f.controls).toEqual([]);
        f.controller.exit();
        expect(f.store.set).toHaveBeenCalledWith('wallpaper_mode', false);
        expect(f.onExit).toHaveBeenCalledOnce();
        expect(f.controller.isAttached()).toBe(false);
    });
    it('restores normal mode when the first frame never arrives', () => {
        const f = fixture(); f.controller.attach(); vi.advanceTimersByTime(12000);
        expect(f.store.set).toHaveBeenCalledWith('wallpaper_mode', false);
        expect(f.onDegrade).toHaveBeenCalledOnce();
        expect(f.contents.listenerCount('paint')).toBe(0);
    });
    it('degrades if Explorer destroys the native surface while Chromium survives', () => {
        const f = fixture(); f.controller.attach(); f.contents.emit('paint', {}, {}, f.image);
        f.presenter.reassert.mockImplementation(() => { throw new Error('lost'); });
        vi.advanceTimersByTime(2000);
        expect(f.onDegrade).toHaveBeenCalledOnce();
        expect(f.presenter.close).toHaveBeenCalledOnce();
    });
    it('keeps taskbar and tray exit usable when the emergency hotkey is occupied', () => {
        const f = fixture(); f.globalShortcut.register.mockReturnValue(false); f.controller.attach();
        expect(f.controller.isAttached()).toBe(true);
        f.controller.exit();
        expect(f.onExit).toHaveBeenCalledOnce();
        expect(f.globalShortcut.unregister).not.toHaveBeenCalled();
    });
    it('provides an emergency exit even if the full player renderer is stuck', () => {
        const f = fixture(); f.controller.attach();
        const callback = f.globalShortcut.register.mock.calls[0][1] as () => void;
        callback();
        expect(f.onExit).toHaveBeenCalledOnce();
        expect(f.globalShortcut.unregister).toHaveBeenCalledWith('CommandOrControl+Alt+Shift+F12');
    });
});
