using NAudio.Wave;

namespace MertaScript.Audio;

public class AudioClip {
  private readonly IWavePlayer _waveOutEvent = new WaveOutEvent();
  private bool _playing;
  private Thread? _playThread;

  public void Play(byte[] audioStream, float volume) {
    _playing = true;
    _playThread = new Thread(() => PlayClip(audioStream, volume));
    _playThread.Start();
  }

  public void Play(string audioFilePath, float volume) {
    _playing = true;
    _playThread = new Thread(() => PlayClip(audioFilePath, volume));
    _playThread.Start();
  }

  private void PlayClip(string audioFilePath, float volume) {
    using var audioFile = new AudioFileReader(audioFilePath);
    _waveOutEvent.Init(audioFile);
    _waveOutEvent.Volume = volume;
    _waveOutEvent.Play();
    while (_playing && _waveOutEvent.PlaybackState == PlaybackState.Playing) Thread.Sleep(10);
    Stop();
  }

  private void PlayClip(byte[] audioStream, float volume) {
    using var ms = new MemoryStream(audioStream);
    using WaveStream waveStream = new Mp3FileReader(ms); // Assume the audio is always sent in mp3 format

    _waveOutEvent.Init(waveStream);
    _waveOutEvent.Volume = volume;
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