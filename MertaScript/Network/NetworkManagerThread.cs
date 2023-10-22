namespace MertaScript.Network;

public class NetworkManagerThread {
  private readonly string _method;
  private readonly Thread _thread;

  public NetworkManagerThread(string method) {
    _method = method;
    _thread = new Thread(Run);
  }

  public void Start() {
    _thread.Start();
  }

  private void Run() {
    switch (_method) {
      case "host":
        NetworkManager.GetInstance().StartHost(Program.GetInstance().HostPort);
        break;
      case "join":
        NetworkManager.GetInstance().StartJoin(Program.GetInstance().JoinIp, Program.GetInstance().JoinPort);
        break;
    }
  }
}