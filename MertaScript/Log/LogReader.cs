using MertaScript.Utils;

namespace MertaScript.Log;

internal class LogReader {
  private const double CheckNewestLogFileIntervalInSeconds = 60;
  private const double LogFileMaxAgeInSeconds = 5 * 60;
  private const double ReadFileMinIntervalMs = 500;
  private static LogReader? _instance;
  private static double _newestLogFileCheckedTimestampInSeconds;
  private int _alreadyProcessesLinesCount;
  private string _currentLogFile = FindMostRecentlyEditedLogFile(Config.PathLogs);
  private DateTime _readFileTimestamp;

  public static LogReader GetInstance() {
    return _instance ??= new LogReader();
  }

  public void UpdateState() {
    var currentTime = DateTime.Now;

    if (currentTime < _readFileTimestamp.Add(TimeSpan.FromMilliseconds(ReadFileMinIntervalMs))) return;

    _readFileTimestamp = currentTime;
    _currentLogFile = CheckNewestLogFile(_currentLogFile);
    ProcessNewLinesInFile(_currentLogFile);
  }

  private static string FindMostRecentlyEditedLogFile(string pathLogs) {
    Console.WriteLine("Finding the active log file...");

    string mostRecentlyEditedFileName = null;
    double mostRecentlyEditedFileTime = 0;

    while (mostRecentlyEditedFileName == null) {
      try {
        var fileNames = Directory.GetFiles(pathLogs);
        foreach (var fileName in fileNames) {
          double modificationTimestamp = IoUtils.FileModificationTimeAsUnixTimestamp(fileName);
          if (TimeUtils.UnixTimestamp() < modificationTimestamp + LogFileMaxAgeInSeconds) {
            if (!(modificationTimestamp > mostRecentlyEditedFileTime)) continue;

            mostRecentlyEditedFileTime = modificationTimestamp;
            mostRecentlyEditedFileName = fileName;
          }
          else {
            Console.WriteLine($"Found file {fileName}, but it is too old. Searching more...");
          }
        }
      }
      catch (DirectoryNotFoundException) {
        Console.WriteLine("Warning: log folder is empty.");
      }

      // File not found, pause and try again
      if (mostRecentlyEditedFileName == null) Thread.Sleep(2000);
    }

    Console.WriteLine($"Found active file {mostRecentlyEditedFileName}");
    return mostRecentlyEditedFileName;
  }

  // Returns the most recently edited log file if over x seconds have passed since the last
  // call of this method. If x seconds have not passed, returns file_name
  private string CheckNewestLogFile(string fileName) {
    if (TimeUtils.UnixTimestamp() < _newestLogFileCheckedTimestampInSeconds
        + CheckNewestLogFileIntervalInSeconds)
      return fileName;

    Console.WriteLine("Re-checking the active log file...");
    _newestLogFileCheckedTimestampInSeconds = TimeUtils.UnixTimestamp();
    var newestFileName = FindMostRecentlyEditedLogFile(Config.PathLogs);

    if (newestFileName.Equals(fileName)) return newestFileName;
    Console.WriteLine($"Switching the newest log file from {fileName} to {newestFileName}");
    _alreadyProcessesLinesCount = 0;
    return newestFileName;
  }

  private void ProcessNewLinesInFile(string filePath) {
    var lines = new List<string>();
    using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
    using (var reader = new StreamReader(fileStream)) {
      while (!reader.EndOfStream) {
        var line = reader.ReadLine();
        if (line != null) lines.Add(line);
      }
    }

    var lastReadLineIndex = _alreadyProcessesLinesCount == 0 ? 0 : _alreadyProcessesLinesCount;

    // Process the new lines
    // This is skipped if the log file is read for the first time so that
    // old lines are not processed if the program was restarted.

    if (_alreadyProcessesLinesCount == 0) {
      Console.WriteLine("Not analysing log file on first run.");
      _alreadyProcessesLinesCount = lines.Count;
      return;
    }

    for (var i = lastReadLineIndex; i < lines.Count; i++) {
      var previousLine1 = i > 0 ? lines[i - 1] : "";
      var previousLine2 = i > 1 ? lines[i - 2] : "";
      var previousLine3 = i > 2 ? lines[i - 3] : "";
      var previousLines = new List<string> { previousLine1, previousLine2, previousLine3 };
      var line = lines[i];
      GameEventHandler.ScanLine(line, previousLines);
      PlayerEventHandler.ScanLine(line, previousLines);
    }

    _alreadyProcessesLinesCount = lines.Count;
  }
}