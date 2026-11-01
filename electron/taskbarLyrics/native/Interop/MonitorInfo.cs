// MonitorInfo supports Folia's native taskbar lyric card.
namespace TaskbarMusic.Interop;

internal sealed record MonitorInfo(nint Handle, string DeviceName, RECT Bounds, RECT WorkArea, bool IsPrimary, uint Dpi)
{
	public double Scale => (double)Dpi / 96.0;

	public string Key => $"{DeviceName}|{Bounds.Width}x{Bounds.Height}";
}
