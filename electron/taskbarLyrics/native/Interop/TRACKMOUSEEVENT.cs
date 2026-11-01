// TRACKMOUSEEVENT supports Folia's native taskbar lyric card.
namespace TaskbarMusic.Interop;

internal struct TRACKMOUSEEVENT
{
	public uint cbSize;

	public uint dwFlags;

	public nint hwndTrack;

	public uint dwHoverTime;
}
