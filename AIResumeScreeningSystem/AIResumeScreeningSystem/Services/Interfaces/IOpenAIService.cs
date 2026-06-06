using AIResumeScreeningSystem.DTOs.OpenAI;
using AIResumeScreeningSystem.ViewModels.AI;

namespace AIResumeScreeningSystem.Services.Interfaces
{
    public interface IOpenAIService
    {
        // Core API
        Task<string?> GetCompletionAsync(
            string systemPrompt,
            string userPrompt,
            int maxTokens = 1500,
            double temperature = 0.7);

        Task<string?> GetChatCompletionAsync(
            List<ChatMessage> history,
            string systemPrompt,
            int maxTokens = 1000);

        // Resume Features
        Task<ResumeSummaryViewModel> GenerateResumeSummaryAsync(int resumeId);
        Task<string?> GenerateResumeSummaryTextAsync(string resumeText, string candidateName);

        // Candidate Evaluation
        Task<AIEvaluationViewModel> EvaluateCandidateAsync(int applicationId);

        // Skill Gap
        Task<SkillGapAIViewModel> GenerateSkillGapAnalysisAsync(int applicationId);

        // Interview Questions
        Task<InterviewQuestionsViewModel> GenerateInterviewQuestionsAsync(
            int jobId,
            int? applicationId = null,
            int count = 10);

        // Recommendations
        Task<AIRecommendationViewModel> GenerateCandidateRecommendationsAsync(int candidateId);

        // Chatbot
        Task<string> GetChatbotResponseAsync(
            string userMessage,
            List<ChatMessage> history,
            string? context = null);

        // Quick helpers
        Task<string?> ImproveResumeTextAsync(string resumeText);
        Task<decimal> PredictApplicationSuccessAsync(int applicationId);
        bool IsConfigured { get; }
    }
}