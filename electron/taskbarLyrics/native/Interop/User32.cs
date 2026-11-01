using System;
using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

// User32 supports Folia's native taskbar lyric card.
namespace TaskbarMusic.Interop;

internal static class User32
{
	private const string Dll = "user32.dll";

	[LibraryImport("user32.dll", EntryPoint = "RegisterClassExW", SetLastError = true)]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	internal unsafe static ushort RegisterClassEx(in WNDCLASSEX lpwcx)
	{
		ushort result;
		int lastSystemError;
		fixed (WNDCLASSEX* _lpwcx_native = &lpwcx)
		{
			Marshal.SetLastSystemError(0);
			result = __PInvoke(_lpwcx_native);
			lastSystemError = Marshal.GetLastSystemError();
		}
		Marshal.SetLastPInvokeError(lastSystemError);
		return result;
		[DllImport("user32.dll", EntryPoint = "RegisterClassExW", ExactSpelling = true)]
		static extern unsafe ushort __PInvoke(WNDCLASSEX* __lpwcx_native);
	}

	[LibraryImport("user32.dll", EntryPoint = "UnregisterClassW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal unsafe static bool UnregisterClass(string lpClassName, nint hInstance)
	{
		int num;
		int lastSystemError;
		fixed (char* ptr = &Utf16StringMarshaller.GetPinnableReference(lpClassName))
		{
			void* _lpClassName_native = ptr;
			Marshal.SetLastSystemError(0);
			num = __PInvoke((ushort*)_lpClassName_native, hInstance);
			lastSystemError = Marshal.GetLastSystemError();
		}
		bool result = num != 0;
		Marshal.SetLastPInvokeError(lastSystemError);
		return result;
		[DllImport("user32.dll", EntryPoint = "UnregisterClassW", ExactSpelling = true)]
		static extern unsafe int __PInvoke(ushort* __lpClassName_native, nint __hInstance_native);
	}

	[LibraryImport("user32.dll", EntryPoint = "CreateWindowExW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	internal unsafe static nint CreateWindowEx(int dwExStyle, string lpClassName, string? lpWindowName, int dwStyle, int x, int y, int nWidth, int nHeight, nint hWndParent, nint hMenu, nint hInstance, nint lpParam)
	{
		nint result;
		int lastSystemError;
		fixed (char* ptr = &Utf16StringMarshaller.GetPinnableReference(lpWindowName))
		{
			void* _lpWindowName_native = ptr;
			fixed (char* ptr2 = &Utf16StringMarshaller.GetPinnableReference(lpClassName))
			{
				void* _lpClassName_native = ptr2;
				Marshal.SetLastSystemError(0);
				result = __PInvoke(dwExStyle, (ushort*)_lpClassName_native, (ushort*)_lpWindowName_native, dwStyle, x, y, nWidth, nHeight, hWndParent, hMenu, hInstance, lpParam);
				lastSystemError = Marshal.GetLastSystemError();
			}
		}
		Marshal.SetLastPInvokeError(lastSystemError);
		return result;
		[DllImport("user32.dll", EntryPoint = "CreateWindowExW", ExactSpelling = true)]
		static extern unsafe nint __PInvoke(int __dwExStyle_native, ushort* __lpClassName_native, ushort* __lpWindowName_native, int __dwStyle_native, int __x_native, int __y_native, int __nWidth_native, int __nHeight_native, nint __hWndParent_native, nint __hMenu_native, nint __hInstance_native, nint __lpParam_native);
	}

	[LibraryImport("user32.dll", SetLastError = true)]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static bool DestroyWindow(nint hWnd)
	{
		Marshal.SetLastSystemError(0);
		int num = __PInvoke(hWnd);
		int lastSystemError = Marshal.GetLastSystemError();
		bool result = num != 0;
		Marshal.SetLastPInvokeError(lastSystemError);
		return result;
		[DllImport("user32.dll", EntryPoint = "DestroyWindow", ExactSpelling = true)]
		static extern int __PInvoke(nint __hWnd_native);
	}

	[DllImport("user32.dll", EntryPoint = "DefWindowProcW", ExactSpelling = true)]
	[LibraryImport("user32.dll", EntryPoint = "DefWindowProcW")]
	internal static extern nint DefWindowProc(nint hWnd, uint msg, nuint wParam, nint lParam);

	[LibraryImport("user32.dll", SetLastError = true)]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static bool SetWindowPos(nint hWnd, nint hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags)
	{
		Marshal.SetLastSystemError(0);
		int num = __PInvoke(hWnd, hWndInsertAfter, x, y, cx, cy, uFlags);
		int lastSystemError = Marshal.GetLastSystemError();
		bool result = num != 0;
		Marshal.SetLastPInvokeError(lastSystemError);
		return result;
		[DllImport("user32.dll", EntryPoint = "SetWindowPos", ExactSpelling = true)]
		static extern int __PInvoke(nint __hWnd_native, nint __hWndInsertAfter_native, int __x_native, int __y_native, int __cx_native, int __cy_native, uint __uFlags_native);
	}

	[LibraryImport("user32.dll", SetLastError = true)]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	internal static nint BeginDeferWindowPos(int nNumWindows)
	{
		Marshal.SetLastSystemError(0);
		nint result = __PInvoke(nNumWindows);
		Marshal.SetLastPInvokeError(Marshal.GetLastSystemError());
		return result;
		[DllImport("user32.dll", EntryPoint = "BeginDeferWindowPos", ExactSpelling = true)]
		static extern nint __PInvoke(int __nNumWindows_native);
	}

	[LibraryImport("user32.dll", SetLastError = true)]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	internal static nint DeferWindowPos(nint hWinPosInfo, nint hWnd, nint hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags)
	{
		Marshal.SetLastSystemError(0);
		nint result = __PInvoke(hWinPosInfo, hWnd, hWndInsertAfter, x, y, cx, cy, uFlags);
		Marshal.SetLastPInvokeError(Marshal.GetLastSystemError());
		return result;
		[DllImport("user32.dll", EntryPoint = "DeferWindowPos", ExactSpelling = true)]
		static extern nint __PInvoke(nint __hWinPosInfo_native, nint __hWnd_native, nint __hWndInsertAfter_native, int __x_native, int __y_native, int __cx_native, int __cy_native, uint __uFlags_native);
	}

	[LibraryImport("user32.dll", SetLastError = true)]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static bool EndDeferWindowPos(nint hWinPosInfo)
	{
		Marshal.SetLastSystemError(0);
		int num = __PInvoke(hWinPosInfo);
		int lastSystemError = Marshal.GetLastSystemError();
		bool result = num != 0;
		Marshal.SetLastPInvokeError(lastSystemError);
		return result;
		[DllImport("user32.dll", EntryPoint = "EndDeferWindowPos", ExactSpelling = true)]
		static extern int __PInvoke(nint __hWinPosInfo_native);
	}

	[LibraryImport("user32.dll", SetLastError = true)]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal unsafe static bool GetWindowRect(nint hWnd, out RECT lpRect)
	{
		lpRect = default(RECT);
		int num;
		int lastSystemError;
		fixed (RECT* _lpRect_native = &lpRect)
		{
			Marshal.SetLastSystemError(0);
			num = __PInvoke(hWnd, _lpRect_native);
			lastSystemError = Marshal.GetLastSystemError();
		}
		bool result = num != 0;
		Marshal.SetLastPInvokeError(lastSystemError);
		return result;
		[DllImport("user32.dll", EntryPoint = "GetWindowRect", ExactSpelling = true)]
		static extern unsafe int __PInvoke(nint __hWnd_native, RECT* __lpRect_native);
	}

	[LibraryImport("user32.dll", SetLastError = true)]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal unsafe static bool GetClientRect(nint hWnd, out RECT lpRect)
	{
		lpRect = default(RECT);
		int num;
		int lastSystemError;
		fixed (RECT* _lpRect_native = &lpRect)
		{
			Marshal.SetLastSystemError(0);
			num = __PInvoke(hWnd, _lpRect_native);
			lastSystemError = Marshal.GetLastSystemError();
		}
		bool result = num != 0;
		Marshal.SetLastPInvokeError(lastSystemError);
		return result;
		[DllImport("user32.dll", EntryPoint = "GetClientRect", ExactSpelling = true)]
		static extern unsafe int __PInvoke(nint __hWnd_native, RECT* __lpRect_native);
	}

	[LibraryImport("user32.dll", SetLastError = true)]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	internal unsafe static int MapWindowPoints(nint hWndFrom, nint hWndTo, ref RECT lpPoints, uint cPoints)
	{
		int result;
		int lastSystemError;
		fixed (RECT* _lpPoints_native = &lpPoints)
		{
			Marshal.SetLastSystemError(0);
			result = __PInvoke(hWndFrom, hWndTo, _lpPoints_native, cPoints);
			lastSystemError = Marshal.GetLastSystemError();
		}
		Marshal.SetLastPInvokeError(lastSystemError);
		return result;
		[DllImport("user32.dll", EntryPoint = "MapWindowPoints", ExactSpelling = true)]
		static extern unsafe int __PInvoke(nint __hWndFrom_native, nint __hWndTo_native, RECT* __lpPoints_native, uint __cPoints_native);
	}

	[LibraryImport("user32.dll", EntryPoint = "FindWindowW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	internal unsafe static nint FindWindow(string? lpClassName, string? lpWindowName)
	{
		nint result;
		int lastSystemError;
		fixed (char* ptr = &Utf16StringMarshaller.GetPinnableReference(lpWindowName))
		{
			void* _lpWindowName_native = ptr;
			fixed (char* ptr2 = &Utf16StringMarshaller.GetPinnableReference(lpClassName))
			{
				void* _lpClassName_native = ptr2;
				Marshal.SetLastSystemError(0);
				result = __PInvoke((ushort*)_lpClassName_native, (ushort*)_lpWindowName_native);
				lastSystemError = Marshal.GetLastSystemError();
			}
		}
		Marshal.SetLastPInvokeError(lastSystemError);
		return result;
		[DllImport("user32.dll", EntryPoint = "FindWindowW", ExactSpelling = true)]
		static extern unsafe nint __PInvoke(ushort* __lpClassName_native, ushort* __lpWindowName_native);
	}

	[LibraryImport("user32.dll", EntryPoint = "FindWindowExW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	internal unsafe static nint FindWindowEx(nint hWndParent, nint hWndChildAfter, string? lpszClass, string? lpszWindow)
	{
		nint result;
		int lastSystemError;
		fixed (char* ptr = &Utf16StringMarshaller.GetPinnableReference(lpszWindow))
		{
			void* _lpszWindow_native = ptr;
			fixed (char* ptr2 = &Utf16StringMarshaller.GetPinnableReference(lpszClass))
			{
				void* _lpszClass_native = ptr2;
				Marshal.SetLastSystemError(0);
				result = __PInvoke(hWndParent, hWndChildAfter, (ushort*)_lpszClass_native, (ushort*)_lpszWindow_native);
				lastSystemError = Marshal.GetLastSystemError();
			}
		}
		Marshal.SetLastPInvokeError(lastSystemError);
		return result;
		[DllImport("user32.dll", EntryPoint = "FindWindowExW", ExactSpelling = true)]
		static extern unsafe nint __PInvoke(nint __hWndParent_native, nint __hWndChildAfter_native, ushort* __lpszClass_native, ushort* __lpszWindow_native);
	}

	[LibraryImport("user32.dll")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static bool IsWindow(nint hWnd)
	{
		return __PInvoke(hWnd) != 0;
		[DllImport("user32.dll", EntryPoint = "IsWindow", ExactSpelling = true)]
		static extern int __PInvoke(nint __hWnd_native);
	}

	[LibraryImport("user32.dll")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static bool IsWindowVisible(nint hWnd)
	{
		return __PInvoke(hWnd) != 0;
		[DllImport("user32.dll", EntryPoint = "IsWindowVisible", ExactSpelling = true)]
		static extern int __PInvoke(nint __hWnd_native);
	}

	[LibraryImport("user32.dll", EntryPoint = "GetWindowLongPtrW", SetLastError = true)]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	private static nint GetWindowLongPtr64(nint hWnd, int nIndex)
	{
		Marshal.SetLastSystemError(0);
		nint result = __PInvoke(hWnd, nIndex);
		Marshal.SetLastPInvokeError(Marshal.GetLastSystemError());
		return result;
		[DllImport("user32.dll", EntryPoint = "GetWindowLongPtrW", ExactSpelling = true)]
		static extern nint __PInvoke(nint __hWnd_native, int __nIndex_native);
	}

	[LibraryImport("user32.dll", EntryPoint = "GetWindowLongW", SetLastError = true)]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	private static int GetWindowLong32(nint hWnd, int nIndex)
	{
		Marshal.SetLastSystemError(0);
		int result = __PInvoke(hWnd, nIndex);
		Marshal.SetLastPInvokeError(Marshal.GetLastSystemError());
		return result;
		[DllImport("user32.dll", EntryPoint = "GetWindowLongW", ExactSpelling = true)]
		static extern int __PInvoke(nint __hWnd_native, int __nIndex_native);
	}

	[LibraryImport("user32.dll", EntryPoint = "SetWindowLongPtrW", SetLastError = true)]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	private static nint SetWindowLongPtr64(nint hWnd, int nIndex, nint dwNewLong)
	{
		Marshal.SetLastSystemError(0);
		nint result = __PInvoke(hWnd, nIndex, dwNewLong);
		Marshal.SetLastPInvokeError(Marshal.GetLastSystemError());
		return result;
		[DllImport("user32.dll", EntryPoint = "SetWindowLongPtrW", ExactSpelling = true)]
		static extern nint __PInvoke(nint __hWnd_native, int __nIndex_native, nint __dwNewLong_native);
	}

	[LibraryImport("user32.dll", EntryPoint = "SetWindowLongW", SetLastError = true)]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	private static int SetWindowLong32(nint hWnd, int nIndex, int dwNewLong)
	{
		Marshal.SetLastSystemError(0);
		int result = __PInvoke(hWnd, nIndex, dwNewLong);
		Marshal.SetLastPInvokeError(Marshal.GetLastSystemError());
		return result;
		[DllImport("user32.dll", EntryPoint = "SetWindowLongW", ExactSpelling = true)]
		static extern int __PInvoke(nint __hWnd_native, int __nIndex_native, int __dwNewLong_native);
	}

	internal static nint GetWindowLongPtr(nint hWnd, int nIndex)
	{
		if (IntPtr.Size != 8)
		{
			return GetWindowLong32(hWnd, nIndex);
		}
		return GetWindowLongPtr64(hWnd, nIndex);
	}

	internal static bool TrySetWindowLongPtr(nint hWnd, int nIndex, nint value)
	{
		Marshal.SetLastPInvokeError(0);
		if (((IntPtr.Size == 8) ? SetWindowLongPtr64(hWnd, nIndex, value) : SetWindowLong32(hWnd, nIndex, (int)value)) == 0)
		{
			return Marshal.GetLastPInvokeError() == 0;
		}
		return true;
	}

	[LibraryImport("user32.dll", SetLastError = true)]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	internal static nint SetParent(nint hWndChild, nint hWndNewParent)
	{
		Marshal.SetLastSystemError(0);
		nint result = __PInvoke(hWndChild, hWndNewParent);
		Marshal.SetLastPInvokeError(Marshal.GetLastSystemError());
		return result;
		[DllImport("user32.dll", EntryPoint = "SetParent", ExactSpelling = true)]
		static extern nint __PInvoke(nint __hWndChild_native, nint __hWndNewParent_native);
	}

	[DllImport("user32.dll", ExactSpelling = true)]
	[LibraryImport("user32.dll")]
	internal static extern nint GetParent(nint hWnd);

	[DllImport("user32.dll", ExactSpelling = true)]
	[LibraryImport("user32.dll")]
	internal static extern nint GetAncestor(nint hWnd, uint gaFlags);

	[DllImport("user32.dll", ExactSpelling = true)]
	[LibraryImport("user32.dll")]
	internal static extern nint GetWindow(nint hWnd, uint uCmd);

	[LibraryImport("user32.dll", SetLastError = true)]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal unsafe static bool UpdateLayeredWindow(nint hWnd, nint hdcDst, POINT* pptDst, SIZE* psize, nint hdcSrc, POINT* pptSrc, uint crKey, BLENDFUNCTION* pblend, uint dwFlags)
	{
		Marshal.SetLastSystemError(0);
		int num = __PInvoke(hWnd, hdcDst, pptDst, psize, hdcSrc, pptSrc, crKey, pblend, dwFlags);
		int lastSystemError = Marshal.GetLastSystemError();
		bool result = num != 0;
		Marshal.SetLastPInvokeError(lastSystemError);
		return result;
		[DllImport("user32.dll", EntryPoint = "UpdateLayeredWindow", ExactSpelling = true)]
		static extern unsafe int __PInvoke(nint __hWnd_native, nint __hdcDst_native, POINT* __pptDst_native, SIZE* __psize_native, nint __hdcSrc_native, POINT* __pptSrc_native, uint __crKey_native, BLENDFUNCTION* __pblend_native, uint __dwFlags_native);
	}

	[DllImport("user32.dll", ExactSpelling = true)]
	[LibraryImport("user32.dll")]
	internal static extern nint GetDC(nint hWnd);

	[DllImport("user32.dll", ExactSpelling = true)]
	[LibraryImport("user32.dll")]
	internal static extern int ReleaseDC(nint hWnd, nint hDC);

	[LibraryImport("user32.dll", SetLastError = true)]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal unsafe static bool TrackMouseEvent(ref TRACKMOUSEEVENT lpEventTrack)
	{
		int num;
		int lastSystemError;
		fixed (TRACKMOUSEEVENT* _lpEventTrack_native = &lpEventTrack)
		{
			Marshal.SetLastSystemError(0);
			num = __PInvoke(_lpEventTrack_native);
			lastSystemError = Marshal.GetLastSystemError();
		}
		bool result = num != 0;
		Marshal.SetLastPInvokeError(lastSystemError);
		return result;
		[DllImport("user32.dll", EntryPoint = "TrackMouseEvent", ExactSpelling = true)]
		static extern unsafe int __PInvoke(TRACKMOUSEEVENT* __lpEventTrack_native);
	}

	[LibraryImport("user32.dll", SetLastError = true)]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal unsafe static bool GetCursorPos(out POINT lpPoint)
	{
		lpPoint = default(POINT);
		int num;
		int lastSystemError;
		fixed (POINT* _lpPoint_native = &lpPoint)
		{
			Marshal.SetLastSystemError(0);
			num = __PInvoke(_lpPoint_native);
			lastSystemError = Marshal.GetLastSystemError();
		}
		bool result = num != 0;
		Marshal.SetLastPInvokeError(lastSystemError);
		return result;
		[DllImport("user32.dll", EntryPoint = "GetCursorPos", ExactSpelling = true)]
		static extern unsafe int __PInvoke(POINT* __lpPoint_native);
	}

	[DllImport("user32.dll", ExactSpelling = true)]
	[LibraryImport("user32.dll")]
	internal static extern short GetAsyncKeyState(int vKey);

	[DllImport("user32.dll", ExactSpelling = true)]
	[LibraryImport("user32.dll")]
	internal static extern uint GetDpiForWindow(nint hWnd);

	[DllImport("user32.dll", ExactSpelling = true)]
	[LibraryImport("user32.dll")]
	internal static extern int GetSystemMetricsForDpi(int nIndex, uint dpi);

	[DllImport("user32.dll", ExactSpelling = true)]
	[LibraryImport("user32.dll")]
	internal static extern nint MonitorFromPoint(POINT pt, uint dwFlags);

	[DllImport("user32.dll", ExactSpelling = true)]
	[LibraryImport("user32.dll")]
	internal static extern nint MonitorFromWindow(nint hWnd, uint dwFlags);

	[LibraryImport("user32.dll")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	internal unsafe static nint MonitorFromRect(in RECT lprc, uint dwFlags)
	{
		nint result;
		fixed (RECT* _lprc_native = &lprc)
		{
			result = __PInvoke(_lprc_native, dwFlags);
		}
		return result;
		[DllImport("user32.dll", EntryPoint = "MonitorFromRect", ExactSpelling = true)]
		static extern unsafe nint __PInvoke(RECT* __lprc_native, uint __dwFlags_native);
	}

	[LibraryImport("user32.dll", EntryPoint = "GetMonitorInfoW")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal unsafe static bool GetMonitorInfo(nint hMonitor, ref MONITORINFOEX lpmi)
	{
		int num;
		fixed (MONITORINFOEX* _lpmi_native = &lpmi)
		{
			num = __PInvoke(hMonitor, _lpmi_native);
		}
		return num != 0;
		[DllImport("user32.dll", EntryPoint = "GetMonitorInfoW", ExactSpelling = true)]
		static extern unsafe int __PInvoke(nint __hMonitor_native, MONITORINFOEX* __lpmi_native);
	}

	[LibraryImport("user32.dll", EntryPoint = "EnumDisplaySettingsW", StringMarshalling = StringMarshalling.Utf16)]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal unsafe static bool EnumDisplaySettings(string? lpszDeviceName, int iModeNum, ref DEVMODEW lpDevMode)
	{
		int num;
		fixed (DEVMODEW* _lpDevMode_native = &lpDevMode)
		{
			fixed (char* ptr = &Utf16StringMarshaller.GetPinnableReference(lpszDeviceName))
			{
				void* _lpszDeviceName_native = ptr;
				num = __PInvoke((ushort*)_lpszDeviceName_native, iModeNum, _lpDevMode_native);
			}
		}
		return num != 0;
		[DllImport("user32.dll", EntryPoint = "EnumDisplaySettingsW", ExactSpelling = true)]
		static extern unsafe int __PInvoke(ushort* __lpszDeviceName_native, int __iModeNum_native, DEVMODEW* __lpDevMode_native);
	}

	[LibraryImport("user32.dll")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal unsafe static bool EnumDisplayMonitors(nint hdc, nint lprcClip, delegate* unmanaged[Stdcall]<nint, nint, RECT*, nint, int> lpfnEnum, nint dwData)
	{
		return __PInvoke(hdc, lprcClip, lpfnEnum, dwData) != 0;
		[DllImport("user32.dll", EntryPoint = "EnumDisplayMonitors", ExactSpelling = true)]
		static extern unsafe int __PInvoke(nint __hdc_native, nint __lprcClip_native, delegate* unmanaged[Stdcall]<nint, nint, RECT*, nint, int> __lpfnEnum_native, nint __dwData_native);
	}

	[LibraryImport("user32.dll", EntryPoint = "RegisterWindowMessageW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	internal unsafe static uint RegisterWindowMessage(string lpString)
	{
		uint result;
		int lastSystemError;
		fixed (char* ptr = &Utf16StringMarshaller.GetPinnableReference(lpString))
		{
			void* _lpString_native = ptr;
			Marshal.SetLastSystemError(0);
			result = __PInvoke((ushort*)_lpString_native);
			lastSystemError = Marshal.GetLastSystemError();
		}
		Marshal.SetLastPInvokeError(lastSystemError);
		return result;
		[DllImport("user32.dll", EntryPoint = "RegisterWindowMessageW", ExactSpelling = true)]
		static extern unsafe uint __PInvoke(ushort* __lpString_native);
	}

	[LibraryImport("user32.dll")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static bool SetForegroundWindow(nint hWnd)
	{
		return __PInvoke(hWnd) != 0;
		[DllImport("user32.dll", EntryPoint = "SetForegroundWindow", ExactSpelling = true)]
		static extern int __PInvoke(nint __hWnd_native);
	}

	[DllImport("user32.dll", ExactSpelling = true)]
	[LibraryImport("user32.dll")]
	internal static extern nint GetForegroundWindow();

	[LibraryImport("user32.dll", SetLastError = true)]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static bool AllowSetForegroundWindow(int dwProcessId)
	{
		Marshal.SetLastSystemError(0);
		int num = __PInvoke(dwProcessId);
		int lastSystemError = Marshal.GetLastSystemError();
		bool result = num != 0;
		Marshal.SetLastPInvokeError(lastSystemError);
		return result;
		[DllImport("user32.dll", EntryPoint = "AllowSetForegroundWindow", ExactSpelling = true)]
		static extern int __PInvoke(int __dwProcessId_native);
	}

	[LibraryImport("user32.dll", SetLastError = true)]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	internal unsafe static uint GetWindowThreadProcessId(nint hWnd, out uint lpdwProcessId)
	{
		lpdwProcessId = 0u;
		uint result;
		int lastSystemError;
		fixed (uint* _lpdwProcessId_native = &lpdwProcessId)
		{
			Marshal.SetLastSystemError(0);
			result = __PInvoke(hWnd, _lpdwProcessId_native);
			lastSystemError = Marshal.GetLastSystemError();
		}
		Marshal.SetLastPInvokeError(lastSystemError);
		return result;
		[DllImport("user32.dll", EntryPoint = "GetWindowThreadProcessId", ExactSpelling = true)]
		static extern unsafe uint __PInvoke(nint __hWnd_native, uint* __lpdwProcessId_native);
	}

	[LibraryImport("user32.dll", SetLastError = true)]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static bool ChangeWindowMessageFilterEx(nint hwnd, uint message, uint action, nint pChangeFilterStruct)
	{
		Marshal.SetLastSystemError(0);
		int num = __PInvoke(hwnd, message, action, pChangeFilterStruct);
		int lastSystemError = Marshal.GetLastSystemError();
		bool result = num != 0;
		Marshal.SetLastPInvokeError(lastSystemError);
		return result;
		[DllImport("user32.dll", EntryPoint = "ChangeWindowMessageFilterEx", ExactSpelling = true)]
		static extern int __PInvoke(nint __hwnd_native, uint __message_native, uint __action_native, nint __pChangeFilterStruct_native);
	}

	[LibraryImport("user32.dll", EntryPoint = "GetClassNameW", SetLastError = true)]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	internal unsafe static int GetClassName(nint hWnd, char* lpClassName, int nMaxCount)
	{
		Marshal.SetLastSystemError(0);
		int result = __PInvoke(hWnd, lpClassName, nMaxCount);
		Marshal.SetLastPInvokeError(Marshal.GetLastSystemError());
		return result;
		[DllImport("user32.dll", EntryPoint = "GetClassNameW", ExactSpelling = true)]
		static extern unsafe int __PInvoke(nint __hWnd_native, char* __lpClassName_native, int __nMaxCount_native);
	}

	[LibraryImport("user32.dll", EntryPoint = "PostMessageW", SetLastError = true)]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static bool PostMessage(nint hWnd, uint msg, nuint wParam, nint lParam)
	{
		Marshal.SetLastSystemError(0);
		int num = __PInvoke(hWnd, msg, wParam, lParam);
		int lastSystemError = Marshal.GetLastSystemError();
		bool result = num != 0;
		Marshal.SetLastPInvokeError(lastSystemError);
		return result;
		[DllImport("user32.dll", EntryPoint = "PostMessageW", ExactSpelling = true)]
		static extern int __PInvoke(nint __hWnd_native, uint __msg_native, nuint __wParam_native, nint __lParam_native);
	}

	[DllImport("user32.dll", ExactSpelling = true)]
	[LibraryImport("user32.dll")]
	internal static extern nint GetShellWindow();

	[DllImport("user32.dll", ExactSpelling = true)]
	[LibraryImport("user32.dll")]
	internal static extern nint GetDesktopWindow();

	[LibraryImport("user32.dll", SetLastError = true)]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static bool DestroyIcon(nint hIcon)
	{
		Marshal.SetLastSystemError(0);
		int num = __PInvoke(hIcon);
		int lastSystemError = Marshal.GetLastSystemError();
		bool result = num != 0;
		Marshal.SetLastPInvokeError(lastSystemError);
		return result;
		[DllImport("user32.dll", EntryPoint = "DestroyIcon", ExactSpelling = true)]
		static extern int __PInvoke(nint __hIcon_native);
	}

	[LibraryImport("user32.dll", SetLastError = true)]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	internal unsafe static nint CreateIconIndirect(in ICONINFO piconinfo)
	{
		nint result;
		int lastSystemError;
		fixed (ICONINFO* _piconinfo_native = &piconinfo)
		{
			Marshal.SetLastSystemError(0);
			result = __PInvoke(_piconinfo_native);
			lastSystemError = Marshal.GetLastSystemError();
		}
		Marshal.SetLastPInvokeError(lastSystemError);
		return result;
		[DllImport("user32.dll", EntryPoint = "CreateIconIndirect", ExactSpelling = true)]
		static extern unsafe nint __PInvoke(ICONINFO* __piconinfo_native);
	}

	[LibraryImport("user32.dll", EntryPoint = "PrivateExtractIconsW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	internal unsafe static uint PrivateExtractIcons(string szFileName, int nIconIndex, int cxIcon, int cyIcon, nint* phicon, uint* piconid, uint nIcons, uint flags)
	{
		uint result;
		int lastSystemError;
		fixed (char* ptr = &Utf16StringMarshaller.GetPinnableReference(szFileName))
		{
			void* _szFileName_native = ptr;
			Marshal.SetLastSystemError(0);
			result = __PInvoke((ushort*)_szFileName_native, nIconIndex, cxIcon, cyIcon, phicon, piconid, nIcons, flags);
			lastSystemError = Marshal.GetLastSystemError();
		}
		Marshal.SetLastPInvokeError(lastSystemError);
		return result;
		[DllImport("user32.dll", EntryPoint = "PrivateExtractIconsW", ExactSpelling = true)]
		static extern unsafe uint __PInvoke(ushort* __szFileName_native, int __nIconIndex_native, int __cxIcon_native, int __cyIcon_native, nint* __phicon_native, uint* __piconid_native, uint __nIcons_native, uint __flags_native);
	}

	[DllImport("user32.dll", EntryPoint = "LoadCursorW", ExactSpelling = true)]
	[LibraryImport("user32.dll", EntryPoint = "LoadCursorW")]
	internal static extern nint LoadCursor(nint hInstance, nint lpCursorName);

	[LibraryImport("user32.dll", EntryPoint = "SetWindowTextW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal unsafe static bool SetWindowText(nint hWnd, string lpString)
	{
		int num;
		int lastSystemError;
		fixed (char* ptr = &Utf16StringMarshaller.GetPinnableReference(lpString))
		{
			void* _lpString_native = ptr;
			Marshal.SetLastSystemError(0);
			num = __PInvoke(hWnd, (ushort*)_lpString_native);
			lastSystemError = Marshal.GetLastSystemError();
		}
		bool result = num != 0;
		Marshal.SetLastPInvokeError(lastSystemError);
		return result;
		[DllImport("user32.dll", EntryPoint = "SetWindowTextW", ExactSpelling = true)]
		static extern unsafe int __PInvoke(nint __hWnd_native, ushort* __lpString_native);
	}

	[LibraryImport("user32.dll")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static bool ShowWindow(nint hWnd, int nCmdShow)
	{
		return __PInvoke(hWnd, nCmdShow) != 0;
		[DllImport("user32.dll", EntryPoint = "ShowWindow", ExactSpelling = true)]
		static extern int __PInvoke(nint __hWnd_native, int __nCmdShow_native);
	}

	[DllImport("user32.dll", ExactSpelling = true)]
	[LibraryImport("user32.dll")]
	internal unsafe static extern nint SetWinEventHook(uint eventMin, uint eventMax, nint hmodWinEventProc, delegate* unmanaged[Stdcall]<nint, uint, nint, int, int, uint, uint, void> pfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

	[LibraryImport("user32.dll")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static bool UnhookWinEvent(nint hWinEventHook)
	{
		return __PInvoke(hWinEventHook) != 0;
		[DllImport("user32.dll", EntryPoint = "UnhookWinEvent", ExactSpelling = true)]
		static extern int __PInvoke(nint __hWinEventHook_native);
	}

	internal unsafe static string GetClassName(nint hWnd)
	{
		char* ptr = stackalloc char[256];
		int className = GetClassName(hWnd, ptr, 256);
		if (className <= 0)
		{
			return string.Empty;
		}
		return new string(ptr, 0, className);
	}
}
