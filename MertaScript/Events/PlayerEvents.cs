using MertaScript.EventHandling;
using Newtonsoft.Json;

namespace MertaScript.Events;

public abstract class PlayerEvents {
  private static readonly Dictionary<PlayerEventId, string> EventAudioFolder = new() {
    { PlayerEventId.PlayerDie, "Die" }, {
      PlayerEventId.PlayerKill, "Kill"
    }, {
      PlayerEventId.PlayerStart, "Start"
    }, {
      PlayerEventId.PlayerSuicide, "Suicide"
    }, {
      PlayerEventId.PlayerThrow, "Throw"
    }, {
      PlayerEventId.PlayerDefusedBomb, "Defused-Bomb"
    }, {
      PlayerEventId.PlayerDieHeadshot, "Die-Headshot"
    }, {
      PlayerEventId.PlayerDieKnife, "Die-Knife"
    }, {
      PlayerEventId.PlayerDieMolotov, "Die-Molotov"
    }, {
      PlayerEventId.PlayerKillHeadshot, "Kill-Headshot"
    }, {
      PlayerEventId.PlayerKillKnife, "Kill-Knife"
    }, {
      PlayerEventId.PlayerKillMolotov, "Kill-Molotov"
    }, {
      PlayerEventId.PlayerRescuedHostage, "Rescued-Hostage"
    }, {
      PlayerEventId.PlayerDieHeGrenade, "Die-He"
    }, {
      PlayerEventId.PlayerKillHeGrenade, "Kill-He"
    }
  };

  private static readonly Dictionary<Tuple<string, PlayerEventId>, List<FileInfo>> EventAudioFiles = new();

  public static readonly List<CsPlayer> Players;

  static PlayerEvents() {
    const string filePath = "players.json";
    Players = LoadPlayersFromFile(filePath);

    foreach (var player in Players) Console.WriteLine($"Loaded player: {player.Name}");

    foreach (var player in Players)
    foreach (PlayerEventId playerEventId in Enum.GetValues(typeof(PlayerEventId))) {
      var files = LoadSoundFiles(player.Name, EventAudioFolderByEventId(playerEventId));
      EventAudioFiles.Add(new Tuple<string, PlayerEventId>(player.Name, playerEventId), files);
    }
  }

  private static List<CsPlayer> LoadPlayersFromFile(string filePath) {
    List<CsPlayer> players;

    try {
      var json = File.ReadAllText(filePath);
      players = JsonConvert.DeserializeObject<List<CsPlayer>>(json);
    }
    catch (Exception e) {
      Console.WriteLine("Error loading player data: " + e.Message);
      players = new List<CsPlayer>();
    }

    return players;
  }

  private static List<FileInfo> LoadSoundFiles(string playerName, string eventAudioFolder) {
    var searchPath = Config.PathPlayerEventSounds + playerName + "/" + eventAudioFolder;

    try {
      var wavFiles = Directory.GetFiles(searchPath, "*.wav", SearchOption.AllDirectories);
      var mp3Files = Directory.GetFiles(searchPath, "*.mp3", SearchOption.AllDirectories);
      var audioFiles = wavFiles.Concat(mp3Files).ToArray();
      return audioFiles.Select(fileName => new FileInfo(fileName)).ToList();
    }
    catch (Exception) {
      // Not all players have comments for every event, this is ok.
      return new List<FileInfo>();
    }
  }

  public static FileInfo? RandomSoundFileByPlayerAndEventId(string playerName, PlayerEventId playerEventId) {
    var files = EventAudioFiles[new Tuple<string, PlayerEventId>(playerName, playerEventId)];

    if (files.Count == 0) return null;

    var random = new Random();
    var randomIndex = random.Next(0, files.Count);
    return files[randomIndex];
  }

  public static string EventAudioFolderByEventId(PlayerEventId playerEventId) {
    return EventAudioFolder[playerEventId];
  }
}