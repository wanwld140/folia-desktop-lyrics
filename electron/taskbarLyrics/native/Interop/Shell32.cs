using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Shell32 supports Folia's native taskbar lyric card.
namespace TaskbarMusic.Interop;

internal static class Shell32
{
	private const string Dll = "shell32.dll";

	[LibraryImport("shell32.dll", EntryPoint = "Shell_NotifyIconW")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal unsafe static bool Shell_NotifyIcon(uint dwMessage, ref NOTIFYICONDATA lpData)
	{
		int num;
		fixed (NOTIFYICONDATA* _lpData_native = &lpData)
		{
			num = __PInvoke(dwMessage, _lpData_native);
		}
		return num != 0;
		[DllImport("shell32.dll", EntryPoint = "Shell_NotifyIconW", ExactSpelling = true)]
		static extern unsafe int __PInvoke(uint __dwMessage_native, NOTIFYICONDATA* __lpData_native);
	}

	[LibraryImport("shell32.dll")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	internal unsafe static int Shell_NotifyIconGetRect(in NOTIFYICONIDENTIFIER identifier, out RECT iconLocation)
	{
		iconLocation = default(RECT);
		int result;
		fixed (RECT* _iconLocation_native = &iconLocation)
		{
			fixed (NOTIFYICONIDENTIFIER* _identifier_native = &identifier)
			{
				result = __PInvoke(_identifier_native, _iconLocation_native);
			}
		}
		return result;
		[DllImport("shell32.dll", EntryPoint = "Shell_NotifyIconGetRect", ExactSpelling = true)]
		static extern unsafe int __PInvoke(NOTIFYICONIDENTIFIER* __identifier_native, RECT* __iconLocation_native);
	}

	[LibraryImport("shell32.dll")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	internal unsafe static nuint SHAppBarMessage(uint dwMessage, ref APPBARDATA pData)
	{
		nuint result;
		fixed (APPBARDATA* _pData_native = &pData)
		{
			result = __PInvoke(dwMessage, _pData_native);
		}
		return result;
		[DllImport("shell32.dll", EntryPoint = "SHAppBarMessage", ExactSpelling = true)]
		static extern unsafe nuint __PInvoke(uint __dwMessage_native, APPBARDATA* __pData_native);
	}

	[LibraryImport("shell32.dll")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	internal unsafe static int SHQueryUserNotificationState(out int pquns)
	{
		pquns = 0;
		int result;
		fixed (int* _pquns_native = &pquns)
		{
			result = __PInvoke(_pquns_native);
		}
		return result;
		[DllImport("shell32.dll", EntryPoint = "SHQueryUserNotificationState", ExactSpelling = true)]
		static extern unsafe int __PInvoke(int* __pquns_native);
	}
}
