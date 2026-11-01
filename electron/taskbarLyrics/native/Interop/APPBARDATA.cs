// APPBARDATA supports Folia's native taskbar lyric card.
namespace TaskbarMusic.Interop;

internal struct APPBARDATA
{
	public uint cbSize;

	public nint hWnd;

	public uint uCallbackMessage;

	public uint uEdge;

	public RECT rc;

	public nint lParam;
}
