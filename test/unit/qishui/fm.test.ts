import { beforeEach, describe, expect, it, vi } from 'vitest';
import { usePersonalFmModeStore, getPersonalFmSelection } from '@/stores/usePersonalFmModeStore';
import { qishuiRecommendations, getQishuiFmCatalog } from '@/services/onlineMusic/qishui/qishuiRecommendations';
import { requestQishui } from '@/services/onlineMusic/qishui/qishuiTransport';
import { qishuiProvider } from '@/services/onlineMusic/qishui/qishuiProvider';
import { omni } from '@/services/onlineMusic/omni';
import { normalizeQishuiSong } from '@/services/onlineMusic/qishui/qishuiNormalize';
import { track } from './fixtures';

vi.mock('@/services/onlineMusic/qishui/qishuiTransport', () => ({ requestQishui: vi.fn(), getQishuiAvailability: vi.fn(),
    getQishuiLastFailure: vi.fn(), readQishuiSceneIcon: vi.fn(), QishuiRequestError: class extends Error {} }));
beforeEach(() => {
    vi.mocked(requestQishui).mockReset().mockResolvedValue({ items: [track()] });
    usePersonalFmModeStore.setState({ selection: { mode: 'DEFAULT', scene: null }, providerSelections: {}, catalogs: {} });
});
describe('Qishui phone radio', () => {
    it('offers only the verified default stream and no car scene cards', async () => {
        const catalog = await getQishuiFmCatalog();
        expect(catalog.modes.map(mode => mode.id)).toEqual(['DEFAULT']);
        expect(catalog.scenes).toEqual([]);
        expect(await qishuiRecommendations.getRecommendedCollections!(35)).toEqual([]);
        expect(qishuiProvider.capabilities.personalFmModes).toBe(false);
        expect(requestQishui).not.toHaveBeenCalled();
    });
    it('migrates old car scene choices back to default without changing other providers', async () => {
        usePersonalFmModeStore.setState({ providerSelections: {
            qishui: { mode: 'SCENE_RCMD', scene: '33' }, netease: { mode: 'FAMILIAR', scene: null },
        } });
        usePersonalFmModeStore.getState().setCatalog('qishui', await getQishuiFmCatalog());
        expect(getPersonalFmSelection('qishui')).toEqual({ mode: 'DEFAULT', scene: null });
        expect(getPersonalFmSelection('netease')).toEqual({ mode: 'FAMILIAR', scene: null });
    });
    it('requests phone FM without forwarding car scene preferences', async () => {
        const songs = await qishuiRecommendations.getPersonalFm!();
        expect(songs[0].id).toBe(track().base_info.id);
        expect(requestQishui).toHaveBeenCalledExactlyOnceWith('feed', { queue: 'fm', limit: 20 });
        await expect(qishuiRecommendations.getPersonalFm!({ mode: 'SCENE_RCMD', submode: '33' })).rejects.toMatchObject({ code: 'unsupported' });
    });
    it('shows phone guest scope independently of the compatibility account', () => {
        expect(omni.getPersonalFmNoticeKey(normalizeQishuiSong(track()))).toBe('qishui.mobileRecommendationNotice');
        expect(qishuiRecommendations.dislikeSong).toBeUndefined();
    });
});
