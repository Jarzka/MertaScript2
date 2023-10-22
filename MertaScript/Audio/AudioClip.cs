using NAudio.Wave;

namespace MertaScript.Audio;

public class AudioClip {
  private readonly IWavePlayer _waveOutEvent = new WaveOutEvent();
  private string? _audioFilePath;
  private bool _playing;
  private Thread? _playThread;

  public void Play(string audioFilePath) {
    _audioFilePath = audioFilePath;
    _playing = true;
    _playThread = new Thread(PlayClip);
    _playThread.Start();
  }

  private void PlayClip() {
    using var audioFile = new AudioFileReader(_audioFilePath);
    _waveOutEvent.Init(audioFile);
    _waveOutEvent.Play();
    while (_playing && _waveOutEvent.PlaybackState == PlaybackState.Playing) Thread.Sleep(10);
    Stop();
  }

  public void Stop() {
    _playing = false;
    _waveOutEvent.Stop();
  }

  public bool IsPlaying() {
    return _waveOutEvent.PlaybackState == PlaybackState.Playing || _playing;
  }
}