using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIResumeScreeningSystem.Models
{
    public enum ResumeStatus
    {
        Uploaded = 0,
        Parsing = 1,
        Parsed = 2,
        Failed = 3
    }

    public class Resume
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CandidateId { get; set; }

        [Required]
        [MaxLength(500)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string FilePath { get; set; } = string.Empty;

        [MaxLength(20)]
        public string FileExtension { get; set; } = string.Empty;

        public long FileSizeBytes { get; set; }

        public ResumeStatus Status { get; set; } = ResumeStatus.Uploaded;

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ParsedAt { get; set; }

        // Parsed Data
        public string? ParsedName { get; set; }
        public string? ParsedEmail { get; set; }
        public string? ParsedPhone { get; set; }
        public string? ParsedSkills { get; set; }
        public string? ParsedEducation { get; set; }
        public string? ParsedExperience { get; set; }
        public string? ParsedSummary { get; set; }
        public string? RawText { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation Properties
        [ForeignKey("CandidateId")]
        public virtual Candidate Candidate { get; set; } = null!;
        public virtual ICollection<Application> Applications { get; set; } = new List<Application>();
    }
}