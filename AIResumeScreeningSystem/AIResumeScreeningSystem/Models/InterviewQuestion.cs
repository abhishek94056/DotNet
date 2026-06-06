using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIResumeScreeningSystem.Models
{
    public enum QuestionType
    {
        Technical = 0,
        Behavioral = 1,
        Situational = 2,
        CulturalFit = 3
    }

    public enum DifficultyLevel
    {
        Easy = 1,
        Medium = 2,
        Hard = 3
    }

    public class InterviewQuestion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int JobId { get; set; }

        public int? ApplicationId { get; set; }

        [Required]
        public string Question { get; set; } = string.Empty;

        public string? ExpectedAnswer { get; set; }

        public QuestionType QuestionType { get; set; } = QuestionType.Technical;

        public DifficultyLevel DifficultyLevel { get; set; } = DifficultyLevel.Medium;

        public bool IsAIGenerated { get; set; } = true;

        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("JobId")]
        public virtual Job Job { get; set; } = null!;

        [ForeignKey("ApplicationId")]
        public virtual Application? Application { get; set; }
    }
}