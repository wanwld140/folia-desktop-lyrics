// WS supports Folia's native taskbar lyric card.
namespace TaskbarMusic.Interop;

internal static class WS
{
	public const int CHILD = 1073741824;

	public const int VISIBLE = 268435456;

	public const int CLIPSIBLINGS = 67108864;

	public const int CLIPCHILDREN = 33554432;

	public const int POPUP = int.MinValue;

	public const int SYSMENU = 524288;

	public const int MINIMIZEBOX = 131072;

	public const int MAXIMIZEBOX = 65536;
}
