// SWP supports Folia's native taskbar lyric card.
namespace TaskbarMusic.Interop;

internal static class SWP
{
	public const uint NOSIZE = 1u;

	public const uint NOMOVE = 2u;

	public const uint NOZORDER = 4u;

	public const uint NOREDRAW = 8u;

	public const uint NOACTIVATE = 16u;

	public const uint FRAMECHANGED = 32u;

	public const uint SHOWWINDOW = 64u;

	public const uint HIDEWINDOW = 128u;

	public const uint NOOWNERZORDER = 512u;

	public const uint ASYNCWINDOWPOS = 16384u;
}
