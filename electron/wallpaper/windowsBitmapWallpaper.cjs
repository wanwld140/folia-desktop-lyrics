const { createWindowsBitmapPresenter } = require('./windowsBitmapPresenter.cjs');

// Keeps Chromium independent of Explorer; the taskbar, tray and emergency shortcut own exit.
function prepareWallpaperHandoff(handoff, enabled) {
    return enabled && handoff ? { ...handoff, ui: { ...handoff.ui, currentView: 'player' } } : handoff;
}
function createWindowsBitmapWallpaper({ globalShortcut, screen, store, getWindow, getBounds,
    onExit, onDegrade, createPresenter = createWindowsBitmapPresenter, log = console }) {
    let session = null, shortcutOwned = false;
    const shortcut = 'CommandOrControl+Alt+Shift+F12';
    const stop = () => {
        const previous = session; session = null;
        if (previous) {
            clearTimeout(previous.firstFrameTimer); clearInterval(previous.monitor);
            previous.win.webContents.removeListener('paint', previous.paint);
            previous.win.removeListener('closed', previous.closed);
            previous.presenter?.close();
        }
        if (shortcutOwned) globalShortcut.unregister(shortcut);
        shortcutOwned = false;
    };
    const fail = error => {
        log.error('[WallpaperWin] bitmap wallpaper failed; restoring the player', error);
        stop(); store.set('wallpaper_mode', false); onDegrade();
    };
    const leave = () => {
        if (!session) return;
        stop(); store.set('wallpaper_mode', false); onExit();
    };
    const attach = () => {
        const win = getWindow();
        if (!win || win.isDestroyed() || !win.webContents.isOffscreen()) return 'missing';
        if (session?.win === win) return 'attached';
        stop();
        const active = { win, presenter: null, firstFrameTimer: null, monitor: null, paint: null, closed: null };
        session = active;
        active.closed = () => { if (session === active) fail(new Error('The wallpaper renderer window closed.')); };
        active.paint = (_event, _dirty, image) => {
            if (session !== active || image.isEmpty()) return;
            try {
                if (!active.presenter) {
                    active.presenter = createPresenter({ bounds: screen.dipToScreenRect(null, getBounds()) });
                }
                active.presenter.present(image);
                clearTimeout(active.firstFrameTimer);
            } catch (error) { fail(error); }
        };
        win.webContents.on('paint', active.paint);
        win.once('closed', active.closed);
        win.webContents.setFrameRate(30);
        active.firstFrameTimer = setTimeout(() => { if (session === active) fail(new Error('No wallpaper frame arrived.')); }, 12000);
        active.monitor = setInterval(() => {
            if (session !== active || !active.presenter) return;
            try { active.presenter.reassert(); } catch (error) { fail(error); }
        }, 2000);
        try {
            shortcutOwned = globalShortcut.register(shortcut, leave);
            if (!shortcutOwned) log.warn?.('[WallpaperWin] emergency shortcut unavailable; exit remains available from the taskbar and tray');
            win.webContents.invalidate();
            log.info('[WallpaperWin] full-player bitmap wallpaper started at 30 FPS');
        } catch (error) { fail(error); return 'missing'; }
        return 'attached';
    };
    return { attach, detach({ onDetached } = {}) { const had = Boolean(session); stop(); onDetached?.(); return had; },
        killHelper: stop, isAttached: () => Boolean(session), exit: leave };
}

module.exports = { createWindowsBitmapWallpaper, prepareWallpaperHandoff };
