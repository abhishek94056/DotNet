using System.ComponentModel.DataAnnotations;

namespace AIResumeScreeningSystem.ViewModels.Candidate
{
    public class CandidateProfileViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [MaxLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required.")]
        [MaxLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Phone]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [MaxLength(500)]
        [Display(Name = "Professional Headline")]
        public string? Headline { get; set; }

        [Display(Name = "Professional Summary")]
        public string? Summary { get; set; }

        [MaxLength(200)]
        [Display(Name = "Current Job Title")]
        public string? CurrentJobTitle { get; set; }

        [MaxLength(200)]
        [Display(Name = "Current Company")]
        public string? CurrentCompany { get; set; }

        [Display(Name = "Total Experience (years)")]
        [Range(0, 60)]
        public int TotalExperienceYears { get; set; }

        [MaxLength(200)]
        [Display(Name = "Location")]
        public string? Location { get; set; }

        [Url]
        [MaxLength(200)]
        [Display(Name = "LinkedIn URL")]
        public string? LinkedInUrl { get; set; }

        [Url]
        [MaxLength(200)]
        [Display(Name = "GitHub URL")]
        public string? GitHubUrl { get; set; }

        [Url]
        [MaxLength(200)]
        [Display(Name = "Portfolio URL")]
        public string? PortfolioUrl { get; set; }

        [MaxLength(100)]
        [Display(Name = "Highest Education")]
        public string? HighestEducation { get; set; }

        [MaxLength(200)]
        [Display(Name = "University / Institution")]
        public string? University { get; set; }

        [Display(Name = "Graduation Year")]
        [Range(1950, 2100)]
        public int? GraduationYear { get; set; }

        [Display(Name = "Available for Work")]
        public bool IsAvailable { get; set; } = true;

        [Display(Name = "Profile Photo")]
        public IFormFile? ProfileImage { get; set; }

        public string? ExistingProfileImagePath { get; set; }
    }
}