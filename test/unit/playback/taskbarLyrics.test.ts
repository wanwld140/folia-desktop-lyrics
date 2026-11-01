import { describe, expect, it } from 'vitest';
import { PlayerState, type LyricData } from '../../../src/types';
import type { PlaybackSyncBridgeModel } from '../../../src/utils/playbackSyncBridge';
import { buildTaskbarLyricsSnapshot } from '../../../src/components/app/taskbarLyrics/snapshot';

// Verify timing boundaries, preview-clip offsets, seeks and empty lyrics using Folia's line model.
const lyrics: LyricData = { lines: [
    { fullText: '第一句', startTime: 10, endTime: 12, words: [] },
    { fullText: '下一句', startTime: 15, endTime: 18, words: [] },
] };
const model = (time: number, offset = 0) => ({
    currentTimeSec: time, lyricOffsetMs: offset, title: 'Song', artist: 'Artist',
    durationSec: 90, playerState: PlayerState.PAUSED, controlsDisabled: false,
    canGoPrevious: false, canGoNext: true, currentSong: null,
} as PlaybackSyncBridgeModel);

describe('taskbar lyric timing', () => {
    it('uses the synthetic stage clock without applying the manual lyric delay again', () => {
        const stage = { ...model(0, 5000), activePlaybackContext: 'stage' as const, stagePositionSec: 16, stageDurationSec: 60 };
        expect(buildTaskbarLyricsSnapshot(stage, lyrics, '', true)).toMatchObject({
            currentLyric: '下一句', position: 16, duration: 60,
        });
    });
    it('shows current and upcoming lines and immediately follows a backward seek while paused', () => {
        expect(buildTaskbarLyricsSnapshot(model(16), lyrics)).toMatchObject({ currentLyric: '下一句', nextLyric: '', playing: false });
        expect(buildTaskbarLyricsSnapshot(model(11), lyrics)).toMatchObject({ currentLyric: '第一句', nextLyric: '下一句' });
    });
    it('applies positive manual delay and the clip-adjusted offset exactly once', () => {
        expect(buildTaskbarLyricsSnapshot(model(13, 2000), lyrics).currentLyric).toBe('第一句');
        expect(buildTaskbarLyricsSnapshot(model(1, -10000), lyrics).currentLyric).toBe('第一句');
    });
    it('clears the last lyric during gaps, before the first line, and when no lyrics exist', () => {
        expect(buildTaskbarLyricsSnapshot(model(14), lyrics)).toMatchObject({ currentLyric: '', nextLyric: '下一句' });
        expect(buildTaskbarLyricsSnapshot(model(0), lyrics)).toMatchObject({ currentLyric: '', nextLyric: '第一句' });
        expect(buildTaskbarLyricsSnapshot(model(16), null)).toMatchObject({ title: 'Song', currentLyric: '', nextLyric: '' });
    });
});
