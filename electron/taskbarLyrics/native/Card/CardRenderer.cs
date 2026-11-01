using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using TaskbarMusic.Media;

// CardRenderer supports Folia's native taskbar lyric card.
namespace TaskbarMusic.Taskbar;

internal static class CardRenderer
{
	public const double Width = 352.0;

	public const double Height = 40.0;

	public static bool IsDark(string theme)
	{
		if (!(theme == "dark"))
		{
			if (theme == "system" && Registry.GetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", "SystemUsesLightTheme", 1) is int num)
			{
				return num == 0;
			}
			return false;
		}
		return true;
	}

	public static CardAction Hit(double x)
	{
		if (!(x >= 316.0))
		{
			if (!(x >= 283.0))
			{
				if (!(x >= 241.0))
				{
					if (!(x >= 208.0))
					{
						return CardAction.Open;
					}
					return CardAction.Previous;
				}
				return CardAction.Toggle;
			}
			return CardAction.Next;
		}
		return CardAction.Menu;
	}

	public static RenderTargetBitmap Render(MusicSnapshot state, double scale, int pixelHeight, bool floating, bool dark, CardAction? hover = null, double marqueeSeconds = 0)
	{
		int pixelWidth = (int)Math.Round(352.0 * scale);
		int num = Math.Max(1, pixelHeight);
		DrawingVisual drawingVisual = new DrawingVisual();
		DrawingContext dc = drawingVisual.RenderOpen();
		try
		{
			dc.PushTransform(new ScaleTransform(scale, scale));
			double num2 = (double)num / scale;
			double offsetY = (num2 - 40.0) / 2.0;
			SolidColorBrush foreground = Brush(dark ? "#FFFFFF" : "#101820");
			SolidColorBrush brush = Brush(dark ? "#E1E7ED" : "#3D4A56");
			dc.DrawRoundedRectangle(floating ? Brush(dark ? "#F024272E" : "#F6FFFFFF") : new SolidColorBrush(Color.FromArgb(1, 0, 0, 0)), floating ? new Pen(Brush(dark ? "#354047" : "#25000000"), 0.6) : null, new Rect(0.5, 0.5, 351.0, num2 - 1.0), floating ? 12 : 0, floating ? 12 : 0);
			dc.PushTransform(new TranslateTransform(0.0, offsetY));
			dc.DrawRoundedRectangle(Brush(dark ? "#D3DBE4" : "#64717D"), null, new Rect(7.0, 15.0, 2.0, 10.0), 1.0, 1.0);
			Rect rect = new Rect(17.0, 6.0, 28.0, 28.0);
			dc.PushClip(new RectangleGeometry(rect, 5.0, 5.0));
			if (state.Artwork != null)
			{
				dc.DrawImage(state.Artwork, rect);
			}
			else
			{
				dc.DrawRectangle(Brush(dark ? "#394955" : "#D7E7EC"), null, rect);
				Text(dc, "♪", 24.0, 44.0, 7.0, Brush(dark ? "#E7F4FA" : "#315968"), 21.0, bold: false);
			}
			dc.Pop();
			bool lyrics = !string.IsNullOrWhiteSpace(state.CurrentLyric);
			if (lyrics) LyricText(dc, state.CurrentLyric, foreground, marqueeSeconds);
			else Text(dc, state.Title, 53.0, 203.0, 3.0, foreground, 13.0, bold: true);
			string value = !string.IsNullOrWhiteSpace(state.NextLyric) ? state.NextLyric
				: (lyrics ? "" : (string.IsNullOrWhiteSpace(state.Artist) ? state.PlayerLabel : state.Artist));
			Text(dc, value, 53.0, 203.0, 21.0, brush, 11.0, bold: false);
			Button(dc, CardAction.Previous, 224.0, state.CanPrevious, hover, dark, delegate
			{
				dc.DrawRectangle(foreground, null, new Rect(219.0, 15.0, 2.0, 10.0));
				Triangle(dc, new Point(228.0, 15.0), new Point(222.0, 20.0), new Point(228.0, 25.0), foreground);
			});
			Button(dc, CardAction.Toggle, 262.0, state.CanToggle, hover, dark, delegate
			{
				if (state.Playing)
				{
					dc.DrawRoundedRectangle(foreground, null, new Rect(257.0, 14.0, 3.0, 12.0), 1.0, 1.0);
					dc.DrawRoundedRectangle(foreground, null, new Rect(264.0, 14.0, 3.0, 12.0), 1.0, 1.0);
				}
				else
				{
					Triangle(dc, new Point(259.0, 13.0), new Point(259.0, 27.0), new Point(269.0, 20.0), foreground);
				}
			});
			Button(dc, CardAction.Next, 300.0, state.CanNext, hover, dark, delegate
			{
				Triangle(dc, new Point(296.0, 15.0), new Point(302.0, 20.0), new Point(296.0, 25.0), foreground);
				dc.DrawRectangle(foreground, null, new Rect(303.0, 15.0, 2.0, 10.0));
			});
			Button(dc, CardAction.Menu, 333.0, enabled: true, hover, dark, delegate
			{
				for (int i = 0; i < 3; i++)
				{
					dc.DrawEllipse(foreground, null, new Point(328 + i * 5, 20.0), 1.2, 1.2);
				}
			});
			if (state.Duration > 0.0)
			{
				dc.DrawRoundedRectangle(Brush(dark ? "#40545E" : "#D3E0E5"), null, new Rect(53.0, 38.0, 148.0, 1.5), 0.75, 0.75);
				dc.DrawRoundedRectangle(Brush(dark ? "#96C2D3" : "#547A88"), null, new Rect(53.0, 38.0, 148.0 * Math.Clamp(state.Position / state.Duration, 0.0, 1.0), 1.5), 0.75, 0.75);
			}
			dc.Pop();
			dc.Pop();
		}
		finally
		{
			if (dc != null)
			{
				((IDisposable)dc).Dispose();
			}
		}
		RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(pixelWidth, num, 96.0, 96.0, PixelFormats.Pbgra32);
		renderTargetBitmap.Render(drawingVisual);
		renderTargetBitmap.Freeze();
		return renderTargetBitmap;
	}

	private static void Button(DrawingContext dc, CardAction action, double center, bool enabled, CardAction? hover, bool dark, Action draw)
	{
		if (action == CardAction.Toggle || hover == action)
		{
			dc.DrawRoundedRectangle(Brush((!dark) ? ((hover == action) ? "#BFCFD8" : "#D5E2E8") : ((hover == action) ? "#5A6572" : "#424F5D")), null, new Rect(center - (double)((action == CardAction.Toggle) ? 19 : 14), 7.0, (action == CardAction.Toggle) ? 38 : 28, 26.0), 13.0, 13.0);
		}
		dc.PushOpacity(enabled ? 1.0 : 0.5);
		draw();
		dc.Pop();
	}

	private static void Triangle(DrawingContext dc, Point a, Point b, Point c, Brush brush)
	{
		StreamGeometry streamGeometry = new StreamGeometry();
		using (StreamGeometryContext streamGeometryContext = streamGeometry.Open())
		{
			streamGeometryContext.BeginFigure(a, isFilled: true, isClosed: true);
			streamGeometryContext.LineTo(b, isStroked: true, isSmoothJoin: false);
			streamGeometryContext.LineTo(c, isStroked: true, isSmoothJoin: false);
		}
		streamGeometry.Freeze();
		dc.DrawGeometry(brush, null, streamGeometry);
	}

	private static void Text(DrawingContext dc, string value, double left, double right, double y, Brush brush, double size, bool bold)
	{
		FormattedText formattedText = new FormattedText(value.Replace('\r', ' ').Replace('\n', ' '), CultureInfo.GetCultureInfo("zh-CN"), FlowDirection.LeftToRight, new Typeface(new FontFamily("Segoe UI, Microsoft YaHei UI"), FontStyles.Normal, bold ? FontWeights.SemiBold : FontWeights.Normal, FontStretches.Normal), size, brush, 1.0)
		{
			MaxTextWidth = Math.Max(1.0, right - left),
			MaxLineCount = 1,
			Trimming = TextTrimming.CharacterEllipsis
		};
		dc.DrawText(formattedText, new Point(left, y));
	}

	private static SolidColorBrush Brush(string color)
	{
		return new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
	}

	private static void LyricText(DrawingContext dc, string value, Brush foreground, double seconds)
	{
		var text = new FormattedText(value, CultureInfo.GetCultureInfo("zh-CN"), FlowDirection.LeftToRight,
			new Typeface(new FontFamily("Segoe UI, Microsoft YaHei UI"), FontStyles.Normal, FontWeights.SemiBold, FontStretches.Normal), 12, foreground, 1);
		double overflow = Math.Max(0, text.WidthIncludingTrailingWhitespace - 150);
		double offset = Math.Min(overflow, Math.Max(0, seconds - 0.8) * 24);
		dc.PushClip(new RectangleGeometry(new Rect(53, 2, 150, 18)));
		dc.DrawText(text, new Point(53 - offset, 3));
		dc.Pop();
	}
}
