import { create } from 'zustand';
import { getStoredBoolean, setStoredBoolean } from '../../../stores/storagePrimitives';

// The opt-in Windows taskbar lyric preference and transient helper failure.
export const TASKBAR_LYRICS_STORAGE_KEY = 'folia_taskbar_lyrics_enabled';
export const hasTaskbarLyrics = () => typeof window !== 'undefined' && window.electron?.platform === 'win32';

export const useTaskbarLyricsStore = create<{
    enabled: boolean;
    error: string | null;
    setEnabled: (enabled: boolean) => void;
    setError: (error: string | null) => void;
}>((set) => ({
    enabled: getStoredBoolean(TASKBAR_LYRICS_STORAGE_KEY, false),
    error: null,
    setEnabled: (enabled) => {
        setStoredBoolean(TASKBAR_LYRICS_STORAGE_KEY, enabled);
        set({ enabled, error: null });
    },
    setError: (error) => set(state => state.error === error ? state : { error }),
}));
