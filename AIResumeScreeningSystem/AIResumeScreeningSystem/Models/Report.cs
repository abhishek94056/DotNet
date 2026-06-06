using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIResumeScreeningSystem.Models
{
    public enum ReportType
    {
        CandidateReport = 0,
        JobReport = 1,
        AIRankingReport = 2
    }

    public enum ReportFormat
    {
        Excel = 0,
        PDF = 1
    }

    public class Report
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        public ReportType ReportType { get; set; }

        public ReportFormat Format { get; set; }

        [MaxLength(1000)]
        public string? FilePath { get; set; }

        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public string GeneratedByUserId { get; set; } = string.Empty;

        public int? JobId { get; set; }

        // Navigation Properties
        [ForeignKey("GeneratedByUserId")]
        public virtual ApplicationUser GeneratedBy { get; set; } = null!;

        [ForeignKey("JobId")]
        public virtual Job? Job { get; set; }
    }
}