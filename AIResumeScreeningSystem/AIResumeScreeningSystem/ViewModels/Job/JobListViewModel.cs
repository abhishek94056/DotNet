namespace AIResumeScreeningSystem.ViewModels.Job
{
    public class JobListViewModel
    {
        public List<JobViewModel> Jobs { get; set; } = new();
        public JobSearchViewModel Search { get; set; } = new();

        // Pagination
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; } = 10;

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;

        // Summary counts
        public int ActiveJobsCount { get; set; }
        public int DraftJobsCount { get; set; }
        public int ClosedJobsCount { get; set; }
        public int TotalApplicationsCount { get; set; }
    }
}