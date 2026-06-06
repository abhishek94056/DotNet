namespace AIResumeScreeningSystem.ViewModels.AI
{
    public class AIEvaluationViewModel
    {
        public int ApplicationId { get; set; }
        public int JobId { get; set; }
        public int CandidateId { get; set; }

        public string JobTitle { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string CandidateName { get; set; } = string.Empty;
        public decimal OverallMatchScore { get; set; }

        // AI-generated content
        public string? EvaluationSummary { get; set; }
        public string? StrengthsAnalysis { get; set; }
        public string? WeaknessesAnalysis { get; set; }
        public string? HiringRecommendation { get; set; }
        public string? CulturalFitAssessment { get; set; }
        public string? CareerProgressionNote { get; set; }

        // Parsed structured output
        public List<string> KeyStrengths { get; set; } = new();
        public List<string> KeyWeaknesses { get; set; } = new();
        public List<string> RedFlags { get; set; } = new();

        // Decision
        public string RecommendationLevel { get; set; } = string.Empty;
        public string RecommendationBadgeClass => RecommendationLevel.ToLower() switch
        {
            "strongly recommend" => "bg-success",
            "recommend" => "bg-primary",
            "consider" => "bg-warning text-dark",
            "not recommended" => "bg-danger",
            _ => "bg-secondary"
        };

        public bool IsLoaded { get; set; }
        public string? Error { get; set; }
    }
}