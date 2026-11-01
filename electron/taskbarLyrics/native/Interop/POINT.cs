// POINT supports Folia's native taskbar lyric card.
namespace TaskbarMusic.Interop;

internal struct POINT(int x, int y)
{
	public int X = x;

	public int Y = y;
}
