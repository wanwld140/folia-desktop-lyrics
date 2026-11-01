// RECT supports Folia's native taskbar lyric card.
namespace TaskbarMusic.Interop;

internal struct RECT(int left, int top, int right, int bottom)
{
	public int Left = left;

	public int Top = top;

	public int Right = right;

	public int Bottom = bottom;

	public readonly int Width => Right - Left;

	public readonly int Height => Bottom - Top;

	public readonly bool IsEmpty
	{
		get
		{
			if (Width > 0)
			{
				return Height <= 0;
			}
			return true;
		}
	}

	public override readonly string ToString()
	{
		return $"[{Left},{Top},{Right},{Bottom}]";
	}
}
