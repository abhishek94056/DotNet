using AIResumeScreeningSystem.Data;
using AIResumeScreeningSystem.Models;
using AIResumeScreeningSystem.Repositories.Interfaces;
using AIResumeScreeningSystem.Services.Interfaces;
using AIResumeScreeningSystem.ViewModels.SkillMatching;
using Microsoft.EntityFrameworkCore;

namespace AIResumeScreeningSystem.Services.Implementations
{
    public class CandidateRankingService : ICandidateRankingService
    {
        private readonly IApplicationRepository _applicationRepo;
        private readonly ISkillMatchingService _skillMatchingService;
        private readonly AppDbContext _context;
        private readonly ILogger<CandidateRankingService> _logger;

        public CandidateRankingService(
            IApplicationRepository applicationRepo,
            ISkillMatchingService skillMatchingService,
            AppDbContext context,
            ILogger<CandidateRankingService> logger)
        {
            _applicationRepo = applicationRepo;
            _skillMatchingService = skillMatchingService;
            _context = context;
            _logger = logger;
        }

        // ─── Ranked Candidate List ─────────────────────────────────────────

        public async Task<CandidateRankingViewModel> GetRankedCandidatesAsync(
            int jobId,
            string? statusFilter = null,
            decimal? minScore = null)
        {
            var job = await _context.Jobs
                .Include(j => j.JobSkills).ThenInclude(js => js.Skill)
                .FirstOrDefaultAsync(j => j.Id == jobId);

            if (job == null)
                return new CandidateRankingViewModel { JobId = jobId };

            var applications = await _applicationRepo.GetApplicationsByJobAsync(jobId);

            // Apply filters
            if (!string.IsNullOrEmpty(statusFilter) &&
                Enum.TryParse<ApplicationStatus>(statusFilter, out var statusEnum))
                applications = applications.Where(a => a.Status == statusEnum).ToList();

            if (minScore.HasValue)
                applications = applications
                    .Where(a => a.AIMatchScore >= minScore.Value).ToList();

            // Build ranked items
            var ranked = new List<RankedCandidateItem>();

            foreach (var app in applications.OrderByDescending(a => a.AIMatchScore))
            {
                var candidate = app.Candidate;
                if (candidate == null) continue;

                var fullName = $"{candidate.User.FirstName} {candidate.User.LastName}";

                // Calculate match if not yet scored
                decimal score = app.AIMatchScore ?? 0;
                List<string> matchedSkills = new();
                List<string> missingSkills = new();

                if (app.AIMatchScore == null)
                {
                    var match = await _skillMatchingService
                        .CalculateMatchAsync(jobId, candidate.Id);
                    score = match.OverallMatchScore;
                    matchedSkills = match.MatchedRequiredSkills;
                    missingSkills = match.MissingRequiredSkills;
                }
                else
                {
                    // Use stored missing skills
                    missingSkills = string.IsNullOrEmpty(app.MissingSkills)
                        ? new List<string>()
                        : app.MissingSkills
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => s.Trim())
                            .ToList();

                    matchedSkills = candidate.CandidateSkills
                        .Where(cs => job.JobSkills.Any(js =>
                            js.Skill.Name.Equals(cs.Skill.Name,
                                StringComparison.OrdinalIgnoreCase)))
                        .Select(cs => cs.Skill.Name)
                        .ToList();
                }

                ranked.Add(new RankedCandidateItem
                {
                    ApplicationId = app.Id,
                    CandidateId = candidate.Id,
                    CandidateName = fullName,
                    CandidateHeadline = candidate.Headline,
                    CandidateLocation = candidate.Location,
                    ProfileImagePath = candidate.User.ProfileImagePath,
                    InitialsAvatar = fullName.Length >= 2
                        ? $"{fullName.Split(' ').First()[0]}{fullName.Split(' ').Last()[0]}".ToUpper()
                        : fullName[..1].ToUpper(),
                    OverallMatchScore = score,
                    RequiredSkillScore = app.SkillMatchPercentage ?? 0,
                    ExperienceScore = 0,
                    SkillMatchPercentage = app.SkillMatchPercentage ?? 0,
                    RankPosition = app.RankPosition ?? 0,
                    MatchedSkills = matchedSkills,
                    MissingSkills = missingSkills,
                    TotalSkills = candidate.CandidateSkills.Count,
                    CandidateExperienceYears = candidate.TotalExperienceYears,
                    Status = app.Status,
                    AppliedAt = app.AppliedAt,
                    RecruiterNotes = app.RecruiterNotes
                });
            }

            // Assign proper rank positions
            var sortedRanked = ranked
                .OrderByDescending(r => r.OverallMatchScore)
                .ToList();

            for (int i = 0; i < sortedRanked.Count; i++)
                sortedRanked[i].RankPosition = i + 1;

            return new CandidateRankingViewModel
            {
                JobId = jobId,
                JobTitle = job.Title,
                Company = job.Company,
                TotalApplications = applications.Count,
                RankedCandidates = sortedRanked,
                StatusFilter = statusFilter,
                MinScoreFilter = minScore
            };
        }

        // ─── Rank All Applications for a Job ──────────────────────────────

        public async Task RankAllApplicationsAsync(int jobId)
        {
            await _skillMatchingService.RecalculateAllApplicationScoresAsync(jobId);
            _logger.LogInformation("Ranked all applications for Job {JobId}", jobId);
        }

        // ─── Status Management ─────────────────────────────────────────────

        public async Task<(bool Success, string Error)> ShortlistCandidateAsync(
            int applicationId, string recruiterId)
        {
            return await UpdateApplicationStatusAsync(
                applicationId,
                nameof(ApplicationStatus.Shortlisted),
                recruiterId);
        }

        public async Task<(bool Success, string Error)> RejectCandidateAsync(
            int applicationId, string recruiterId, string? notes = null)
        {
            return await UpdateApplicationStatusAsync(
                applicationId,
                nameof(ApplicationStatus.Rejected),
                recruiterId,
                notes);
        }

        public async Task<(bool Success, string Error)> ApproveCandidateAsync(
            int applicationId, string recruiterId)
        {
            return await UpdateApplicationStatusAsync(
                applicationId,
                nameof(ApplicationStatus.Approved),
                recruiterId);
        }

        public async Task<(bool Success, string Error)> UpdateApplicationStatusAsync(
            int applicationId,
            string newStatus,
            string recruiterId,
            string? notes = null)
        {
            try
            {
                if (!Enum.TryParse<ApplicationStatus>(newStatus, out var statusEnum))
                    return (false, $"Invalid status: {newStatus}");

                var application = await _context.Applications.FindAsync(applicationId);
                if (application == null)
                    return (false, "Application not found.");

                application.Status = statusEnum;
                application.ReviewedByUserId = recruiterId;
                application.ReviewedAt = DateTime.UtcNow;
                application.UpdatedAt = DateTime.UtcNow;

                if (!string.IsNullOrEmpty(notes))
                    application.RecruiterNotes = notes;

                _context.Applications.Update(application);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Application {AppId} status updated to {Status} by {Recruiter}",
                    applicationId, newStatus, recruiterId);

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating status for application {AppId}", applicationId);
                return (false, "An error occurred while updating the status.");
            }
        }

        // ─── Bulk Shortlist Top N ──────────────────────────────────────────

        public async Task<(bool Success, string Error)> BulkShortlistTopCandidatesAsync(
            int jobId, int topN, string recruiterId)
        {
            try
            {
                var applications = await _applicationRepo.GetApplicationsByJobAsync(jobId);

                var topCandidates = applications
                    .Where(a => a.Status == ApplicationStatus.Submitted ||
                                a.Status == ApplicationStatus.UnderReview)
                    .OrderByDescending(a => a.AIMatchScore)
                    .Take(topN)
                    .ToList();

                foreach (var app in topCandidates)
                {
                    app.Status = ApplicationStatus.Shortlisted;
                    app.ReviewedByUserId = recruiterId;
                    app.ReviewedAt = DateTime.UtcNow;
                    app.UpdatedAt = DateTime.UtcNow;
                    _context.Applications.Update(app);
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Bulk shortlisted top {N} candidates for Job {JobId}", topN, jobId);

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error bulk shortlisting for Job {JobId}", jobId);
                return (false, "An error occurred during bulk shortlisting.");
            }
        }
    }
}