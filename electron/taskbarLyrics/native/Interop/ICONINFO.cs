// ICONINFO supports Folia's native taskbar lyric card.
namespace TaskbarMusic.Interop;

internal struct ICONINFO
{
	public int fIcon;

	public uint xHotspot;

	public uint yHotspot;

	public nint hbmMask;

	public nint hbmColor;
}
