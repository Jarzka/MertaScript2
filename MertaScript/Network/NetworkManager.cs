using System.Net;
using System.Net.Sockets;
using System.Text;
using MertaScript.EventHandling;

namespace MertaScript.Network;

public class NetworkManager {
  private const int BufferSize = 1024;
  private static NetworkManager? _instance;
  private readonly List<Client> _clients = new();
  private bool _isHost;
  private int _nextFreeId;
  private bool _running = true;
  private Socket? _serverSocket;

  public static int GetBufferSize() {
    return BufferSize;
  }

  public static NetworkManager GetInstance() {
    return _instance ??= new NetworkManager();
  }

  public void StartHost(int port) {
    _isHost = true;
    try {
      Console.WriteLine("Server starting...");
      _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
      _serverSocket.Bind(new IPEndPoint(IPAddress.Any, port));
      _serverSocket.Listen(5);
      Console.WriteLine($"Server started.\nListening at {_serverSocket.LocalEndPoint}");
    }
    catch (SocketException e) {
      Console.WriteLine($"Could not start server: {e.Message}");
      return;
    }

    while (_running)
      try {
        var clientSocket = _serverSocket.Accept();
        Console.WriteLine($"Got connection from {clientSocket.RemoteEndPoint}");
        Console.WriteLine($"Assigning id {_nextFreeId} to the client.");
        var client = new Client(_nextFreeId++, clientSocket);
        _clients.Add(client);

        var connectedClientThread = new NetworkClientThread(client);
        connectedClientThread.Start();
      }
      catch (SocketException e) {
        Console.WriteLine($"Error listening to client connections: {e.Message}");
        break;
      }

    Console.WriteLine("Server shutting down...");
    _serverSocket.Close();
  }

  public void Disconnect() {
    _serverSocket?.Close();
    _running = false;
  }

  public void StartJoin(string ip, int port) {
    // Connect and keep re-connecting as long as the program is running
    var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    while (true) {
      Console.WriteLine("Connecting to: " + ip + " " + port);

      try {
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        clientSocket.Connect(ip, port);
      }
      catch (Exception e) {
        Console.WriteLine("Connection failed: " + e.Message);
        Console.WriteLine("Trying again...");
      }

      if (!clientSocket.Connected) continue;

      Console.WriteLine("Connection established!");

      // Listen server messages

      var stringBuilder = new StringBuilder();

      while (_running)
        try {
          var buffer = new byte[BufferSize];
          var bytesRead = clientSocket.Receive(buffer);
          if (bytesRead <= 0) continue;

          var receivedString = Encoding.UTF8.GetString(buffer, 0, bytesRead);
          stringBuilder.Append(receivedString);

          var allReceivedText = stringBuilder.ToString();

          if (!allReceivedText.Contains('>')) continue; // End of message NOT reached

          Console.WriteLine("Handling network message...");

          var completeMessages = ParseCompleteNetworkMessages(allReceivedText);
          var partialMessageAtTheEnd = ParsePartialEndMessage(allReceivedText);

          foreach (var message in completeMessages) {
            var decodeMessage = new NetworkMessageDecoderThread(message);
            decodeMessage.Run();
          }

          stringBuilder.Clear();
          stringBuilder.Append(partialMessageAtTheEnd);
        }
        catch (SocketException e) {
          Console.WriteLine("Error: " + e.Message);
          break;
        }

      Console.WriteLine("Disconnecting...");
      clientSocket.Close();
      Console.WriteLine("Disconnected. Trying again in a few seconds.");
      Thread.Sleep(2000);
    }
  }

  public static List<string> ParseCompleteNetworkMessages(string receivedString) {
    var completeMessages = new List<string>();

    var newMessageStartIndex = 0;
    for (var i = 0; i < receivedString.Length; i++)
      if (receivedString[i] == '>') {
        var substring = receivedString.Substring(newMessageStartIndex, i - newMessageStartIndex + 1);
        completeMessages.Add(substring);
        newMessageStartIndex = i + 1;
      }

    return completeMessages;
  }

  public static string ParsePartialEndMessage(string input) {
    if (input.EndsWith(">")) return ""; // There is no partial message available

    var lastIndex = input.LastIndexOf('>');
    return input.Substring(lastIndex + 1);
  }

  public bool IsHost() {
    return _isHost;
  }

  public static void HandleNetworkMessage(string message) {
    foreach (var splittedMessage in SplitNetworkMessage(message)) {
      HandleMessageTypePlayLiveGameCommentatorSound(splittedMessage);
      HandleMessageTypeConMsg(splittedMessage);
      HandleMessageTypePlayGameCommentatorSound(splittedMessage);
      HandleMessageTypePlayPlayerSound(splittedMessage);
    }
  }

  private static List<string> SplitNetworkMessage(string message) {
    var messages = message.Split('>').ToList();

    for (var index = 0; index < messages.Count; index++) messages[index] += ">";

    return messages;
  }

  private static void HandleMessageTypeConMsg(string message) {
    if (!message.StartsWith("<CON_MSG|")) return;

    var arrayMessage = message.Split('|');
    var printMessage = arrayMessage[1].Substring(0, arrayMessage[1].Length - 1); // Remove last character (>)
    Console.WriteLine(printMessage);
  }

  private static void HandleMessageTypePlayLiveGameCommentatorSound(string message) {
    if (!message.StartsWith("<PLAY_LIVE_SOUND_GAME|")) return;

    var arrayMessage = message.Split('|');
    var base64Audio = arrayMessage[1].Substring(0, arrayMessage[1].Length - 1); // Remove last character (>)

    GameCommentator.GetInstance().PlayBase64Audio(base64Audio);
  }

  private static void HandleMessageTypePlayGameCommentatorSound(string message) {
    if (!message.StartsWith("<PLAY_SOUND_GAME|")) return;

    var arrayMessage = message.Split('|');
    var path = arrayMessage[1].Substring(0, arrayMessage[1].Length - 1); // Remove last character (>)

    GameCommentator.GetInstance().PlayFile(path);
  }

  private static void HandleMessageTypePlayPlayerSound(string message) {
    if (!message.StartsWith("<PLAY_SOUND_PLAYER|")) return;

    var arrayMessage = message.Split('|');
    var playerName = arrayMessage[1];
    var path = arrayMessage[2].Substring(0, arrayMessage[2].Length - 1); // Remove last character (>)

    PlayerCommentator.PlayFile(playerName, path);
  }

  private void RemoveDisconnectedClients() {
    var i = 0;
    while (i < _clients.Count)
      if (!_clients[i].IsConnected()) {
        _clients.RemoveAt(i);
        i = 0;
      }
      else {
        i++;
      }
  }

  public void SendMessageToClients(string message, string messageToBeLogged) {
    RemoveDisconnectedClients();

    foreach (var client in _clients) {
      Console.WriteLine("Sending {0} to client {1}", messageToBeLogged, client.GetSocket());
      var messageBytes = Encoding.UTF8.GetBytes(message);
      client.GetSocket().Send(messageBytes);
    }
  }
}