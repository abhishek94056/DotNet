using AIResumeScreeningSystem.Models;

namespace AIResumeScreeningSystem.ViewModels.Job
{
    public class JobSearchViewModel
    {
        public string? Keyword { get; set; }
        public string? Location { get; set; }
        public JobType? JobType { get; set; }
        public JobStatus? Status { get; set; }
        public string? Department { get; set; }
        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }
        public int? ExperienceYears { get; set; }
        public string SortBy { get; set; } = "PostedDate";
        public string SortDirection { get; set; } = "desc";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}