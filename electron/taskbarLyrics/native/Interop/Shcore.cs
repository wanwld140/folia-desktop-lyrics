using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Shcore supports Folia's native taskbar lyric card.
namespace TaskbarMusic.Interop;

internal static class Shcore
{
	internal const int MDT_EFFECTIVE_DPI = 0;

	[LibraryImport("shcore.dll")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	internal unsafe static int GetDpiForMonitor(nint hmonitor, int dpiType, out uint dpiX, out uint dpiY)
	{
		dpiX = 0u;
		dpiY = 0u;
		int result;
		fixed (uint* _dpiY_native = &dpiY)
		{
			fixed (uint* _dpiX_native = &dpiX)
			{
				result = __PInvoke(hmonitor, dpiType, _dpiX_native, _dpiY_native);
			}
		}
		return result;
		[DllImport("shcore.dll", EntryPoint = "GetDpiForMonitor", ExactSpelling = true)]
		static extern unsafe int __PInvoke(nint __hmonitor_native, int __dpiType_native, uint* __dpiX_native, uint* __dpiY_native);
	}
}
