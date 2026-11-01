// NIF supports Folia's native taskbar lyric card.
namespace TaskbarMusic.Interop;

internal static class NIF
{
	public const uint MESSAGE = 1u;

	public const uint ICON = 2u;

	public const uint TIP = 4u;

	public const uint STATE = 8u;

	public const uint INFO = 16u;

	public const uint SHOWTIP = 128u;
}
