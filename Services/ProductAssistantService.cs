using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Proiect_DAW_2025.Models;

namespace Proiect_DAW_2025.Services
{
    public class ProductAssistantService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        private const string BaseUrl = "https://generativelanguage.googleapis.com/v1beta/models/";
        private const string ModelName = "gemini-2.5-flash-lite";

        public ProductAssistantService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();

            _apiKey = configuration["GoogleAI:ApiKey"]
                ?? throw new ArgumentNullException("GoogleAI:ApiKey nu este configurat în appsettings.json");
        }

        public async Task<string> GetAnswerAsync(Product product, string userQuestion)
        {
            var contextBuilder = new StringBuilder();
            contextBuilder.AppendLine($"Produs: {product.Title}");
            contextBuilder.AppendLine($"Descriere: {product.Description}");

            if (product.FAQs != null && product.FAQs.Any())
            {
                contextBuilder.AppendLine("Întrebări frecvente deja existente:");
                foreach (var faq in product.FAQs)
                {
                    contextBuilder.AppendLine($"Q: {faq.Text} A: {faq.Answer}");
                }
            }

            var prompt = $@"
                Ești un asistent util pentru magazinul online.
                Folosește DOAR informațiile de mai jos pentru a răspunde la întrebarea utilizatorului.
                INFORMAȚII PRODUS:
                {contextBuilder}
                
                ÎNTREBARE UTILIZATOR: {userQuestion}

                REGULI:
                - Răspunde scurt și la obiect.
                - Dacă informația nu există în descriere sau FAQ, răspunde exact: 'Momentan nu avem detalii despre acest aspect.'
            ";

            var requestBody = new GoogleAiRequest
            {
                Contents = new List<GoogleAiContent>
                {
                    new GoogleAiContent { Parts = new List<GoogleAiPart> { new GoogleAiPart { Text = prompt } } }
                }
            };

            var jsonContent = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var requestUrl = $"{BaseUrl}{ModelName}:generateContent?key={_apiKey}";

            var response = await _httpClient.PostAsync(requestUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                return "Eroare de comunicare cu asistentul AI.";
            }

            var responseString = await response.Content.ReadAsStringAsync();
            var googleResponse = JsonSerializer.Deserialize<GoogleAiResponse>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var answer = googleResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;

            return answer ?? "Nu am putut genera un răspuns.";
        }
    }
}