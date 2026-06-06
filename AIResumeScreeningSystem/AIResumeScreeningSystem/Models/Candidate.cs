using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIResumeScreeningSystem.Models
{
    public class Candidate
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Headline { get; set; }

        public string? Summary { get; set; }

        [MaxLength(200)]
        public string? CurrentJobTitle { get; set; }

        [MaxLength(200)]
        public string? CurrentCompany { get; set; }

        public int TotalExperienceYears { get; set; } = 0;

        [MaxLength(200)]
        public string? Location { get; set; }

        [MaxLength(200)]
        public string? LinkedInUrl { get; set; }

        [MaxLength(200)]
        public string? GitHubUrl { get; set; }

        [MaxLength(200)]
        public string? PortfolioUrl { get; set; }

        [MaxLength(100)]
        public string? HighestEducation { get; set; }

        [MaxLength(200)]
        public string? University { get; set; }

        public int? GraduationYear { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public bool IsAvailable { get; set; } = true;

        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = null!;
        public virtual ICollection<Resume> Resumes { get; set; } = new List<Resume>();
        public virtual ICollection<CandidateSkill> CandidateSkills { get; set; } = new List<CandidateSkill>();
        public virtual ICollection<Application> Applications { get; set; } = new List<Application>();
    }
}