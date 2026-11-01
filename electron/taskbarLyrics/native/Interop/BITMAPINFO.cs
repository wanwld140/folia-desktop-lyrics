// BITMAPINFO supports Folia's native taskbar lyric card.
namespace TaskbarMusic.Interop;

internal struct BITMAPINFO
{
	public BITMAPINFOHEADER bmiHeader;

	public uint bmiColors;
}
