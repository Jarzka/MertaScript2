namespace MertaScript.Events;

public abstract class GameEvents {
  private static readonly CommentableGameEvent[] GameEventsList = {
    new() {
      Id = GameEventId.ScoreEnemyTeam, CommentProbability = 100, Importance = 5
    },
    new() {
      Id = GameEventId.ScoreClientTeam, CommentProbability = 100, Importance = 5
    },
    new() {
      Id = GameEventId.ScoreClientTeam10, CommentProbability = 100, Importance = 5
    },
    new() {
      Id = GameEventId.ScoreClientTeam11, CommentProbability = 100, Importance = 5
    },
    new() {
      Id = GameEventId.ScoreClientTeam20, CommentProbability = 100, Importance = 5
    },
    new() {
      Id = GameEventId.ScoreClientTeam21, CommentProbability = 100, Importance = 5
    },
    new() {
      Id = GameEventId.ScoreClientTeam22, CommentProbability = 100, Importance = 5
    },
    new() {
      Id = GameEventId.ScoreClientTeam30, CommentProbability = 100, Importance = 5
    },
    new() {
      Id = GameEventId.ScoreClientTeam31, CommentProbability = 100, Importance = 5
    },
    new() {
      Id = GameEventId.ScoreClientTeam32, CommentProbability = 100, Importance = 5
    },
    new() {
      Id = GameEventId.ScoreClientTeam40, CommentProbability = 100, Importance = 5
    },
    new() {
      Id = GameEventId.ScoreClientTeam51, CommentProbability = 100, Importance = 5
    },
    new() {
      Id = GameEventId.ScoreClientTeam61, CommentProbability = 100, Importance = 5
    },
    new() {
      Id = GameEventId.ScoreClientTeam17, CommentProbability = 100, Importance = 5
    },
    new() {
      Id = GameEventId.ScoreClientTeam23, CommentProbability = 100, Importance = 5
    },
    new() {
      Id = GameEventId.ScoreEnemyTeam10, CommentProbability = 100, Importance = 5
    },
    new() {
      Id = GameEventId.ScoreEnemyTeam20, CommentProbability = 100, Importance = 5
    },
    new() {
      Id = GameEventId.ScoreEnemyTeam31, CommentProbability = 100, Importance = 5
    },
    new() {
      Id = GameEventId.ScoreEnemyTeam11, CommentProbability = 100, Importance = 5
    },
    new() {
      Id = GameEventId.ScoreEnemyTeam22, CommentProbability = 100, Importance = 5
    },
    new() {
      Id = GameEventId.ScoreEvenClient, CommentProbability = 100, Importance = 5
    },
    new() {
      Id = GameEventId.RoundDraw, CommentProbability = 100, Importance = 5
    },
    new() {
      Id = GameEventId.WinClient, CommentProbability = 100, Importance = 10
    },
    new() {
      Id = GameEventId.WinEnemy, CommentProbability = 100, Importance = 10
    },
    new() {
      Id = GameEventId.Suicide, CommentProbability = 100, Importance = 5
    },
    new() {
      Id = GameEventId.KillKnifeClientTeam, CommentProbability = 100, Importance = 8
    },
    new() {
      Id = GameEventId.KillKnifeEnemyTeam, CommentProbability = 100, Importance = 5
    },
    new() {
      Id = GameEventId.KillHegrenadeClientTeam, CommentProbability = 100, Importance = 5
    },
    new() {
      Id = GameEventId.KillHegrenadeEnemyTeam, CommentProbability = 100, Importance = 5
    },
    new() {
      Id = GameEventId.KillInfernoClientTeam, CommentProbability = 100, Importance = 5
    },
    new() {
      Id = GameEventId.KillInfernoEnemyTeam, CommentProbability = 100, Importance = 5
    },
    new() {
      Id = GameEventId.TeamkillerClientTeam, CommentProbability = 100, Importance = 4
    },
    new() {
      Id = GameEventId.TeamkillerEnemyTeam, CommentProbability = 100, Importance = 4
    },
    new() {
      Id = GameEventId.KillHeadshotClientTeam, CommentProbability = 100, Importance = 3
    },
    new() {
      Id = GameEventId.KillHeadshotEnemyTeam, CommentProbability = 100, Importance = 3
    },
    new() {
      Id = GameEventId.RoundStartClientTeamWinning, CommentProbability = 40, Importance = 3
    },
    new() {
      Id = GameEventId.RoundStartClientTeamWinningMassively, CommentProbability = 70, Importance = 3
    },
    new() {
      Id = GameEventId.RoundStartEnemyTeamWinning, CommentProbability = 40, Importance = 3
    },
    new() {
      Id = GameEventId.RoundStartEnemyTeamWinningMassively, CommentProbability = 70, Importance = 3
    },
    new() {
      Id = GameEventId.BombPlantedClientTeam, CommentProbability = 20, Importance = 3
    },
    new() {
      Id = GameEventId.BombPlantedEnemyTeam, CommentProbability = 30, Importance = 3
    },
    new() {
      Id = GameEventId.DefuseClientTeam, CommentProbability = 30, Importance = 3
    },
    new() {
      Id = GameEventId.HostageTakenEnemyTeam, CommentProbability = 50, Importance = 3
    },
    new() {
      Id = GameEventId.KillHeadshotMachineGunClientTeam, CommentProbability = 100, Importance = 2
    },
    new() {
      Id = GameEventId.KillHeadshotJuhisClientTeam, CommentProbability = 100, Importance = 2
    },
    new() {
      Id = GameEventId.Time002, CommentProbability = 30, Importance = 2
    },
    new() {
      Id = GameEventId.Time003, CommentProbability = 30, Importance = 2
    },
    new() {
      Id = GameEventId.Time010, CommentProbability = 30, Importance = 2
    },
    new() {
      Id = GameEventId.Time015, CommentProbability = 30, Importance = 2
    },
    new() {
      Id = GameEventId.Time028, CommentProbability = 30, Importance = 1
    },
    new() {
      Id = GameEventId.Time020, CommentProbability = 30, Importance = 1
    },
    new() {
      Id = GameEventId.Time030, CommentProbability = 30, Importance = 1
    },
    new() {
      Id = GameEventId.Time040, CommentProbability = 30, Importance = 1
    },
    new() {
      Id = GameEventId.Time060, CommentProbability = 30, Importance = 1
    }
  };

  private static readonly Dictionary<GameEventId, List<FileInfo>> EventAudioFiles;

  private static readonly Dictionary<GameEventId, string> EventAudioFolder;

  static GameEvents() {
    EventAudioFolder = new Dictionary<GameEventId, string> {
      { GameEventId.ScoreEnemyTeam, "score-enemy" },
      { GameEventId.ScoreClientTeam, "score-client" },
      { GameEventId.ScoreClientTeam10, "score-client-1-0" },
      { GameEventId.ScoreClientTeam11, "score-client-1-1" },
      { GameEventId.ScoreClientTeam20, "score-client-2-0" },
      { GameEventId.ScoreClientTeam21, "score-client-2-1" },
      { GameEventId.ScoreClientTeam22, "score-client-2-2" },
      { GameEventId.ScoreClientTeam30, "score-client-3-0" },
      { GameEventId.ScoreClientTeam31, "score-client-3-1" },
      { GameEventId.ScoreClientTeam32, "score-client-3-2" },
      { GameEventId.ScoreClientTeam40, "score-client-4-0" },
      { GameEventId.ScoreClientTeam51, "score-client-5-1" },
      { GameEventId.ScoreClientTeam61, "score-client-6-1" },
      { GameEventId.ScoreClientTeam17, "score-client-1-7" },
      { GameEventId.ScoreClientTeam23, "score-client-2-3" },
      { GameEventId.ScoreEnemyTeam10, "score-enemy-1-0" },
      { GameEventId.ScoreEnemyTeam20, "score-enemy-2-0" },
      { GameEventId.ScoreEnemyTeam31, "score-enemy-3-1" },
      { GameEventId.ScoreEnemyTeam11, "score-enemy-1-1" },
      { GameEventId.ScoreEnemyTeam22, "score-enemy-2-2" },
      { GameEventId.ScoreEvenClient, "score-even-client" },
      { GameEventId.RoundDraw, "round-draw" },
      { GameEventId.WinClient, "score-win-client" },
      { GameEventId.WinEnemy, "score-win-enemy" },
      { GameEventId.Suicide, "suicide" },
      { GameEventId.KillKnifeClientTeam, "kill-knife-client" },
      { GameEventId.KillKnifeEnemyTeam, "kill-knife-enemy" },
      { GameEventId.KillHegrenadeClientTeam, "kill-hegrenade-client" },
      { GameEventId.KillHegrenadeEnemyTeam, "kill-hegrenade-enemy" },
      { GameEventId.KillInfernoClientTeam, "kill-inferno-client" },
      { GameEventId.KillInfernoEnemyTeam, "kill-inferno-enemy" },
      { GameEventId.TeamkillerClientTeam, "teamkiller-client" },
      { GameEventId.TeamkillerEnemyTeam, "teamkiller-enemy" },
      { GameEventId.KillHeadshotClientTeam, "kill-headshot-client" },
      { GameEventId.KillHeadshotEnemyTeam, "kill-headshot-enemy" },
      { GameEventId.RoundStartClientTeamWinning, "round-start-client-winning" },
      { GameEventId.RoundStartClientTeamWinningMassively, "round-start-client-winning-massively" },
      { GameEventId.RoundStartEnemyTeamWinning, "round-start-enemy-winning" },
      { GameEventId.RoundStartEnemyTeamWinningMassively, "round-start-enemy-winning-massively" },
      { GameEventId.BombPlantedClientTeam, "bomb-planted-client" },
      { GameEventId.BombPlantedEnemyTeam, "bomb-planted-enemy" },
      { GameEventId.DefuseClientTeam, "defuse-client" },
      { GameEventId.HostageTakenEnemyTeam, "hostage-taken-enemy" },
      { GameEventId.KillHeadshotMachineGunClientTeam, "kill-headshot-machine-gun-client" },
      { GameEventId.KillHeadshotJuhisClientTeam, "kill-headshot-juhis-client" },
      { GameEventId.Time002, "time-0-02" },
      { GameEventId.Time003, "time-0-03" },
      { GameEventId.Time010, "time-0-10" },
      { GameEventId.Time015, "time-0-15" },
      { GameEventId.Time028, "time-0-28" },
      { GameEventId.Time020, "time-0-20" },
      { GameEventId.Time030, "time-0-30" },
      { GameEventId.Time040, "time-0-40" },
      { GameEventId.Time060, "time-1-00" }
    };
    EventAudioFiles = new Dictionary<GameEventId, List<FileInfo>> {
      { GameEventId.ScoreEnemyTeam, LoadSoundFiles(EventAudioFolderByEventId(GameEventId.ScoreEnemyTeam)) },
      { GameEventId.ScoreClientTeam, LoadSoundFiles(EventAudioFolderByEventId(GameEventId.ScoreClientTeam)) },
      { GameEventId.ScoreClientTeam10, LoadSoundFiles(EventAudioFolderByEventId(GameEventId.ScoreClientTeam10)) },
      { GameEventId.ScoreClientTeam11, LoadSoundFiles(EventAudioFolderByEventId(GameEventId.ScoreClientTeam11)) },
      { GameEventId.ScoreClientTeam20, LoadSoundFiles(EventAudioFolderByEventId(GameEventId.ScoreClientTeam20)) },
      { GameEventId.ScoreClientTeam21, LoadSoundFiles(EventAudioFolderByEventId(GameEventId.ScoreClientTeam21)) },
      { GameEventId.ScoreClientTeam22, LoadSoundFiles(EventAudioFolderByEventId(GameEventId.ScoreClientTeam22)) },
      { GameEventId.ScoreClientTeam30, LoadSoundFiles(EventAudioFolderByEventId(GameEventId.ScoreClientTeam30)) },
      { GameEventId.ScoreClientTeam31, LoadSoundFiles(EventAudioFolderByEventId(GameEventId.ScoreClientTeam31)) },
      { GameEventId.ScoreClientTeam32, LoadSoundFiles(EventAudioFolderByEventId(GameEventId.ScoreClientTeam32)) },
      { GameEventId.ScoreClientTeam40, LoadSoundFiles(EventAudioFolderByEventId(GameEventId.ScoreClientTeam40)) },
      { GameEventId.ScoreClientTeam51, LoadSoundFiles(EventAudioFolderByEventId(GameEventId.ScoreClientTeam51)) },
      { GameEventId.ScoreClientTeam61, LoadSoundFiles(EventAudioFolderByEventId(GameEventId.ScoreClientTeam61)) },
      { GameEventId.ScoreClientTeam17, LoadSoundFiles(EventAudioFolderByEventId(GameEventId.ScoreClientTeam17)) },
      { GameEventId.ScoreClientTeam23, LoadSoundFiles(EventAudioFolderByEventId(GameEventId.ScoreClientTeam23)) },
      { GameEventId.ScoreEnemyTeam10, LoadSoundFiles(EventAudioFolderByEventId(GameEventId.ScoreEnemyTeam10)) },
      { GameEventId.ScoreEnemyTeam20, LoadSoundFiles(EventAudioFolderByEventId(GameEventId.ScoreEnemyTeam20)) },
      { GameEventId.ScoreEnemyTeam31, LoadSoundFiles(EventAudioFolderByEventId(GameEventId.ScoreEnemyTeam31)) },
      { GameEventId.ScoreEnemyTeam11, LoadSoundFiles(EventAudioFolderByEventId(GameEventId.ScoreEnemyTeam11)) },
      { GameEventId.ScoreEnemyTeam22, LoadSoundFiles(EventAudioFolderByEventId(GameEventId.ScoreEnemyTeam22)) },
      { GameEventId.ScoreEvenClient, LoadSoundFiles(EventAudioFolderByEventId(GameEventId.ScoreEvenClient)) },
      { GameEventId.RoundDraw, LoadSoundFiles(EventAudioFolderByEventId(GameEventId.RoundDraw)) },
      { GameEventId.WinClient, LoadSoundFiles(EventAudioFolderByEventId(GameEventId.WinClient)) },
      { GameEventId.WinEnemy, LoadSoundFiles(EventAudioFolderByEventId(GameEventId.WinEnemy)) },
      { GameEventId.Suicide, LoadSoundFiles(EventAudioFolderByEventId(GameEventId.Suicide)) },
      { GameEventId.KillKnifeClientTeam, LoadSoundFiles(EventAudioFolderByEventId(GameEventId.KillKnifeClientTeam)) },
      { GameEventId.KillKnifeEnemyTeam, LoadSoundFiles(EventAudioFolderByEventId(GameEventId.KillKnifeEnemyTeam)) }, {
        GameEventId.KillHegrenadeClientTeam,
        LoadSoundFiles(EventAudioFolderByEventId(GameEventId.KillHegrenadeClientTeam))
      }, {
        GameEventId.KillHegrenadeEnemyTeam,
        LoadSoundFiles(EventAudioFolderByEventId(GameEventId.KillHegrenadeEnemyTeam))
      }, {
        GameEventId.KillInfernoClientTeam, LoadSoundFiles(EventAudioFolderByEventId(GameEventId.KillInfernoClientTeam))
      },
      { GameEventId.KillInfernoEnemyTeam, LoadSoundFiles(EventAudioFolderByEventId(GameEventId.KillInfernoEnemyTeam)) },
      { GameEventId.TeamkillerClientTeam, LoadSoundFiles(EventAudioFolderByEventId(GameEventId.TeamkillerClientTeam)) },
      { GameEventId.TeamkillerEnemyTeam, LoadSoundFiles(EventAudioFolderByEventId(GameEventId.TeamkillerEnemyTeam)) }, {
        GameEventId.KillHeadshotClientTeam,
        LoadSoundFiles(EventAudioFolderByEventId(GameEventId.KillHeadshotClientTeam))
      }, {
        GameEventId.KillHeadshotEnemyTeam, LoadSoundFiles(EventAudioFolderByEventId(GameEventId.KillHeadshotEnemyTeam))
      }, {
        GameEventId.RoundStartClientTeamWinning,
        LoadSoundFiles(EventAudioFolderByEventId(GameEventId.RoundStartClientTeamWinning))
      }, {
        GameEventId.RoundStartClientTeamWinningMassively,
        LoadSoundFiles(EventAudioFolderByEventId(GameEventId.RoundStartClientTeamWinningMassively))
      }, {
        GameEventId.RoundStartEnemyTeamWinning,
        LoadSoundFiles(EventAudioFolderByEventId(GameEventId.RoundStartEnemyTeamWinning))
      }, {
        GameEventId.RoundStartEnemyTeamWinningMassively,
        LoadSoundFiles(EventAudioFolderByEventId(GameEventId.RoundStartEnemyTeamWinningMassively))
      }, {
        GameEventId.BombPlantedClientTeam, LoadSoundFiles(EventAudioFolderByEventId(GameEventId.BombPlantedClientTeam))
      },
      { GameEventId.BombPlantedEnemyTeam, LoadSoundFiles(EventAudioFolderByEventId(GameEventId.BombPlantedEnemyTeam)) },
      { GameEventId.DefuseClientTeam, LoadSoundFiles(EventAudioFolderByEventId(GameEventId.DefuseClientTeam)) }, {
        GameEventId.HostageTakenEnemyTeam, LoadSoundFiles(EventAudioFolderByEventId(GameEventId.HostageTakenEnemyTeam))
      }, {
        GameEventId.KillHeadshotMachineGunClientTeam,
        LoadSoundFiles(EventAudioFolderByEventId(GameEventId.KillHeadshotMachineGunClientTeam))
      }, {
        GameEventId.KillHeadshotJuhisClientTeam,
        LoadSoundFiles(EventAudioFolderByEventId(GameEventId.KillHeadshotJuhisClientTeam))
      },
      { GameEventId.Time002, LoadSoundFiles(EventAudioFolderByEventId(GameEventId.Time002)) },
      { GameEventId.Time003, LoadSoundFiles(EventAudioFolderByEventId(GameEventId.Time003)) },
      { GameEventId.Time010, LoadSoundFiles(EventAudioFolderByEventId(GameEventId.Time010)) },
      { GameEventId.Time015, LoadSoundFiles(EventAudioFolderByEventId(GameEventId.Time015)) },
      { GameEventId.Time028, LoadSoundFiles(EventAudioFolderByEventId(GameEventId.Time028)) },
      { GameEventId.Time020, LoadSoundFiles(EventAudioFolderByEventId(GameEventId.Time020)) },
      { GameEventId.Time030, LoadSoundFiles(EventAudioFolderByEventId(GameEventId.Time030)) },
      { GameEventId.Time040, LoadSoundFiles(EventAudioFolderByEventId(GameEventId.Time040)) },
      { GameEventId.Time060, LoadSoundFiles(EventAudioFolderByEventId(GameEventId.Time060)) }
    };
  }

  public static CommentableGameEvent EventById(GameEventId id) {
    return GameEventsList.FirstOrDefault(item => item.Id == id) ?? throw new InvalidOperationException();
  }

  private static List<FileInfo> LoadSoundFiles(string path) {
    var searchPath = Path.Combine(Config.PathGameEventSounds, path);

    try {
      var wavFiles = Directory.GetFiles(searchPath, "*.wav", SearchOption.AllDirectories);
      var mp3Files = Directory.GetFiles(searchPath, "*.mp3", SearchOption.AllDirectories);
      var audioFiles = wavFiles.Concat(mp3Files).ToArray();
      return audioFiles.Select(fileName => new FileInfo(fileName)).ToList();
    }
    catch (DirectoryNotFoundException e) {
      Console.WriteLine("Warning: " + searchPath + " is empty.");
      return new List<FileInfo>();
    }
  }

  public static FileInfo? RandomSoundFileByEventId(GameEventId gameEventId) {
    var files = EventAudioFiles[gameEventId];

    if (files.Count == 0) return null;

    var random = new Random();
    var randomIndex = random.Next(0, files.Count);
    return files[randomIndex];
  }

  public static string EventAudioFolderByEventId(GameEventId gameEventId) {
    return EventAudioFolder[gameEventId];
  }
}