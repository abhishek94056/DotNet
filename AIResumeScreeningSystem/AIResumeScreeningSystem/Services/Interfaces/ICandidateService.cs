using AIResumeScreeningSystem.Models;
using AIResumeScreeningSystem.ViewModels.Candidate;

namespace AIResumeScreeningSystem.Services.Interfaces
{
    public interface ICandidateService
    {
        // Profile
        Task<CandidateViewModel?> GetCandidateViewModelByUserIdAsync(string userId);
        Task<CandidateViewModel?> GetCandidateViewModelByIdAsync(int candidateId);
        Task<CandidateProfileViewModel?> GetProfileForEditAsync(string userId);
        Task<(bool Success, string Error)> UpdateProfileAsync(
            string userId, CandidateProfileViewModel model);
        Task<(bool Success, string Error)> ToggleAvailabilityAsync(string userId);

        // Skills
        Task<List<CandidateSkillViewModel>> GetCandidateSkillsAsync(int candidateId);
        Task<(bool Success, string Error)> AddSkillAsync(string userId, AddSkillViewModel model);
        Task<(bool Success, string Error)> RemoveSkillAsync(string userId, int skillId);
        Task<(bool Success, string Error)> UpdateSkillAsync(
            string userId, int candidateSkillId, ProficiencyLevel level, int years);

        // Listing & Search (for Admin/Recruiter)
        Task<CandidateListViewModel> GetCandidatesAsync(CandidateSearchViewModel search);

        // Dashboard
        Task<CandidateDashboardViewModel> GetDashboardAsync(string userId);

        // Helpers
        Task<int?> GetCandidateIdByUserIdAsync(string userId);
        Task<bool> CandidateExistsAsync(string userId);
    }
}