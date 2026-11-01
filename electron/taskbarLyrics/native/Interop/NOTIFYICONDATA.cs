using System;

// NOTIFYICONDATA supports Folia's native taskbar lyric card.
namespace TaskbarMusic.Interop;

internal struct NOTIFYICONDATA
{
	public uint cbSize;

	public nint hWnd;

	public uint uID;

	public uint uFlags;

	public uint uCallbackMessage;

	public nint hIcon;

	public unsafe fixed char szTip[128];

	public uint dwState;

	public uint dwStateMask;

	public unsafe fixed char szInfo[256];

	public uint uVersion;

	public unsafe fixed char szInfoTitle[64];

	public uint dwInfoFlags;

	public Guid guidItem;

	public nint hBalloonIcon;
}
