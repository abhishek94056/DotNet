using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIResumeScreeningSystem.Models
{
    public enum JobStatus
    {
        Draft = 0,
        Active = 1,
        Closed = 2,
        Expired = 3
    }

    public enum JobType
    {
        FullTime = 0,
        PartTime = 1,
        Contract = 2,
        Internship = 3,
        Remote = 4
    }

    public class Job
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Company { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Requirements { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Location { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? SalaryMin { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? SalaryMax { get; set; }

        public JobType JobType { get; set; } = JobType.FullTime;

        public JobStatus Status { get; set; } = JobStatus.Draft;

        public DateTime PostedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ExpiryDate { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [Required]
        public string PostedByUserId { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Department { get; set; }

        public int ExperienceYearsMin { get; set; } = 0;
        public int ExperienceYearsMax { get; set; } = 0;

        // Navigation Properties
        [ForeignKey("PostedByUserId")]
        public virtual ApplicationUser PostedBy { get; set; } = null!;
        public virtual ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();
        public virtual ICollection<Application> Applications { get; set; } = new List<Application>();
        public virtual ICollection<InterviewQuestion> InterviewQuestions { get; set; } = new List<InterviewQuestion>();
    }
}