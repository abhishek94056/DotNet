using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AIResumeScreeningSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;

        [MaxLength(500)]
        public string? ProfileImagePath { get; set; }

        // Navigation Properties
        public virtual Candidate? Candidate { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}