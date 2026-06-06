using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MauliFarm.Models
{
    /// <summary>
    /// Custom Role with description for Mauli Farm
    /// </summary>
    public class ApplicationRole : IdentityRole
    {
        [StringLength(300)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Created On")]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// Predefined farm roles as constants
    /// </summary>
    public static class FarmRoles
    {
        public const string SuperAdmin    = "SuperAdmin";
        public const string Admin         = "Admin";
        public const string FarmManager   = "FarmManager";
        public const string Supervisor    = "Supervisor";
        public const string AccountsStaff = "AccountsStaff";
        public const string ViewOnly      = "ViewOnly";

        public static readonly string[] AllRoles =
        [
            SuperAdmin,
            Admin,
            FarmManager,
            Supervisor,
            AccountsStaff,
            ViewOnly
        ];

        public static readonly Dictionary<string, string> RoleDescriptions = new()
        {
            { SuperAdmin,    "Full system access — owner / developer level" },
            { Admin,         "Full operational access across all modules" },
            { FarmManager,   "Manage labour, harvest, expenses, and reports" },
            { Supervisor,    "Manage daily field operations and labour attendance" },
            { AccountsStaff, "Access to expenses, payroll, and financial reports only" },
            { ViewOnly,      "Read-only access to reports and dashboards" }
        };
    }
}
