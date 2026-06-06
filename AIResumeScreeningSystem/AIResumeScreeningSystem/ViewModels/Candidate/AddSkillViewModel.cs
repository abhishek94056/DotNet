using AIResumeScreeningSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AIResumeScreeningSystem.ViewModels.Candidate
{
    public class AddSkillViewModel
    {
        [Required(ErrorMessage = "Please select a skill.")]
        [Display(Name = "Skill")]
        public int SkillId { get; set; }

        [Required]
        [Display(Name = "Proficiency Level")]
        public ProficiencyLevel ProficiencyLevel { get; set; } = ProficiencyLevel.Intermediate;

        [Display(Name = "Years of Experience")]
        [Range(0, 50, ErrorMessage = "Years must be between 0 and 50.")]
        public int YearsOfExperience { get; set; } = 0;

        // For new skill entry
        [MaxLength(100)]
        [Display(Name = "Or add a new skill")]
        public string? NewSkillName { get; set; }

        public SkillCategory NewSkillCategory { get; set; } = SkillCategory.Technical;

        // Dropdowns
        public List<SelectListItem> AvailableSkills { get; set; } = new();
        public IEnumerable<SelectListItem> ProficiencyOptions =>
            Enum.GetValues<ProficiencyLevel>()
                .Select(p => new SelectListItem(p.ToString(), ((int)p).ToString()));
        public IEnumerable<SelectListItem> CategoryOptions =>
            Enum.GetValues<SkillCategory>()
                .Select(c => new SelectListItem(c.ToString(), ((int)c).ToString()));
    }
}