using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MauliFarm.Models
{
    /// <summary>
    /// Extended Identity User for Mauli Farm Management System
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [StringLength(20)]
        [Display(Name = "Employee Code")]
        public string? EmployeeCode { get; set; }

        [StringLength(50)]
        [Display(Name = "Role / Designation")]
        public string? Designation { get; set; }

        [Display(Name = "Profile Picture")]
        [StringLength(500)]
        public string? ProfilePicturePath { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Created On")]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        [Display(Name = "Last Login")]
        public DateTime? LastLogin { get; set; }

        [StringLength(200)]
        [Display(Name = "Address")]
        public string? Address { get; set; }

        [StringLength(500)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        // Navigation - User created audit trails
        public virtual ICollection<UserActivityLog> ActivityLogs { get; set; } = new List<UserActivityLog>();
    }
}
