using Newtonsoft.Json;
using RestSharp;

namespace MertaScript.Ai;

public class ChatGPT {
  public static string GenerateComment(string text) {
    var API_KEY = Config.ChatGptApiKey;
    const string baseUrl = "https://api.openai.com/v1";
    const string endpoint = "/chat/completions";

    var client = new RestClient(baseUrl);
    var request = new RestRequest(endpoint, Method.Post);
    request.AddHeader("Authorization", $"Bearer {API_KEY}");
    request.AddHeader("Content-Type", "application/json");
    var model = PickModel();

    var requestBody = new {
      model,
      messages = new[] {
        new { role = "user", content = text }
      },
      temperature = 1,
      max_tokens = 500
    };

    request.AddParameter("application/json", JsonConvert.SerializeObject(requestBody), ParameterType.RequestBody);

    var response = client.Execute<dynamic>(request);

    if (!response.IsSuccessful) throw new Exception("ChatGPT response error: " + response.ErrorMessage);

    dynamic responseData = JsonConvert.DeserializeObject(response.Content);
    return responseData.choices[0].message?.content;
  }

  /**
   * Picks a model with heavy emphasis on gpt-3.5-turbo (cheap), but may occasionally use gpt-4o.
   */
  private static string PickModel() {
    var random = new Random();
    var randomNumber = random.Next(100);
    return randomNumber < 90 ? "gpt-3.5-turbo" : "gpt-4o";
  }
}