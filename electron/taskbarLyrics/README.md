# Windows taskbar lyrics

Enable **Settings → Desktop → Taskbar lyrics**, or search for **Taskbar lyrics** in the command palette.
The card displays the current and next lyric, scrolls long lines, and controls Folia's previous/play/pause/next actions.
Drag its cover to move between the taskbar and a floating card. The menu button and right click open Folia's tray menu at the pointer, including **Exit Wallpaper Mode**. Taskbar lyric settings are available in that menu while the ordinary player is open.

Windows wallpaper mode has no persistent control window. Exit from the taskbar card menu or the Folia tray menu; opening the player from the card, clicking the tray icon, or launching Folia again also restores the ordinary player. The emergency shortcut remains Ctrl+Alt+Shift+F12.

The renderer reuses Folia's displayed playback model and parsed lyrics (including manual offsets and preview clips).
It sends small snapshots through the isolated preload API; no HTTP server, player scanning, or external lyric lookup is enabled.
`controller.cjs` owns a versioned JSONL child process with bounded crash retries and pipe backpressure.
The helper exits on pipe EOF or parent exit, and stores only its position under Folia's userData directory.
Native card, renderer and Win32 bindings are adapted from the supplied TaskbarMusic V1 implementation. See [third-party attribution](../../THIRD_PARTY_NOTICES.md) and the retained AgentSpeed MIT notice.
The standalone TaskbarMusic executable and its settings are independent.

Build on Windows with Node 24+ and the .NET 10 SDK:

```powershell
npm ci
npm run build:taskbar-lyrics
npm run dev:electron
```

`FOLIA_DOTNET` may point to a portable `dotnet.exe`. The generated helper is self-contained;
end users do not need .NET installed. The normal Windows packaging hook builds and bundles it automatically.
The feature is off by default and available only on Windows.
