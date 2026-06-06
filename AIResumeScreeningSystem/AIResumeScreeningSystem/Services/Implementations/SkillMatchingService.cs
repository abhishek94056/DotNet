using AIResumeScreeningSystem.Data;
using AIResumeScreeningSystem.DTOs;
using AIResumeScreeningSystem.Models;
using AIResumeScreeningSystem.Repositories.Interfaces;
using AIResumeScreeningSystem.Services.Interfaces;
using AIResumeScreeningSystem.ViewModels.SkillMatching;
using Microsoft.EntityFrameworkCore;

namespace AIResumeScreeningSystem.Services.Implementations
{
    public class SkillMatchingService : ISkillMatchingService
    {
        private readonly IApplicationRepository _applicationRepo;
        private readonly AppDbContext _context;
        private readonly ILogger<SkillMatchingService> _logger;

        // ─── Scoring Weights (must sum to 100) ────────────────────────────
        private const decimal RequiredSkillWeight = 55m;
        private const decimal OptionalSkillWeight = 15m;
        private const decimal ExperienceWeight = 20m;
        private const decimal EducationWeight = 10m;

        public SkillMatchingService(
            IApplicationRepository applicationRepo,
            AppDbContext context,
            ILogger<SkillMatchingService> logger)
        {
            _applicationRepo = applicationRepo;
            _context = context;
            _logger = logger;
        }

        // ─── Core Match Calculation ────────────────────────────────────────

        public async Task<SkillMatchDto> CalculateMatchAsync(int jobId, int candidateId)
        {
            var result = new SkillMatchDto
            {
                JobId = jobId,
                CandidateId = candidateId
            };

            try
            {
                // Load job with skills
                var job = await _context.Jobs
                    .Include(j => j.JobSkills)
                        .ThenInclude(js => js.Skill)
                    .FirstOrDefaultAsync(j => j.Id == jobId);

                if (job == null)
                    return result;

                // Load candidate with skills and profile
                var candidate = await _context.Candidates
                    .Include(c => c.CandidateSkills)
                        .ThenInclude(cs => cs.Skill)
                    .FirstOrDefaultAsync(c => c.Id == candidateId);

                if (candidate == null)
                    return result;

                // ── Skill Sets ─────────────────────────────────────────────
                var jobRequiredSkills = job.JobSkills
                    .Where(js => js.IsRequired)
                    .Select(js => js.Skill.Name.ToLower().Trim())
                    .ToHashSet();

                var jobOptionalSkills = job.JobSkills
                    .Where(js => !js.IsRequired)
                    .Select(js => js.Skill.Name.ToLower().Trim())
                    .ToHashSet();

                var candidateSkills = candidate.CandidateSkills
                    .Select(cs => cs.Skill.Name.ToLower().Trim())
                    .ToHashSet();

                // Also parse skills from active resume raw text for broader matching
                var activeResume = await _context.Resumes
                    .Where(r => r.CandidateId == candidateId && r.IsActive)
                    .OrderByDescending(r => r.UploadedAt)
                    .FirstOrDefaultAsync();

                if (activeResume?.ParsedSkills != null)
                {
                    var resumeSkills = activeResume.ParsedSkills
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Trim().ToLower());

                    foreach (var skill in resumeSkills)
                        candidateSkills.Add(skill);
                }

                // ── Required Skill Score ───────────────────────────────────
                result.TotalRequiredSkills = jobRequiredSkills.Count;
                result.TotalOptionalSkills = jobOptionalSkills.Count;

                result.MatchedRequiredSkills = jobRequiredSkills
                    .Where(rs => candidateSkills.Any(cs =>
                        cs.Contains(rs) || rs.Contains(cs) ||
                        IsSkillAlias(cs, rs)))
                    .Select(rs => CapitalizeFirst(rs))
                    .ToList();

                result.MissingRequiredSkills = jobRequiredSkills
                    .Where(rs => !result.MatchedRequiredSkills
                        .Select(s => s.ToLower()).Contains(rs))
                    .Select(rs => CapitalizeFirst(rs))
                    .ToList();

                result.MatchedRequiredCount = result.MatchedRequiredSkills.Count;

                decimal requiredScore = jobRequiredSkills.Count > 0
                    ? (decimal)result.MatchedRequiredCount / jobRequiredSkills.Count * 100
                    : 100;
                result.RequiredSkillScore = Math.Round(requiredScore, 2);

                // ── Optional Skill Score ───────────────────────────────────
                result.MatchedOptionalSkills = jobOptionalSkills
                    .Where(os => candidateSkills.Any(cs =>
                        cs.Contains(os) || os.Contains(cs) ||
                        IsSkillAlias(cs, os)))
                    .Select(os => CapitalizeFirst(os))
                    .ToList();

                result.MissingOptionalSkills = jobOptionalSkills
                    .Where(os => !result.MatchedOptionalSkills
                        .Select(s => s.ToLower()).Contains(os))
                    .Select(os => CapitalizeFirst(os))
                    .ToList();

                result.MatchedOptionalCount = result.MatchedOptionalSkills.Count;

                decimal optionalScore = jobOptionalSkills.Count > 0
                    ? (decimal)result.MatchedOptionalCount / jobOptionalSkills.Count * 100
                    : 100;
                result.OptionalSkillScore = Math.Round(optionalScore, 2);

                // ── Extra Skills (candidate has but job doesn't require) ────
                var allJobSkillNames = jobRequiredSkills.Union(jobOptionalSkills);
                result.CandidateExtraSkills = candidateSkills
                    .Where(cs => !allJobSkillNames.Any(js =>
                        js.Contains(cs) || cs.Contains(js)))
                    .Select(CapitalizeFirst)
                    .Take(10)
                    .ToList();

                // ── Experience Score ───────────────────────────────────────
                result.ExperienceScore = Math.Round(
                    CalculateExperienceScore(
                        candidate.TotalExperienceYears,
                        job.ExperienceYearsMin,
                        job.ExperienceYearsMax), 2);

                // ── Education Score ────────────────────────────────────────
                result.EducationScore = Math.Round(
                    CalculateEducationScore(candidate.HighestEducation), 2);

                // ── Overall Score ──────────────────────────────────────────
                result.OverallMatchScore = Math.Round(
                    (result.RequiredSkillScore * RequiredSkillWeight / 100) +
                    (result.OptionalSkillScore * OptionalSkillWeight / 100) +
                    (result.ExperienceScore * ExperienceWeight / 100) +
                    (result.EducationScore * EducationWeight / 100), 2);

                // Cap at 100
                result.OverallMatchScore = Math.Min(result.OverallMatchScore, 100);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error calculating match for Job {JobId}, Candidate {CandidateId}",
                    jobId, candidateId);
            }

            return result;
        }

        public async Task<SkillMatchDto> CalculateMatchForApplicationAsync(int applicationId)
        {
            var application = await _applicationRepo.GetApplicationWithDetailsAsync(applicationId);
            if (application == null)
                return new SkillMatchDto { ApplicationId = applicationId };

            var result = await CalculateMatchAsync(application.JobId, application.CandidateId);
            result.ApplicationId = applicationId;
            return result;
        }

        public async Task<decimal> GetQuickMatchScoreAsync(int jobId, int candidateId)
        {
            var result = await CalculateMatchAsync(jobId, candidateId);
            return result.OverallMatchScore;
        }

        // ─── Skill Gap Analysis ────────────────────────────────────────────

        public async Task<SkillGapViewModel> GetSkillGapAsync(int applicationId)
        {
            var application = await _applicationRepo.GetApplicationWithDetailsAsync(applicationId);
            if (application == null)
                return new SkillGapViewModel();

            var matchDto = await CalculateMatchForApplicationAsync(applicationId);

            var viewModel = new SkillGapViewModel
            {
                ApplicationId = applicationId,
                JobId = application.JobId,
                CandidateId = application.CandidateId,
                JobTitle = application.Job?.Title ?? string.Empty,
                CandidateName = application.Candidate?.User != null
                    ? $"{application.Candidate.User.FirstName} {application.Candidate.User.LastName}"
                    : "Unknown",
                OverallMatchScore = matchDto.OverallMatchScore,
                AIGapAnalysis = application.SkillGapAnalysis
            };

            // Build gap items for required skills
            foreach (var skill in matchDto.MissingRequiredSkills)
            {
                viewModel.RequiredGaps.Add(new SkillGapItem
                {
                    SkillName = skill,
                    IsRequired = true,
                    IsMatched = false,
                    SuggestedResource = GetLearningResource(skill)
                });
            }

            // Build gap items for optional skills
            foreach (var skill in matchDto.MissingOptionalSkills)
            {
                viewModel.OptionalGaps.Add(new SkillGapItem
                {
                    SkillName = skill,
                    IsRequired = false,
                    IsMatched = false,
                    SuggestedResource = GetLearningResource(skill)
                });
            }

            // Matched skills
            foreach (var skill in matchDto.MatchedRequiredSkills)
            {
                viewModel.MatchedSkills.Add(new SkillGapItem
                {
                    SkillName = skill,
                    IsRequired = true,
                    IsMatched = true
                });
            }
            foreach (var skill in matchDto.MatchedOptionalSkills)
            {
                viewModel.MatchedSkills.Add(new SkillGapItem
                {
                    SkillName = skill,
                    IsRequired = false,
                    IsMatched = true
                });
            }

            // Extra skills the candidate has
            foreach (var skill in matchDto.CandidateExtraSkills)
            {
                viewModel.ExtraSkills.Add(new SkillGapItem
                {
                    SkillName = skill,
                    IsMatched = true
                });
            }

            return viewModel;
        }

        // ─── Application Match Result View ─────────────────────────────────

        public async Task<SkillMatchResultViewModel> GetApplicationMatchResultAsync(
            int applicationId)
        {
            var application = await _applicationRepo.GetApplicationWithDetailsAsync(applicationId);
            if (application == null)
                return new SkillMatchResultViewModel();

            var matchDto = await CalculateMatchForApplicationAsync(applicationId);
            var candidate = application.Candidate;
            var job = application.Job;

            return new SkillMatchResultViewModel
            {
                ApplicationId = applicationId,
                JobId = application.JobId,
                JobTitle = job?.Title ?? string.Empty,
                Company = job?.Company ?? string.Empty,
                CandidateId = application.CandidateId,
                CandidateName = candidate?.User != null
                    ? $"{candidate.User.FirstName} {candidate.User.LastName}" : "Unknown",
                CandidateHeadline = candidate?.Headline,
                CandidateExperienceYears = candidate?.TotalExperienceYears ?? 0,
                CandidateEducation = candidate?.HighestEducation,
                OverallMatchScore = matchDto.OverallMatchScore,
                RequiredSkillScore = matchDto.RequiredSkillScore,
                OptionalSkillScore = matchDto.OptionalSkillScore,
                ExperienceScore = matchDto.ExperienceScore,
                EducationScore = matchDto.EducationScore,
                MatchedRequiredSkills = matchDto.MatchedRequiredSkills,
                MatchedOptionalSkills = matchDto.MatchedOptionalSkills,
                MissingRequiredSkills = matchDto.MissingRequiredSkills,
                MissingOptionalSkills = matchDto.MissingOptionalSkills,
                CandidateExtraSkills = matchDto.CandidateExtraSkills,
                TotalRequiredSkills = matchDto.TotalRequiredSkills,
                TotalOptionalSkills = matchDto.TotalOptionalSkills,
                Status = application.Status,
                RankPosition = application.RankPosition,
                AIEvaluation = application.AIEvaluation,
                SkillGapAnalysis = application.SkillGapAnalysis,
                RecruiterNotes = application.RecruiterNotes,
                AppliedAt = application.AppliedAt
            };
        }

        // ─── Recalculate Scores ────────────────────────────────────────────

        public async Task RecalculateAllApplicationScoresAsync(int jobId)
        {
            var applications = await _applicationRepo.GetApplicationsByJobAsync(jobId);

            int rank = 1;
            var scored = new List<(Application App, SkillMatchDto Match)>();

            foreach (var app in applications)
            {
                var match = await CalculateMatchAsync(jobId, app.CandidateId);
                match.ApplicationId = app.Id;
                scored.Add((app, match));
            }

            // Sort by score descending, assign ranks
            var ranked = scored
                .OrderByDescending(x => x.Match.OverallMatchScore)
                .ToList();

            for (int i = 0; i < ranked.Count; i++)
            {
                var (app, match) = ranked[i];
                await _applicationRepo.UpdateApplicationScoresAsync(
                    app.Id,
                    match.OverallMatchScore,
                    match.RequiredSkillScore,
                    i + 1,
                    null,
                    null,
                    string.Join(", ", match.MissingRequiredSkills));
            }

            _logger.LogInformation(
                "Recalculated scores for {Count} applications on Job {JobId}",
                ranked.Count, jobId);
        }

        public async Task RecalculateSingleApplicationScoreAsync(int applicationId)
        {
            var match = await CalculateMatchForApplicationAsync(applicationId);

            // Get current rank (preserve existing or set 0)
            var app = await _applicationRepo.GetByIdAsync(applicationId);
            int rank = app?.RankPosition ?? 0;

            await _applicationRepo.UpdateApplicationScoresAsync(
                applicationId,
                match.OverallMatchScore,
                match.RequiredSkillScore,
                rank,
                null,
                null,
                string.Join(", ", match.MissingRequiredSkills));
        }

        // ─── Private Scoring Helpers ───────────────────────────────────────

        private static decimal CalculateExperienceScore(
            int candidateYears, int minRequired, int maxRequired)
        {
            if (minRequired == 0 && maxRequired == 0) return 100m;

            if (maxRequired == 0) maxRequired = minRequired + 5;

            if (candidateYears >= minRequired && candidateYears <= maxRequired)
                return 100m;

            if (candidateYears < minRequired)
            {
                // Partial credit: within 1 year = 70, 2 years = 40, else 10
                int shortfall = minRequired - candidateYears;
                return shortfall switch
                {
                    1 => 70m,
                    2 => 40m,
                    _ => 10m
                };
            }

            // Overqualified — still good but slightly penalized
            int surplus = candidateYears - maxRequired;
            return surplus <= 3 ? 90m : 75m;
        }

        private static decimal CalculateEducationScore(string? highestEducation)
        {
            if (string.IsNullOrEmpty(highestEducation)) return 50m;

            return highestEducation.ToLower() switch
            {
                var e when e.Contains("phd") || e.Contains("doctor") => 100m,
                var e when e.Contains("master") || e.Contains("mba") => 90m,
                var e when e.Contains("bachelor") => 80m,
                var e when e.Contains("diploma") || e.Contains("associate") => 65m,
                var e when e.Contains("high school") || e.Contains("secondary") => 50m,
                _ => 60m
            };
        }

        // Handles common aliases and abbreviations
        private static bool IsSkillAlias(string candidateSkill, string jobSkill)
        {
            var aliases = new Dictionary<string, HashSet<string>>(
                StringComparer.OrdinalIgnoreCase)
            {
                ["c#"] = new() { "csharp", "c sharp", ".net" },
                ["javascript"] = new() { "js", "ecmascript" },
                ["typescript"] = new() { "ts" },
                ["node.js"] = new() { "nodejs", "node" },
                ["asp.net core"] = new() { "asp.net", "aspnet", ".net core" },
                ["vue.js"] = new() { "vue", "vuejs" },
                ["react"] = new() { "reactjs", "react.js" },
                ["sql server"] = new() { "mssql", "sqlserver", "microsoft sql" },
                ["postgresql"] = new() { "postgres" },
                ["mongodb"] = new() { "mongo" },
                ["kubernetes"] = new() { "k8s" },
                ["amazon web services"] = new() { "aws" },
                ["google cloud"] = new() { "gcp", "google cloud platform" },
                ["machine learning"] = new() { "ml" },
                ["artificial intelligence"] = new() { "ai" }
            };

            foreach (var (key, values) in aliases)
            {
                if ((key.Equals(candidateSkill, StringComparison.OrdinalIgnoreCase) ||
                     values.Contains(candidateSkill)) &&
                    (key.Equals(jobSkill, StringComparison.OrdinalIgnoreCase) ||
                     values.Contains(jobSkill)))
                    return true;
            }

            return false;
        }

        private static string CapitalizeFirst(string s) =>
            string.IsNullOrEmpty(s) ? s :
            char.ToUpper(s[0]) + s[1..];

        private static string GetLearningResource(string skillName)
        {
            var lower = skillName.ToLower();
            return lower switch
            {
                var s when s.Contains("c#") || s.Contains(".net") || s.Contains("asp") =>
                    "https://learn.microsoft.com/en-us/dotnet",
                var s when s.Contains("python") =>
                    "https://docs.python.org/3/tutorial",
                var s when s.Contains("javascript") || s.Contains("typescript") =>
                    "https://javascript.info",
                var s when s.Contains("react") =>
                    "https://react.dev/learn",
                var s when s.Contains("azure") =>
                    "https://learn.microsoft.com/en-us/azure",
                var s when s.Contains("aws") =>
                    "https://aws.amazon.com/training",
                var s when s.Contains("docker") || s.Contains("kubernetes") =>
                    "https://docs.docker.com/get-started",
                var s when s.Contains("sql") =>
                    "https://www.w3schools.com/sql",
                _ => "https://www.udemy.com/courses/search/?q=" +
                     Uri.EscapeDataString(skillName)
            };
        }
    }
}