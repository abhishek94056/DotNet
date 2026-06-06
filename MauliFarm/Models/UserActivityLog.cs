using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MauliFarm.Models
{
    /// <summary>
    /// Tracks user login/logout events and important system actions
    /// </summary>
    public class UserActivityLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; } = null!;

        [Required]
        [StringLength(100)]
        [Display(Name = "Activity Type")]
        public string ActivityType { get; set; } = string.Empty;   // Login, Logout, PasswordChange, etc.

        [StringLength(500)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [StringLength(50)]
        [Display(Name = "IP Address")]
        public string? IpAddress { get; set; }

        [StringLength(300)]
        [Display(Name = "User Agent")]
        public string? UserAgent { get; set; }

        [Display(Name = "Timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [Display(Name = "Was Successful")]
        public bool IsSuccess { get; set; } = true;
    }

    /// <summary>
    /// Activity type constants
    /// </summary>
    public static class ActivityTypes
    {
        public const string Login          = "Login";
        public const string Logout         = "Logout";
        public const string LoginFailed    = "LoginFailed";
        public const string PasswordChange = "PasswordChange";
        public const string ProfileUpdate  = "ProfileUpdate";
        public const string UserCreated    = "UserCreated";
        public const string UserDeactivated = "UserDeactivated";
        public const string RoleAssigned   = "RoleAssigned";
    }
}
