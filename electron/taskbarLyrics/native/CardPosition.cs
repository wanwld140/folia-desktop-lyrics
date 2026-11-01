using System;
using TaskbarMusic.Interop;

// CardPosition supports Folia's native taskbar lyric card.
namespace TaskbarMusic;

internal sealed record CardPosition(bool Floating, string Display, double X, double Y)
{
	public bool IsValid
	{
		get
		{
			if (!string.IsNullOrWhiteSpace(Display) && double.IsFinite(X) && double.IsFinite(Y))
			{
				double x = X;
				if (x >= 0.0 && x <= 1.0)
				{
					x = Y;
					if (x >= 0.0)
					{
						return x <= 1.0;
					}
					return false;
				}
			}
			return false;
		}
	}

	public static RECT Place(RECT area, int width, int height, double x, double y)
	{
		int num = area.Left + (int)Math.Round((double)Math.Max(0, area.Width - width) * x);
		int num2 = area.Top + (int)Math.Round((double)Math.Max(0, area.Height - height) * y);
		return new RECT(num, num2, num + width, num2 + height);
	}

	public static (double X, double Y) Ratios(RECT area, RECT card)
	{
		return (X: Ratio(card.Left - area.Left, area.Width - card.Width), Y: Ratio(card.Top - area.Top, area.Height - card.Height));
	}

	private static double Ratio(int offset, int range)
	{
		if (range <= 0)
		{
			return 0.0;
		}
		return Math.Clamp((double)offset / (double)range, 0.0, 1.0);
	}
}
