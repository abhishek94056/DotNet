using AIResumeScreeningSystem.ViewModels.SkillMatching;

namespace AIResumeScreeningSystem.Services.Interfaces
{
    public interface ICandidateRankingService
    {
        Task<CandidateRankingViewModel> GetRankedCandidatesAsync(
            int jobId,
            string? statusFilter = null,
            decimal? minScore = null);
        Task RankAllApplicationsAsync(int jobId);
        Task<(bool Success, string Error)> ShortlistCandidateAsync(
            int applicationId, string recruiterId);
        Task<(bool Success, string Error)> RejectCandidateAsync(
            int applicationId, string recruiterId, string? notes = null);
        Task<(bool Success, string Error)> ApproveCandidateAsync(
            int applicationId, string recruiterId);
        Task<(bool Success, string Error)> UpdateApplicationStatusAsync(
            int applicationId,
            string newStatus,
            string recruiterId,
            string? notes = null);
        Task<(bool Success, string Error)> BulkShortlistTopCandidatesAsync(
            int jobId, int topN, string recruiterId);
    }
}