using AIResumeScreeningSystem.Models;

namespace AIResumeScreeningSystem.ViewModels.Candidate
{
    public class CandidateDashboardViewModel
    {
        // Profile summary
        public int CandidateId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Headline { get; set; }
        public string? ProfileImagePath { get; set; }
        public int ProfileCompletionPercent { get; set; }
        public bool IsAvailable { get; set; }
        public string InitialsAvatar { get; set; } = string.Empty;

        // Application stats
        public int TotalApplications { get; set; }
        public int PendingApplications { get; set; }
        public int ShortlistedApplications { get; set; }
        public int RejectedApplications { get; set; }
        public int ApprovedApplications { get; set; }

        // Resume stats
        public int TotalResumes { get; set; }
        public bool HasActiveResume { get; set; }

        // Skill stats
        public int TotalSkills { get; set; }

        // Recent Applications
        public List<RecentApplicationItem> RecentApplications { get; set; } = new();

        // Recommended Jobs (active jobs with matching skills)
        public List<RecommendedJobItem> RecommendedJobs { get; set; } = new();

        // AI match scores over time (for chart)
        public List<string> ChartLabels { get; set; } = new();
        public List<decimal> ChartScores { get; set; } = new();
    }

    public class RecentApplicationItem
    {
        public int ApplicationId { get; set; }
        public int JobId { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public ApplicationStatus Status { get; set; }
        public decimal? AIMatchScore { get; set; }
        public DateTime AppliedAt { get; set; }

        public string StatusBadgeClass => Status switch
        {
            ApplicationStatus.Submitted => "bg-secondary",
            ApplicationStatus.UnderReview => "bg-info",
            ApplicationStatus.Shortlisted => "bg-warning text-dark",
            ApplicationStatus.Approved => "bg-success",
            ApplicationStatus.Rejected => "bg-danger",
            _ => "bg-secondary"
        };
    }

    public class RecommendedJobItem
    {
        public int JobId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string SalaryRange { get; set; } = string.Empty;
        public int MatchPercent { get; set; }
        public List<string> MatchingSkills { get; set; } = new();
    }
}