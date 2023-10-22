namespace MertaScript.Utils;

public abstract class TimeUtils {
  public static long UnixTimestamp() {
    var dateTimeOffset = DateTimeOffset.UtcNow; // Gets the current UTC time
    var unixTimeSeconds = dateTimeOffset.ToUnixTimeSeconds();
    return unixTimeSeconds;
  }
}