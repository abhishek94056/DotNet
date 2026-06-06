namespace AIResumeScreeningSystem.ViewModels.SkillMatching
{
    public class SkillGapViewModel
    {
        public int ApplicationId { get; set; }
        public int JobId { get; set; }
        public int CandidateId { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string CandidateName { get; set; } = string.Empty;

        public List<SkillGapItem> RequiredGaps { get; set; } = new();
        public List<SkillGapItem> OptionalGaps { get; set; } = new();
        public List<SkillGapItem> MatchedSkills { get; set; } = new();
        public List<SkillGapItem> ExtraSkills { get; set; } = new();

        public decimal OverallMatchScore { get; set; }
        public string? AIGapAnalysis { get; set; }

        public int TotalGaps => RequiredGaps.Count + OptionalGaps.Count;
        public bool HasCriticalGaps => RequiredGaps.Any();
    }

    public class SkillGapItem
    {
        public string SkillName { get; set; } = string.Empty;
        public bool IsRequired { get; set; }
        public bool IsMatched { get; set; }
        public string? SuggestedResource { get; set; }
        public string Priority => IsRequired ? "Critical" : "Nice-to-have";
        public string PriorityBadgeClass => IsRequired
            ? "bg-danger bg-opacity-10 text-danger"
            : "bg-warning bg-opacity-10 text-warning";
    }
}