using AIResumeScreeningSystem.Models;

namespace AIResumeScreeningSystem.DTOs
{
    public class JobDto
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
        public JobStatus Status { get; set; }
        public DateTime PostedDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string PostedByUserId { get; set; } = string.Empty;
        public string PostedByName { get; set; } = string.Empty;
        public string? Department { get; set; }
        public int ExperienceYearsMin { get; set; }
        public int ExperienceYearsMax { get; set; }
        public int TotalApplications { get; set; }
        public List<string> RequiredSkills { get; set; } = new();
    }
}