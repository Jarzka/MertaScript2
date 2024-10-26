using MertaScript.Ai;
using MertaScript.EventHandling;
using MertaScript.Log;
using MertaScript.Network;

namespace MertaScript;

internal class Program {
  private static Program? _instance;
  private readonly bool _running;
  private readonly string _startMethod;

  private Program() {
    _startMethod = Config.GetValueFromConfigFile("start");
    if (_startMethod is not ("host" or "join"))
      throw new InvalidOperationException("config.txt start type should be host or join, but it was " +
                                          _startMethod);

    HostPort = int.Parse(Config.GetValueFromConfigFile("host_port"));
    JoinIp = Config.GetValueFromConfigFile("join_ip");
    JoinPort = int.Parse(Config.GetValueFromConfigFile("join_port"));

    _running = true;
  }

  public int HostPort { get; }
  public string JoinIp { get; }
  public int JoinPort { get; }

  public static Program GetInstance() {
    return _instance ??= new Program();
  }

  private void Exec() {
    if (Config.UseAiAnalysis) {
      Console.WriteLine("AI analysis enabled. Testing APIs...");

      try {
        Console.WriteLine("Testing ChatGPT...");
        var comment = ChatGPT.GenerateComment("Testing API access. Say anything.");
        Console.WriteLine("ChatGPT OK: " + comment);
      }
      catch (Exception e) {
        Console.WriteLine("ChatGPT failed: " + e.Message);
        throw e;
      }

      try {
        Console.WriteLine("Testing ElevenLabs...");
        var audioFilePath = ElevenLabs.GenerateAudio("Testing AI access");
        Console.WriteLine("ElevenLabs OK: " + audioFilePath);
      }
      catch (Exception e) {
        Console.WriteLine("ElevenLabs failed: " + e.Message);
        throw e;
      }

      Console.WriteLine("AI APIs OK!");
    }

    switch (_startMethod) {
      case "host":
        Host();
        break;
      case "join":
        Join();
        break;
    }
  }

  private void Host() {
    Console.WriteLine("Starting as host...");
    StartNetworkManagerThread("host");

    while (_running) {
      LogReader.GetInstance().UpdateState();
      GameCommentator.GetInstance().UpdateState();
      Thread.Sleep(200); // Save CPU
    }

    Console.WriteLine("Quitting...");
    NetworkManager.GetInstance().Disconnect();
  }

  private static void Join() {
    Console.WriteLine("Starting as client...");
    StartNetworkManagerThread("join");
  }

  private static void StartNetworkManagerThread(string method) {
    var networkManagerThread = new NetworkManagerThread(method);
    networkManagerThread.Start();
  }

  private static void Main(string[] args) {
    try {
      var program = GetInstance();
      program.Exec();
    }
    catch (Exception e) {
      Console.WriteLine($"FATAL ERROR: {e.Message}");
      e.StackTrace?.Split("\n").ToList().ForEach(Console.WriteLine);
      Thread.Sleep(1000 * 60 * 60);
    }
  }
}