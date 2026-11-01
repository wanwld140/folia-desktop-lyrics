using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Gdi32 supports Folia's native taskbar lyric card.
namespace TaskbarMusic.Interop;

internal static class Gdi32
{
	private const string Dll = "gdi32.dll";

	internal const uint DIB_RGB_COLORS = 0u;

	internal const uint BI_RGB = 0u;

	[LibraryImport("gdi32.dll", SetLastError = true)]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	internal static nint CreateCompatibleDC(nint hdc)
	{
		Marshal.SetLastSystemError(0);
		nint result = __PInvoke(hdc);
		Marshal.SetLastPInvokeError(Marshal.GetLastSystemError());
		return result;
		[DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC", ExactSpelling = true)]
		static extern nint __PInvoke(nint __hdc_native);
	}

	[LibraryImport("gdi32.dll")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static bool DeleteDC(nint hdc)
	{
		return __PInvoke(hdc) != 0;
		[DllImport("gdi32.dll", EntryPoint = "DeleteDC", ExactSpelling = true)]
		static extern int __PInvoke(nint __hdc_native);
	}

	[DllImport("gdi32.dll", ExactSpelling = true)]
	[LibraryImport("gdi32.dll")]
	internal static extern nint SelectObject(nint hdc, nint h);

	[LibraryImport("gdi32.dll")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static bool DeleteObject(nint ho)
	{
		return __PInvoke(ho) != 0;
		[DllImport("gdi32.dll", EntryPoint = "DeleteObject", ExactSpelling = true)]
		static extern int __PInvoke(nint __ho_native);
	}

	[LibraryImport("gdi32.dll", SetLastError = true)]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	internal unsafe static nint CreateDIBSection(nint hdc, in BITMAPINFO pbmi, uint usage, out nint ppvBits, nint hSection, uint offset)
	{
		ppvBits = 0;
		nint result;
		int lastSystemError;
		fixed (nint* _ppvBits_native = &ppvBits)
		{
			fixed (BITMAPINFO* _pbmi_native = &pbmi)
			{
				Marshal.SetLastSystemError(0);
				result = __PInvoke(hdc, _pbmi_native, usage, _ppvBits_native, hSection, offset);
				lastSystemError = Marshal.GetLastSystemError();
			}
		}
		Marshal.SetLastPInvokeError(lastSystemError);
		return result;
		[DllImport("gdi32.dll", EntryPoint = "CreateDIBSection", ExactSpelling = true)]
		static extern unsafe nint __PInvoke(nint __hdc_native, BITMAPINFO* __pbmi_native, uint __usage_native, nint* __ppvBits_native, nint __hSection_native, uint __offset_native);
	}

	[LibraryImport("gdi32.dll", SetLastError = true)]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	internal static nint CreateBitmap(int nWidth, int nHeight, uint nPlanes, uint nBitCount, nint lpBits)
	{
		Marshal.SetLastSystemError(0);
		nint result = __PInvoke(nWidth, nHeight, nPlanes, nBitCount, lpBits);
		Marshal.SetLastPInvokeError(Marshal.GetLastSystemError());
		return result;
		[DllImport("gdi32.dll", EntryPoint = "CreateBitmap", ExactSpelling = true)]
		static extern nint __PInvoke(int __nWidth_native, int __nHeight_native, uint __nPlanes_native, uint __nBitCount_native, nint __lpBits_native);
	}
}
