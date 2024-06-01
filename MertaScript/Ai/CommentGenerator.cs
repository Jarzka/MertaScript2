using MertaScript.EventHandling;
using MertaScript.Log;
using MertaScript.Utils;

namespace MertaScript.Ai;

public enum PromptType {
  Generic,
  GameLog
}

public class CommentGenerator {
  private static bool _isGeneratingComment;
  private static readonly object lockObject = new();

  public static bool IsGeneratingComment {
    get {
      lock (lockObject) {
        return _isGeneratingComment;
      }
    }
    private set {
      lock (lockObject) {
        _isGeneratingComment = value;
      }
    }
  }

  public static void MaybeAnalyseLogToGenerateComment() {
    var log = LogStorage.GetLog();

    var randomValue = RandomGenerator.RandomNumber(100);
    const int probabilityThreshold = 28;

    if (!Config.UseAiAnalysis || IsGeneratingComment || log.Count <= 15 ||
        GameCommentator.GetInstance().IsMatchEnded() ||
        randomValue > probabilityThreshold)
      return;

    IsGeneratingComment = true;

    Console.WriteLine("Initialising AI comment generation...");
    var thread = new Thread(AnalyseLogToGenerateComment);
    thread.Start();
  }

  private static void AnalyseLogToGenerateComment() {
    try {
      if (Config.ElevenLabsApiKey == null || Config.ElevenLabsApiKey == null)
        throw new Exception("Unable to use AI generator without API keys!");

      Console.WriteLine("Generating AI comment...");
      var logWithReplacedTexts = ReplaceTextsInLog(LogStorage.AsText(), Config.LogStorageReplcamenets);
      var logPrompt = Config.ChatGptLogPromptGuide + " " +
                      Config.ChatGptLogPromptOptions[
                        RandomGenerator.RandomNumber(Config.ChatGptLogPromptOptions.Length)] +
                      " " +
                      Config.ChatGptLogPromptLengths[
                        RandomGenerator.RandomNumber(Config.ChatGptLogPromptLengths.Length)] +
                      "\n\nThe log file:\n\n" + logWithReplacedTexts;
      var genericPrompt =
        Config.ChatGptGenericPrompts[RandomGenerator.RandomNumber(Config.ChatGptGenericPrompts.Length)];
      var promptType = PickPromptType();
      var finalPrompt = promptType == PromptType.Generic ? genericPrompt : logPrompt;

      var filteredComment = "";
      var currentAttempt = 0;
      const int maxAttempts = 5;


      while (true) {
        var comment = ChatGPT.GenerateComment(finalPrompt);
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
      while (GameCommentator.GetInstance().IsPlayingAudio()) Thread.Sleep(3000);
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

  private static PromptType PickPromptType() {
    var randomNumber = RandomGenerator.RandomNumber(100);
    return randomNumber <= 8 ? PromptType.Generic : PromptType.GameLog;
  }
}