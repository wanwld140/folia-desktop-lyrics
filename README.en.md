# Folia Desktop Lyrics · Unofficial Derivative

This is an **unofficial derivative** of [Folia by chthollyphile](https://github.com/chthollyphile/folia-major), maintained by [wanwld140](https://github.com/wanwld140). Original contributor credits are preserved in [CONTRIBUTORS.md](CONTRIBUTORS.md). This repository is independent of the official project.

[简体中文](README.md)

## Changes in this version

- Display Folia's complete animated lyrics behind Windows desktop icons.
- Show current and next lyrics in a taskbar card, with scrolling text, artwork, and playback controls.
- Exit wallpaper mode through the taskbar card menu or system tray. Clicking the tray icon, opening the player from the card, or launching the app again restores the ordinary player.
- Revise Windows rendering to address black screens and flicker, remove the persistent recovery window, and include the tray icon resources.

Enable these features in **Settings → Desktop**. **Ctrl+Alt+Shift+F12** restores the ordinary window from wallpaper mode. Taskbar lyrics follow playback inside Folia.

## Build

Use Node.js 24+ and, for the Windows taskbar helper, the .NET 10 SDK.

```powershell
npm ci
npm run build:taskbar-lyrics
npm run dev:electron
```

For packaging, run `npm run build:electron -- --win --publish never`. The upstream build scripts download native runtimes and model files; network access and platform toolchains are required.

## Download

Download Windows x64 builds from [Releases](https://github.com/wanwld140/folia-desktop-lyrics/releases). Use the `.exe` installer, or extract the `.zip` and launch `FoliaDesktopLyrics.exe`. Required runtimes and the taskbar helper are bundled.

## Attribution and license

Based on upstream commit `d695db52a313abe2077ce4c2e5894cc4a82190f1`; derivative changes dated 2026-11-01. The original [GNU AGPL v3 license](LICENSE), author credits, and contributor list are retained. See [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md) for the taskbar component and its AgentSpeed MIT notice.

The upstream README also contains personal-study, non-commercial-use, and online-content statements; see its [legal section](https://github.com/chthollyphile/folia-major/blob/main/README.en.md#legal-and-disclaimer). This reference preserves their source without resolving differences from the license text. Streams, lyrics, and artwork remain the property of their respective rights holders.
