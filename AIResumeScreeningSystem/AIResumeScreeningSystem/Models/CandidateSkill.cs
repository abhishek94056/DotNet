using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIResumeScreeningSystem.Models
{
    public enum ProficiencyLevel
    {
        Beginner = 1,
        Intermediate = 2,
        Advanced = 3,
        Expert = 4
    }

    public class CandidateSkill
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CandidateId { get; set; }

        [Required]
        public int SkillId { get; set; }

        public ProficiencyLevel ProficiencyLevel { get; set; } = ProficiencyLevel.Intermediate;

        public int YearsOfExperience { get; set; } = 0;

        public bool IsVerified { get; set; } = false;

        // Navigation Properties
        [ForeignKey("CandidateId")]
        public virtual Candidate Candidate { get; set; } = null!;

        [ForeignKey("SkillId")]
        public virtual Skill Skill { get; set; } = null!;
    }
}