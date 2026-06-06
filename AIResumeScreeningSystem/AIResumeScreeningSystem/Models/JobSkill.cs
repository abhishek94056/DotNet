using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIResumeScreeningSystem.Models
{
    public class JobSkill
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int JobId { get; set; }

        [Required]
        public int SkillId { get; set; }

        public bool IsRequired { get; set; } = true;

        public int WeightagePercent { get; set; } = 10;

        // Navigation Properties
        [ForeignKey("JobId")]
        public virtual Job Job { get; set; } = null!;

        [ForeignKey("SkillId")]
        public virtual Skill Skill { get; set; } = null!;
    }
}