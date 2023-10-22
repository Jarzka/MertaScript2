using System.Text.RegularExpressions;
using MertaScript.EventHandling;
using MertaScript.Events;

namespace MertaScript;

public abstract class PlayerEventHandler {
  public static bool ScanLine(string line, List<string> previousLines) {
    return ScanLinePlayerDefusedBomb(line) ||
           ScanLinePlayerRescuedHostage(line) ||
           ScanLinePlayerKillHeadshot(line) ||
           ScanLinePlayerKillKnife(line) ||
           ScanLinePlayerKillHE(line) ||
           ScanLinePlayerKillMolotov(line) ||
           ScanLinePlayerKill(line) ||
           ScanLinePlayerDieHeadshot(line) ||
           ScanLinePlayerDieKnife(line) ||
           ScanLinePlayerDieHe(line) ||
           ScanLinePlayerDieMolotov(line) ||
           ScanLinePlayerDie(line) ||
           ScanLinePlayerStart(line) ||
           ScanLinePlayerSuicide(line, previousLines) ||
           ScanLinePlayerThrow(line);
  }

  private static bool ScanLinePlayerRescuedHostage(string line) {
    foreach (var player in PlayerEvents.Players)
    foreach (var alias in player.PlayerNameWithAliases()) {
      var regEx = alias;
      regEx += ".*triggered.*Rescued_A_Hostage.*";
      var match = Regex.Match(line, regEx);
      if (!match.Success) continue;

      Console.WriteLine($"Catch: {line}");
      PlayerCommentator.HandleEvent(player.Name, PlayerEventId.PlayerRescuedHostage);
      return true;
    }

    return false;
  }

  private static bool ScanLinePlayerDefusedBomb(string line) {
    foreach (var player in PlayerEvents.Players)
    foreach (var alias in player.PlayerNameWithAliases()) {
      var regEx = alias;
      regEx += ".*triggered.*Defused_The_Bomb.*";
      var match = Regex.Match(line, regEx);
      if (!match.Success) continue;

      Console.WriteLine($"Catch: {line}");
      PlayerCommentator.HandleEvent(player.Name, PlayerEventId.PlayerDefusedBomb);
      return true;
    }

    return false;
  }

  private static bool ScanLinePlayerKillHeadshot(string line) {
    foreach (var player in PlayerEvents.Players)
    foreach (var alias in player.PlayerNameWithAliases()) {
      var regEx = alias;
      regEx += ".* killed \".*";
      regEx += "headshot";
      var match = Regex.Match(line, regEx);
      if (!match.Success) continue;

      Console.WriteLine($"Catch: {line}");
      PlayerCommentator.HandleEvent(player.Name, PlayerEventId.PlayerKillHeadshot);
      return true;
    }

    return false;
  }

  private static bool ScanLinePlayerKillKnife(string line) {
    foreach (var player in PlayerEvents.Players)
    foreach (var alias in player.PlayerNameWithAliases()) {
      var regEx = alias;
      regEx += ".* killed \".*";
      regEx += "knife";
      var match = Regex.Match(line, regEx);
      if (!match.Success) continue;

      Console.WriteLine($"Catch: {line}");
      PlayerCommentator.HandleEvent(player.Name, PlayerEventId.PlayerKillKnife);
      return true;
    }

    return false;
  }

  private static bool ScanLinePlayerKillHE(string line) {
    foreach (var player in PlayerEvents.Players)
    foreach (var alias in player.PlayerNameWithAliases()) {
      var regEx = alias;
      regEx += ".* killed \".*";
      regEx += "hegrenade";
      var match = Regex.Match(line, regEx);
      if (!match.Success) continue;

      Console.WriteLine($"Catch: {line}");
      PlayerCommentator.HandleEvent(player.Name, PlayerEventId.PlayerKillHeGrenade);
      return true;
    }

    return false;
  }

  private static bool ScanLinePlayerKillMolotov(string line) {
    foreach (var player in PlayerEvents.Players)
    foreach (var alias in player.PlayerNameWithAliases()) {
      var regEx = alias;
      regEx += ".* killed \".*";
      regEx += "inferno";
      var match = Regex.Match(line, regEx);
      if (!match.Success) continue;

      Console.WriteLine($"Catch: {line}");
      PlayerCommentator.HandleEvent(player.Name, PlayerEventId.PlayerKillMolotov);
      return true;
    }

    return false;
  }

  private static bool ScanLinePlayerKill(string line) {
    foreach (var player in PlayerEvents.Players)
    foreach (var alias in player.PlayerNameWithAliases()) {
      var regEx = alias;
      regEx += ".* killed \".*";
      var match = Regex.Match(line, regEx);
      if (!match.Success) continue;

      Console.WriteLine($"Catch: {line}");
      PlayerCommentator.HandleEvent(player.Name, PlayerEventId.PlayerKill);
      return true;
    }

    return false;
  }

  private static bool ScanLinePlayerDieHeadshot(string line) {
    foreach (var player in PlayerEvents.Players)
    foreach (var alias in player.PlayerNameWithAliases()) {
      var regEx = ".* killed \".*";
      regEx += alias;
      regEx += ".*headshot.*";
      var match = Regex.Match(line, regEx);
      if (!match.Success) continue;

      Console.WriteLine($"Catch: {line}");
      PlayerCommentator.HandleEvent(player.Name, PlayerEventId.PlayerDieHeadshot);
      return true;
    }

    return false;
  }

  private static bool ScanLinePlayerDieKnife(string line) {
    foreach (var player in PlayerEvents.Players)
    foreach (var alias in player.PlayerNameWithAliases()) {
      var regEx = ".* killed \".*";
      regEx += alias;
      regEx += ".*knife.*";
      var match = Regex.Match(line, regEx);
      if (!match.Success) continue;

      Console.WriteLine($"Catch: {line}");
      PlayerCommentator.HandleEvent(player.Name, PlayerEventId.PlayerDieKnife);
      return true;
    }

    return false;
  }

  private static bool ScanLinePlayerDieHe(string line) {
    foreach (var player in PlayerEvents.Players)
    foreach (var alias in player.PlayerNameWithAliases()) {
      var regEx = ".* killed \".*";
      regEx += alias;
      regEx += ".*hegrenade.*";
      var match = Regex.Match(line, regEx);
      if (!match.Success) continue;

      Console.WriteLine($"Catch: {line}");
      PlayerCommentator.HandleEvent(player.Name, PlayerEventId.PlayerDieHeGrenade);
      return true;
    }

    return false;
  }

  private static bool ScanLinePlayerDieMolotov(string line) {
    foreach (var player in PlayerEvents.Players)
    foreach (var alias in player.PlayerNameWithAliases()) {
      var regEx = ".* killed \".*";
      regEx += alias;
      regEx += ".*inferno.*";
      var match = Regex.Match(line, regEx);
      if (!match.Success) continue;

      Console.WriteLine($"Catch: {line}");
      PlayerCommentator.HandleEvent(player.Name, PlayerEventId.PlayerDieMolotov);
      return true;
    }

    return false;
  }

  private static bool ScanLinePlayerDie(string line) {
    foreach (var player in PlayerEvents.Players)
    foreach (var alias in player.PlayerNameWithAliases()) {
      var regEx = ".* killed \".*";
      regEx += alias;
      var match = Regex.Match(line, regEx);
      if (!match.Success) continue;

      Console.WriteLine($"Catch: {line}");
      PlayerCommentator.HandleEvent(player.Name, PlayerEventId.PlayerDie);
      return true;
    }

    return false;
  }

  private static bool ScanLinePlayerStart(string line) {
    foreach (var player in PlayerEvents.Players)
    foreach (var alias in player.PlayerNameWithAliases()) {
      var regEx = alias;
      regEx += ".* entered the game.*";
      var match = Regex.Match(line, regEx);
      if (!match.Success) continue;

      Console.WriteLine($"Catch: {line}");
      PlayerCommentator.HandleEvent(player.Name, PlayerEventId.PlayerStart);
      return true;
    }

    return false;
  }

  private static bool ScanLinePlayerSuicide(string line, IReadOnlyCollection<string> previousLines) {
    foreach (var player in PlayerEvents.Players)
    foreach (var alias in player.PlayerNameWithAliases()) {
      var regEx = alias;
      regEx += ".* committed suicide .*";
      var match = Regex.Match(line, regEx);

      if (!match.Success ||
          (match.Success &&
           // Suicide by C4 bomb or team switch should be ignored
           (previousLines.Any(previousLine => previousLine.Contains("by the bomb")) ||
            previousLines.Any(previousLine => previousLine.Contains("switched from team"))))) continue;

      Console.WriteLine($"Catch: {line}");
      PlayerCommentator.HandleEvent(player.Name, PlayerEventId.PlayerSuicide);
      return true;
    }

    return false;
  }

  private static bool ScanLinePlayerThrow(string line) {
    foreach (var player in PlayerEvents.Players)
    foreach (var alias in player.PlayerNameWithAliases()) {
      var regEx = alias;
      regEx += ".*threw.*";
      var match = Regex.Match(line, regEx);
      if (!match.Success) continue;

      Console.WriteLine($"Catch: {line}");
      PlayerCommentator.HandleEvent(player.Name, PlayerEventId.PlayerThrow);
      return true;
    }

    return false;
  }
}