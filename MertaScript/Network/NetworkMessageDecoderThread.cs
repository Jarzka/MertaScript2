namespace MertaScript.Network;

public class NetworkMessageDecoderThread {
  private readonly string _message;

  public NetworkMessageDecoderThread(string message) {
    _message = message;
  }

  public void Run() {
    NetworkManager.HandleNetworkMessage(_message);
  }
}