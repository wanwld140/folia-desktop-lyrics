using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using TaskbarMusic.Interop;
using TaskbarMusic.Media;

// CardHost supports Folia's native taskbar lyric card.
namespace TaskbarMusic.Taskbar;

internal sealed class CardHost : IDisposable
{
	private const string ClassName = "Folia.TaskbarLyrics.Card";

	private static CardHost? _instance;

	private readonly SettingsStore _settings;

	private readonly Action<CardAction> _action;

	private readonly Action<string> _error;

	private readonly LayeredSurface _surface = new LayeredSurface();

	private readonly DispatcherTimer _dragTimer;

	private readonly DispatcherTimer _safety;

	private readonly DispatcherTimer _lyricAnimation;
	private double _marqueeSeconds;
	private long _marqueeTick;

	private MusicSnapshot _state = new MusicSnapshot();

	private nint _hwnd;

	private nint _parent;

	private RECT _bounds;

	private double _scale = 1.0;

	private POINT? _press;

	private double _grabX;

	private double _grabY;

	private bool _moving;

	private bool _tracking;

	private bool _disposed;
	private bool _renderSuppressed;

	private CardAction? _hover;

	private CardAction _pressedAction;

	public RECT Bounds => _bounds;

	public nint Window => _hwnd;

	public bool Embedded => _parent != 0;

	internal bool LastPaintSucceeded { get; private set; }

	public unsafe CardHost(SettingsStore settings, Action<CardAction> action, Action<string> error)
	{
		_settings = settings;
		_marqueeTick = Stopwatch.GetTimestamp();
		_lyricAnimation = new DispatcherTimer(TimeSpan.FromMilliseconds(50), DispatcherPriority.Background, delegate
		{
			long now = Stopwatch.GetTimestamp();
			if (_state.Playing && _state.CurrentLyric.Length > 10)
			{
				_marqueeSeconds += Stopwatch.GetElapsedTime(_marqueeTick, now).TotalSeconds;
				Render();
			}
			_marqueeTick = now;
		}, Dispatcher.CurrentDispatcher);
		_lyricAnimation.Stop();
		_action = action;
		_error = error;
		_instance = this;
		fixed (char* lpszClassName = &"Folia.TaskbarLyrics.Card".GetPinnableReference())
		{
			WNDCLASSEX lpwcx = new WNDCLASSEX
			{
				cbSize = (uint)sizeof(WNDCLASSEX),
				lpfnWndProc = &WindowProc,
				hInstance = Kernel32.GetModuleHandle(null),
				hCursor = User32.LoadCursor(0, 32512),
				lpszClassName = lpszClassName
			};
			if (User32.RegisterClassEx(in lpwcx) == 0 && Marshal.GetLastPInvokeError() != 1410)
			{
				throw new InvalidOperationException("无法创建任务栏窗口");
			}
		}
		_dragTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(16L), DispatcherPriority.Input, delegate
		{
			PollDrag();
		}, Dispatcher.CurrentDispatcher);
		_dragTimer.Stop();
		_safety = new DispatcherTimer(TimeSpan.FromSeconds(1L), DispatcherPriority.Background, delegate
		{
			POINT? press = _press;
			if (!press.HasValue)
			{
				Ensure();
			}
		}, Dispatcher.CurrentDispatcher);
		Ensure();
	}

	public void Update(MusicSnapshot state)
	{
		if (_state.SourceId != state.SourceId || _state.CurrentLyric != state.CurrentLyric) _marqueeSeconds = 0;
		_marqueeTick = Stopwatch.GetTimestamp();
		_state = state;
		UpdateAnimation();
		POINT? press = _press;
		if (!press.HasValue)
		{
			Ensure();
		}
		else
		{
			Render();
		}
	}

	private void UpdateAnimation()
	{
		if (!_renderSuppressed && _state.Playing && _state.CurrentLyric.Length > 10 && _settings.Value.CardVisible)
			_lyricAnimation.Start();
		else _lyricAnimation.Stop();
	}

	public void Ensure()
	{
		if (_disposed)
		{
			return;
		}
		POINT? press = _press;
		if (press.HasValue)
		{
			return;
		}
		Preferences value = _settings.Value;
		if (!value.CardVisible)
		{
			Destroy();
			return;
		}
		CardPosition position = value.Position;
		if ((object)position == null || !position.Floating)
		{
			BarGeometry? barGeometry = TaskbarLocator.Find(position?.Display);
			if (barGeometry.HasValue)
			{
				BarGeometry valueOrDefault = barGeometry.GetValueOrDefault();
				if ((double)valueOrDefault.Area.Width >= 352.0 * valueOrDefault.Scale)
				{
					_scale = valueOrDefault.Scale;
					int width = (int)Math.Round(352.0 * _scale);
					RECT desired = CardPosition.Place(valueOrDefault.Area, width, valueOrDefault.Bounds.Height, position?.X ?? 0.05, 0.0);
					if (TaskbarLocator.IsHidden(valueOrDefault) || !User32.IsWindowVisible(valueOrDefault.Handle))
					{
						_renderSuppressed = true;
						UpdateAnimation();
						if (_hwnd != 0) User32.ShowWindow(_hwnd, 0);
						return;
					}
					_renderSuppressed = false;
					Place(desired, valueOrDefault.Handle);
					UpdateAnimation();
					return;
				}
			}
		}
		MonitorInfo monitorInfo = Monitors.All().FirstOrDefault((MonitorInfo m) => m.DeviceName == position?.Display) ?? Monitors.Primary();
		if ((object)monitorInfo != null)
		{
			_renderSuppressed = false;
			_scale = monitorInfo.Scale;
			Place(CardPosition.Place(monitorInfo.WorkArea, (int)Math.Round(352.0 * _scale), (int)Math.Round(48.0 * _scale), position?.X ?? 0.1, position?.Y ?? 0.9), 0);
			UpdateAnimation();
		}
	}

	private void Place(RECT desired, nint parent)
	{
		if (_hwnd == 0 || !User32.IsWindow(_hwnd) || parent != _parent)
		{
			Destroy();
			RECT rECT = ((parent == 0) ? desired : TaskbarLocator.ToClient(parent, desired));
			int dwExStyle = 0x8080080 | ((parent == 0) ? 8 : 4);
			_hwnd = User32.CreateWindowEx(dwExStyle, "Folia.TaskbarLyrics.Card", "TaskbarMusic", (parent == 0) ? int.MinValue : 1409286144, rECT.Left, rECT.Top, desired.Width, desired.Height, parent, 0, Kernel32.GetModuleHandle(null), 0);
			if (_hwnd == 0)
			{
				AppLog.Write("Card window creation failed: " + Marshal.GetLastPInvokeError());
				return;
			}
			_parent = parent;
			AppLog.Write((parent == 0) ? "Card floating." : "Card embedded.");
		}
		_bounds = desired;
		Render();
		RECT rECT2 = ((parent == 0) ? desired : TaskbarLocator.ToClient(parent, desired));
		User32.SetWindowPos(_hwnd, (parent == 0) ? HWND.TOPMOST : HWND.TOP, rECT2.Left, rECT2.Top, desired.Width, desired.Height, 80u);
	}

	private void Render()
	{
		if (_renderSuppressed || _hwnd == 0 || _bounds.IsEmpty)
		{
			return;
		}
		RenderTargetBitmap renderTargetBitmap = CardRenderer.Render(_state, _scale, _bounds.Height, _parent == 0, CardRenderer.IsDark(_settings.Value.Theme), _hover, _marqueeSeconds);
		if (!_surface.Ensure(renderTargetBitmap.PixelWidth, renderTargetBitmap.PixelHeight))
		{
			LastPaintSucceeded = false;
			return;
		}
		renderTargetBitmap.CopyPixels(Int32Rect.Empty, _surface.Bits, _surface.ByteCount, renderTargetBitmap.PixelWidth * 4);
		LastPaintSucceeded = _surface.Present(_hwnd, out var error);
		if (!LastPaintSucceeded)
		{
			AppLog.Write("Card paint failed: " + error);
		}
		User32.SetWindowText(_hwnd, "Folia · " + _state.PlayerLabel + " · " + _state.Title);
	}

	private void BeginPress()
	{
		if (User32.GetCursorPos(out var lpPoint))
		{
			BeginPressAt(lpPoint);
		}
	}

	private void BeginPressAt(POINT point)
	{
		_press = point;
		_pressedAction = CardRenderer.Hit((double)(point.X - _bounds.Left) / _scale);
		_grabX = (double)(point.X - _bounds.Left) / (double)_bounds.Width;
		_grabY = (double)(point.Y - _bounds.Top) / (double)_bounds.Height;
		_moving = false;
		_dragTimer.Start();
	}

	private void PollDrag()
	{
		POINT? press = _press;
		POINT lpPoint;
		if (!press.HasValue)
		{
			_dragTimer.Stop();
		}
		else if ((User32.GetAsyncKeyState(27) & 0x8000) != 0)
		{
			Cancel();
		}
		else if (!User32.GetCursorPos(out lpPoint))
		{
			Cancel();
		}
		else if ((User32.GetAsyncKeyState(1) & 0x8000) == 0)
		{
			EndPress(lpPoint);
		}
		else
		{
			MoveDrag(lpPoint);
		}
	}

	private void MoveDrag(POINT point)
	{
		POINT? press = _press;
		if (press.HasValue)
		{
			POINT valueOrDefault = press.GetValueOrDefault();
			if (_pressedAction == CardAction.Open && (_moving || !((double)Math.Abs(point.X - valueOrDefault.X) < 6.0 * _scale) || !((double)Math.Abs(point.Y - valueOrDefault.Y) < 6.0 * _scale)))
			{
				_moving = true;
				_scale = Monitors.FromPoint(point)?.Scale ?? _scale;
				int num = (int)Math.Round(352.0 * _scale);
				int num2 = (int)Math.Round(48.0 * _scale);
				int num3 = point.X - (int)Math.Round((double)num * _grabX);
				int num4 = point.Y - (int)Math.Round((double)num2 * _grabY);
				Place(new RECT(num3, num4, num3 + num, num4 + num2), 0);
			}
		}
	}

	private void EndPress(POINT point)
	{
		POINT? press = _press;
		if (!press.HasValue)
		{
			return;
		}
		MoveDrag(point);
		_press = null;
		_dragTimer.Stop();
		if (!_moving)
		{
			if (TaskbarLocator.Contains(_bounds, point) && CardRenderer.Hit((double)(point.X - _bounds.Left) / _scale) == _pressedAction)
			{
				_action(_pressedAction);
			}
			return;
		}
		CardPosition position = null;
		BarGeometry? barGeometry = TaskbarLocator.At(point);
		if (barGeometry.HasValue)
		{
			BarGeometry valueOrDefault = barGeometry.GetValueOrDefault();
			MonitorInfo monitorInfo = Monitors.FromWindow(valueOrDefault.Handle);
			if ((object)monitorInfo != null)
			{
				int num = (int)Math.Round(352.0 * valueOrDefault.Scale);
				(double, double) tuple = CardPosition.Ratios(card: new RECT(point.X - (int)Math.Round((double)num * _grabX), valueOrDefault.Bounds.Top, point.X - (int)Math.Round((double)num * _grabX) + num, valueOrDefault.Bounds.Bottom), area: valueOrDefault.Area);
				position = new CardPosition(Floating: false, monitorInfo.DeviceName, tuple.Item1, 0.0);
				goto IL_0198;
			}
		}
		MonitorInfo monitorInfo2 = Monitors.FromPoint(point);
		if ((object)monitorInfo2 != null)
		{
			(double, double) tuple2 = CardPosition.Ratios(monitorInfo2.WorkArea, _bounds);
			position = new CardPosition(Floating: true, monitorInfo2.DeviceName, tuple2.Item1, tuple2.Item2);
		}
		goto IL_0198;
		IL_0198:
		_moving = false;
		if ((object)position != null && _settings.Update((Preferences p) => p with
		{
			Position = position
		}))
		{
			AppLog.Write($"Card drop saved: floating={position.Floating}, display={position.Display}, x={position.X}, y={position.Y}.");
		}
		else
		{
			_error("位置未能保存，已恢复原位");
		}
		Ensure();
	}

	private void Cancel()
	{
		_press = null;
		_moving = false;
		_dragTimer.Stop();
		Ensure();
	}

	private unsafe nint HandleMessage(nint hwnd, uint message, nuint wParam, nint lParam)
	{
		switch (message)
		{
		case 33u:
			return 3;
		case 20u:
			return 1;
		case 513u:
			BeginPress();
			return 0;
		case 514u:
		{
			if (User32.GetCursorPos(out var lpPoint2))
			{
				EndPress(lpPoint2);
			}
			return 0;
		}
		case 517u:
			_action(CardAction.Menu);
			return 0;
		case 123u:
			return 0;
		case 512u:
		{
			if (!_tracking)
			{
				TRACKMOUSEEVENT lpEventTrack = new TRACKMOUSEEVENT
				{
					cbSize = (uint)sizeof(TRACKMOUSEEVENT),
					hwndTrack = hwnd,
					dwFlags = 2u
				};
				_tracking = User32.TrackMouseEvent(ref lpEventTrack);
			}
			if (User32.GetCursorPos(out var lpPoint))
			{
				_hover = CardRenderer.Hit((double)(lpPoint.X - _bounds.Left) / _scale);
			}
			Render();
			return 0;
		}
		case 675u:
			_tracking = false;
			_hover = null;
			Render();
			return 0;
		case 130u:
			if (_hwnd == hwnd)
			{
				_hwnd = 0;
			}
			break;
		}
		return User32.DefWindowProc(hwnd, message, wParam, lParam);
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private static nint WindowProc(nint hwnd, uint message, nuint wParam, nint lParam)
	{
		try
		{
			return _instance?.HandleMessage(hwnd, message, wParam, lParam) ?? User32.DefWindowProc(hwnd, message, wParam, lParam);
		}
		catch (Exception ex)
		{
			AppLog.Write("Card message: " + ex.Message);
			return User32.DefWindowProc(hwnd, message, wParam, lParam);
		}
	}

	private void Destroy()
	{
		nint hwnd = _hwnd;
		_hwnd = 0;
		_parent = 0;
		_tracking = false;
		if (hwnd != 0 && User32.IsWindow(hwnd))
		{
			User32.DestroyWindow(hwnd);
		}
	}

	public void Dispose()
	{
		if (!_disposed)
		{
			_disposed = true;
			_press = null;
			_dragTimer.Stop();
			_safety.Stop();
			_lyricAnimation.Stop();
			Destroy();
			_surface.Dispose();
			if (_instance == this)
			{
				_instance = null;
			}
		}
	}
}
