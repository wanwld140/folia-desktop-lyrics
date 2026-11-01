using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

// Kernel32 supports Folia's native taskbar lyric card.
namespace TaskbarMusic.Interop;

internal static class Kernel32
{
	[LibraryImport("kernel32.dll", EntryPoint = "GetModuleHandleW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.15.2326")]
	[SkipLocalsInit]
	internal unsafe static nint GetModuleHandle(string? lpModuleName)
	{
		nint result;
		int lastSystemError;
		fixed (char* ptr = &Utf16StringMarshaller.GetPinnableReference(lpModuleName))
		{
			void* _lpModuleName_native = ptr;
			Marshal.SetLastSystemError(0);
			result = __PInvoke((ushort*)_lpModuleName_native);
			lastSystemError = Marshal.GetLastSystemError();
		}
		Marshal.SetLastPInvokeError(lastSystemError);
		return result;
		[DllImport("kernel32.dll", EntryPoint = "GetModuleHandleW", ExactSpelling = true)]
		static extern unsafe nint __PInvoke(ushort* __lpModuleName_native);
	}
}
