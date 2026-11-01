const { spawn } = require('node:child_process');
const fs = require('node:fs');
const path = require('node:path');

// Owns the Windows taskbar card process and its bounded, latest-snapshot-only JSONL pipe.
function sanitizeSnapshot(value) {
    if (!value || typeof value !== 'object') throw new TypeError('Invalid taskbar snapshot');
    const text = (key, limit = 2048) => typeof value[key] === 'string' ? value[key].slice(0, limit) : '';
    const number = key => Number.isFinite(value[key]) ? Math.max(0, value[key]) : 0;
    const coverPng = text('coverPng', 131072);
    return {
        trackId: text('trackId'), title: text('title'), artist: text('artist'),
        connected: value.connected === true, playing: value.playing === true,
        canPrevious: value.canPrevious === true, canToggle: value.canToggle === true,
        canNext: value.canNext === true, position: number('position'), duration: number('duration'),
        currentLyric: text('currentLyric'), nextLyric: text('nextLyric'),
        coverPng: /^[A-Za-z0-9+/=]*$/.test(coverPng) ? coverPng : '',
    };
}

function createTaskbarLyricsController({ app, getMainWindow, resourcesPath = process.resourcesPath,
    platform = process.platform, spawnFn = spawn, exists = fs.existsSync, now = Date.now, onOpen, onMenu }) {
    let child = null;
    let ready = false;
    let blocked = false;
    let latest = null;
    let sequence = 0;
    let failures = 0;
    let retryAt = 0;
    let error = null;
    let startupTimer = null;
    let disposed = false;
    const status = () => ({ running: Boolean(child && ready), error });
    const stop = () => {
        clearTimeout(startupTimer);
        startupTimer = null;
        const previous = child;
        child = null;
        ready = false;
        blocked = false;
        // EOF lets the helper dispose the child HWND; kill is only the bounded fallback.
        if (previous) {
            previous.stdin?.end();
            const timeout = setTimeout(() => previous.kill(), 1500);
            timeout.unref?.();
            previous.once('exit', () => clearTimeout(timeout));
        }
    };
    const fail = (source, cause) => {
        if (child !== source) return;
        error = String(cause?.message || cause);
        console.warn('[TaskbarLyrics] Helper stopped', cause);
        failures += 1;
        retryAt = now() + 2000;
        stop();
    };
    const writeLatest = () => {
        if (!child || !ready || blocked || !latest) return;
        const source = child;
        try {
            blocked = !source.stdin.write(JSON.stringify({ ...latest, version: 1, sequence: ++sequence }) + '\n');
        } catch (cause) { fail(source, cause); }
    };
    const start = () => {
        const helperPath = app.isPackaged
            ? path.join(resourcesPath, 'folia-taskbar-lyrics.exe')
            : path.join(app.getAppPath(), 'build', 'folia-taskbar-lyrics.exe');
        if (!exists(helperPath)) {
            error = 'Taskbar helper missing. Run npm run build:taskbar-lyrics.';
            return;
        }
        let source;
        try {
            source = spawnFn(helperPath, [String(process.pid), path.join(app.getPath('userData'), 'taskbar-lyrics', 'position.json')], {
                stdio: ['pipe', 'pipe', 'pipe'], windowsHide: true,
            });
        } catch (cause) {
            error = String(cause?.message || cause);
            failures += 1;
            retryAt = now() + 2000;
            return;
        }
        child = source;
        startupTimer = setTimeout(() => fail(source, new Error('Taskbar helper ready timeout')), 10000);
        startupTimer.unref?.();
        source.stdin.on('error', cause => fail(source, cause));
        source.stdin.on('drain', () => {
            if (child !== source) return;
            blocked = false;
            writeLatest();
        });
        let buffer = '';
        source.stdout.setEncoding('utf8');
        source.stdout.on('data', chunk => {
            if (child !== source) return;
            buffer += chunk;
            if (buffer.length > 65536) { fail(source, new Error('Oversized helper event')); return; }
            let index;
            while ((index = buffer.indexOf('\n')) !== -1) {
                const line = buffer.slice(0, index);
                buffer = buffer.slice(index + 1);
                let event;
                try { event = JSON.parse(line); } catch { continue; }
                if (event.version !== 1) continue;
                if (event.type === 'ready') {
                    clearTimeout(startupTimer);
                    startupTimer = null;
                    ready = true;
                    error = null;
                    console.info('[TaskbarLyrics] Native card ready');
                    writeLatest();
                } else if (event.type === 'control') {
                    const handler = event.action === 'menu' ? onMenu : event.action === 'open' ? onOpen : null;
                    if (handler) {
                        try { handler(); } catch (cause) { console.warn('[TaskbarLyrics] Window action failed', cause); }
                        continue;
                    }
                    const window = getMainWindow();
                    if (!window || window.isDestroyed()) continue;
                    const action = event.action;
                    if (action === 'open' || action === 'menu') {
                        if (window.isMinimized()) window.restore();
                        window.show();
                        window.focus();
                        if (action === 'menu') window.webContents.send('taskbar-lyrics-open-settings');
                    } else if ((action === 'previous' && latest?.canPrevious)
                        || (action === 'toggle' && latest?.canToggle)
                        || (action === 'next' && latest?.canNext)) {
                        window.webContents.send('thumbar-action', action);
                    }
                }
            }
        });
        source.stderr.on('data', chunk => console.info('[TaskbarLyrics]', chunk.toString().trim().slice(0, 4096)));
        source.once('error', cause => fail(source, cause));
        source.once('exit', (code, signal) => fail(source, new Error(`Helper exit ${code ?? signal}`)));
    };
    return {
        publish(value) {
            if (disposed) return status();
            if (value === null) {
                latest = null;
                stop();
                failures = 0;
                retryAt = 0;
                error = null;
                return status();
            }
            if (platform !== 'win32') return { running: false, error: 'Windows only' };
            latest = sanitizeSnapshot(value);
            if (!child && failures < 3 && now() >= retryAt) start();
            else writeLatest();
            return status();
        },
        stop() { latest = null; stop(); },
        dispose() { disposed = true; latest = null; stop(); },
    };
}

module.exports = { createTaskbarLyricsController, sanitizeSnapshot };
