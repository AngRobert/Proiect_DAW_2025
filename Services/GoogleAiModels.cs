using System.Text.Json.Serialization;

namespace Proiect_DAW_2025.Services
{
    public class GoogleAiRequest
    {
        [JsonPropertyName("contents")]
        public List<GoogleAiContent> Contents { get; set; } = new();
    }

    public class GoogleAiContent
    {
        [JsonPropertyName("parts")]
        public List<GoogleAiPart> Parts { get; set; } = new();
    }

    public class GoogleAiPart
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;
    }

    public class GoogleAiResponse
    {
        [JsonPropertyName("candidates")]
        public List<GoogleAiCandidate>? Candidates { get; set; }
    }

    public class GoogleAiCandidate
    {
        [JsonPropertyName("content")]
        public GoogleAiContent? Content { get; set; }
    }
}