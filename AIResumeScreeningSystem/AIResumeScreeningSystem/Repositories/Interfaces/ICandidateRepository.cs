using AIResumeScreeningSystem.Models;
using AIResumeScreeningSystem.ViewModels.Candidate;

namespace AIResumeScreeningSystem.Repositories.Interfaces
{
    public interface ICandidateRepository : IGenericRepository<Candidate>
    {
        Task<Candidate?> GetCandidateByUserIdAsync(string userId);
        Task<Candidate?> GetCandidateWithDetailsAsync(int candidateId);
        Task<Candidate?> GetCandidateWithDetailsByUserIdAsync(string userId);
        Task<(IEnumerable<Candidate> Candidates, int TotalCount)> GetPagedCandidatesAsync(
            CandidateSearchViewModel search);
        Task<IEnumerable<Candidate>> GetCandidatesWithSkillsAsync();
        Task<bool> AddCandidateSkillAsync(CandidateSkill skill);
        Task<bool> RemoveCandidateSkillAsync(int candidateId, int skillId);
        Task<bool> UpdateCandidateSkillAsync(int candidateSkillId, ProficiencyLevel level, int years);
        Task<CandidateSkill?> GetCandidateSkillAsync(int candidateId, int skillId);
        Task<List<CandidateSkill>> GetCandidateSkillsAsync(int candidateId);
        Task<Dictionary<string, int>> GetCandidateStatsAsync();
        Task<int> GetProfileCompletionPercentAsync(int candidateId);
    }
}