using AIResumeScreeningSystem.Models;

namespace AIResumeScreeningSystem.Repositories.Interfaces
{
    public interface IApplicationRepository : IGenericRepository<Application>
    {
        Task<Application?> GetApplicationWithDetailsAsync(int applicationId);
        Task<List<Application>> GetApplicationsByJobAsync(int jobId);
        Task<List<Application>> GetApplicationsByCandidateAsync(int candidateId);
        Task<Application?> GetApplicationByJobAndCandidateAsync(int jobId, int candidateId);
        Task<List<Application>> GetApplicationsWithScoresAsync(int jobId);
        Task<List<Application>> GetShortlistedApplicationsAsync(int jobId);
        Task<bool> HasAlreadyAppliedAsync(int jobId, int candidateId);
        Task UpdateApplicationStatusAsync(int applicationId, ApplicationStatus status, string reviewedByUserId);
        Task UpdateApplicationScoresAsync(
            int applicationId,
            decimal overallScore,
            decimal skillMatchPct,
            int rank,
            string? evaluation,
            string? gapAnalysis,
            string? missingSkills);
        Task<Dictionary<string, int>> GetApplicationStatsByJobAsync(int jobId);
        Task<Dictionary<string, int>> GetApplicationStatsByRecruiterAsync(string recruiterId);
    }
}