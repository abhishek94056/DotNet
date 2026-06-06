namespace AIResumeScreeningSystem.ViewModels.AI
{
    public class AIRecommendationViewModel
    {
        public int CandidateId { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public List<string> CandidateSkills { get; set; } = new();

        // AI-generated recommendations
        public string? CareerAdvice { get; set; }
        public string? JobSearchStrategy { get; set; }
        public string? SkillDevelopmentPlan { get; set; }
        public string? IndustryInsights { get; set; }

        // Recommended job types
        public List<RecommendedRoleItem> RecommendedRoles { get; set; } = new();

        public bool IsLoaded { get; set; }
        public string? Error { get; set; }
    }

    public class RecommendedRoleItem
    {
        public string RoleTitle { get; set; } = string.Empty;
        public string Reasoning { get; set; } = string.Empty;
        public int MatchConfidence { get; set; }
        public string Industry { get; set; } = string.Empty;
        public List<string> KeySkillsNeeded { get; set; } = new();
    }
}