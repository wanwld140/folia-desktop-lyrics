// WM supports Folia's native taskbar lyric card.
namespace TaskbarMusic.Interop;

internal static class WM
{
	public const uint DESTROY = 2u;

	public const uint SIZE = 5u;

	public const uint ACTIVATE = 6u;

	public const uint SETTEXT = 12u;

	public const uint PAINT = 15u;

	public const uint CLOSE = 16u;

	public const uint QUERYENDSESSION = 17u;

	public const uint ERASEBKGND = 20u;

	public const uint ENDSESSION = 22u;

	public const uint SETTINGCHANGE = 26u;

	public const uint SETCURSOR = 32u;

	public const uint MOUSEACTIVATE = 33u;

	public const uint WINDOWPOSCHANGED = 71u;

	public const uint CONTEXTMENU = 123u;

	public const uint STYLECHANGING = 124u;

	public const uint DISPLAYCHANGE = 126u;

	public const uint NCDESTROY = 130u;

	public const uint NCHITTEST = 132u;

	public const uint KEYDOWN = 256u;

	public const uint TIMER = 275u;

	public const uint MOUSEMOVE = 512u;

	public const uint LBUTTONDOWN = 513u;

	public const uint LBUTTONUP = 514u;

	public const uint LBUTTONDBLCLK = 515u;

	public const uint RBUTTONDOWN = 516u;

	public const uint RBUTTONUP = 517u;

	public const uint MOUSEHOVER = 673u;

	public const uint MOUSELEAVE = 675u;

	public const uint DPICHANGED = 736u;

	public const uint DPICHANGED_BEFOREPARENT = 738u;

	public const uint DPICHANGED_AFTERPARENT = 739u;

	public const uint THEMECHANGED = 794u;

	public const uint DWMCOMPOSITIONCHANGED = 798u;

	public const uint USER = 1024u;

	public const uint APP = 32768u;
}
