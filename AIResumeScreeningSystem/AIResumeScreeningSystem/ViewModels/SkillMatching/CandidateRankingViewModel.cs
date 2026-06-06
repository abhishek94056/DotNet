using AIResumeScreeningSystem.Models;

namespace AIResumeScreeningSystem.ViewModels.SkillMatching
{
    public class CandidateRankingViewModel
    {
        public int JobId { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public int TotalApplications { get; set; }
        public List<RankedCandidateItem> RankedCandidates { get; set; } = new();

        // Filter/sort options
        public string? StatusFilter { get; set; }
        public decimal? MinScoreFilter { get; set; }
        public string SortBy { get; set; } = "Score";

        // Stats
        public decimal AverageScore =>
            RankedCandidates.Any()
                ? Math.Round(RankedCandidates.Average(r => r.OverallMatchScore), 1)
                : 0;

        public int ExcellentCount =>
            RankedCandidates.Count(r => r.OverallMatchScore >= 75);

        public int ShortlistedCount =>
            RankedCandidates.Count(r => r.Status == ApplicationStatus.Shortlisted);
    }

    public class RankedCandidateItem
    {
        public int ApplicationId { get; set; }
        public int CandidateId { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public string? CandidateHeadline { get; set; }
        public string? CandidateLocation { get; set; }
        public string? ProfileImagePath { get; set; }
        public string InitialsAvatar { get; set; } = string.Empty;

        // Scores
        public decimal OverallMatchScore { get; set; }
        public decimal RequiredSkillScore { get; set; }
        public decimal ExperienceScore { get; set; }
        public decimal SkillMatchPercentage { get; set; }

        // Rank
        public int RankPosition { get; set; }

        // Matched skills summary
        public List<string> MatchedSkills { get; set; } = new();
        public List<string> MissingSkills { get; set; } = new();
        public int TotalSkills { get; set; }
        public int CandidateExperienceYears { get; set; }

        // Application
        public ApplicationStatus Status { get; set; }
        public DateTime AppliedAt { get; set; }
        public string? RecruiterNotes { get; set; }

        // Computed
        public string GradeBadgeClass => OverallMatchScore switch
        {
            >= 90 => "bg-success",
            >= 75 => "bg-primary",
            >= 60 => "bg-info text-dark",
            >= 45 => "bg-warning text-dark",
            _ => "bg-danger"
        };

        public string MatchGrade => OverallMatchScore switch
        {
            >= 90 => "Excellent",
            >= 75 => "Strong",
            >= 60 => "Good",
            >= 45 => "Fair",
            _ => "Weak"
        };

        public string StatusBadgeClass => Status switch
        {
            ApplicationStatus.Submitted => "bg-secondary",
            ApplicationStatus.UnderReview => "bg-info text-dark",
            ApplicationStatus.Shortlisted => "bg-warning text-dark",
            ApplicationStatus.Approved => "bg-success",
            ApplicationStatus.Rejected => "bg-danger",
            _ => "bg-secondary"
        };

        public string RankMedal => RankPosition switch
        {
            1 => "🥇",
            2 => "🥈",
            3 => "🥉",
            _ => $"#{RankPosition}"
        };
    }
}