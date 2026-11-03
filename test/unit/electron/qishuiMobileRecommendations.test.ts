import { createRequire } from 'node:module';
import { describe, expect, it, vi } from 'vitest';
import { normalizeQishuiSong } from '@/services/onlineMusic/qishui/qishuiNormalize';

const require = createRequire(import.meta.url);
const { createMobileRecommendations } = require('../../../electron/qishui/mobileRecommendations.cjs');
const row = () => ({ type: 'track', entity: { track_wrapper: { track: {
    id: '7676337516358617115', name: 'Phone song', duration: 200123,
    artists: [{ id: '42', name: 'Artist' }], album: { id: '43', name: 'Album', url_cover: { urls: ['https://p3.douyinpic.com/img/'], uri: 'cover' } },
    player_info: { video_model: 'private-model' }, lyric: { content: 'private-lyric' }, sim_id_v3: 'private-context',
} } } });
const reply = (extra = {}) => new Response(JSON.stringify({ status_info: { code: 0 }, items: [row()], has_more: true, ...extra }));

describe('Qishui phone recommendation client', () => {
    it('uses the verified phone identity and only exposes public metadata that the renderer can normalize', async () => {
        const fetch = vi.fn(async () => reply());
        const result = await createMobileRecommendations({ fetch }).request('feed', { limit: 20 });
        expect(result.ok).toBe(true);
        const song = normalizeQishuiSong(result.data.items[0]);
        expect(song).toMatchObject({ id: '7676337516358617115', name: 'Phone song', durationMs: 200123,
            artists: [{ id: '42', name: 'Artist' }], album: { id: '43', name: 'Album', coverUrl: 'https://p3.douyinpic.com/img/cover' } });
        expect(JSON.stringify(result)).not.toContain('private-');
        const [rawUrl, options] = fetch.mock.calls[0] as unknown as [string, RequestInit];
        const url = new URL(rawUrl);
        expect(url.origin + url.pathname).toBe('https://beta-luna.douyin.com/luna/feed/song-tab');
        expect(url.searchParams.get('version_code')).toBe('100214030');
        expect(url.searchParams.get('aid')).toBe('8478');
        expect([...url.searchParams.keys()]).not.toContain('device_id');
        expect(options).toMatchObject({ credentials: 'omit', redirect: 'error', headers: { 'user-agent': 'Luna/21.4.0 Android' } });
        expect(JSON.parse(options.body as string)).toMatchObject({ count: 20, feed_mode: 'track', played_media: [], full_media: true });
    });
    it('separates FM and Explore initialization and resets on explicit refresh', async () => {
        const fetch = vi.fn(async () => reply());
        const client = createMobileRecommendations({ fetch });
        await client.request('feed', { queue: 'fm' });
        await client.request('feed', { queue: 'fm' });
        await client.request('feed', { queue: 'explore' });
        await client.request('feed', { queue: 'explore', first: true });
        const bodies = fetch.mock.calls.map(call => JSON.parse((call as unknown as [string, RequestInit])[1].body as string));
        expect(bodies.map(body => body.is_first_request)).toEqual([true, false, true, true]);
        expect(bodies.map(body => body.is_did_first_request)).toEqual([true, false, false, false]);
        expect(bodies.every(body => body.played_media.length === 0)).toBe(true);
    });
    it('serializes the same queue and does not mark a failed read as initialized', async () => {
        let finish!: (value: Response) => void;
        const fetch = vi.fn().mockImplementationOnce(() => new Promise(resolve => { finish = resolve; }))
            .mockResolvedValueOnce(reply());
        const client = createMobileRecommendations({ fetch });
        const first = client.request('feed');
        const second = client.request('feed');
        await Promise.resolve();
        expect(fetch).toHaveBeenCalledTimes(1);
        finish(new Response('{}', { status: 503 }));
        expect(await first).toMatchObject({ ok: false });
        expect(await second).toMatchObject({ ok: true });
        expect(JSON.parse(fetch.mock.calls[1][1].body).is_first_request).toBe(true);
    });
    it('invalidates in-flight and queued reads when cleared', async () => {
        let finish!: (value: Response) => void;
        const fetch = vi.fn(() => new Promise(resolve => { finish = resolve; }));
        const client = createMobileRecommendations({ fetch });
        const pending = client.request('feed');
        const queued = client.request('feed');
        await Promise.resolve();
        client.clear(); finish(reply());
        expect(await pending).toMatchObject({ ok: false, error: { code: 'unavailable' } });
        expect(await queued).toMatchObject({ ok: false, error: { code: 'unavailable' } });
        expect(fetch).toHaveBeenCalledTimes(1);
    });
    it('discards superseded same-queue reads on explicit refresh', async () => {
        let finish!: (value: Response) => void;
        const fetch = vi.fn().mockImplementationOnce(() => new Promise(resolve => { finish = resolve; })).mockResolvedValueOnce(reply());
        const client = createMobileRecommendations({ fetch });
        const pending = client.request('feed', { queue: 'explore' });
        await Promise.resolve();
        expect(await client.request('feed', { queue: 'explore', first: true })).toMatchObject({ ok: true });
        finish(reply());
        expect(await pending).toMatchObject({ ok: false, error: { code: 'unavailable' } });
    });
    it('returns the upstream continuation flag and accepts valid empty/non-track responses', async () => {
        const client = createMobileRecommendations({ fetch: async () => reply({ items: [{ type: 'video' }], has_more: false }) });
        expect(await client.request('feed')).toEqual({ ok: true, data: { items: [], hasMore: false } });
    });
    it.each([null, [], { limit: 0 }, { limit: 1.2 }, { limit: 101 }, { queue: 'arbitrary' }, { first: 'yes' }])('rejects invalid params before network access (%j)', async params => {
        const fetch = vi.fn();
        expect(await createMobileRecommendations({ fetch }).request('feed', params)).toMatchObject({ ok: false, error: { code: 'invalid-response' } });
        expect(fetch).not.toHaveBeenCalled();
    });
    it('refuses unverified scene/preference modes without making a request', async () => {
        const fetch = vi.fn(), client = createMobileRecommendations({ fetch });
        for (const params of [{ scene: '33' }, { preference: 'fresh' }]) {
            expect(await client.request('feed', params)).toMatchObject({ ok: false, error: { code: 'unsupported' } });
        }
        expect(await client.request('feed_modes')).toEqual({ ok: true, data: [] });
        expect(fetch).not.toHaveBeenCalled();
    });
    it.each([{ items: null }, { has_more: 'yes' }, { items: [{ type: 'track' }] }])('rejects malformed successful envelopes (%j)', async malformed => {
        expect(await createMobileRecommendations({ fetch: async () => reply(malformed) }).request('feed'))
            .toMatchObject({ ok: false, error: { code: 'invalid-response' } });
    });
    it('rejects numeric media IDs rather than accepting rounded identities', async () => {
        const item = row(); (item.entity.track_wrapper.track.id as unknown) = 7676337516358617000;
        expect(await createMobileRecommendations({ fetch: async () => reply({ items: [item] }) }).request('feed'))
            .toMatchObject({ ok: false, error: { code: 'invalid-response' } });
    });
    it('keeps phone refusal separate from car sign-in and strips server text/network secrets', async () => {
        const fetch = vi.fn().mockResolvedValueOnce(new Response('{}', { status: 403 }))
            .mockResolvedValueOnce(reply({ status_info: { code: 1000062, message: 'private-server-text' } }))
            .mockRejectedValueOnce(new Error('private-url'));
        const client = createMobileRecommendations({ fetch });
        expect(await client.request('feed')).toMatchObject({ ok: false, error: { code: 'unavailable' } });
        const refused = await client.request('feed');
        expect(refused).toMatchObject({ ok: false, error: { code: 'unavailable', upstreamCode: '1000062' } });
        expect(JSON.stringify(refused)).not.toContain('private-');
        const failed = await client.request('feed');
        expect(failed).toMatchObject({ ok: false, error: { code: 'network' } });
        expect(JSON.stringify(failed)).not.toContain('private-');
    });
});
