import { useEffect, useRef } from 'react';
import type { LyricData } from '../../../types';
import type { PlaybackSyncBridgeModel } from '../../../utils/playbackSyncBridge';
import { useSettingsModalStore } from '../../../stores/useSettingsModalStore';
import { hasTaskbarLyrics, useTaskbarLyricsStore } from './useTaskbarLyricsStore';
import { buildTaskbarLyricsSnapshot } from './snapshot';

// Publishes outside React's render loop and keeps asynchronous covers tied to their displayed song.
export function useTaskbarLyricsBridge(readModel: () => PlaybackSyncBridgeModel, lyrics: LyricData | null, coverUrl: string | null, syntheticStageLyrics = false) {
    const enabled = useTaskbarLyricsStore(state => state.enabled);
    const input = useRef({ readModel, lyrics, syntheticStageLyrics });
    const cover = useRef('');
    useEffect(() => { input.current = { readModel, lyrics, syntheticStageLyrics }; });

    useEffect(() => {
        cover.current = '';
        if (!enabled || !hasTaskbarLyrics() || !coverUrl) return;
        let cancelled = false;
        const image = new Image();
        image.crossOrigin = 'anonymous';
        image.onload = () => {
            if (cancelled) return;
            try {
                const canvas = document.createElement('canvas');
                canvas.width = canvas.height = 64;
                canvas.getContext('2d')?.drawImage(image, 0, 0, 64, 64);
                cover.current = canvas.toDataURL('image/png').split(',')[1] ?? '';
            } catch { /* A remote cover may disallow canvas access; the music icon remains usable. */ }
        };
        image.src = coverUrl;
        return () => { cancelled = true; image.onload = null; image.src = ''; };
    }, [enabled, coverUrl]);

    useEffect(() => {
        if (!hasTaskbarLyrics() || !window.electron?.publishTaskbarLyrics) return;
        let active = true;
        let pending = false;
        const publish = () => {
            if (pending) return;
            const { readModel, lyrics, syntheticStageLyrics } = input.current;
            pending = true;
            void window.electron!.publishTaskbarLyrics(enabled
                ? buildTaskbarLyricsSnapshot(readModel(), lyrics, cover.current, syntheticStageLyrics) : null)
                .then(result => { if (active) useTaskbarLyricsStore.getState().setError(result.error); })
                .catch(error => {
                    if (active) useTaskbarLyricsStore.getState().setError(String(error));
                }).finally(() => { pending = false; });
        };
        publish();
        const timer = enabled ? window.setInterval(publish, 150) : undefined;
        const unsubscribe = window.electron.onTaskbarLyricsOpenSettings(() => {
            useSettingsModalStore.getState().openSettings('options', 'desktop', null, 'desktopTrayBehavior', 'taskbarLyrics');
        });
        return () => {
            active = false;
            window.clearInterval(timer);
            unsubscribe();
            void window.electron?.publishTaskbarLyrics(null).catch(() => {});
        };
    }, [enabled]);
}
