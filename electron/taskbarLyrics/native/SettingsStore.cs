using System;
using System.IO;
using System.Text.Json;

// SettingsStore supports Folia's native taskbar lyric card.
namespace TaskbarMusic;

internal sealed class SettingsStore
{
	private readonly object _gate = new object();

	private Preferences _value;

	public string FilePath { get; }

	public Preferences Value
	{
		get
		{
			lock (_gate)
			{
				return _value;
			}
		}
	}

	public SettingsStore(string filePath)
	{
		FilePath = Path.GetFullPath(filePath);
		try
		{
			_value = (File.Exists(FilePath) ? (JsonSerializer.Deserialize<Preferences>(File.ReadAllText(FilePath)) ?? new Preferences()) : new Preferences());
			bool flag;
			switch (_value.Theme)
			{
			case "system":
			case "light":
			case "dark":
				flag = true;
				break;
			default:
				flag = false;
				break;
			}
			if (!flag)
			{
				_value = _value with
				{
					Theme = "system"
				};
			}
			_value = _value with { CardVisible = true };
			CardPosition position = _value.Position;
			if ((object)position != null && !position.IsValid)
			{
				_value = _value with
				{
					Position = null
				};
			}
		}
		catch (Exception ex) when (((ex is IOException || ex is JsonException || ex is UnauthorizedAccessException) ? 1 : 0) != 0)
		{
			_value = new Preferences();
			AppLog.Write("Settings could not be read: " + ex.Message);
		}
	}

	public bool Update(Func<Preferences, Preferences> change)
	{
		lock (_gate)
		{
			Preferences value = change(_value);
			try
			{
				Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
				string text = FilePath + ".tmp";
				File.WriteAllText(text, JsonSerializer.Serialize(value, new JsonSerializerOptions
				{
					WriteIndented = true
				}));
				File.Move(text, FilePath, overwrite: true);
				_value = value;
				return true;
			}
			catch (Exception ex) when (((ex is IOException || ex is UnauthorizedAccessException) ? 1 : 0) != 0)
			{
				AppLog.Write("Settings could not be saved: " + ex.Message);
				return false;
			}
		}
	}
}
