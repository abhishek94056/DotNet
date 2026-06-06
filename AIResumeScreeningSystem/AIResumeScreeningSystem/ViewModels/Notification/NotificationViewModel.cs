using AIResumeScreeningSystem.Models;

namespace AIResumeScreeningSystem.ViewModels.Notification
{
    public class NotificationViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public string? ActionUrl { get; set; }

        public string TypeBadgeClass => Type switch
        {
            NotificationType.Success => "bg-success",
            NotificationType.Warning => "bg-warning text-dark",
            NotificationType.Error => "bg-danger",
            _ => "bg-primary"
        };

        public string TypeIcon => Type switch
        {
            NotificationType.Success => "bi-check-circle-fill text-success",
            NotificationType.Warning => "bi-exclamation-triangle-fill text-warning",
            NotificationType.Error => "bi-x-circle-fill text-danger",
            _ => "bi-info-circle-fill text-primary"
        };

        public string TimeAgo
        {
            get
            {
                var diff = DateTime.UtcNow - CreatedAt;
                if (diff.TotalMinutes < 1) return "just now";
                if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes}m ago";
                if (diff.TotalHours < 24) return $"{(int)diff.TotalHours}h ago";
                if (diff.TotalDays < 7) return $"{(int)diff.TotalDays}d ago";
                return CreatedAt.ToString("MMM dd, yyyy");
            }
        }
    }
}