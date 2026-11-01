import { PlayerState, type LyricData } from '../../../types';
import type { PlaybackSyncBridgeModel } from '../../../utils/playbackSyncBridge';
import { findLatestActiveLineIndex } from '../../../utils/appPlaybackHelpers';
import { getUpcomingLine } from '../../visualizer/runtime';
import { getPlaybackSongKey } from '../../../utils/appPlaybackGuards';

// Adapts Folia's displayed track and lyric clock to the native card's small wire snapshot.
export function buildTaskbarLyricsSnapshot(model: PlaybackSyncBridgeModel, lyrics: LyricData | null, coverPng = '', syntheticStageLyrics = false) {
    const position = model.activePlaybackContext === 'stage' ? model.stagePositionSec : model.currentTimeSec;
    // Pure-lyric stage entries already use their lyric timeline as the synthetic playback clock.
    const lyricTime = position - (syntheticStageLyrics ? 0 : (model.lyricOffsetMs ?? 0) / 1000);
    const lines = lyrics?.lines ?? [];
    const activeIndex = findLatestActiveLineIndex(lines, lyricTime);
    const active = lines[activeIndex];
    const upcoming = getUpcomingLine(lines, activeIndex, lyricTime);
    return {
        trackId: model.currentSong ? getPlaybackSongKey(model.currentSong) : '',
        title: model.title ?? 'Folia', artist: model.artist ?? '',
        connected: Boolean(model.currentSong), playing: model.playerState === PlayerState.PLAYING,
        canPrevious: !model.controlsDisabled && model.canGoPrevious,
        canToggle: !model.controlsDisabled, canNext: !model.controlsDisabled && model.canGoNext,
        position, duration: model.activePlaybackContext === 'stage' ? model.stageDurationSec : model.durationSec,
        currentLyric: active?.fullText ?? '', nextLyric: upcoming?.fullText ?? '', coverPng,
    };
}

export type TaskbarLyricsSnapshot = ReturnType<typeof buildTaskbarLyricsSnapshot>;
