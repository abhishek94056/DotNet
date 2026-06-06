using AIResumeScreeningSystem.Models;

namespace AIResumeScreeningSystem.ViewModels.AI
{
    public class InterviewQuestionsViewModel
    {
        public int JobId { get; set; }
        public int? ApplicationId { get; set; }
        public int? CandidateId { get; set; }

        public string JobTitle { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string? CandidateName { get; set; }

        public List<GeneratedQuestion> Questions { get; set; } = new();

        // Saved questions from DB
        public List<InterviewQuestion> SavedQuestions { get; set; } = new();

        public bool IsLoaded { get; set; }
        public string? Error { get; set; }
    }

    public class GeneratedQuestion
    {
        public string Question { get; set; } = string.Empty;
        public string? ExpectedAnswer { get; set; }
        public string Category { get; set; } = "Technical";
        public string Difficulty { get; set; } = "Medium";

        public string CategoryBadgeClass => Category.ToLower() switch
        {
            "technical" => "bg-primary bg-opacity-10 text-primary",
            "behavioral" => "bg-success bg-opacity-10 text-success",
            "situational" => "bg-warning bg-opacity-10 text-warning",
            "cultural fit" => "bg-info bg-opacity-10 text-info",
            _ => "bg-secondary bg-opacity-10 text-secondary"
        };

        public string DifficultyBadgeClass => Difficulty.ToLower() switch
        {
            "easy" => "bg-success",
            "medium" => "bg-warning text-dark",
            "hard" => "bg-danger",
            _ => "bg-secondary"
        };
    }
}