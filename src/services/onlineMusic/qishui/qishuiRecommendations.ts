import type { OnlineRecommendationProvider } from '../../../types/onlineMusic';
import { OnlineProviderError } from '../../../types/onlineMusic';
import type { PersonalFmCatalog } from '../../../types/personalFm';
import { DEFAULT_PERSONAL_FM_CATALOG } from '../fmModes';
import { requestQishui } from './qishuiTransport';
import { normalizeQishuiSong } from './qishuiNormalize';

// Only the phone's default song stream is verified. Car scene IDs and preference modes do not
// establish equivalent phone modes; a default-only catalogue also resets previously saved choices.
export async function getQishuiFmCatalog(): Promise<PersonalFmCatalog> {
    return { modes: DEFAULT_PERSONAL_FM_CATALOG.modes.filter(entry => entry.id === 'DEFAULT'), categories: [], scenes: [] };
}
export const qishuiRecommendations: OnlineRecommendationProvider = {
    personalFmNoticeKey: 'qishui.mobileRecommendationNotice',
    getPersonalFmCatalog: getQishuiFmCatalog,
    async getPersonalFm(options) {
        if (options?.mode && options.mode !== 'DEFAULT') {
            throw new OnlineProviderError('unsupported', 'This Qishui mobile recommendation mode is not available', 'qishui');
        }
        const data = await requestQishui<{ items: unknown[] }>('feed', { queue: 'fm', limit: 20 });
        return data.items.map(normalizeQishuiSong);
    },
    // Do not advertise car scene cards or send car dislike actions for the phone stream.
    async getRecommendedCollections() { return []; },
};
