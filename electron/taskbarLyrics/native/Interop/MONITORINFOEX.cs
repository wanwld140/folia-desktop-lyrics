// MONITORINFOEX supports Folia's native taskbar lyric card.
namespace TaskbarMusic.Interop;

internal struct MONITORINFOEX
{
	public uint cbSize;

	public RECT rcMonitor;

	public RECT rcWork;

	public uint dwFlags;

	public unsafe fixed char szDevice[32];
}
