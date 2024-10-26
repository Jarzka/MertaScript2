using System.Text.RegularExpressions;
using MertaScript.Utils;
using Newtonsoft.Json;
using RestSharp;

namespace MertaScript.Ai;

public static class ElevenLabs {
  public static string GenerateAudio(string text) {
    var API_KEY = Config.ElevenLabsApiKey;
    var VOICE_ID = Config.ElevenLabsVoiceId;
    var OUTPUT_FOLDER = Config.PathGameEventSounds + "/live";
    const int CHUNK_SIZE = 1024;
    var URL = "https://api.elevenlabs.io/v1";
    var END_POINT = $"/text-to-speech/{VOICE_ID}/stream";
    var FILE_PATH = $"{
      OUTPUT_FOLDER
    }/{
      SanitizeAsFilename(text)
    }.mp3";
    var client = new RestClient(URL);
    var request = new RestRequest(END_POINT, Method.Post);

    request.AddHeader("Accept", "application/json");
    request.AddHeader("xi-api-key", API_KEY);
    var requestBody = new {
      text,
      model_id = "eleven_multilingual_v2",
      voice_settings = new {
        stability = RandomGenerator.RandomNumber(0.25, 0.4),
        similarity_boost = RandomGenerator.RandomNumber(0.7, 0.9),
        style = RandomGenerator.RandomNumber(0, 0.7),
        use_speaker_boost = true
      }
    };
    request.AddParameter("application/json", JsonConvert.SerializeObject(requestBody), ParameterType.RequestBody);

    var response = client.Execute<dynamic>(request);

    if (!response.IsSuccessful)
      throw new Exception("ElevenLabs response error: " + response.ErrorMessage + " " + response.Content);

    using var outputStream = new FileStream(FILE_PATH, FileMode.Create, FileAccess.Write, FileShare.None);
    var buffer = new byte[CHUNK_SIZE];
    using var responseStream = new MemoryStream(response.RawBytes);
    int bytesRead;
    while ((bytesRead = responseStream.Read(buffer, 0, buffer.Length)) > 0)
      outputStream.Write(buffer, 0, bytesRead);

    return FILE_PATH;
  }

  private static string SanitizeAsFilename(string input) {
    var lowerCase = input.ToLower();

    // Replace spaces with underscores
    var sanitized = lowerCase.Replace(" ", "_");

    // Replace Ä and Ö
    sanitized = sanitized.Replace("ä", "a").Replace("ö", "o");

    // Remove any non-alphabetical characters
    var regex = new Regex("[^a-zA-Z_]");
    var alphaOnly = regex.Replace(sanitized, "");

    // Remove trailing periods (.)
    alphaOnly = alphaOnly.TrimEnd('.');

    // Truncate the string to a maximum of characters
    const int maxLength = 100;
    return alphaOnly.Length > maxLength ? alphaOnly[..maxLength] : alphaOnly;
  }
}