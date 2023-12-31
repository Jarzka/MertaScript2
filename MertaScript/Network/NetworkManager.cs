using System.Net;
using System.Net.Sockets;
using System.Text;
using MertaScript.EventHandling;

namespace MertaScript.Network;

public class NetworkManager {
  private const int BufferSize = 65535;
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

      while (_running)
        try {
          var buffer = new byte[BufferSize];
          var bytesRead = clientSocket.Receive(buffer);
          if (bytesRead <= 0) continue;

          var data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
          var decodeMessage = new NetworkMessageDecoderThread(data);
          decodeMessage.Run();
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

  public bool IsHost() {
    return _isHost;
  }

  public static void HandleNetworkMessage(string message) {
    foreach (var splittedMessage in SplitNetworkMessage(message)) {
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

  private static void HandleMessageTypePlayGameCommentatorSound(string message) {
    if (!message.StartsWith("<PLAY_SOUND_GAME|")) return;

    var arrayMessage = message.Split('|');
    var path = arrayMessage[1].Substring(0, arrayMessage[1].Length - 1); // Remove last character (>)

    GameCommentator.GetInstance().PlayFile(path);
    Console.WriteLine("Playing sound \"{0}\"", path);
  }

  private static void HandleMessageTypePlayPlayerSound(string message) {
    if (!message.StartsWith("<PLAY_SOUND_PLAYER|")) return;

    var arrayMessage = message.Split('|');
    var playerName = arrayMessage[1];
    var path = arrayMessage[2].Substring(0, arrayMessage[2].Length - 1); // Remove last character (>)

    PlayerCommentator.PlayFile(playerName, path);
    Console.WriteLine("Playing sound \"{0}\"", path);
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

  public void SendMessageToClients(string message) {
    RemoveDisconnectedClients();

    foreach (var client in _clients) {
      Console.WriteLine("Sending {0} to client {1}", message, client.GetSocket());
      var messageBytes = Encoding.UTF8.GetBytes(message);
      client.GetSocket().Send(messageBytes);
    }
  }
}