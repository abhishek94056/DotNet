using AIResumeScreeningSystem.ViewModels.Resume;

namespace AIResumeScreeningSystem.Services.Interfaces
{
    public interface IResumeService
    {
        Task<ResumeUploadViewModel> GetUploadViewModelAsync(string userId);
        Task<(bool Success, int ResumeId, string Error)> UploadResumeAsync(
            string userId, ResumeUploadViewModel model);
        Task<(bool Success, string Error)> ParseResumeAsync(int resumeId);
        Task<ResumeViewModel?> GetResumeByIdAsync(int resumeId);
        Task<ResumeListViewModel> GetCandidateResumesAsync(int candidateId);
        Task<ResumeListViewModel> GetCandidateResumesByUserIdAsync(string userId);
        Task<(bool Success, string Error)> SetActiveResumeAsync(string userId, int resumeId);
        Task<(bool Success, string Error)> DeleteResumeAsync(string userId, int resumeId);
        Task<string?> GetActiveResumePathAsync(int candidateId);
        Task<int?> GetActiveResumeIdAsync(int candidateId);
    }
}