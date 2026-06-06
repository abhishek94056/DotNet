using AIResumeScreeningSystem.ViewModels.Job;

namespace AIResumeScreeningSystem.Services.Interfaces
{
    public interface IJobService
    {
        Task<JobListViewModel> GetJobsAsync(JobSearchViewModel search, string? postedByUserId = null);
        Task<JobViewModel?> GetJobByIdAsync(int jobId);
        Task<(bool Success, int JobId, string Error)> CreateJobAsync(CreateJobViewModel model, string postedByUserId);
        Task<EditJobViewModel?> GetJobForEditAsync(int jobId);
        Task<(bool Success, string Error)> UpdateJobAsync(EditJobViewModel model, string currentUserId);
        Task<(bool Success, string Error)> DeleteJobAsync(int jobId, string currentUserId);
        Task<(bool Success, string Error)> ToggleJobStatusAsync(int jobId, string currentUserId);
        Task<List<JobViewModel>> GetActiveJobsAsync();
        Task<JobListViewModel> GetPublicJobsAsync(JobSearchViewModel search);
        Task<bool> IsJobOwnedByUserAsync(int jobId, string userId);
        Task<Dictionary<string, int>> GetJobStatsByUserAsync(string userId);
    }
}