using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using TaskbarMusic.Media;

// TaskbarCard supports Folia's native taskbar lyric card.
namespace TaskbarMusic.Taskbar;

internal sealed class TaskbarCard : IDisposable
{
	private readonly Thread _thread;

	private Dispatcher? _dispatcher;

	private CardHost? _host;

	private bool _disposed;

	public TaskbarCard(SettingsStore settings, Action<CardAction> action, Action<string> error)
	{
		TaskbarCard taskbarCard = this;
		TaskCompletionSource ready = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
		int abandoned = 0;
		_thread = new Thread((ThreadStart)delegate
		{
			try
			{
				taskbarCard._dispatcher = Dispatcher.CurrentDispatcher;
				taskbarCard._host = new CardHost(settings, action, error);
				ready.TrySetResult();
				if (Volatile.Read(in abandoned) == 0)
				{
					Dispatcher.Run();
				}
			}
			catch (Exception ex)
			{
				ready.TrySetException(ex);
				AppLog.Write("Card thread: " + ex);
			}
			finally
			{
				taskbarCard._host?.Dispose();
			}
		})
		{
			IsBackground = true,
			Name = "Folia.TaskbarLyrics.Card"
		};
		_thread.SetApartmentState(ApartmentState.STA);
		_thread.Start();
		try
		{
			ready.Task.WaitAsync(TimeSpan.FromSeconds(8L)).GetAwaiter().GetResult();
		}
		catch
		{
			Interlocked.Exchange(ref abandoned, 1);
			_dispatcher?.BeginInvokeShutdown(DispatcherPriority.Send);
			throw;
		}
	}

	public void Update(MusicSnapshot state)
	{
		if (!_disposed)
		{
			_dispatcher?.BeginInvoke((Action)delegate
			{
				_host?.Update(state);
			});
		}
	}

	public void Dispose()
	{
		if (!_disposed)
		{
			_disposed = true;
			Dispatcher dispatcher = _dispatcher;
			if (dispatcher != null && !dispatcher.HasShutdownStarted)
			{
				dispatcher.BeginInvokeShutdown(DispatcherPriority.Send);
			}
			_thread.Join(TimeSpan.FromSeconds(3L));
		}
	}
}
