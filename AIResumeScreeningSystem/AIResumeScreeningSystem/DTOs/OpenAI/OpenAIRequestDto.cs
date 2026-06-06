using System.Text.Json.Serialization;

namespace AIResumeScreeningSystem.DTOs.OpenAI
{
    public class OpenAIRequestDto
    {
        [JsonPropertyName("model")]
        public string Model { get; set; } = "gpt-4o";

        [JsonPropertyName("messages")]
        public List<OpenAIMessage> Messages { get; set; } = new();

        [JsonPropertyName("max_tokens")]
        public int MaxTokens { get; set; } = 1500;

        [JsonPropertyName("temperature")]
        public double Temperature { get; set; } = 0.7;

        [JsonPropertyName("top_p")]
        public double TopP { get; set; } = 1.0;
    }

    public class OpenAIMessage
    {
        [JsonPropertyName("role")]
        public string Role { get; set; } = "user";

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;

        public static OpenAIMessage System(string content) =>
            new() { Role = "system", Content = content };

        public static OpenAIMessage User(string content) =>
            new() { Role = "user", Content = content };

        public static OpenAIMessage Assistant(string content) =>
            new() { Role = "assistant", Content = content };
    }
}