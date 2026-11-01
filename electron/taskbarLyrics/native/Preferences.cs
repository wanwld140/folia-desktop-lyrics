// Preferences supports Folia's native taskbar lyric card.
namespace TaskbarMusic;

internal sealed record Preferences
{
	public string Theme { get; init; } = "system";

	public bool CardVisible { get; init; } = true;

	public CardPosition? Position { get; init; }
}
