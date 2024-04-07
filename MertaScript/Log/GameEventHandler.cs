using System.Text.RegularExpressions;
using MertaScript.EventHandling;
using MertaScript.Events;

namespace MertaScript.Log;

public abstract class GameEventHandler {
  private static readonly List<string> ClientTeamPlayerNames = Config.GetClientTeamPlayerNamesFromConfigFile();

  // Constructs Regex from client team player names in the following format:
  // (player1|player2|player3)
  private static string ConstructRegexClientTeamPlayers() {
    var regexPattern = "(";
    regexPattern += string.Join("|", ClientTeamPlayerNames);
    regexPattern += ")";
    return regexPattern;
  }

  private static bool IsClientTeamPlayerKiller(string input) {
    var regexPattern = ConstructRegexClientTeamPlayers() + ".+killed.+";
    var match = Regex.Match(input, regexPattern);
    return match.Success;
  }

  private static bool IsClientTeamPlayerVictim(string input) {
    var regexPattern = ".+killed.+" + ConstructRegexClientTeamPlayers();
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
      ScanLineEnemyTeamKillEnemyHeadshot(line) ||
      ScanLineClientTeamKillEnemyKnife(line) ||
      ScanLineEnemyTeamKillClientTeamKnife(line) ||
      ScanLineClientTeamKillHegrenade(line) ||
      ScanLineEnemyTeamKillClientTeamHegrenade(line) ||
      ScanLineClientTeamKillEnemyInferno(line) ||
      ScanLineEnemyTeamKillClientInferno(line) ||
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
      ScanLineDefuse(line) ||
      ScanLineBombPlant(line) ||
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

    var clientCtKilledSomeone = ConstructRegexClientTeamPlayers();
    clientCtKilledSomeone += ".+<CT>.* killed .+";
    var clientTKilledSomeone = ConstructRegexClientTeamPlayers();
    clientTKilledSomeone += ".+<TERRORIST>.* killed .+";

    var clientCtKilledSomeoneMatch = Regex.Match(line, clientCtKilledSomeone);
    var clientTKilledSomeoneMatch = Regex.Match(line, clientTKilledSomeone);

    if (someoneKilledSomeoneMatch.Success) {
      Console.WriteLine($"Catch: {line}");
      GameCommentator.GetInstance().HandleEventSomeoneKilledSomeone();
    }

    if (clientCtKilledSomeoneMatch.Success) {
      Console.WriteLine($"Catch: {line}");
      GameCommentator.GetInstance().HandleEventClientSwitchedTeam(TeamSide.Ct);
    }

    if (clientTKilledSomeoneMatch.Success) {
      Console.WriteLine($"Catch: {line}");
      GameCommentator.GetInstance().HandleEventClientSwitchedTeam(TeamSide.T);
    }

    return false; // This event should never stop other events from being checked
  }

  private static bool ScanLineClientTeamTeamkiller(string line) {
    var regEx = ConstructRegexClientTeamPlayers();
    regEx += ".* killed \".*";
    regEx += ConstructRegexClientTeamPlayers();
    var match = Regex.Match(line, regEx);

    if (!match.Success) return false;

    Console.WriteLine($"Catch: {line}");
    Console.WriteLine("Teamkiller in client team!");
    GameCommentator.GetInstance().HandleEventAsAudioComment(GameEventId.TeamkillerClientTeam);
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
    return true;
  }

  private static bool ScanLineClientTeamKillEnemyMachineGunHeadshot(string line) {
    var regEx = ConstructRegexClientTeamPlayers();
    regEx += ".* killed \".*";
    regEx += "with.*(negev|m249).*";
    regEx += "headshot";
    var match = Regex.Match(line, regEx);

    if (!match.Success) return false;
    if (IsClientTeamPlayerVictim(match.Groups[0].Value)) return false;

    Console.WriteLine($"Catch: {line}");
    GameCommentator.GetInstance().HandleEventAsAudioComment(GameEventId.KillHeadshotMachineGunClientTeam);
    return true;
  }

  private static bool ScanLineClientTeamJuhisKillEnemyHeadshot(string line) {
    var regEx = "(Juhiz|Juhis|Raikiri)";
    regEx += ".* killed \".*";
    regEx += ".*with.*elite.*";
    regEx += "headshot";
    var match = Regex.Match(line, regEx);

    if (!match.Success) return false;
    if (IsClientTeamPlayerVictim(match.Groups[0].Value)) return false;

    Console.WriteLine($"Catch: {line}");
    GameCommentator.GetInstance().HandleEventAsAudioComment(GameEventId.KillHeadshotJuhisClientTeam);
    return true;
  }

  private static bool ScanLineClientTeamKillEnemyHeadshot(string line) {
    var regEx = ConstructRegexClientTeamPlayers();
    regEx += ".* killed \".*";
    regEx += "headshot";
    var match = Regex.Match(line, regEx);

    if (!match.Success) return false;
    if (IsClientTeamPlayerVictim(match.Groups[0].Value)) return false;

    Console.WriteLine($"Catch: {line}");
    GameCommentator.GetInstance().HandleEventAsAudioComment(GameEventId.KillHeadshotClientTeam);
    return true;
  }

  private static bool ScanLineEnemyTeamKillEnemyHeadshot(string line) {
    var regEx = ".* killed \".*";
    regEx += ConstructRegexClientTeamPlayers();
    regEx += ".+headshot";
    var match = Regex.Match(line, regEx);

    if (!match.Success) return false;
    if (IsClientTeamPlayerKiller(match.Groups[0].Value)) return false;

    Console.WriteLine($"Catch: {line}");
    GameCommentator.GetInstance().HandleEventAsAudioComment(GameEventId.KillHeadshotEnemyTeam);
    return true;
  }

  private static bool ScanLineClientTeamKillEnemyKnife(string line) {
    var regEx = ConstructRegexClientTeamPlayers() + ".* killed \".*with.+knife";
    var match = Regex.Match(line, regEx);

    if (!match.Success) return false;
    if (IsClientTeamPlayerVictim(match.Groups[0].Value)) return false;

    Console.WriteLine("Catch: " + line);
    GameCommentator.GetInstance().HandleEventAsAudioComment(GameEventId.KillKnifeClientTeam);
    return true;
  }

  private static bool ScanLineClientTeamKillHegrenade(string line) {
    var regEx = ConstructRegexClientTeamPlayers() + ".* killed \".*with.+hegrenade";
    var match = Regex.Match(line, regEx);

    if (!match.Success) return false;
    if (IsClientTeamPlayerVictim(match.Groups[0].Value)) return false;

    Console.WriteLine("Catch: " + line);
    GameCommentator.GetInstance().HandleEventAsAudioComment(GameEventId.KillHegrenadeClientTeam);
    return false;
  }

  private static bool ScanLineEnemyTeamKillClientTeamHegrenade(string line) {
    // Actually, the RegEx thinks that someone, who does not play in client team, killed client team player.
    // However, it is very likely that the killer was enemy team player.

    var regEx = ".* killed \".*" + ConstructRegexClientTeamPlayers() + ".+with.+hegrenade";
    var match = Regex.Match(line, regEx);

    if (!match.Success) return false;

    if (IsClientTeamPlayerKiller(match.Groups[0].Value)) return false;
    Console.WriteLine("Catch: " + line);
    GameCommentator.GetInstance().HandleEventAsAudioComment(GameEventId.KillHegrenadeEnemyTeam);
    return true;
  }

  private static bool ScanLineEnemyTeamKillClientTeamKnife(string line) {
    var regEx = ".* killed \".*";
    regEx += ConstructRegexClientTeamPlayers();
    regEx += ".+with.+";
    regEx += "knife";
    var match = Regex.Match(line, regEx);

    if (!match.Success) return false;
    if (IsClientTeamPlayerKiller(match.Groups[0].Value)) return false;

    Console.WriteLine("Catch: " + line);
    GameCommentator.GetInstance().HandleEventAsAudioComment(GameEventId.KillKnifeEnemyTeam);
    return true;
  }

  private static bool ScanLineClientTeamKillEnemyInferno(string line) {
    var regEx = ConstructRegexClientTeamPlayers();
    regEx += ".* killed \".*";
    regEx += "with.+";
    regEx += "inferno";
    var match = Regex.Match(line, regEx);

    if (!match.Success) return false;
    if (IsClientTeamPlayerVictim(match.Groups[0].Value)) return false;

    Console.WriteLine("Catch: " + line);
    GameCommentator.GetInstance().HandleEventAsAudioComment(GameEventId.KillInfernoClientTeam);
    return true;
  }

  private static bool ScanLineEnemyTeamKillClientInferno(string line) {
    var regEx = ".* killed \".*";
    regEx += ConstructRegexClientTeamPlayers();
    regEx += ".+with.+";
    regEx += "inferno";
    var match = Regex.Match(line, regEx);

    if (!match.Success) return false;
    if (IsClientTeamPlayerKiller(match.Groups[0].Value)) return false;

    Console.WriteLine("Catch: " + line);
    GameCommentator.GetInstance().HandleEventAsAudioComment(GameEventId.KillInfernoEnemyTeam);
    return true;
  }

  private static bool ScanLineSuicide(string line, IReadOnlyCollection<string> previousLines) {
    const string suicideRegEx = ".+committed suicide.+";
    var match = Regex.Match(line, suicideRegEx);

    if (!match.Success ||
        (match.Success &&
         // Suicide by C4 bomb or team switch should be ignored
         (previousLines.Any(previousLine => previousLine.Contains("by the bomb")) ||
          previousLines.Any(previousLine => previousLine.Contains("switched from team"))))) return false;

    Console.WriteLine("Catch: " + line);
    GameCommentator.GetInstance().HandleEventAsAudioComment(GameEventId.Suicide);
    return true;
  }

  private static bool ScanLineRoundDraw(string line) {
    var regEx = "World triggered.*";
    regEx += "Round_Draw";
    var match = Regex.Match(line, regEx);

    if (!match.Success) return false;

    Console.WriteLine("Catch: " + line);
    GameCommentator.GetInstance().HandleEventAsAudioComment(GameEventId.RoundDraw);
    return true;
  }

  private static bool ScanLineRoundStart(string line) {
    var regEx = "World triggered.*";
    regEx += "Round_Start";
    var match = Regex.Match(line, regEx);

    if (!match.Success) return false;

    Console.WriteLine("Catch: " + line);

    GameCommentator.GetInstance().HandleEventRoundStart();
    return true;
  }

  private static bool ScanLineRoundEnd(string line) {
    var regEx = ".*World triggered.*";
    regEx += "Round_End";
    var match = Regex.Match(line, regEx);

    if (!match.Success) return false;

    Console.WriteLine("Catch: " + line);
    GameCommentator.GetInstance().ResetRoundTime();
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

    GameCommentator.GetInstance().HandleEventRoundEnd(TeamSide.T, int.Parse(match2.Value));
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

    GameCommentator.GetInstance().HandleEventRoundEnd(TeamSide.Ct, int.Parse(match2.Value));
    return true;
  }

  private static bool ScanLineClientTeamPlayerJoinsTeam(string line) {
    // client team player joins T
    var regEx = ConstructRegexClientTeamPlayers();
    regEx += ".+switched from team.+";
    regEx += "to.*";
    regEx += "<TERRORIST>";
    var match = Regex.Match(line, regEx);

    if (match.Success) {
      Console.WriteLine("Catch: " + line);
      // We can assume that all client team players play on T
      GameCommentator.GetInstance().HandleEventClientSwitchedTeam(TeamSide.T);
      return true;
    }

    // client team player joins CT
    regEx = ConstructRegexClientTeamPlayers();
    regEx += ".+switched from team.+";
    regEx += "to.*";
    regEx += "<CT>";
    match = Regex.Match(line, regEx);

    if (!match.Success) return false;

    Console.WriteLine("Catch: " + line);
    // We can assume that all client team players play on CT
    GameCommentator.GetInstance().HandleEventClientSwitchedTeam(TeamSide.Ct);
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
    var regEx = "triggered.+";
    regEx += "Touched_A_Hostage";
    var match = Regex.Match(line, regEx);

    if (!match.Success) return false;

    Console.WriteLine("Catch: " + line);
    GameCommentator.GetInstance().HandleEventHostageTaken();
    return true;
  }

  private static bool ScanLineDefuse(string line) {
    var regEx = "triggered.+";
    regEx += "Begin_Bomb_Defuse";
    var match = Regex.Match(line, regEx);

    if (!match.Success) return false;

    Console.WriteLine("Catch: " + line);
    GameCommentator.GetInstance().HandleEventClientBeginsBombDefuse();
    return true;
  }

  private static bool ScanLineBombPlant(string line) {
    var regEx = "triggered.+";
    regEx += "Planted_The_Bomb";
    var match = Regex.Match(line, regEx);

    if (!match.Success) return false;

    Console.WriteLine("Catch: " + line);
    GameCommentator.GetInstance().HandleEventBombPlanted();

    return true;
  }

  private static bool ScanLineLoadingMap(string line) {
    const string regEx = "Loading map";
    var match = Regex.Match(line, regEx);

    if (!match.Success) return false;

    Console.WriteLine("Catch: " + line);
    GameCommentator.GetInstance().HandleEventLoadingMap();
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