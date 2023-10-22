using System.Text;

namespace MertaScript.Network;

internal class NetworkClientThread {
  private readonly Client _client;
  private readonly Thread _thread;

  public NetworkClientThread(Client client) {
    _client = client;
    _thread = new Thread(Run);
  }

  public void Start() {
    _thread.Start();
  }

  private void Run() {
    var welcomeMessageBytes = "<CON_MSG|Welcome to the server.>"u8.ToArray();
    _client.GetSocket().Send(welcomeMessageBytes);

    while (true)
      try {
        var buffer = new byte[NetworkManager.GetBufferSize()];
        var bytesRead = _client.GetSocket().Receive(buffer);
        if (bytesRead > 0) {
          var data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
          var decodeMessage = new NetworkMessageDecoderThread(data);
          decodeMessage.Run();
        }
        else {
          Console.WriteLine("Client {0} disconnected", _client.GetId());
          break;
        }
      }
      catch (Exception ex) {
        Console.WriteLine("Error: " + ex.Message);
        Console.WriteLine("Client {0} disconnected", _client.GetId());
        break;
      }

    _client.Disconnect();
  }
}