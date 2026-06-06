namespace AIResumeScreeningSystem.ViewModels.Dashboard
{
    public class AdminDashboardViewModel
    {
        // ── KPI Stats ──────────────────────────────────────────────────────
        public int TotalUsers { get; set; }
        public int TotalCandidates { get; set; }
        public int TotalRecruiters { get; set; }
        public int TotalJobs { get; set; }
        public int ActiveJobs { get; set; }
        public int TotalApplications { get; set; }
        public int TotalResumes { get; set; }
        public int ParsedResumes { get; set; }
        public int ShortlistedApplications { get; set; }
        public int ApprovedApplications { get; set; }

        // Month-on-month growth
        public int NewUsersThisMonth { get; set; }
        public int NewJobsThisMonth { get; set; }
        public int NewApplicationsThisMonth { get; set; }
        public decimal AverageMatchScore { get; set; }

        // ── Charts ─────────────────────────────────────────────────────────
        public ChartDataSet ApplicationsByStatus { get; set; } = new();
        public ChartDataSet ApplicationsTrend { get; set; } = new();
        public ChartDataSet UserGrowth { get; set; } = new();
        public ChartDataSet TopSkillsDistribution { get; set; } = new();
        public ChartDataSet JobsByType { get; set; } = new();

        // ── Lists ──────────────────────────────────────────────────────────
        public List<TopJobItem> TopJobs { get; set; } = new();
        public List<TopCandidateItem> TopCandidates { get; set; } = new();
        public List<ActivityFeedItem> RecentActivity { get; set; } = new();

        // ── Quick Stats ────────────────────────────────────────────────────
        public int TotalInterviewQuestions { get; set; }
        public int AIEvaluationsRun { get; set; }
        public decimal AIAccuracyRate { get; set; }

        // ── System Health ──────────────────────────────────────────────────
        public int PendingParseQueue { get; set; }
        public int FailedParseCount { get; set; }
    }
}