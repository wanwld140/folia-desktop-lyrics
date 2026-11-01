import { createRequire } from 'node:module';
import { EventEmitter } from 'node:events';
import { describe, expect, it, vi } from 'vitest';

// Exercise the real controller with deterministic pipes instead of creating Windows child windows.
const require = createRequire(import.meta.url);
const { createTaskbarLyricsController, sanitizeSnapshot } = require('../../../electron/taskbarLyrics/controller.cjs');
function fixture(actions: Record<string, any> = {}) {
    const writes: string[] = [];
    const child = new EventEmitter() as any;
    child.stdin = Object.assign(new EventEmitter(), { write: vi.fn((line: string) => { writes.push(line); return true; }), end: vi.fn() });
    child.stdout = Object.assign(new EventEmitter(), { setEncoding: vi.fn() });
    child.stderr = new EventEmitter();
    child.kill = vi.fn();
    const window = { isDestroyed: () => false, isMinimized: () => false, show: vi.fn(), focus: vi.fn(), webContents: { send: vi.fn() } };
    let time = 100;
    const spawn = vi.fn(() => child);
    const controller = createTaskbarLyricsController({
        app: { isPackaged: false, getAppPath: () => '/folia', getPath: () => '/profile' },
        getMainWindow: () => window, platform: 'win32', spawnFn: spawn, exists: () => true, now: () => time,
        ...actions,
    });
    const emit = (event: object) => child.stdout.emit('data', JSON.stringify({ version: 1, ...event }) + '\n');
    return { controller, child, writes, window, spawn, emit, advance: () => { time += 3000; } };
}

describe('taskbar lyric controller', () => {
    it('bounds wire data and rejects non-finite progress', () => {
        expect(sanitizeSnapshot({ title: 'a'.repeat(9000), position: Infinity, duration: -1, playing: 'yes', coverPng: 'bad!' }))
            .toMatchObject({ title: 'a'.repeat(2048), position: 0, duration: 0, playing: false, coverPng: '' });
    });
    it('waits for the version handshake and sends only the newest startup snapshot', () => {
        const f = fixture();
        f.controller.publish({ title: 'old' });
        f.controller.publish({ title: 'new' });
        expect(f.writes).toEqual([]);
        f.emit({ type: 'ready', version: 2 });
        expect(f.writes).toEqual([]);
        f.emit({ type: 'ready' });
        expect(JSON.parse(f.writes[0])).toMatchObject({ version: 1, sequence: 1, title: 'new' });
        f.controller.dispose();
        f.child.emit('exit', 0);
    });
    it('coalesces blocked pipe writes instead of building a stale playback backlog', () => {
        const f = fixture();
        f.child.stdin.write.mockImplementation((line: string) => { f.writes.push(line); return false; });
        f.controller.publish({ title: 'first' });
        f.emit({ type: 'ready' });
        f.controller.publish({ title: 'second' });
        f.controller.publish({ title: 'last' });
        expect(f.writes).toHaveLength(1);
        f.child.stdin.emit('drain');
        expect(JSON.parse(f.writes[1]).title).toBe('last');
        f.controller.dispose();
        f.child.emit('exit', 0);
    });
    it('gates controls, reuses Folia controls, and opens settings from the card menu', () => {
        const f = fixture();
        f.controller.publish({ canToggle: true, canNext: false });
        f.emit({ type: 'ready' });
        f.emit({ type: 'control', action: 'next' });
        expect(f.window.webContents.send).not.toHaveBeenCalled();
        f.emit({ type: 'control', action: 'toggle' });
        expect(f.window.webContents.send).toHaveBeenCalledWith('thumbar-action', 'toggle');
        f.emit({ type: 'control', action: 'menu' });
        expect(f.window.webContents.send).toHaveBeenCalledWith('taskbar-lyrics-open-settings');
        f.controller.publish(null);
        f.emit({ type: 'control', action: 'toggle' });
        expect(f.child.stdin.end).toHaveBeenCalledOnce();
        f.child.emit('exit', 0);
    });
    it('caps crash retries and lets an explicit off/on start a fresh session', () => {
        const f = fixture();
        for (let attempt = 0; attempt < 3; attempt++) {
            f.controller.publish({});
            f.child.emit('exit', 1);
            f.advance();
        }
        f.controller.publish({});
        expect(f.spawn).toHaveBeenCalledTimes(3);
        f.controller.publish(null);
        f.controller.publish({});
        expect(f.spawn).toHaveBeenCalledTimes(4);
        f.controller.dispose();
        f.child.emit('exit', 0);
    });
    it('routes the menu and open actions to the main process without showing an offscreen window', () => {
        const onMenu = vi.fn(), onOpen = vi.fn();
        const f = fixture({ onMenu, onOpen });
        f.controller.publish({}); f.emit({ type: 'ready' });
        f.emit({ type: 'control', action: 'menu' });
        f.emit({ type: 'control', action: 'open' });
        expect(onMenu).toHaveBeenCalledOnce(); expect(onOpen).toHaveBeenCalledOnce();
        expect(f.window.show).not.toHaveBeenCalled(); expect(f.window.focus).not.toHaveBeenCalled();
        expect(f.window.webContents.send).not.toHaveBeenCalled();
        f.controller.dispose(); f.child.emit('exit', 0);
        f.emit({ type: 'control', action: 'menu' });
        expect(onMenu).toHaveBeenCalledOnce();
    });
});
