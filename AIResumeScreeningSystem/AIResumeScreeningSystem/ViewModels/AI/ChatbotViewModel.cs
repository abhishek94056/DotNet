using AIResumeScreeningSystem.DTOs.OpenAI;

namespace AIResumeScreeningSystem.ViewModels.AI
{
    public class ChatbotViewModel
    {
        public string? UserMessage { get; set; }
        public List<ChatMessage> History { get; set; } = new();
        public string? Context { get; set; }
        public string AssistantName { get; set; } = "AI Recruiter Assistant";
    }
}