using System.Net.Sockets;

namespace MertaScript.Network;

public class Client {
  private readonly Socket _socket;
  private bool _connected;
  private readonly int _id;

  public Client(int id, Socket socket) {
    _socket = socket;
    _connected = true;
    _id = id;
  }

  public int GetId() {
    return _id;
  }

  public Socket GetSocket() {
    return _socket;
  }

  public bool IsConnected() {
    return _connected;
  }

  public void Disconnect() {
    _connected = false;
    _socket.Close();
  }
}