using TaskbarMusic.Interop;

// BarGeometry supports Folia's native taskbar lyric card.
namespace TaskbarMusic.Taskbar;

internal readonly record struct BarGeometry(nint Handle, RECT Bounds, RECT Area, double Scale, bool Vertical);
