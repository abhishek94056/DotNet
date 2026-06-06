using System.ComponentModel.DataAnnotations;

namespace AIResumeScreeningSystem.Models
{
    public enum SkillCategory
    {
        Technical = 0,
        Soft = 1,
        Language = 2,
        Tool = 3,
        Framework = 4,
        Other = 5
    }

    public class Skill
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public SkillCategory Category { get; set; } = SkillCategory.Technical;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual ICollection<CandidateSkill> CandidateSkills { get; set; } = new List<CandidateSkill>();
        public virtual ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();
    }
}