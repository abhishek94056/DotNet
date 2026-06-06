using AIResumeScreeningSystem.DTOs;
using AIResumeScreeningSystem.ViewModels.SkillMatching;

namespace AIResumeScreeningSystem.Services.Interfaces
{
    public interface ISkillMatchingService
    {
        Task<SkillMatchDto> CalculateMatchAsync(int jobId, int candidateId);
        Task<SkillMatchDto> CalculateMatchForApplicationAsync(int applicationId);
        Task<decimal> GetQuickMatchScoreAsync(int jobId, int candidateId);
        Task<SkillGapViewModel> GetSkillGapAsync(int applicationId);
        Task<SkillMatchResultViewModel> GetApplicationMatchResultAsync(int applicationId);
        Task RecalculateAllApplicationScoresAsync(int jobId);
        Task RecalculateSingleApplicationScoreAsync(int applicationId);
    }
}