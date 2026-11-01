using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using TaskbarMusic.Media;
using TaskbarMusic.Taskbar;

namespace TaskbarMusic;

// Pipe-owned Folia card: no system player scanning, global singleton, or standalone app settings.
internal static class Program
{
    private static readonly JsonSerializerOptions Json = new() { PropertyNameCaseInsensitive = true };
    private static readonly object OutputGate = new();

    private static void Emit(object value)
    {
        lock (OutputGate)
        {
            Console.Out.WriteLine(JsonSerializer.Serialize(value));
            Console.Out.Flush();
        }
    }

    private static async Task<int> Main(string[] args)
    {
        Console.InputEncoding = new UTF8Encoding(false);
        Console.OutputEncoding = new UTF8Encoding(false);
        if (args.Length != 2 || !int.TryParse(args[0], out int parentId)) return 2;
        try
        {
            using var parent = Process.GetProcessById(parentId);
            using var ended = new CancellationTokenSource();
            parent.EnableRaisingEvents = true;
            parent.Exited += (_, _) => ended.Cancel();
            if (parent.HasExited) return 0;
            var settings = new SettingsStore(Path.GetFullPath(args[1]));
            using var card = new TaskbarCard(settings, action => Emit(new
            {
                version = 1,
                type = "control",
                action = action.ToString().ToLowerInvariant()
            }), message => AppLog.Write(message));
            Emit(new { version = 1, type = "ready" });
            long sequence = -1;
            string lastCover = "";
            BitmapSource? artwork = null;
            while (!ended.IsCancellationRequested)
            {
                string? line;
                try { line = await Console.In.ReadLineAsync().WaitAsync(ended.Token); }
                catch (OperationCanceledException) { break; }
                if (line == null) break;
                if (line.Length > 262144) throw new InvalidDataException("Oversized snapshot");
                var frame = JsonSerializer.Deserialize<Frame>(line, Json);
                if (frame == null || frame.Version != 1 || frame.Sequence <= sequence) continue;
                sequence = frame.Sequence;
                if (frame.CoverPng != lastCover)
                {
                    lastCover = frame.CoverPng;
                    artwork = null;
                    try
                    {
                        if (lastCover.Length > 0)
                        {
                            using var stream = new MemoryStream(Convert.FromBase64String(lastCover));
                            var bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.DecodePixelWidth = 64;
                            bitmap.StreamSource = stream;
                            bitmap.EndInit();
                            bitmap.Freeze();
                            artwork = bitmap;
                        }
                    }
                    catch (Exception error) { AppLog.Write("Cover decode: " + error.Message); }
                }
                card.Update(new MusicSnapshot
                {
                    SourceId = frame.TrackId, Player = "folia", PlayerLabel = "Folia",
                    Title = frame.Title, Artist = frame.Artist, Connected = frame.Connected,
                    Playing = frame.Playing, CanPrevious = frame.CanPrevious,
                    CanToggle = frame.CanToggle, CanNext = frame.CanNext,
                    Position = frame.Position, Duration = frame.Duration, PositionKnown = true,
                    CurrentLyric = frame.CurrentLyric, NextLyric = frame.NextLyric, Artwork = artwork
                });
            }
            return 0;
        }
        catch (Exception error)
        {
            AppLog.Write(error.ToString());
            return 1;
        }
    }

    private sealed record Frame
    {
        public int Version { get; init; }
        public long Sequence { get; init; }
        public string TrackId { get; init; } = "";
        public string Title { get; init; } = "Folia";
        public string Artist { get; init; } = "";
        public bool Connected { get; init; }
        public bool Playing { get; init; }
        public bool CanPrevious { get; init; }
        public bool CanToggle { get; init; }
        public bool CanNext { get; init; }
        public double Position { get; init; }
        public double Duration { get; init; }
        public string CurrentLyric { get; init; } = "";
        public string NextLyric { get; init; } = "";
        public string CoverPng { get; init; } = "";
    }
}
