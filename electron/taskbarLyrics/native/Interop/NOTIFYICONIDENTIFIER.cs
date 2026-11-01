using System;

// NOTIFYICONIDENTIFIER supports Folia's native taskbar lyric card.
namespace TaskbarMusic.Interop;

internal struct NOTIFYICONIDENTIFIER
{
	public uint cbSize;

	public nint hWnd;

	public uint uID;

	public Guid guidItem;
}
