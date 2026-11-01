using System;
using System.Runtime.InteropServices;
using TaskbarMusic.Interop;

// LayeredSurface supports Folia's native taskbar lyric card.
namespace TaskbarMusic.Taskbar;

internal sealed class LayeredSurface : IDisposable
{
	private nint _memoryDc;

	private nint _bitmap;

	private nint _previousBitmap;

	public int Width { get; private set; }

	public int Height { get; private set; }

	public nint Bits { get; private set; }

	public int ByteCount => Width * Height * 4;

	public unsafe bool Ensure(int width, int height)
	{
		width = Math.Max(1, width);
		height = Math.Max(1, height);
		if (_bitmap != 0 && width == Width && height == Height)
		{
			return true;
		}
		ReleaseBitmap();
		if (_memoryDc == 0)
		{
			_memoryDc = Gdi32.CreateCompatibleDC(0);
			if (_memoryDc == 0)
			{
				return false;
			}
		}
		BITMAPINFO pbmi = new BITMAPINFO
		{
			bmiHeader = new BITMAPINFOHEADER
			{
				biSize = (uint)sizeof(BITMAPINFOHEADER),
				biWidth = width,
				biHeight = -height,
				biPlanes = 1,
				biBitCount = 32,
				biCompression = 0u
			}
		};
		_bitmap = Gdi32.CreateDIBSection(_memoryDc, in pbmi, 0u, out var ppvBits, 0, 0u);
		if (_bitmap == 0 || ppvBits == 0)
		{
			if (_bitmap != 0)
			{
				Gdi32.DeleteObject(_bitmap);
			}
			_bitmap = 0;
			return false;
		}
		_previousBitmap = Gdi32.SelectObject(_memoryDc, _bitmap);
		Bits = ppvBits;
		Width = width;
		Height = height;
		return true;
	}

	public unsafe bool Present(nint hwnd, out int error)
	{
		error = 0;
		if (_bitmap == 0 || hwnd == 0)
		{
			return false;
		}
		SIZE sIZE = new SIZE(Width, Height);
		POINT pOINT = new POINT(0, 0);
		BLENDFUNCTION bLENDFUNCTION = new BLENDFUNCTION
		{
			BlendOp = 0,
			BlendFlags = 0,
			SourceConstantAlpha = byte.MaxValue,
			AlphaFormat = 1
		};
		if (User32.UpdateLayeredWindow(hwnd, 0, null, &sIZE, _memoryDc, &pOINT, 0u, &bLENDFUNCTION, 2u))
		{
			return true;
		}
		error = Marshal.GetLastPInvokeError();
		return false;
	}

	public void Dispose()
	{
		ReleaseBitmap();
		if (_memoryDc != 0)
		{
			Gdi32.DeleteDC(_memoryDc);
			_memoryDc = 0;
		}
	}

	private void ReleaseBitmap()
	{
		if (_bitmap != 0)
		{
			if (_memoryDc != 0 && _previousBitmap != 0)
			{
				Gdi32.SelectObject(_memoryDc, _previousBitmap);
			}
			Gdi32.DeleteObject(_bitmap);
			_bitmap = 0;
			_previousBitmap = 0;
			Bits = 0;
			Width = 0;
			Height = 0;
		}
	}
}
