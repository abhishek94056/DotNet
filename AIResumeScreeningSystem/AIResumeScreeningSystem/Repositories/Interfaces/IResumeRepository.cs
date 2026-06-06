using AIResumeScreeningSystem.Models;

namespace AIResumeScreeningSystem.Repositories.Interfaces
{
    public interface IResumeRepository : IGenericRepository<Resume>
    {
        Task<Resume?> GetResumeWithDetailsAsync(int resumeId);
        Task<List<Resume>> GetResumesByCandidateAsync(int candidateId);
        Task<Resume?> GetActiveResumeAsync(int candidateId);
        Task DeactivateAllResumesAsync(int candidateId);
        Task<List<Resume>> GetAllResumesWithCandidatesAsync();
        Task<Resume?> GetLatestResumeAsync(int candidateId);
    }
}