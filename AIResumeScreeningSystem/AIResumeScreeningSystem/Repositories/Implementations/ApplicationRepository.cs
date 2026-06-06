using AIResumeScreeningSystem.Data;
using AIResumeScreeningSystem.Models;
using AIResumeScreeningSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AIResumeScreeningSystem.Repositories.Implementations
{
    public class ApplicationRepository : GenericRepository<Application>, IApplicationRepository
    {
        public ApplicationRepository(AppDbContext context) : base(context) { }

        public async Task<Application?> GetApplicationWithDetailsAsync(int applicationId)
        {
            return await _context.Applications
                .Include(a => a.Job)
                    .ThenInclude(j => j.JobSkills)
                        .ThenInclude(js => js.Skill)
                .Include(a => a.Candidate)
                    .ThenInclude(c => c.User)
                .Include(a => a.Candidate)
                    .ThenInclude(c => c.CandidateSkills)
                        .ThenInclude(cs => cs.Skill)
                .Include(a => a.Resume)
                .Include(a => a.ReviewedBy)
                .FirstOrDefaultAsync(a => a.Id == applicationId);
        }

        public async Task<List<Application>> GetApplicationsByJobAsync(int jobId)
        {
            return await _context.Applications
                .Include(a => a.Candidate)
                    .ThenInclude(c => c.User)
                .Include(a => a.Candidate)
                    .ThenInclude(c => c.CandidateSkills)
                        .ThenInclude(cs => cs.Skill)
                .Include(a => a.Resume)
                .Where(a => a.JobId == jobId)
                .OrderByDescending(a => a.AIMatchScore)
                .ToListAsync();
        }

        public async Task<List<Application>> GetApplicationsByCandidateAsync(int candidateId)
        {
            return await _context.Applications
                .Include(a => a.Job)
                    .ThenInclude(j => j.JobSkills)
                        .ThenInclude(js => js.Skill)
                .Include(a => a.Resume)
                .Where(a => a.CandidateId == candidateId)
                .OrderByDescending(a => a.AppliedAt)
                .ToListAsync();
        }

        public async Task<Application?> GetApplicationByJobAndCandidateAsync(
            int jobId, int candidateId)
        {
            return await _context.Applications
                .Include(a => a.Job)
                .Include(a => a.Resume)
                .FirstOrDefaultAsync(a =>
                    a.JobId == jobId && a.CandidateId == candidateId);
        }

        public async Task<List<Application>> GetApplicationsWithScoresAsync(int jobId)
        {
            return await _context.Applications
                .Include(a => a.Candidate)
                    .ThenInclude(c => c.User)
                .Include(a => a.Candidate)
                    .ThenInclude(c => c.CandidateSkills)
                        .ThenInclude(cs => cs.Skill)
                .Where(a => a.JobId == jobId)
                .OrderByDescending(a => a.AIMatchScore)
                .ThenByDescending(a => a.SkillMatchPercentage)
                .ToListAsync();
        }

        public async Task<List<Application>> GetShortlistedApplicationsAsync(int jobId)
        {
            return await _context.Applications
                .Include(a => a.Candidate)
                    .ThenInclude(c => c.User)
                .Include(a => a.Job)
                .Where(a => a.JobId == jobId &&
                            a.Status == ApplicationStatus.Shortlisted)
                .OrderByDescending(a => a.AIMatchScore)
                .ToListAsync();
        }

        public async Task<bool> HasAlreadyAppliedAsync(int jobId, int candidateId)
        {
            return await _context.Applications
                .AnyAsync(a => a.JobId == jobId && a.CandidateId == candidateId);
        }

        public async Task UpdateApplicationStatusAsync(
            int applicationId,
            ApplicationStatus status,
            string reviewedByUserId)
        {
            var application = await _context.Applications.FindAsync(applicationId);
            if (application == null) return;

            application.Status = status;
            application.ReviewedByUserId = reviewedByUserId;
            application.ReviewedAt = DateTime.UtcNow;
            application.UpdatedAt = DateTime.UtcNow;

            _context.Applications.Update(application);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateApplicationScoresAsync(
            int applicationId,
            decimal overallScore,
            decimal skillMatchPct,
            int rank,
            string? evaluation,
            string? gapAnalysis,
            string? missingSkills)
        {
            var application = await _context.Applications.FindAsync(applicationId);
            if (application == null) return;

            application.AIMatchScore = overallScore;
            application.SkillMatchPercentage = skillMatchPct;
            application.RankPosition = rank;
            application.AIEvaluation = evaluation;
            application.SkillGapAnalysis = gapAnalysis;
            application.MissingSkills = missingSkills;
            application.UpdatedAt = DateTime.UtcNow;

            _context.Applications.Update(application);
            await _context.SaveChangesAsync();
        }

        public async Task<Dictionary<string, int>> GetApplicationStatsByJobAsync(int jobId)
        {
            var apps = await _context.Applications
                .Where(a => a.JobId == jobId)
                .ToListAsync();

            return new Dictionary<string, int>
            {
                { "Total", apps.Count },
                { "Submitted", apps.Count(a => a.Status == ApplicationStatus.Submitted) },
                { "UnderReview", apps.Count(a => a.Status == ApplicationStatus.UnderReview) },
                { "Shortlisted", apps.Count(a => a.Status == ApplicationStatus.Shortlisted) },
                { "Approved", apps.Count(a => a.Status == ApplicationStatus.Approved) },
                { "Rejected", apps.Count(a => a.Status == ApplicationStatus.Rejected) }
            };
        }

        public async Task<Dictionary<string, int>> GetApplicationStatsByRecruiterAsync(
            string recruiterId)
        {
            var apps = await _context.Applications
                .Include(a => a.Job)
                .Where(a => a.Job.PostedByUserId == recruiterId)
                .ToListAsync();

            return new Dictionary<string, int>
            {
                { "Total", apps.Count },
                { "Pending", apps.Count(a => a.Status == ApplicationStatus.Submitted ||
                                             a.Status == ApplicationStatus.UnderReview) },
                { "Shortlisted", apps.Count(a => a.Status == ApplicationStatus.Shortlisted) },
                { "Approved", apps.Count(a => a.Status == ApplicationStatus.Approved) },
                { "Rejected", apps.Count(a => a.Status == ApplicationStatus.Rejected) }
            };
        }
    }
}