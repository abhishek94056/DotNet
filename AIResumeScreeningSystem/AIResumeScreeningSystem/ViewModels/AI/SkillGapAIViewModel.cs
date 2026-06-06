namespace AIResumeScreeningSystem.ViewModels.AI
{
    public class SkillGapAIViewModel
    {
        public int ApplicationId { get; set; }
        public int JobId { get; set; }
        public int CandidateId { get; set; }

        public string JobTitle { get; set; } = string.Empty;
        public string CandidateName { get; set; } = string.Empty;
        public decimal MatchScore { get; set; }

        // Missing skills data
        public List<string> MissingRequiredSkills { get; set; } = new();
        public List<string> MissingOptionalSkills { get; set; } = new();

        // AI-generated analysis
        public string? GapAnalysisNarrative { get; set; }
        public string? LearningRoadmap { get; set; }
        public string? TimeToReadiness { get; set; }
        public string? PriorityRecommendation { get; set; }

        // Structured learning plan
        public List<SkillLearningItem> LearningPlan { get; set; } = new();

        public bool IsLoaded { get; set; }
        public string? Error { get; set; }
    }

    public class SkillLearningItem
    {
        public string SkillName { get; set; } = string.Empty;
        public string Priority { get; set; } = "High";
        public string EstimatedTime { get; set; } = string.Empty;
        public string ResourceType { get; set; } = "Online Course";
        public string? ResourceUrl { get; set; }
        public string? CourseName { get; set; }

        public string PriorityBadgeClass => Priority.ToLower() switch
        {
            "critical" or "high" => "bg-danger bg-opacity-10 text-danger",
            "medium" => "bg-warning bg-opacity-10 text-warning",
            "low" => "bg-success bg-opacity-10 text-success",
            _ => "bg-secondary bg-opacity-10 text-secondary"
        };
    }
}