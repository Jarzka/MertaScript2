using MertaScript.Ai;
using MertaScript.Audio;
using MertaScript.Events;
using MertaScript.Log;
using MertaScript.Network;
using MertaScript.Utils;
using NAudio.Wave;

namespace MertaScript.EventHandling;

internal class GameCommentator {
  private const int CheckTimeIntervalInSeconds = 1;
  private static GameCommentator? _instance;

  private readonly int _hostageTakenTimeBonus =
    int.Parse(Config.GetValueFromConfigFile("host_hostage_taken_time_bonus"));

  private AudioClip? _audioClip;

  private int _c4Time = int.Parse(Config.GetValueFromConfigFile("host_c4_time"));
  private long _checkTimeTimestampInSeconds;
  private bool _clientScoredLastRound;
  private int _clientTeamPoints;
  private TeamSide? _clientTeamSide;
  private int _enemyTeamPoints;
  private bool _hostageTakenTimeBonusGivenInThisRound;
  private bool _isGameRoundActive;
  private bool _isMatchEnded;
  private int _lastAudioFileDurationInSeconds;
  private int _lastAudioPlayPriority;
  private long _lastAudioPlayTimestampInSeconds;
  private bool _majorGameActionsInThisRound;
  private int _maxRounds = int.Parse(Config.GetValueFromConfigFile("host_max_rounds"));
  private long _roundStartTimestampInSeconds;
  private long _roundTimeInSeconds = int.Parse(Config.GetValueFromConfigFile("host_round_time"));

  private GameCommentator() {
    _lastAudioPlayPriority = 0;
    _lastAudioPlayTimestampInSeconds = 0;
    _lastAudioFileDurationInSeconds = 0;
    _roundStartTimestampInSeconds = 0;
    _isGameRoundActive = false;
    _checkTimeTimestampInSeconds = 0;
    _clientTeamSide = null;
    _clientTeamPoints = 0;
    _enemyTeamPoints = 0;
    _clientScoredLastRound = false;
    _hostageTakenTimeBonusGivenInThisRound = false;
  }

  public static GameCommentator GetInstance() {
    return _instance ??= new GameCommentator();
  }

  public void SetRoundTime(int timeInSeconds) {
    _roundTimeInSeconds = timeInSeconds;
    _roundStartTimestampInSeconds = 0;
  }

  public void SetMaxRounds(int rounds) {
    _maxRounds = rounds;
  }

  public void SetC4Time(int time) {
    _c4Time = time;
  }

  private void SetTimeLeftToC4Time() {
    // The easiest way to set the new amount of remaining time is to "fake" the round start timestamp.
    // This may not be the most convenient way to solve this problem, but it works and does not
    // cause problems.
    SetRoundStartTime(TimeUtils.UnixTimestamp() - _roundTimeInSeconds + _c4Time);
  }

  private void AddHostageTakenTimeBonusIfNecessary() {
    // The easiest way to set the new amount of remaining time is to "fake" the round start timestamp.
    // This may not be the most convenient way to solve this problem, but it works and does not
    // cause problems.
    if (_hostageTakenTimeBonusGivenInThisRound) return;
    SetRoundStartTime(TimeUtils.UnixTimestamp() - GetRoundTimePassed() + _hostageTakenTimeBonus);
    _hostageTakenTimeBonusGivenInThisRound = true;
  }

  /**
   * Randomly returns true or false based on the given percentage.
   */
  private static bool ProbabilityToBool(int percent) {
    var random = new Random();
    var value = random.Next(0, 101);
    return percent >= value;
  }

  public bool IsPlayingAudio() {
    return _lastAudioPlayTimestampInSeconds > 0 &&
           TimeUtils.UnixTimestamp() <
           _lastAudioPlayTimestampInSeconds + _lastAudioFileDurationInSeconds;
  }

  /**
   * Returns true if there is no audio currently playing.
   * If audio is playing, return true only if the given event is more important.
   */
  private bool EventPriorityToBool(float importance) {
    return !IsPlayingAudio() || importance >= _lastAudioPlayPriority;
  }

  public void HandleEventAsAudioComment(GameEventId gameEventId) {
    var importance = GameEvents.EventById(gameEventId).Priority;
    var probability = GameEvents.EventById(gameEventId).CommentProbability;

    if (!ProbabilityToBool(probability) ||
        !EventPriorityToBool(importance)) return;

    var audioFolder = GameEvents.EventAudioFolderByEventId(gameEventId);
    var file = GameEvents.RandomSoundFileByEventId(gameEventId);
    SendPlaySoundCommandToClients(audioFolder, file, importance);
  }

  public void HandleEventAsLiveAudioComment(string audioFilePath) {
    SendPlayLiveSoundCommandToClients(audioFilePath, 7); // Live audio has hardcoded priority of 7
  }

  private void SendPlayLiveSoundCommandToClients(string audioFilePath, int audioFilePriority) {
    var file = new FileInfo(audioFilePath);
    var fileBytes = File.ReadAllBytes(audioFilePath);
    var base64Audio = Convert.ToBase64String(fileBytes);

    _lastAudioFileDurationInSeconds = GetFileDuration(file) + 2; // Include 2 seconds of transferring overload
    _lastAudioPlayTimestampInSeconds = TimeUtils.UnixTimestamp();
    _lastAudioPlayPriority = audioFilePriority;

    if (!NetworkManager.GetInstance().IsHost()) return;

    var message = "<PLAY_LIVE_SOUND_GAME|" + base64Audio + ">";
    NetworkManager.GetInstance().SendMessageToClients(message, "<PLAY_LIVE_SOUND_GAME|BASE64>");
  }


  private void SendPlaySoundCommandToClients(string audioFolder, FileSystemInfo? file, int audioFilePriority) {
    if (file == null) return;

    var path = Config.PathGameEventSounds + audioFolder + "/" + file.Name;

    _lastAudioFileDurationInSeconds = GetFileDuration(file);
    _lastAudioPlayTimestampInSeconds = TimeUtils.UnixTimestamp();
    _lastAudioPlayPriority = audioFilePriority;

    if (!NetworkManager.GetInstance().IsHost()) return;

    var message = "<PLAY_SOUND_GAME|" + path + ">";
    NetworkManager.GetInstance().SendMessageToClients(message, message);
  }

  public void PlayFile(string path) {
    Console.WriteLine("Playing: " + path);

    if (_audioClip != null && _audioClip.IsPlaying()) {
      _audioClip?.Stop();
      Thread.Sleep(10); // Wait for the audio file to stop playing
    }

    _audioClip = new AudioClip();
    _audioClip.Play(path, 0.85f);
  }

  public void PlayBase64Audio(string base64Audio) {
    Console.WriteLine("Playing live sound!");
    var audioBytes = Convert.FromBase64String(base64Audio);

    if (_audioClip != null && _audioClip.IsPlaying()) {
      _audioClip?.Stop();
      Thread.Sleep(10); // Wait for the audio file to stop playing
    }

    _audioClip = new AudioClip();
    _audioClip.Play(audioBytes, 0.85f);
  }

  private static int GetFileDuration(FileSystemInfo file) {
    using var reader = new AudioFileReader(file.FullName);
    return reader.TotalTime.TotalSeconds > 0 ? (int)reader.TotalTime.TotalSeconds : 0;
  }

  public int GetClientTeamPoints() {
    return _clientTeamPoints;
  }

  public int GetEnemyTeamPoints() {
    return _enemyTeamPoints;
  }

  public TeamSide? GetClientTeamSide() {
    return _clientTeamSide;
  }

  private long GetRoundTimeLeft() {
    if (_roundStartTimestampInSeconds > 0) return _roundTimeInSeconds - GetRoundTimePassed();

    return 0;
  }

  private long GetRoundTimePassed() {
    if (_roundStartTimestampInSeconds > 0) return TimeUtils.UnixTimestamp() - _roundStartTimestampInSeconds;

    return 0;
  }

  private void StartNewRound() {
    _isGameRoundActive = true;
    _roundStartTimestampInSeconds = TimeUtils.UnixTimestamp();
    _hostageTakenTimeBonusGivenInThisRound = false;
  }

  private void SetRoundStartTime(long timeInSeconds) {
    _roundStartTimestampInSeconds = timeInSeconds;
  }

  private void ResetPoints() {
    Console.WriteLine("Reseting team points");
    _clientTeamPoints = 0;
    _enemyTeamPoints = 0;
  }

  public void ResetRoundTime() {
    Console.WriteLine("Reseting round time");
    _roundStartTimestampInSeconds = 0;
  }

  public void UpdateState() {
    if (TimeUtils.UnixTimestamp() < _checkTimeTimestampInSeconds + CheckTimeIntervalInSeconds) return;

    CheckTime();
  }

  private void CheckTime() {
    _checkTimeTimestampInSeconds = TimeUtils.UnixTimestamp();
    var roundTimeLeft = GetRoundTimeLeft();

    if (roundTimeLeft > 0) Console.WriteLine("Round time left: " + roundTimeLeft);

    if (!_isGameRoundActive) return; // No need to comment time if the game round is not active

    var logStorageTimeLeftPrefix = "Round time left: ";

    switch (roundTimeLeft) {
      case 2:
        LogStorage.StoreText($"{logStorageTimeLeftPrefix} {roundTimeLeft} seconds");
        HandleEventAsAudioComment(GameEventId.Time002);
        break;
      case 3:
        LogStorage.StoreText($"{logStorageTimeLeftPrefix} {roundTimeLeft} seconds");
        HandleEventAsAudioComment(GameEventId.Time003);
        break;
      case 10:
        LogStorage.StoreText($"{logStorageTimeLeftPrefix} {roundTimeLeft} seconds");
        HandleEventAsAudioComment(GameEventId.Time010);
        break;
      case 15:
        LogStorage.StoreText($"{logStorageTimeLeftPrefix} {roundTimeLeft} seconds");
        HandleEventAsAudioComment(GameEventId.Time015);
        break;
      case 20:
        LogStorage.StoreText($"{logStorageTimeLeftPrefix} {roundTimeLeft} seconds");
        HandleEventAsAudioComment(GameEventId.Time020);
        break;
      case 28:
        LogStorage.StoreText($"{logStorageTimeLeftPrefix} {roundTimeLeft} seconds");
        HandleEventAsAudioComment(GameEventId.Time028);
        break;
      case 30:
        LogStorage.StoreText($"{logStorageTimeLeftPrefix} {roundTimeLeft} seconds");
        HandleEventAsAudioComment(GameEventId.Time030);
        break;
      case 40:
        LogStorage.StoreText($"{logStorageTimeLeftPrefix} {roundTimeLeft} seconds");
        HandleEventAsAudioComment(GameEventId.Time040);
        break;
      case 60:
        LogStorage.StoreText($"{logStorageTimeLeftPrefix} {roundTimeLeft} seconds");
        HandleEventAsAudioComment(GameEventId.Time060);
        break;
    }
  }

  public void HandleEventRoundEnd(TeamSide scoredSide, int scoredPoints) {
    _isGameRoundActive = false;

    // If old points are greater than the new value, then we know that a new match as started
    if (_clientTeamSide == scoredSide && scoredPoints < _clientTeamPoints)
      ResetPoints();
    else if (_clientTeamSide != scoredSide && scoredPoints < _enemyTeamPoints) ResetPoints();

    // Probably warmup round? Skip.
    if ((GetClientTeamPoints() <= 1) & (GetEnemyTeamPoints() <= 1) && _majorGameActionsInThisRound == false) return;

    var clientTeamPointsOld = GetClientTeamPoints();
    var enemyTeamPointsOld = GetEnemyTeamPoints();

    if (_clientTeamSide == scoredSide) {
      Console.WriteLine("Client team scored " + scoredPoints);
      _clientTeamPoints = scoredPoints;
      _clientScoredLastRound = true;
    }
    else {
      Console.WriteLine("Enemy team scored " + scoredPoints);
      _enemyTeamPoints = scoredPoints;
      _clientScoredLastRound = false;
    }

    HandleEventScore(clientTeamPointsOld, enemyTeamPointsOld);
    ResetRoundTime();
  }

  private void HandleEventScore(int clientTeamPointsOld, int enemyTeamPointsOld) {
    if (GetClientTeamPoints() > clientTeamPointsOld) {
      Console.WriteLine("Client team got more points");

      if (GetClientTeamPoints() > _maxRounds / 2) {
        HandleEventAsAudioComment(GameEventId.WinClient);
        _isMatchEnded = true;
      }
      else if (GetClientTeamPoints() == 1 && GetEnemyTeamPoints() == 0) {
        HandleEventAsAudioComment(GameEventId.ScoreClientTeam10);
      }
      else if (GetClientTeamPoints() == 1 && GetEnemyTeamPoints() == 1) {
        HandleEventAsAudioComment(GameEventId.ScoreClientTeam11);
      }
      else if (GetClientTeamPoints() == 2 && GetEnemyTeamPoints() == 0) {
        HandleEventAsAudioComment(GameEventId.ScoreClientTeam20);
      }
      else if (GetClientTeamPoints() == 2 && GetEnemyTeamPoints() == 1) {
        HandleEventAsAudioComment(GameEventId.ScoreClientTeam21);
      }
      else if (GetClientTeamPoints() == 2 && GetEnemyTeamPoints() == 3) {
        HandleEventAsAudioComment(GameEventId.ScoreClientTeam23);
      }
      else if (GetClientTeamPoints() == 3 && GetEnemyTeamPoints() == 0) {
        HandleEventAsAudioComment(GameEventId.ScoreClientTeam30);
      }
      else if (GetClientTeamPoints() == 3 && GetEnemyTeamPoints() == 1) {
        HandleEventAsAudioComment(GameEventId.ScoreClientTeam31);
      }
      else if (GetClientTeamPoints() == 3 && GetEnemyTeamPoints() == 2) {
        HandleEventAsAudioComment(GameEventId.ScoreClientTeam32);
      }
      else if (GetClientTeamPoints() == 4 && GetEnemyTeamPoints() == 0) {
        HandleEventAsAudioComment(GameEventId.ScoreClientTeam40);
      }
      else if (GetClientTeamPoints() == 5 && GetEnemyTeamPoints() == 1) {
        HandleEventAsAudioComment(GameEventId.ScoreClientTeam51);
      }
      else if (GetClientTeamPoints() == 6 && GetEnemyTeamPoints() == 1) {
        HandleEventAsAudioComment(GameEventId.ScoreClientTeam61);
      }
      else if (GetClientTeamPoints() == 1 && GetEnemyTeamPoints() == 7) {
        HandleEventAsAudioComment(GameEventId.ScoreClientTeam17);
      }
      else if (GetClientTeamPoints() == GetEnemyTeamPoints()) {
        HandleEventAsAudioComment(GameEventId.ScoreEvenClient);
      }
      else {
        HandleEventAsAudioComment(GameEventId.ScoreClientTeam);
      }
    }

    if (GetEnemyTeamPoints() <= enemyTeamPointsOld) return;

    Console.WriteLine("Enemy team got more points");

    if (GetEnemyTeamPoints() > _maxRounds / 2) {
      HandleEventAsAudioComment(GameEventId.WinEnemy);
      _isMatchEnded = true;
    }
    else if (GetClientTeamPoints() == 0 && GetEnemyTeamPoints() == 1) {
      HandleEventAsAudioComment(GameEventId.ScoreEnemyTeam10);
    }
    else if (GetClientTeamPoints() == 1 && GetEnemyTeamPoints() == 1) {
      HandleEventAsAudioComment(GameEventId.ScoreEnemyTeam11);
    }
    else if (GetClientTeamPoints() == 2 && GetEnemyTeamPoints() == 2) {
      HandleEventAsAudioComment(GameEventId.ScoreEnemyTeam22);
    }
    else if (GetClientTeamPoints() == 0 && GetEnemyTeamPoints() == 2) {
      HandleEventAsAudioComment(GameEventId.ScoreEnemyTeam20);
    }
    else if (GetClientTeamPoints() == 1 && GetEnemyTeamPoints() == 3) {
      HandleEventAsAudioComment(GameEventId.ScoreEnemyTeam31);
    }
    else {
      HandleEventAsAudioComment(GameEventId.ScoreEnemyTeam);
    }
  }

  private void SetClientTeamSide(TeamSide side) {
    _clientTeamSide = side;
  }

  public void HandleEventRoundStart() {
    _majorGameActionsInThisRound = false;
    var isRoundEnded = _clientTeamPoints >= _maxRounds / 2 || _enemyTeamPoints >= _maxRounds / 2;
    const int massiveScoreDifference = 5;

    // Round start event may occur in the game log after the match has ended.
    // Do not handle if the match has ended.
    // Loading a new map should reset everything.
    if (isRoundEnded) return;

    StartNewRound();

    // Do not comment the round start if AI-based analysis is being generated.
    if (CommentGenerator.IsGeneratingComment) return;

    // Note: Massive difference needs to be checked first.
    if (GetClientTeamPoints() > GetEnemyTeamPoints() + massiveScoreDifference)
      HandleEventAsAudioComment(GameEventId.RoundStartClientTeamWinningMassively);
    else if (GetClientTeamPoints() > GetEnemyTeamPoints())
      HandleEventAsAudioComment(GameEventId.RoundStartClientTeamWinning);
    // Do not insult the client team if they have just scored
    else if (GetClientTeamPoints() + massiveScoreDifference < GetEnemyTeamPoints() && !_clientScoredLastRound)
      HandleEventAsAudioComment(GameEventId.RoundStartEnemyTeamWinningMassively);
    else if (GetClientTeamPoints() < GetEnemyTeamPoints() && !_clientScoredLastRound)
      HandleEventAsAudioComment(GameEventId.RoundStartEnemyTeamWinning);
  }

  public void SetClientTeam(TeamSide teamSide) {
    SetClientTeamSide(teamSide);
  }

  public void HandleEventHostageTaken() {
    _majorGameActionsInThisRound = true;

    AddHostageTakenTimeBonusIfNecessary();

    if (GetClientTeamSide() == TeamSide.T)
      HandleEventAsAudioComment(GameEventId.HostageTakenEnemyTeam);
  }

  public void HandleEventClientBeginsBombDefuse() {
    if (GetClientTeamSide() == TeamSide.Ct)
      HandleEventAsAudioComment(GameEventId.DefuseClientTeam);
  }

  public void HandleEventBombPlanted() {
    if (!_isGameRoundActive) return;

    _majorGameActionsInThisRound = true;

    SetTimeLeftToC4Time();

    HandleEventAsAudioComment(GetClientTeamSide() == TeamSide.T
      ? GameEventId.BombPlantedClientTeam
      : GameEventId.BombPlantedEnemyTeam);
  }

  public void HandleEventLoadingMap() {
    ResetPoints();
    ResetRoundTime();
    _isMatchEnded = false;
    _majorGameActionsInThisRound = false;
  }

  public void HandleEventSomeoneKilledSomeone() {
    _majorGameActionsInThisRound = true;
  }

  public bool IsMatchEnded() {
    return _isMatchEnded;
  }
}