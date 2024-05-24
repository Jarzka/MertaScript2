using MertaScript.EventHandling;
using MertaScript.Log;
using MertaScript.Utils;

namespace MertaScript.Ai;

public class CommentGenerator {
  private static bool isGeneratingComment;
  private static readonly object lockObject = new();

  public static bool IsGeneratingComment {
    get {
      lock (lockObject) {
        return isGeneratingComment;
      }
    }
    private set {
      lock (lockObject) {
        isGeneratingComment = value;
      }
    }
  }

  public static void MaybeAnalyseLogToGenerateComment() {
    var log = LogStorage.GetLog();

    var random = new Random();
    const double probabilityThreshold = 0.2;
    var randomValue = random.NextDouble(); // Rrandom double between 0.0 and 1.0

    if (!Config.UseAiAnalysis || IsGeneratingComment || log.Count <= 30 ||
        GameCommentator.GetInstance().IsMatchEneded() ||
        !(randomValue < probabilityThreshold)) return;

    IsGeneratingComment = true;

    var thread = new Thread(AnalyseLogToGenerateComment);
    thread.Start();
  }

  private static void AnalyseLogToGenerateComment() {
    try {
      if (Config.ElevenLabsApiKey == null || Config.ElevenLabsApiKey == null)
        throw new Exception("Unable to use AI generator without API keys!");

      Console.WriteLine("Generating AI comment...");
      var logPrompt = ReplaceTextsInLog(LogStorage.AsText(), Config.LogStorageReplcamenets);
      var prompt = Config.ChatGptPromptGuide + " " +
                   Config.ChatGptPromptOptions[RandomGenerator.RandomNumber(Config.ChatGptPromptOptions.Length)] + " " +
                   Config.ChatGptPromptLengths[RandomGenerator.RandomNumber(Config.ChatGptPromptLengths.Length)] +
                   "\n\nThe log file:\n\n" + logPrompt;
      var filteredComment = "";
      var currentAttempt = 0;
      const int maxAttempts = 5;

      while (true) {
        var comment = ChatGPT.GenerateComment(prompt);
        var preprocessedComment = CommentPreprocess.PreProcessGeneratedComment(comment);
        var filterResult = Filters.Filter(preprocessedComment);

        if (filterResult == null) {
          filteredComment = preprocessedComment;
          break;
        }

        Console.WriteLine(
          $"Found issue while filtering the AI comment: {preprocessedComment}, reason: {filterResult.Reason}");

        if (currentAttempt >= maxAttempts) throw new Exception("Too many attempts to get good AI comment.");

        currentAttempt++;
      }

      Console.WriteLine("Got good AI comment: " + filteredComment);
      Console.WriteLine("Converting AI comment to audio...");
      var audioFilePath = ElevenLabs.GenerateAudio(filteredComment);

      // Wait for game commentator to be available
      while (GameCommentator.GetInstance().IsPlayingAudio()) Thread.Sleep(4000);
      GameCommentator.GetInstance().HandleEventAsLiveAudioComment(audioFilePath);
    }
    catch (Exception e) {
      Console.WriteLine("Error while generating AI comment: " + e.Message);
    }

    IsGeneratingComment = false;
  }

  private static string ReplaceTextsInLog(string log, string[] replacements) {
    if (replacements.Length % 2 != 0) throw new ArgumentException("Replacement array must contain pairs of texts.");

    var enumerator = replacements.GetEnumerator();

    while (enumerator.MoveNext()) {
      var textToFind = (string)enumerator.Current;
      enumerator.MoveNext();
      var replacement = (string)enumerator.Current;

      log = log.Replace(textToFind, replacement);
    }

    return log;
  }
}