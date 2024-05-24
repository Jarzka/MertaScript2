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

    var requestBody = new {
      model = "gpt-3.5-turbo",
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
}