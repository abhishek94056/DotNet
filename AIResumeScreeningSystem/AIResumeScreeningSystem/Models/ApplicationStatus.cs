using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIResumeScreeningSystem.Models
{
    public enum ApplicationStatus
    {
        Submitted = 0,
        UnderReview = 1,
        Shortlisted = 2,
        InterviewScheduled = 3,
        Rejected = 4,
        Approved = 5,
        Withdrawn = 6
    }

    public class Application
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int JobId { get; set; }

        [Required]
        public int CandidateId { get; set; }

        [Required]
        public int ResumeId { get; set; }

        public ApplicationStatus Status { get; set; } = ApplicationStatus.Submitted;

        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReviewedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? AIMatchScore { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? SkillMatchPercentage { get; set; }

        public int? RankPosition { get; set; }

        public string? AIEvaluation { get; set; }
        public string? SkillGapAnalysis { get; set; }
        public string? RecruiterNotes { get; set; }
        public string? MissingSkills { get; set; }

        public string? ReviewedByUserId { get; set; }

        // Navigation Properties
        [ForeignKey("JobId")]
        public virtual Job Job { get; set; } = null!;

        [ForeignKey("CandidateId")]
        public virtual Candidate Candidate { get; set; } = null!;

        [ForeignKey("ResumeId")]
        public virtual Resume Resume { get; set; } = null!;

        [ForeignKey("ReviewedByUserId")]
        public virtual ApplicationUser? ReviewedBy { get; set; }
    }
}