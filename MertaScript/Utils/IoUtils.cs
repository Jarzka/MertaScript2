namespace MertaScript.Utils;

public abstract class IoUtils {
  public static long FileModificationTimeAsUnixTimestamp(string path) {
    var lastModified = File.GetLastWriteTimeUtc(path);
    return (long)(lastModified - new DateTime(1970, 1, 1)).TotalSeconds;
  }
}