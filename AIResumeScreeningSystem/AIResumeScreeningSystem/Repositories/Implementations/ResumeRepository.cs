using AIResumeScreeningSystem.Data;
using AIResumeScreeningSystem.Models;
using AIResumeScreeningSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AIResumeScreeningSystem.Repositories.Implementations
{
    public class ResumeRepository : GenericRepository<Resume>, IResumeRepository
    {
        public ResumeRepository(AppDbContext context) : base(context) { }

        public async Task<Resume?> GetResumeWithDetailsAsync(int resumeId)
        {
            return await _context.Resumes
                .Include(r => r.Candidate)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(r => r.Id == resumeId);
        }

        public async Task<List<Resume>> GetResumesByCandidateAsync(int candidateId)
        {
            return await _context.Resumes
                .Where(r => r.CandidateId == candidateId)
                .OrderByDescending(r => r.UploadedAt)
                .ToListAsync();
        }

        public async Task<Resume?> GetActiveResumeAsync(int candidateId)
        {
            return await _context.Resumes
                .Where(r => r.CandidateId == candidateId && r.IsActive)
                .OrderByDescending(r => r.UploadedAt)
                .FirstOrDefaultAsync();
        }

        public async Task DeactivateAllResumesAsync(int candidateId)
        {
            var resumes = await _context.Resumes
                .Where(r => r.CandidateId == candidateId && r.IsActive)
                .ToListAsync();

            foreach (var r in resumes)
                r.IsActive = false;

            await _context.SaveChangesAsync();
        }

        public async Task<List<Resume>> GetAllResumesWithCandidatesAsync()
        {
            return await _context.Resumes
                .Include(r => r.Candidate)
                    .ThenInclude(c => c.User)
                .OrderByDescending(r => r.UploadedAt)
                .ToListAsync();
        }

        public async Task<Resume?> GetLatestResumeAsync(int candidateId)
        {
            return await _context.Resumes
                .Where(r => r.CandidateId == candidateId)
                .OrderByDescending(r => r.UploadedAt)
                .FirstOrDefaultAsync();
        }
    }
}