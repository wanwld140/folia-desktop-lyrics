// WinEvent supports Folia's native taskbar lyric card.
namespace TaskbarMusic.Interop;

internal static class WinEvent
{
	public const uint EVENT_SYSTEM_FOREGROUND = 3u;

	public const uint EVENT_OBJECT_LOCATIONCHANGE = 32779u;

	public const int OBJID_WINDOW = 0;

	public const uint WINEVENT_OUTOFCONTEXT = 0u;

	public const uint WINEVENT_SKIPOWNPROCESS = 2u;
}
