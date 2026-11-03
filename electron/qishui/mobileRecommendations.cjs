// Phone recommendation reads are independent of the car SDK's account and device credentials.
// beta-luna is the production host used by the Android app (21.4.0).
const ENDPOINT = 'https://beta-luna.douyin.com/luna/feed/song-tab';
const QUERY = Object.freeze({ aid: '8478', app_name: 'luna', version_name: '21.4.0',
    version_code: '100214030', update_version_code: '100214030', manifest_version_code: '100214030', device_platform: 'android' });
class MobileFeedError extends Error {
    constructor(code, message, upstreamCode) { super(message); this.code = code; this.upstreamCode = upstreamCode; }
}
const invalid = () => new MobileFeedError('invalid-response', 'Invalid Qishui mobile recommendation response');
const object = value => value && typeof value === 'object' && !Array.isArray(value);
const id = value => {
    // JSON numbers cannot preserve the service's 64-bit identities.
    if (typeof value !== 'string' || !/^\d{1,30}$/.test(value)) throw invalid();
    return value;
};
const name = value => typeof value === 'string' ? value : '';
function cover(raw) {
    if (!object(raw)) return undefined;
    return { uri: name(raw.uri), template_prefix: name(raw.template_prefix),
        urls: Array.isArray(raw.urls) ? raw.urls.filter(value => typeof value === 'string' && value.startsWith('https://')) : [] };
}
function publicTrack(item) {
    if (!object(item)) throw invalid();
    if (item.type !== 'track') return null;
    const track = item.entity?.track_wrapper?.track;
    if (!object(track) || typeof track.name !== 'string' || !Number.isFinite(track.duration) || track.duration < 0) throw invalid();
    const album = object(track.album) ? track.album : {};
    if (!Array.isArray(track.artists)) throw invalid();
    // Only public catalogue metadata crosses IPC; player models, signed URLs and feed context stay out.
    return { id: id(track.id), base_info: { id: track.id, name: track.name, duration_ms: track.duration },
        related_info: { artist_links: track.artists.filter(object).map(artist => ({ id: id(artist.id), name: name(artist.name) })),
            album_link: { ...(album.id == null ? {} : { id: id(album.id) }), name: name(album.name) } },
        display_info: { cover_url: cover(album.url_cover) } };
}
function createMobileRecommendations({ fetch: fetcher = globalThis.fetch } = {}) {
    const queues = new Map();
    let generation = 0;
    let requested = false;
    async function load(params, state, mine, queue) {
        const current = () => mine === generation && queues.get(queue) === state;
        if (!current()) throw new MobileFeedError('unavailable', 'Qishui recommendation request was reset');
        const didFirst = !requested;
        requested = true;
        let response;
        try {
            response = await fetcher(`${ENDPOINT}?${new URLSearchParams(QUERY)}`, {
                method: 'POST', credentials: 'omit', redirect: 'error', signal: AbortSignal.timeout(20000),
                headers: { 'user-agent': 'Luna/21.4.0 Android', 'content-type': 'application/json; charset=utf-8', accept: 'application/json' },
                body: JSON.stringify({ count: params.limit, feed_mode: 'track', is_first_request: !state.started,
                    is_did_first_request: didFirst, with_client_cache: false, played_media: [], full_media: true, limited_free_scene: 0 }),
            });
        } catch { throw new MobileFeedError('network', 'Qishui mobile recommendations could not be reached'); }
        if (!response.ok) throw new MobileFeedError([401, 403, 429].includes(response.status) ? 'unavailable' : 'network', 'Qishui mobile recommendations are unavailable');
        let data;
        try { data = await response.json(); } catch { throw invalid(); }
        if (!object(data)) throw invalid();
        const code = data.status_info?.code ?? data.status_code ?? data.err_no ?? 0;
        if (!/^\d{1,12}$/.test(String(code))) throw invalid();
        if (Number(code)) throw new MobileFeedError('unavailable', 'Qishui mobile recommendations were refused', String(code));
        if (!Array.isArray(data.items) || typeof data.has_more !== 'boolean') throw invalid();
        const items = data.items.map(publicTrack).filter(Boolean);
        if (!current()) throw new MobileFeedError('unavailable', 'Qishui recommendation request was reset');
        state.started = true;
        return { items, hasMore: data.has_more };
    }
    return {
        clear() { generation++; queues.clear(); requested = false; },
        async request(operation, params = {}) {
            try {
                if (operation === 'feed_modes') return { ok: true, data: [] };
                if (operation !== 'feed') throw new MobileFeedError('unsupported', 'Unsupported Qishui mobile operation');
                if (!object(params) || (params.queue != null && !['fm', 'explore'].includes(params.queue))
                    || (params.first != null && typeof params.first !== 'boolean')) throw invalid();
                if (params.scene != null || params.preference != null) throw new MobileFeedError('unsupported', 'This Qishui mobile recommendation mode is not available');
                const limit = params.limit ?? 20;
                if (!Number.isSafeInteger(limit) || limit < 1 || limit > 100) throw invalid();
                const queue = params.queue || 'fm';
                let state = queues.get(queue);
                if (!state || params.first === true) { state = { started: false, task: Promise.resolve() }; queues.set(queue, state); }
                const mine = generation;
                const task = state.task.then(() => load({ limit }, state, mine, queue));
                state.task = task.catch(() => {});
                return { ok: true, data: await task };
            } catch (error) {
                if (error instanceof MobileFeedError) return { ok: false, error: { code: error.code, message: error.message,
                    ...(error.upstreamCode ? { upstreamCode: error.upstreamCode } : {}) } };
                return { ok: false, error: { code: 'invalid-response', message: 'Qishui mobile recommendation request failed' } };
            }
        },
    };
}
module.exports = { createMobileRecommendations };
