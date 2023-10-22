using MertaScript.Audio;
using MertaScript.Events;
using MertaScript.Network;

namespace MertaScript.EventHandling;

internal abstract class PlayerCommentator {
  private static readonly Dictionary<string, AudioClip?> EventAudioFiles = new();

  static PlayerCommentator() {
    foreach (var player in PlayerEvents.Players.Select(player => player.Name))
      EventAudioFiles.Add(player, null);
  }

  public static void HandleEvent(string playerName, PlayerEventId playerEventId) {
    HandleEventAsAudioComment(playerName, playerEventId);
  }

  private static void HandleEventAsAudioComment(string playerName, PlayerEventId playerEventId) {
    var audioFolder = PlayerEvents.EventAudioFolderByEventId(playerEventId);
    var file = PlayerEvents.RandomSoundFileByPlayerAndEventId(playerName, playerEventId);

    if (file == null) {
      Console.WriteLine("Player " + playerName + " does not comment this event.");
      return;
    }

    SendPlaySoundCommandToClients(playerName, audioFolder, file);
  }

  private static void SendPlaySoundCommandToClients(string playerName, string audioFolder, FileSystemInfo file) {
    var path = Config.PathPlayerEventSounds + playerName + "/" + audioFolder + "/" + file.Name;

    if (!NetworkManager.GetInstance().IsHost()) return;

    var message = "<PLAY_SOUND_PLAYER|" + playerName + "|" + path + ">";
    NetworkManager.GetInstance().SendMessageToClients(message);
  }

  public static void PlayFile(string playerName, string path) {
    Console.WriteLine("Playing: " + path);

    var playerAudioClip = EventAudioFiles[playerName];

    if (playerAudioClip != null && playerAudioClip.IsPlaying()) {
      playerAudioClip?.Stop();
      Thread.Sleep(10); // Wait for the audio file to stop playing
    }

    playerAudioClip = new AudioClip();
    playerAudioClip.Play(path);
    EventAudioFiles[playerName] = playerAudioClip;
  }
}