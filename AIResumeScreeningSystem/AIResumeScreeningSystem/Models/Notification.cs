using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIResumeScreeningSystem.Models
{
    public enum NotificationType
    {
        Info = 0,
        Success = 1,
        Warning = 2,
        Error = 3
    }

    public class Notification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;

        public NotificationType Type { get; set; } = NotificationType.Info;

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReadAt { get; set; }

        [MaxLength(500)]
        public string? ActionUrl { get; set; }

        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = null!;
    }
}