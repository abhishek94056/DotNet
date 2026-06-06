namespace AIResumeScreeningSystem.Helpers
{
    public static class AppConstants
    {
        // ── Roles ──────────────────────────────────────────────────────────
        public const string RoleAdmin = "Admin";
        public const string RoleRecruiter = "Recruiter";
        public const string RoleCandidate = "Candidate";

        // ── File Upload ────────────────────────────────────────────────────
        public const int MaxFileSizeMB = 5;
        public const int MaxFileSizeBytes = MaxFileSizeMB * 1024 * 1024;
        public static readonly string[] AllowedResumeExtensions =
            { ".pdf", ".docx", ".doc" };
        public static readonly string[] AllowedImageExtensions =
            { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        public const int MaxProfileImageSizeBytes = 2 * 1024 * 1024; // 2MB

        // ── Pagination ─────────────────────────────────────────────────────
        public const int DefaultPageSize = 10;
        public const int CandidatePageSize = 12;
        public const int MaxPageSize = 100;

        // ── AI Scoring Weights ─────────────────────────────────────────────
        public const decimal RequiredSkillWeight = 55m;
        public const decimal OptionalSkillWeight = 15m;
        public const decimal ExperienceWeight = 20m;
        public const decimal EducationWeight = 10m;

        // ── Score Thresholds ───────────────────────────────────────────────
        public const decimal ExcellentScoreThreshold = 90m;
        public const decimal StrongScoreThreshold = 75m;
        public const decimal GoodScoreThreshold = 60m;
        public const decimal FairScoreThreshold = 45m;

        // ── Notifications ──────────────────────────────────────────────────
        public const int MaxNotificationsPerUser = 50;
        public const int NotificationRefreshSeconds = 30;

        // ── Cache Keys ─────────────────────────────────────────────────────
        public const string CacheKeyDashboardAdmin = "dashboard_admin";
        public const string CacheKeyTopSkills = "top_skills";

        // ── Resume Parsing ─────────────────────────────────────────────────
        public const int MaxRawTextLength = 10000;
        public const int MaxParsedSectionLength = 2000;
        public const int MaxSkillsExtracted = 50;

        // ── Reports ────────────────────────────────────────────────────────
        public const int MaxReportRowsPerSheet = 1000;
        public const string ReportFolderRelative = "/uploads/reports/";
    }
}