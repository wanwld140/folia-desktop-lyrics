// SIZE supports Folia's native taskbar lyric card.
namespace TaskbarMusic.Interop;

internal struct SIZE(int width, int height)
{
	public int cx = width;

	public int cy = height;
}
