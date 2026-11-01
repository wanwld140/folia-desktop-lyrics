// WNDCLASSEX supports Folia's native taskbar lyric card.
namespace TaskbarMusic.Interop;

internal struct WNDCLASSEX
{
	public uint cbSize;

	public uint style;

	public unsafe delegate* unmanaged[Stdcall]<nint, uint, nuint, nint, nint> lpfnWndProc;

	public int cbClsExtra;

	public int cbWndExtra;

	public nint hInstance;

	public nint hIcon;

	public nint hCursor;

	public nint hbrBackground;

	public unsafe char* lpszMenuName;

	public unsafe char* lpszClassName;

	public nint hIconSm;
}
