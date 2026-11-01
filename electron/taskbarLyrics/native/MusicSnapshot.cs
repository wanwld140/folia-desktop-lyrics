using System.Windows.Media.Imaging;

// MusicSnapshot supports Folia's native taskbar lyric card.
namespace TaskbarMusic.Media;

internal sealed record MusicSnapshot
{
	public string SourceId { get; init; } = "";

	public string Player { get; init; } = "auto";

	public string PlayerLabel { get; init; } = "Folia";

	public string Title { get; init; } = "Folia";

	public string Artist { get; init; } = "";

	public string Album { get; init; } = "";

	public bool Connected { get; init; }

	public bool Playing { get; init; }

	public bool CanPrevious { get; init; }

	public bool CanToggle { get; init; }

	public bool CanNext { get; init; }

	public bool CanSeek { get; init; }

	public double Position { get; init; }

	public double Duration { get; init; }

	public bool PositionKnown { get; init; }

	public string CurrentLyric { get; init; } = "";

	public string NextLyric { get; init; } = "";

	public string LyricStatus { get; init; } = "等待播放器";

	public BitmapSource? Artwork { get; init; }

	public string Status { get; init; } = "等待播放器";
}
