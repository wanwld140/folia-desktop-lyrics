using System;

namespace TaskbarMusic;

// Redirect native diagnostics to Folia's existing main-process log sink.
internal static class AppLog
{
    public static void Write(string message) => Console.Error.WriteLine("[TaskbarLyrics] " + message);
}
