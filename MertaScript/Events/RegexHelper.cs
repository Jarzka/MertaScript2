using System.Text.RegularExpressions;

namespace MertaScript.Events;

public class RegexHelper {
  private static readonly List<string> ClientTeamPlayerNames = Config.GetClientTeamPlayerNamesFromConfigFile();

  public static readonly string KilledRegex = ".* killed \".*";
  public static readonly string CommittedSuicideRegex = ".+committed suicide.+";
  public static readonly string PlantedTheBombRegex = ".+triggered.+Planted_The_Bomb.+";
  public static readonly string BeginBombDefuseRegex = ".+triggered.+Begin_Bomb_Defuse.+";

  // Constructs Regex from client team player names in the following format:
  // (player1|player2|player3)
  public static string ConstructRegexClientTeamPlayers() {
    var regexPattern = "(";
    regexPattern += string.Join("|", ClientTeamPlayerNames);
    regexPattern += ")";
    return regexPattern;
  }

  public static string ResolveKillerSourcePlayer(string line) {
    return ResolveSourcePlayer(line, KilledRegex);
  }

  public static string ResolveKillerTargetPlayer(string line) {
    return ResolveTargetPlayer(line, KilledRegex);
  }

  public static string ResolveWhoCommitedSuicide(string line) {
    return ResolveSourcePlayer(line, CommittedSuicideRegex);
  }

  public static string ResolveSourcePlayer(string line, string action) {
    foreach (var client in ClientTeamPlayerNames) {
      var clientRegEx = $"{client}{action}";
      var clientRegexMatch = Regex.Match(line, clientRegEx);

      if (clientRegexMatch.Success) return client;
    }

    foreach (var player in PlayerEvents.Players) {
      var playerRegex = $"{player.Name}{action}";
      var playerRegexMatch = Regex.Match(line, playerRegex);
      if (playerRegexMatch.Success) return player.Name;

      foreach (var alias in player.Aliases) {
        var aliasRegex = $"{alias}{action}";
        var aliasRegexMatch = Regex.Match(line, aliasRegex);
        if (aliasRegexMatch.Success) return player.Name;
      }
    }

    throw new Exception("Unable to resolve source player from text: " + line);
  }

  public static string ResolveSourcePlayerTeam(string line, string action) {
    foreach (var client in ClientTeamPlayerNames) {
      var clientRegEx = $"{client}{action}";
      var clientRegexMatch = Regex.Match(line, clientRegEx);

      if (clientRegexMatch.Success) return Config.ClientTeamName;
    }

    foreach (var player in PlayerEvents.Players) {
      var playerRegex = $"{player.Name}{action}";
      var playerRegexMatch = Regex.Match(line, playerRegex);
      // Currently it is assumed that players.json contains only enemy team players
      if (playerRegexMatch.Success) return Config.EnemyTeamName;

      foreach (var alias in player.Aliases) {
        var aliasRegex = $"{alias}{action}";
        var aliasRegexMatch = Regex.Match(line, aliasRegex);
        // Currently it is assumed that players.json contains only enemy team players
        if (aliasRegexMatch.Success) return Config.EnemyTeamName;
      }
    }

    throw new Exception("Unable to resolve source player from text: " + line);
  }

  public static string ResolveTargetPlayer(string line, string action) {
    foreach (var client in ClientTeamPlayerNames) {
      var clientRegEx = $"{action}{client}";
      var clientRegexMatch = Regex.Match(line, clientRegEx);

      if (clientRegexMatch.Success) return client;
    }

    foreach (var player in PlayerEvents.Players) {
      var playerRegex = $"{action}{player.Name}";
      playerRegex = player.Name;
      var playerRegexMatch = Regex.Match(line, playerRegex);
      if (playerRegexMatch.Success) return player.Name;

      foreach (var alias in player.Aliases) {
        var aliasRegex = $"{action}{alias}";
        var aliasRegexMatch = Regex.Match(line, aliasRegex);
        if (aliasRegexMatch.Success) return player.Name;
      }
    }

    throw new Exception("Unable to resolve target player from text: " + line);
  }
}