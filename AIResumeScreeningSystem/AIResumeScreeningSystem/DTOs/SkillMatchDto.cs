namespace AIResumeScreeningSystem.DTOs
{
    public class SkillMatchDto
    {
        // Input identifiers
        public int JobId { get; set; }
        public int CandidateId { get; set; }
        public int ApplicationId { get; set; }

        // Scores
        public decimal OverallMatchScore { get; set; }
        public decimal RequiredSkillScore { get; set; }
        public decimal OptionalSkillScore { get; set; }
        public decimal ExperienceScore { get; set; }
        public decimal EducationScore { get; set; }

        // Skill breakdown
        public List<string> MatchedRequiredSkills { get; set; } = new();
        public List<string> MatchedOptionalSkills { get; set; } = new();
        public List<string> MissingRequiredSkills { get; set; } = new();
        public List<string> MissingOptionalSkills { get; set; } = new();
        public List<string> CandidateExtraSkills { get; set; } = new();

        // Counts
        public int TotalRequiredSkills { get; set; }
        public int TotalOptionalSkills { get; set; }
        public int MatchedRequiredCount { get; set; }
        public int MatchedOptionalCount { get; set; }

        // Metadata
        public string MatchGrade => OverallMatchScore switch
        {
            >= 90 => "Excellent",
            >= 75 => "Strong",
            >= 60 => "Good",
            >= 45 => "Fair",
            >= 30 => "Weak",
            _ => "Poor"
        };

        public string GradeBadgeClass => OverallMatchScore switch
        {
            >= 90 => "bg-success",
            >= 75 => "bg-primary",
            >= 60 => "bg-info",
            >= 45 => "bg-warning text-dark",
            _ => "bg-danger"
        };
    }
}