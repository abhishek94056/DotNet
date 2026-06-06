using AIResumeScreeningSystem.Models;

namespace AIResumeScreeningSystem.ViewModels.Job
{
    public class JobViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Requirements { get; set; } = string.Empty;
        public string? Location { get; set; }
        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }
        public JobType JobType { get; set; }
        public string JobTypeDisplay => JobType.ToString();
        public JobStatus Status { get; set; }
        public string StatusDisplay => Status.ToString();
        public DateTime PostedDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? Department { get; set; }
        public int ExperienceYearsMin { get; set; }
        public int ExperienceYearsMax { get; set; }
        public string PostedByName { get; set; } = string.Empty;
        public string PostedByUserId { get; set; } = string.Empty;
        public int TotalApplications { get; set; }
        public List<string> RequiredSkills { get; set; } = new();
        public List<string> OptionalSkills { get; set; } = new();

        public string SalaryRange => (SalaryMin.HasValue && SalaryMax.HasValue)
            ? $"${SalaryMin:N0} - ${SalaryMax:N0}"
            : SalaryMin.HasValue ? $"From ${SalaryMin:N0}"
            : SalaryMax.HasValue ? $"Up to ${SalaryMax:N0}"
            : "Not Disclosed";

        public string ExperienceRange => ExperienceYearsMax > 0
            ? $"{ExperienceYearsMin}–{ExperienceYearsMax} years"
            : ExperienceYearsMin > 0 ? $"{ExperienceYearsMin}+ years" : "Any level";

        public bool IsExpired => ExpiryDate.HasValue && ExpiryDate.Value < DateTime.UtcNow;

        public string StatusBadgeClass => Status switch
        {
            JobStatus.Active => "bg-success",
            JobStatus.Draft => "bg-secondary",
            JobStatus.Closed => "bg-danger",
            JobStatus.Expired => "bg-warning text-dark",
            _ => "bg-secondary"
        };
    }
}