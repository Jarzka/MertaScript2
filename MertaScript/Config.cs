using System.Text.RegularExpressions;

namespace MertaScript;

public abstract class Config {
  public static readonly string PathGameEventSounds =
    "sound/" + GetValueFromConfigFile("host_game_event_sounds_folder") + "/";

  public static readonly string PathPlayerEventSounds =
    "sound/" + GetValueFromConfigFile("host_player_event_sounds_folder") + "/";

  public static readonly string PathLogs = GetValueFromConfigFile("host_logs_path") + "/";

  public static string GetValueFromConfigFile(string key) {
    using var file = new StreamReader("config.txt");
    while (file.ReadLine() is { } line)
      if (Regex.IsMatch(line, "^" + key)) {
        var lineArray = line.Split('=');
        var result = lineArray[1].Trim();
        return result;
      }

    throw new ArgumentException($"Key {key} not found in config.txt");
  }

  public static List<string> GetClientTeamPlayerNamesFromConfigFile() {
    using var file = new StreamReader("config.txt");
    while (file.ReadLine() is { } line)
      if (Regex.IsMatch(line, "^host_client_team_player_names")) {
        var lineArray = line.Split('=');
        var result = lineArray[1].Replace(" ", "").Trim();
        return result.Split(',').ToList();
      }

    throw new ArgumentException("Key host_client_team_player_names not found in config.txt");
  }
}