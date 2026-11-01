using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Monitors supports Folia's native taskbar lyric card.
namespace TaskbarMusic.Interop;

internal static class Monitors
{
	public unsafe static IReadOnlyList<MonitorInfo> All()
	{
		List<MonitorInfo> list = new List<MonitorInfo>(4);
		GCHandle value = GCHandle.Alloc(list);
		try
		{
			User32.EnumDisplayMonitors((nint)0, (nint)0, (delegate* unmanaged[Stdcall]<nint, nint, RECT*, nint, int>)(&EnumProc), GCHandle.ToIntPtr(value));
		}
		finally
		{
			value.Free();
		}
		if (list.Count == 0)
		{
			MonitorInfo monitorInfo = FromPoint(new POINT(0, 0));
			if ((object)monitorInfo != null)
			{
				list.Add(monitorInfo);
			}
		}
		return list;
	}

	public static MonitorInfo? FromPoint(POINT point)
	{
		return Describe(User32.MonitorFromPoint(point, 2u));
	}

	public static MonitorInfo? FromWindow(nint hwnd)
	{
		return Describe(User32.MonitorFromWindow(hwnd, 2u));
	}

	public static MonitorInfo? FromRect(RECT rect)
	{
		return Describe(User32.MonitorFromRect(in rect, 2u));
	}

	public unsafe static int? RefreshRate(MonitorInfo monitor)
	{
		DEVMODEW lpDevMode = new DEVMODEW
		{
			dmSize = (ushort)sizeof(DEVMODEW)
		};
		if (!User32.EnumDisplaySettings(monitor.DeviceName, -1, ref lpDevMode) || lpDevMode.dmDisplayFrequency <= 1)
		{
			return null;
		}
		return (int)lpDevMode.dmDisplayFrequency;
	}

	public static MonitorInfo? Primary()
	{
		return All().FirstOrDefault((MonitorInfo m) => m.IsPrimary) ?? FromPoint(new POINT(0, 0));
	}

	public unsafe static MonitorInfo? Describe(nint monitor)
	{
		if (monitor == 0)
		{
			return null;
		}
		MONITORINFOEX lpmi = new MONITORINFOEX
		{
			cbSize = (uint)sizeof(MONITORINFOEX)
		};
		if (!User32.GetMonitorInfo(monitor, ref lpmi))
		{
			return null;
		}
		string text = new string(lpmi.szDevice);
		int num = text.IndexOf('\0');
		if (num >= 0)
		{
			text = text.Substring(0, num);
		}
		uint dpi = 96u;
		try
		{
			if (Shcore.GetDpiForMonitor(monitor, 0, out var dpiX, out var _) == 0 && dpiX != 0)
			{
				dpi = dpiX;
			}
		}
		catch (Exception ex) when (((ex is DllNotFoundException || ex is EntryPointNotFoundException) ? 1 : 0) != 0)
		{
		}
		return new MonitorInfo(monitor, text, lpmi.rcMonitor, lpmi.rcWork, (lpmi.dwFlags & 1) != 0, dpi);
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int EnumProc(nint monitor, nint hdc, RECT* clip, nint data)
	{
		try
		{
			if (GCHandle.FromIntPtr(data).Target is List<MonitorInfo> list)
			{
				MonitorInfo monitorInfo = Describe(monitor);
				if ((object)monitorInfo != null)
				{
					list.Add(monitorInfo);
				}
			}
		}
		catch
		{
		}
		return 1;
	}
}
