using AIResumeScreeningSystem.Models;
using AIResumeScreeningSystem.ViewModels.Job;

namespace AIResumeScreeningSystem.Repositories.Interfaces
{
    public interface IJobRepository : IGenericRepository<Job>
    {
        Task<Job?> GetJobWithDetailsAsync(int jobId);
        Task<(IEnumerable<Job> Jobs, int TotalCount)> GetPagedJobsAsync(
            JobSearchViewModel search,
            string? postedByUserId = null);
        Task<IEnumerable<Job>> GetActiveJobsAsync();
        Task<IEnumerable<Job>> GetJobsByUserAsync(string userId);
        Task<IEnumerable<Job>> GetJobsWithSkillsAsync();
        Task UpdateJobSkillsAsync(int jobId, List<int> requiredSkillIds, List<int> optionalSkillIds);
        Task<Dictionary<string, int>> GetJobStatsByUserAsync(string userId);
        Task<int> GetTotalApplicationsForJobAsync(int jobId);
    }
}