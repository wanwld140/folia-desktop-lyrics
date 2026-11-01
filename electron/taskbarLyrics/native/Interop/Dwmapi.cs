using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Dwmapi supports Folia's native taskbar lyric card.
namespace TaskbarMusic.Interop;

internal static class Dwmapi
{
	private const string Dll = "dwmapi.dll";

	[DllImport("dwmapi.dll", ExactSpelling = true)]
	[LibraryImport("dwmapi.dll")]
	internal unsafe static extern int DwmSetWindowAttribute(nint hwnd, int dwAttribute, void* pvAttribute, int cbAttribute);

	[DllImport("dwmapi.dll", ExactSpelling = true)]
	[LibraryImport("dwmapi.dll")]
	internal unsafe static extern int DwmGetWindowAttribute(nint hwnd, int dwAttribute, void* pvAttribute, int cbAttribute);

	[LibraryImport("dwmapi.dll")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	internal unsafe static int DwmExtendFrameIntoClientArea(nint hWnd, in MARGINS pMarInset)
	{
		int result;
		fixed (MARGINS* _pMarInset_native = &pMarInset)
		{
			result = __PInvoke(hWnd, _pMarInset_native);
		}
		return result;
		[DllImport("dwmapi.dll", EntryPoint = "DwmExtendFrameIntoClientArea", ExactSpelling = true)]
		static extern unsafe int __PInvoke(nint __hWnd_native, MARGINS* __pMarInset_native);
	}
}
