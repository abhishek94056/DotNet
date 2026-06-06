namespace AIResumeScreeningSystem.ViewModels.Dashboard
{
    public class RecruiterDashboardViewModel
    {
        // ── Profile ────────────────────────────────────────────────────────
        public string RecruiterName { get; set; } = string.Empty;
        public string RecruiterEmail { get; set; } = string.Empty;
        public string InitialsAvatar { get; set; } = string.Empty;

        // ── Job Stats ──────────────────────────────────────────────────────
        public int TotalJobs { get; set; }
        public int ActiveJobs { get; set; }
        public int DraftJobs { get; set; }
        public int ClosedJobs { get; set; }

        // ── Application Stats ──────────────────────────────────────────────
        public int TotalApplications { get; set; }
        public int PendingReview { get; set; }
        public int Shortlisted { get; set; }
        public int Approved { get; set; }
        public int Rejected { get; set; }
        public decimal AverageMatchScore { get; set; }

        // ── Charts ─────────────────────────────────────────────────────────
        public ChartDataSet ApplicationsByStatus { get; set; } = new();
        public ChartDataSet ApplicationsTrend { get; set; } = new();
        public ChartDataSet ScoreDistribution { get; set; } = new();
        public ChartDataSet JobPerformance { get; set; } = new();

        // ── Lists ──────────────────────────────────────────────────────────
        public List<TopJobItem> MyTopJobs { get; set; } = new();
        public List<TopCandidateItem> TopCandidates { get; set; } = new();
        public List<ActivityFeedItem> RecentActivity { get; set; } = new();

        // ── Pending Actions ────────────────────────────────────────────────
        public List<PendingActionItem> PendingActions { get; set; } = new();
    }

    public class PendingActionItem
    {
        public int ApplicationId { get; set; }
        public int JobId { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public decimal MatchScore { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime AppliedAt { get; set; }
        public string InitialsAvatar { get; set; } = string.Empty;

        public string TimeAgo
        {
            get
            {
                var diff = DateTime.UtcNow - AppliedAt;
                if (diff.TotalHours < 24) return $"{(int)diff.TotalHours}h ago";
                return $"{(int)diff.TotalDays}d ago";
            }
        }

        public string ScoreColorClass => MatchScore switch
        {
            >= 75 => "text-success",
            >= 50 => "text-primary",
            >= 30 => "text-warning",
            _ => "text-danger"
        };
    }
}