using System;
using System.Collections.Generic;
using System.Linq;
using TaskbarMusic.Interop;

// TaskbarLocator supports Folia's native taskbar lyric card.
namespace TaskbarMusic.Taskbar;

internal static class TaskbarLocator
{
	public static IEnumerable<BarGeometry> All()
	{
		List<nint> list = new List<nint>();
		nint primary = User32.FindWindow("Shell_TrayWnd", null);
		if (primary != 0)
		{
			list.Add(primary);
		}
		nint num = 0;
		while ((num = User32.FindWindowEx(0, num, "Shell_SecondaryTrayWnd", null)) != 0)
		{
			list.Add(num);
		}
		foreach (nint item in list)
		{
			if (User32.GetWindowRect(item, out var lpRect) && !lpRect.IsEmpty)
			{
				double num2 = (double)Math.Max(1u, User32.GetDpiForWindow(item)) / 96.0;
				if (num2 < 0.5)
				{
					num2 = 1.0;
				}
				bool flag = lpRect.Height > lpRect.Width;
				nint num3 = User32.FindWindowEx(item, 0, "TrayNotifyWnd", null);
				int num4 = (flag ? (lpRect.Bottom - (int)(180.0 * num2)) : (lpRect.Right - (int)(220.0 * num2)));
				if (num3 != 0 && User32.GetWindowRect(num3, out var lpRect2) && !lpRect2.IsEmpty)
				{
					num4 = (flag ? lpRect2.Top : lpRect2.Left);
				}
				int num5 = (int)Math.Round(6.0 * num2);
				int num6 = ((item == primary) ? ((int)Math.Round(180.0 * num2)) : num5);
				RECT area = (flag ? new RECT(lpRect.Left, lpRect.Top + num5, lpRect.Right, num4 - num5) : new RECT(lpRect.Left + num6, lpRect.Top, Math.Max(lpRect.Left + num6, num4 - num5), lpRect.Bottom));
				yield return new BarGeometry(item, lpRect, area, num2, flag);
			}
		}
	}

	public static BarGeometry? Find(string? display)
	{
		return (from b in All()
			where !b.Vertical
			orderby Monitors.FromWindow(b.Handle)?.DeviceName == display descending
			select b).Cast<BarGeometry?>().FirstOrDefault();
	}

	public static BarGeometry? At(POINT point)
	{
		return (from b in All()
			where !b.Vertical && User32.IsWindowVisible(b.Handle) && !IsHidden(b) && Contains(b.Bounds, point)
			select b).Cast<BarGeometry?>().FirstOrDefault();
	}

	public static bool Contains(RECT bounds, POINT point)
	{
		if (point.X >= bounds.Left && point.X < bounds.Right && point.Y >= bounds.Top)
		{
			return point.Y < bounds.Bottom;
		}
		return false;
	}

	public static bool IsHidden(BarGeometry bar)
	{
		MonitorInfo monitorInfo = Monitors.FromWindow(bar.Handle);
		if ((object)monitorInfo != null)
		{
			return (double)(Math.Min(bar.Bounds.Bottom, monitorInfo.Bounds.Bottom) - Math.Max(bar.Bounds.Top, monitorInfo.Bounds.Top)) < 8.0 * bar.Scale;
		}
		return false;
	}

	public static RECT ToClient(nint parent, RECT bounds)
	{
		RECT lpPoints = bounds;
		User32.MapWindowPoints(0, parent, ref lpPoints, 2u);
		return lpPoints;
	}
}
