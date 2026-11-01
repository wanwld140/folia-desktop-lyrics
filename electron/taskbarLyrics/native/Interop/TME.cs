// TME supports Folia's native taskbar lyric card.
namespace TaskbarMusic.Interop;

internal static class TME
{
	public const uint HOVER = 1u;

	public const uint LEAVE = 2u;

	public const uint CANCEL = 2147483648u;

	public const uint HOVER_DEFAULT = uint.MaxValue;
}
