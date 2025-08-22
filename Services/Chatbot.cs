using Newtonsoft.Json;
using RestSharp;
using System.Text.Json;

namespace NhaXeMaiLinh.Services
{
    public class Chatbot
    {
        private readonly IConfiguration _configuration;

        public Chatbot(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //private string _apiKey => _configuration["ChatGPT:Key"]!.ToString();
        private string _apiKey => _configuration["Gemini"]!.ToString();

        public async Task<string> GetResponseAsync(string prompt)
        {
            var client = new RestClient("https://generativelanguage.googleapis.com");
            var request = new RestRequest("v1beta/models/gemini-2.0-flash:generateContent", method: Method.Post);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("X-goog-api-key", $"{_apiKey}");

            var body = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }    
                    }
                }
            };

            request.AddJsonBody(body);
            var response = await client.ExecutePostAsync(request);
            using(JsonDocument doc = JsonDocument.Parse(response.Content))
                {
                var root = doc.RootElement;
                var textElement = root.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text");
                return textElement.GetString();
            }
        }
    }
}
