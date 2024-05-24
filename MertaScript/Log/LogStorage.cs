namespace MertaScript.Log;

public class LogStorage {
  private static readonly List<string> _log = new();
  private static readonly object lockObject = new();

  public static List<string> Log {
    get {
      lock (lockObject) {
        return _log;
      }
    }
  }

  public static void StorePlayerKilledPlayer(string sourcePlayer, string sourceTeam, string targetPlayer,
    string targetTeam,
    string details) {
    var text =
      $"Player \"{sourcePlayer}\" (Team {sourceTeam}) killed player \"{targetPlayer}\" (Team {targetTeam}) ({details})";
    StoreText(text);
  }

  public static void StoreText(string line) {
    Log.Add(line);
  }

  public static void Clear() {
    Log.Clear();
  }

  public static List<string> GetLog() {
    return Log;
  }

  public static string AsText() {
    return string.Join("\n", Log);
  }
}