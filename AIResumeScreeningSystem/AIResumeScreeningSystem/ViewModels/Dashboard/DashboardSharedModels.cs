namespace AIResumeScreeningSystem.ViewModels.Dashboard
{
    public class StatCardItem
    {
        public string Label { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string ColorClass { get; set; } = "primary";
        public string? Trend { get; set; }
        public bool TrendUp { get; set; }
        public string? SubLabel { get; set; }
    }

    public class ChartDataSet
    {
        public List<string> Labels { get; set; } = new();
        public List<decimal> Values { get; set; } = new();
        public List<string>? BackgroundColors { get; set; }
        public string DatasetLabel { get; set; } = string.Empty;
    }

    public class ActivityFeedItem
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = "bi-dot";
        public string IconColorClass { get; set; } = "text-primary";
        public DateTime Timestamp { get; set; }
        public string? ActionUrl { get; set; }

        public string TimeAgo
        {
            get
            {
                var diff = DateTime.UtcNow - Timestamp;
                if (diff.TotalMinutes < 1) return "just now";
                if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes}m ago";
                if (diff.TotalHours < 24) return $"{(int)diff.TotalHours}h ago";
                if (diff.TotalDays < 7) return $"{(int)diff.TotalDays}d ago";
                return Timestamp.ToString("MMM dd");
            }
        }
    }

    public class TopJobItem
    {
        public int JobId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public int ApplicationCount { get; set; }
        public decimal AverageMatchScore { get; set; }
        public int ShortlistedCount { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class TopCandidateItem
    {
        public int CandidateId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Headline { get; set; }
        public decimal HighestMatchScore { get; set; }
        public int ApplicationCount { get; set; }
        public string? Location { get; set; }
        public string InitialsAvatar { get; set; } = string.Empty;
        public string? ProfileImagePath { get; set; }
        public List<string> TopSkills { get; set; } = new();
    }

    public class ApplicationTrendItem
    {
        public string Period { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal AverageScore { get; set; }
    }
}