using AIResumeScreeningSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AIResumeScreeningSystem.ViewModels.Job
{
    public class CreateJobViewModel
    {
        [Required(ErrorMessage = "Job title is required.")]
        [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        [Display(Name = "Job Title")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Company name is required.")]
        [MaxLength(200)]
        [Display(Name = "Company")]
        public string Company { get; set; } = string.Empty;

        [Required(ErrorMessage = "Job description is required.")]
        [MinLength(50, ErrorMessage = "Description must be at least 50 characters.")]
        [Display(Name = "Job Description")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Requirements are required.")]
        [Display(Name = "Requirements")]
        public string Requirements { get; set; } = string.Empty;

        [MaxLength(200)]
        [Display(Name = "Location")]
        public string? Location { get; set; }

        [Display(Name = "Minimum Salary")]
        [Range(0, 9999999, ErrorMessage = "Salary must be a positive number.")]
        public decimal? SalaryMin { get; set; }

        [Display(Name = "Maximum Salary")]
        [Range(0, 9999999, ErrorMessage = "Salary must be a positive number.")]
        public decimal? SalaryMax { get; set; }

        [Required]
        [Display(Name = "Job Type")]
        public JobType JobType { get; set; } = JobType.FullTime;

        [Required]
        [Display(Name = "Status")]
        public JobStatus Status { get; set; } = JobStatus.Active;

        [Display(Name = "Application Deadline")]
        [DataType(DataType.Date)]
        public DateTime? ExpiryDate { get; set; }

        [MaxLength(100)]
        [Display(Name = "Department")]
        public string? Department { get; set; }

        [Display(Name = "Min Experience (years)")]
        [Range(0, 50)]
        public int ExperienceYearsMin { get; set; } = 0;

        [Display(Name = "Max Experience (years)")]
        [Range(0, 50)]
        public int ExperienceYearsMax { get; set; } = 0;

        // Skill IDs selected for this job
        [Display(Name = "Required Skills")]
        public List<int> RequiredSkillIds { get; set; } = new();

        [Display(Name = "Optional Skills")]
        public List<int> OptionalSkillIds { get; set; } = new();

        // Populated for dropdowns
        public List<SelectListItem> AvailableSkills { get; set; } = new();
        public IEnumerable<SelectListItem> JobTypeOptions => Enum.GetValues<JobType>()
            .Select(jt => new SelectListItem(jt.ToString(), ((int)jt).ToString()));
        public IEnumerable<SelectListItem> StatusOptions => Enum.GetValues<JobStatus>()
            .Select(s => new SelectListItem(s.ToString(), ((int)s).ToString()));
    }
}
