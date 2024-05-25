using System.Text.RegularExpressions;
using MertaScript.Ai;
using MertaScript.EventHandling;
using MertaScript.Events;

namespace MertaScript.Log;

public abstract class GameEventHandler {
  private static readonly List<string> ClientTeamPlayerNames = Config.GetClientTeamPlayerNamesFromConfigFile();


  private static bool IsClientTeamPlayerKiller(string input) {
    var regexPattern = RegexHelper.ConstructRegexClientTeamPlayers() + ".+killed.+";
    var match = Regex.Match(input, regexPattern);
    return match.Success;
  }

  private static bool IsClientTeamPlayerVictim(string input) {
    var regexPattern = ".+killed.+" + RegexHelper.ConstructRegexClientTeamPlayers();
    var match = Regex.Match(input, regexPattern);
    return match.Success;
  }

  public static bool ScanLine(string line, List<string> previousLines) {
    return
      ScanLineSomeoneKilledSomeone(line) ||
      ScanLineClientTeamTeamkiller(line) ||
      ScanLineEnemyTeamTeamkiller(line) ||
      ScanLineClientTeamKillEnemyMachineGunHeadshot(line) ||
      ScanLineClientTeamJuhisKillEnemyHeadshot(line) ||
      ScanLineClientTeamKillEnemyHeadshot(line) ||
      ScanLineEnemyTeamKillClientHeadshot(line) ||
      ScanLineClientTeamKillEnemyKnife(line) ||
      ScanLineEnemyTeamKillClientTeamKnife(line) ||
      ScanLineClientTeamKillHegrenade(line) ||
      ScanLineEnemyTeamKillClientTeamHegrenade(line) ||
      ScanLineClientTeamKillEnemyInferno(line) ||
      ScanLineEnemyTeamKillClientInferno(line) ||
      ScanLineEnemyTeamKillClient(line) ||
      ScanLineClientTeamKillEnemy(line) ||
      ScanLineSuicide(line, previousLines) ||
      ScanLineRoundDraw(line) ||
      ScanLineRoundStart(line) ||
      ScanLineRoundEnd(line) ||
      ScanLineScoreCt(line) ||
      ScanLineScoreT(line) ||
      ScanLineClientTeamPlayerJoinsTeam(line) ||
      ScanLineMaxRounds(line) ||
      ScanLineC4Time(line) ||
      ScanLineTakeHostage(line) ||
      ScanLineRescueHostage(line) ||
      ScanLineBeginDefuse(line) ||
      ScanLineBombPlant(line) ||
      ScanLineBombExploded(line) ||
      ScanLineBombDefused(line) ||
      ScanLineLoadingMap(line) ||
      ScanLineGameEnd(line);
  }

  private static bool ScanLineSomeoneKilledSomeone(string line) {
    var someoneKilledSomeoneRegEx = ".* killed \".*";
    var someoneKilledSomeoneMatch = Regex.Match(line, someoneKilledSomeoneRegEx);

    if (someoneKilledSomeoneMatch.Success) {
      Console.WriteLine($"Catch: {line}");
      GameCommentator.GetInstance().HandleEventSomeoneKilledSomeone();
    }

    var clientCtKilledSomeone = RegexHelper.ConstructRegexClientTeamPlayers();
    clientCtKilledSomeone += ".+<CT>.* killed .+";
    var clientTKilledSomeone = RegexHelper.ConstructRegexClientTeamPlayers();
    clientTKilledSomeone += ".+<TERRORIST>.* killed .+";

    var clientCtKilledSomeoneMatch = Regex.Match(line, clientCtKilledSomeone);
    var clientTKilledSomeoneMatch = Regex.Match(line, clientTKilledSomeone);

    if (someoneKilledSomeoneMatch.Success) {
      Console.WriteLine($"Catch: {line}");
      GameCommentator.GetInstance().HandleEventSomeoneKilledSomeone();
    }

    if (clientCtKilledSomeoneMatch.Success) {
      Console.WriteLine($"Catch: {line}");
      GameCommentator.GetInstance().SetClientTeam(TeamSide.Ct);
    }

    if (clientTKilledSomeoneMatch.Success) {
      Console.WriteLine($"Catch: {line}");
      GameCommentator.GetInstance().SetClientTeam(TeamSide.T);
    }

    return false; // This event should never stop other events from being checked
  }

  private static bool ScanLineClientTeamTeamkiller(string line) {
    var regEx = RegexHelper.ConstructRegexClientTeamPlayers();
    regEx += ".* killed \".*";
    regEx += RegexHelper.ConstructRegexClientTeamPlayers();
    var match = Regex.Match(line, regEx);

    if (!match.Success) return false;

    Console.WriteLine($"Catch: {line}");
    Console.WriteLine("Teamkiller in client team!");
    GameCommentator.GetInstance().HandleEventAsAudioComment(GameEventId.TeamkillerClientTeam);
    LogStorage.StorePlayerKilledPlayer(RegexHelper.ResolveKillerSourcePlayer(line),
      Config.ClientTeamName,
      RegexHelper.ResolveKillerTargetPlayer(line),
      Config.ClientTeamName,
      "teamkiller!");

    return true;
  }

  private static bool ScanLineEnemyTeamTeamkiller(string line) {
    var regEx = ".* killed \".*";
    var match = Regex.Match(line, regEx);
    if (!match.Success) return false;

    if (IsClientTeamPlayerKiller(match.Groups[0].Value)
        || IsClientTeamPlayerVictim(match.Groups[0].Value)
        || line.Contains("by the bomb")) return false;

    Console.WriteLine($"Catch: {line}");
    Console.WriteLine("Teamkiller in enemy team!");
    GameCommentator.GetInstance().HandleEventAsAudioComment(GameEventId.TeamkillerEnemyTeam);
    LogStorage.StorePlayerKilledPlayer(RegexHelper.ResolveKillerSourcePlayer(line),
      Config.EnemyTeamName,
      RegexHelper.ResolveKillerTargetPlayer(line),
      Config.EnemyTeamName,
      "teamkiller!");

    return true;
  }

  private static bool ScanLineClientTeamKillEnemyMachineGunHeadshot(string line) {
    var regEx = RegexHelper.ConstructRegexClientTeamPlayers();
    regEx += ".* killed \".*";
    regEx += "with.*(negev|m249).*";
    regEx += "headshot";
    var match = Regex.Match(line, regEx);

    if (!match.Success) return false;
    if (IsClientTeamPlayerVictim(match.Groups[0].Value)) return false;

    Console.WriteLine($"Catch: {line}");
    GameCommentator.GetInstance().HandleEventAsAudioComment(GameEventId.KillHeadshotMachineGunClientTeam);
    LogStorage.StorePlayerKilledPlayer(RegexHelper.ResolveKillerSourcePlayer(line),
      Config.ClientTeamName,
      RegexHelper.ResolveKillerTargetPlayer(line),
      Config.EnemyTeamName,
      "with machine gun headhsot");

    return true;
  }

  private static bool ScanLineClientTeamJuhisKillEnemyHeadshot(string line) {
    var regEx = RegexHelper.ConstructRegexClientTeamPlayers();
    regEx += ".* killed \".*";
    regEx += ".*with.*elite.*";
    regEx += "headshot";
    var match = Regex.Match(line, regEx);

    if (!match.Success) return false;
    if (IsClientTeamPlayerVictim(match.Groups[0].Value)) return false;

    Console.WriteLine($"Catch: {line}");
    GameCommentator.GetInstance().HandleEventAsAudioComment(GameEventId.KillHeadshotJuhisClientTeam);
    LogStorage.StorePlayerKilledPlayer(RegexHelper.ResolveKillerSourcePlayer(line),
      Config.ClientTeamName,
      RegexHelper.ResolveKillerTargetPlayer(line),
      Config.EnemyTeamName,
      "headhsot");

    return true;
  }

  private static bool ScanLineClientTeamKillEnemyHeadshot(string line) {
    var regEx = RegexHelper.ConstructRegexClientTeamPlayers();
    regEx += ".* killed \".*";
    regEx += "headshot";
    var match = Regex.Match(line, regEx);

    if (!match.Success) return false;
    if (IsClientTeamPlayerVictim(match.Groups[0].Value)) return false;

    Console.WriteLine($"Catch: {line}");
    GameCommentator.GetInstance().HandleEventAsAudioComment(GameEventId.KillHeadshotClientTeam);
    LogStorage.StorePlayerKilledPlayer(RegexHelper.ResolveKillerSourcePlayer(line),
      Config.ClientTeamName,
      RegexHelper.ResolveKillerTargetPlayer(line),
      Config.EnemyTeamName,
      "headhsot");

    return true;
  }

  private static bool ScanLineClientTeamKillEnemy(string line) {
    // Assuming that specific kills have been processed already and this is just "normal" kill
    var regEx = RegexHelper.ConstructRegexClientTeamPlayers();
    regEx += ".* killed \".*";
    var match = Regex.Match(line, regEx);

    if (!match.Success) return false;
    if (IsClientTeamPlayerVictim(match.Groups[0].Value)) return false;

    Console.WriteLine($"Catch: {line}");
    LogStorage.StorePlayerKilledPlayer(RegexHelper.ResolveKillerSourcePlayer(line),
      Config.ClientTeamName,
      RegexHelper.ResolveKillerTargetPlayer(line),
      Config.EnemyTeamName,
      "with a gun");

    return true;
  }

  private static bool ScanLineEnemyTeamKillClientHeadshot(string line) {
    var regEx = ".* killed \".*";
    regEx += RegexHelper.ConstructRegexClientTeamPlayers();
    regEx += ".+headshot";
    var match = Regex.Match(line, regEx);

    if (!match.Success) return false;
    if (IsClientTeamPlayerKiller(match.Groups[0].Value)) return false;

    Console.WriteLine($"Catch: {line}");
    GameCommentator.GetInstance().HandleEventAsAudioComment(GameEventId.KillHeadshotEnemyTeam);
    LogStorage.StorePlayerKilledPlayer(RegexHelper.ResolveKillerSourcePlayer(line),
      Config.EnemyTeamName,
      RegexHelper.ResolveKillerTargetPlayer(line),
      Config.ClientTeamName,
      "headhsot");

    return true;
  }

  private static bool ScanLineEnemyTeamKillClient(string line) {
    // Assuming that specific kills have been processed already and this is just "normal" kill
    var regEx = ".* killed \".*";
    regEx += RegexHelper.ConstructRegexClientTeamPlayers();
    var match = Regex.Match(line, regEx);

    if (!match.Success) return false;
    if (IsClientTeamPlayerKiller(match.Groups[0].Value)) return false;

    Console.WriteLine($"Catch: {line}");
    LogStorage.StorePlayerKilledPlayer(RegexHelper.ResolveKillerSourcePlayer(line),
      Config.EnemyTeamName,
      RegexHelper.ResolveKillerTargetPlayer(line),
      Config.ClientTeamName,
      "with a gun");

    return true;
  }

  private static bool ScanLineClientTeamKillEnemyKnife(string line) {
    var regEx = RegexHelper.ConstructRegexClientTeamPlayers() + ".* killed \".*with.+knife";
    var match = Regex.Match(line, regEx);

    if (!match.Success) return false;
    if (IsClientTeamPlayerVictim(match.Groups[0].Value)) return false;

    Console.WriteLine("Catch: " + line);
    GameCommentator.GetInstance().HandleEventAsAudioComment(GameEventId.KillKnifeClientTeam);
    LogStorage.StorePlayerKilledPlayer(RegexHelper.ResolveKillerSourcePlayer(line),
      Config.ClientTeamName,
      RegexHelper.ResolveKillerTargetPlayer(line),
      Config.EnemyTeamName,
      "with knife");

    return true;
  }

  private static bool ScanLineClientTeamKillHegrenade(string line) {
    var regEx = RegexHelper.ConstructRegexClientTeamPlayers() + ".* killed \".*with.+hegrenade";
    var match = Regex.Match(line, regEx);

    if (!match.Success) return false;
    if (IsClientTeamPlayerVictim(match.Groups[0].Value)) return false;

    Console.WriteLine("Catch: " + line);
    GameCommentator.GetInstance().HandleEventAsAudioComment(GameEventId.KillHegrenadeClientTeam);
    LogStorage.StorePlayerKilledPlayer(RegexHelper.ResolveKillerSourcePlayer(line),
      Config.ClientTeamName,
      RegexHelper.ResolveKillerTargetPlayer(line),
      Config.EnemyTeamName,
      "with grenade");

    return false;
  }

  private static bool ScanLineEnemyTeamKillClientTeamHegrenade(string line) {
    // Actually, the RegEx thinks that someone, who does not play in client team, killed client team player.
    // However, it is very likely that the killer was enemy team player.

    var regEx = ".* killed \".*" + RegexHelper.ConstructRegexClientTeamPlayers() + ".+with.+hegrenade";
    var match = Regex.Match(line, regEx);

    if (!match.Success) return false;

    if (IsClientTeamPlayerKiller(match.Groups[0].Value)) return false;
    Console.WriteLine("Catch: " + line);
    GameCommentator.GetInstance().HandleEventAsAudioComment(GameEventId.KillHegrenadeEnemyTeam);
    LogStorage.StorePlayerKilledPlayer(RegexHelper.ResolveKillerSourcePlayer(line),
      Config.EnemyTeamName,
      RegexHelper.ResolveKillerTargetPlayer(line),
      Config.ClientTeamName,
      "with grenade");

    return true;
  }

  private static bool ScanLineEnemyTeamKillClientTeamKnife(string line) {
    var regEx = ".* killed \".*";
    regEx += RegexHelper.ConstructRegexClientTeamPlayers();
    regEx += ".+with.+";
    regEx += "knife";
    var match = Regex.Match(line, regEx);

    if (!match.Success) return false;
    if (IsClientTeamPlayerKiller(match.Groups[0].Value)) return false;

    Console.WriteLine("Catch: " + line);
    GameCommentator.GetInstance().HandleEventAsAudioComment(GameEventId.KillKnifeEnemyTeam);
    LogStorage.StorePlayerKilledPlayer(RegexHelper.ResolveKillerSourcePlayer(line),
      Config.EnemyTeamName,
      RegexHelper.ResolveKillerTargetPlayer(line),
      Config.ClientTeamName,
      "with knife");

    return true;
  }

  private static bool ScanLineClientTeamKillEnemyInferno(string line) {
    var regEx = RegexHelper.ConstructRegexClientTeamPlayers();
    regEx += ".* killed \".*";
    regEx += "with.+";
    regEx += "inferno";
    var match = Regex.Match(line, regEx);

    if (!match.Success) return false;
    if (IsClientTeamPlayerVictim(match.Groups[0].Value)) return false;

    Console.WriteLine("Catch: " + line);
    GameCommentator.GetInstance().HandleEventAsAudioComment(GameEventId.KillInfernoClientTeam);
    LogStorage.StorePlayerKilledPlayer(RegexHelper.ResolveKillerSourcePlayer(line),
      Config.ClientTeamName,
      RegexHelper.ResolveKillerTargetPlayer(line),
      Config.EnemyTeamName,
      "with molotov");

    return true;
  }

  private static bool ScanLineEnemyTeamKillClientInferno(string line) {
    var regEx = ".* killed \".*";
    regEx += RegexHelper.ConstructRegexClientTeamPlayers();
    regEx += ".+with.+";
    regEx += "inferno";
    var match = Regex.Match(line, regEx);

    if (!match.Success) return false;
    if (IsClientTeamPlayerKiller(match.Groups[0].Value)) return false;

    Console.WriteLine("Catch: " + line);
    GameCommentator.GetInstance().HandleEventAsAudioComment(GameEventId.KillInfernoEnemyTeam);
    LogStorage.StorePlayerKilledPlayer(RegexHelper.ResolveKillerSourcePlayer(line),
      Config.EnemyTeamName,
      RegexHelper.ResolveKillerTargetPlayer(line),
      Config.ClientTeamName,
      "with molotov");

    return true;
  }

  private static bool ScanLineSuicide(string line, IReadOnlyCollection<string> previousLines) {
    const string suicideRegEx = ".+committed suicide.+";
    var match = Regex.Match(line, suicideRegEx);

    if (!match.Success ||
        (match.Success &&
         // Suicide by C4 bomb, network disconnect or team switch should be ignored
         (previousLines.Any(previousLine => previousLine.Contains("disconnected")) ||
          previousLines.Any(previousLine => previousLine.Contains("by the bomb")) ||
          previousLines.Any(previousLine => previousLine.Contains("switched from team"))))) return false;

    Console.WriteLine("Catch: " + line);
    GameCommentator.GetInstance().HandleEventAsAudioComment(GameEventId.Suicide);
    LogStorage.StoreText(
      $"Player \"{RegexHelper.ResolveWhoCommitedSuicide(line)}\" (Team {RegexHelper.ResolveSourcePlayerTeam(line, RegexHelper.CommittedSuicideRegex)}) commited suicide");

    return true;
  }

  private static bool ScanLineRoundDraw(string line) {
    var regEx = "World triggered.*";
    regEx += "Round_Draw";
    var match = Regex.Match(line, regEx);

    if (!match.Success) return false;

    Console.WriteLine("Catch: " + line);
    GameCommentator.GetInstance().HandleEventAsAudioComment(GameEventId.RoundDraw);
    LogStorage.StoreText("The round ended in a draw");

    return true;
  }

  private static bool ScanLineRoundStart(string line) {
    var regEx = "World triggered.*";
    regEx += "Round_Start";
    var match = Regex.Match(line, regEx);

    if (!match.Success) return false;

    Console.WriteLine("Catch: " + line);

    GameCommentator.GetInstance().HandleEventRoundStart();
    LogStorage.StoreText("New round begins");

    return true;
  }

  private static bool ScanLineRoundEnd(string line) {
    var regEx = ".*World triggered.*";
    regEx += "Round_End";
    var match = Regex.Match(line, regEx);

    if (!match.Success) return false;

    Console.WriteLine("Catch: " + line);
    GameCommentator.GetInstance().ResetRoundTime();
    LogStorage.StoreText("Round ended");
    return true;
  }

  private static bool ScanLineScoreT(string line) {
    // L 08/01/2013 - 23:33:38: Team "TERRORIST" scored "4" with "5" players

    var regEx = "Team.+";
    regEx += "\"TERRORIST\".+";
    regEx += "scored.+?";
    regEx += "\\d+";
    var match = Regex.Match(line, regEx);

    if (!match.Success) return false;

    Console.WriteLine("Catch: " + line);
    // We need to get the score points. Do this by selecting the digits from the match
    var match2 = Regex.Match(match.Value, "\\d+");

    if (!match2.Success) return false;

    var currentPoints = int.Parse(match2.Value);
    var clientScored = GameCommentator.GetInstance().GetClientTeamSide() == TeamSide.T;

    // Store text first before GameCommentator updates team points.
    if (clientScored) {
      if (currentPoints > GameCommentator.GetInstance().GetClientTeamPoints())
        LogStorage.StoreText($"Team {Config.ClientTeamName} scored a point! They now have {currentPoints} points");
    }
    else {
      if (currentPoints > GameCommentator.GetInstance().GetEnemyTeamPoints())
        LogStorage.StoreText($"Team {Config.EnemyTeamName} scored a point! They now have {currentPoints} points");
    }

    GameCommentator.GetInstance().HandleEventRoundEnd(TeamSide.T, currentPoints);

    CommentGenerator.MaybeAnalyseLogToGenerateComment();

    return true;
  }

  private static bool ScanLineScoreCt(string line) {
    // L 08/01/2013 - 23:33:38: Team "CT" scored "1" with "5" players

    var regEx = "Team.+";
    regEx += "\"CT\".+";
    regEx += "scored.+?";
    regEx += "\\d+";
    var match = Regex.Match(line, regEx);

    if (!match.Success) return false;

    Console.WriteLine("Catch: " + line);
    // We need to get the score points. Do this by selecting the digits from the match
    var match2 = Regex.Match(match.Value, "\\d+");

    if (!match2.Success) return false;

    var currentPoints = int.Parse(match2.Value);
    var clientScored = GameCommentator.GetInstance().GetClientTeamSide() == TeamSide.Ct;

    // Store text first before GameCommentator updates team points.
    if (clientScored) {
      if (currentPoints > GameCommentator.GetInstance().GetClientTeamPoints())
        LogStorage.StoreText($"Team {Config.ClientTeamName} scored a point! They now have {currentPoints} points");
    }
    else {
      if (currentPoints > GameCommentator.GetInstance().GetEnemyTeamPoints())
        LogStorage.StoreText($"Team {Config.EnemyTeamName} scored a point! They now have {currentPoints} points");
    }

    GameCommentator.GetInstance().HandleEventRoundEnd(TeamSide.Ct, currentPoints);

    CommentGenerator.MaybeAnalyseLogToGenerateComment();

    return true;
  }

  private static bool ScanLineClientTeamPlayerJoinsTeam(string line) {
    // client team player joins T
    var regEx = RegexHelper.ConstructRegexClientTeamPlayers();
    regEx += ".+switched from team.+";
    regEx += "to.*";
    regEx += "<TERRORIST>";
    var match = Regex.Match(line, regEx);

    if (match.Success) {
      Console.WriteLine("Catch: " + line);
      // We can assume that all client team players play on T
      GameCommentator.GetInstance().SetClientTeam(TeamSide.T);
      return true;
    }

    // client team player joins CT
    regEx = RegexHelper.ConstructRegexClientTeamPlayers();
    regEx += ".+switched from team.+";
    regEx += "to.*";
    regEx += "<CT>";
    match = Regex.Match(line, regEx);

    if (!match.Success) return false;

    Console.WriteLine("Catch: " + line);
    // We can assume that all client team players play on CT
    GameCommentator.GetInstance().SetClientTeam(TeamSide.Ct);

    return true;
  }

  private static bool ScanLineMaxRounds(string line) {
    var regEx = "mp_maxrounds.+?\\d+";
    var match = Regex.Match(line, regEx);

    if (!match.Success) return false;

    Console.WriteLine("Catch: " + line);
    // We need to get the value. Do this by selecting the digits from the match
    var match2 = Regex.Match(match.Value, "\\d+");

    if (!match2.Success) return true;

    var maxRounds = int.Parse(match2.Value);
    GameCommentator.GetInstance().SetMaxRounds(maxRounds);
    Console.WriteLine("Max rounds changed to " + maxRounds);

    return true;
  }

  private static bool ScanLineC4Time(string line) {
    var regEx = "mp_c4timer.+?\\d+";
    var match = Regex.Match(line, regEx);

    if (!match.Success) return false;

    Console.WriteLine("Catch: " + line);
    // We need to get the value. Do this by selecting the digits from the match
    var match2 = Regex.Match(match.Value, "timer.+\\d+");

    if (!match2.Success) return true;

    var match3 = Regex.Match(match2.Value, "\\d+");

    if (!match3.Success) return true;

    var c4Time = int.Parse(match3.Value);
    GameCommentator.GetInstance().SetC4Time(c4Time);
    Console.WriteLine("C4 time changed to " + c4Time);

    return true;
  }

  private static bool ScanLineTakeHostage(string line) {
    var regEx = RegexHelper.TouchHostageRegex;
    var match = Regex.Match(line, regEx);

    if (!match.Success) return false;

    Console.WriteLine("Catch: " + line);
    GameCommentator.GetInstance().HandleEventHostageTaken();
    LogStorage.StoreText(
      $"Player {RegexHelper.ResolveSourcePlayer(line, RegexHelper.TouchHostageRegex)} is trying to save a hostage.");
    return true;
  }

  private static bool ScanLineRescueHostage(string line) {
    var regEx = RegexHelper.RescueHostageRegex;
    var match = Regex.Match(line, regEx);

    if (!match.Success) return false;

    Console.WriteLine("Catch: " + line);
    LogStorage.StoreText(
      $"Player {RegexHelper.ResolveSourcePlayer(line, RegexHelper.RescueHostageRegex)} saved a hostage!");
    return true;
  }

  private static bool ScanLineBeginDefuse(string line) {
    var regEx = RegexHelper.BeginBombDefuseRegex;
    var match = Regex.Match(line, regEx);

    if (!match.Success) return false;

    Console.WriteLine("Catch: " + line);
    GameCommentator.GetInstance().HandleEventClientBeginsBombDefuse();
    LogStorage.StoreText(
      $"Player \"{RegexHelper.ResolveSourcePlayer(line, RegexHelper.BeginBombDefuseRegex)}\" (Team {RegexHelper.ResolveSourcePlayerTeam(line, RegexHelper.BeginBombDefuseRegex)}) started defusing the bomb.");
    return true;
  }

  private static bool ScanLineBombPlant(string line) {
    var regEx = RegexHelper.PlantedTheBombRegex;
    var match = Regex.Match(line, regEx);

    if (!match.Success) return false;

    Console.WriteLine("Catch: " + line);
    GameCommentator.GetInstance().HandleEventBombPlanted();
    LogStorage.StoreText($"Player \"{
      RegexHelper.ResolveSourcePlayer(line, RegexHelper.PlantedTheBombRegex)
    }\" (Team {RegexHelper.ResolveSourcePlayerTeam(line, RegexHelper.PlantedTheBombRegex)}) planted the bomb");

    return true;
  }

  private static bool ScanLineBombExploded(string line) {
    const string regEx = "Target_Bombed";
    var match = Regex.Match(line, regEx);

    if (!match.Success) return false;

    Console.WriteLine("Catch: " + line);
    LogStorage.StoreText("Bomb exploded!");
    return true;
  }

  private static bool ScanLineBombDefused(string line) {
    const string regEx = "Notice_Bomb_Defused";
    var match = Regex.Match(line, regEx);

    if (!match.Success) return false;

    Console.WriteLine("Catch: " + line);
    LogStorage.StoreText("Bomb has been defused!");
    return true;
  }

  private static bool ScanLineLoadingMap(string line) {
    const string regEx = "Loading map";
    var match = Regex.Match(line, regEx);

    if (!match.Success) return false;

    Console.WriteLine("Catch: " + line);
    GameCommentator.GetInstance().HandleEventLoadingMap();
    LogStorage.Clear();
    LogStorage.StoreText("New match is about to begin! Both teams have 0 points.");
    return true;
  }

  private static bool ScanLineGameEnd(string line) {
    const string regEx = "Log file closed";
    var match = Regex.Match(line, regEx);

    if (!match.Success) return false;

    Console.WriteLine("Catch: " + line);
    return true;
  }
}