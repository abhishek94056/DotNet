using AIResumeScreeningSystem.Data;
using AIResumeScreeningSystem.Models;
using AIResumeScreeningSystem.Repositories.Interfaces;
using AIResumeScreeningSystem.Services.Interfaces;
using AIResumeScreeningSystem.ViewModels.Candidate;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AIResumeScreeningSystem.Services.Implementations
{
    public class CandidateService : ICandidateService
    {
        private readonly ICandidateRepository _candidateRepo;
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<CandidateService> _logger;

        public CandidateService(
            ICandidateRepository candidateRepo,
            AppDbContext context,
            IWebHostEnvironment env,
            ILogger<CandidateService> logger)
        {
            _candidateRepo = candidateRepo;
            _context = context;
            _env = env;
            _logger = logger;
        }

        // ─── Profile ───────────────────────────────────────────────────────

        public async Task<CandidateViewModel?> GetCandidateViewModelByUserIdAsync(string userId)
        {
            var candidate = await _candidateRepo.GetCandidateWithDetailsByUserIdAsync(userId);
            return candidate == null ? null : MapToViewModel(candidate);
        }

        public async Task<CandidateViewModel?> GetCandidateViewModelByIdAsync(int candidateId)
        {
            var candidate = await _candidateRepo.GetCandidateWithDetailsAsync(candidateId);
            return candidate == null ? null : MapToViewModel(candidate);
        }

        public async Task<CandidateProfileViewModel?> GetProfileForEditAsync(string userId)
        {
            var candidate = await _candidateRepo.GetCandidateByUserIdAsync(userId);
            if (candidate == null) return null;

            return new CandidateProfileViewModel
            {
                Id = candidate.Id,
                FirstName = candidate.User.FirstName,
                LastName = candidate.User.LastName,
                PhoneNumber = candidate.User.PhoneNumber,
                Headline = candidate.Headline,
                Summary = candidate.Summary,
                CurrentJobTitle = candidate.CurrentJobTitle,
                CurrentCompany = candidate.CurrentCompany,
                TotalExperienceYears = candidate.TotalExperienceYears,
                Location = candidate.Location,
                LinkedInUrl = candidate.LinkedInUrl,
                GitHubUrl = candidate.GitHubUrl,
                PortfolioUrl = candidate.PortfolioUrl,
                HighestEducation = candidate.HighestEducation,
                University = candidate.University,
                GraduationYear = candidate.GraduationYear,
                IsAvailable = candidate.IsAvailable,
                ExistingProfileImagePath = candidate.User.ProfileImagePath
            };
        }

        public async Task<(bool Success, string Error)> UpdateProfileAsync(
            string userId, CandidateProfileViewModel model)
        {
            try
            {
                var candidate = await _candidateRepo.GetCandidateByUserIdAsync(userId);
                if (candidate == null)
                    return (false, "Candidate profile not found.");

                // Handle profile image upload
                if (model.ProfileImage != null && model.ProfileImage.Length > 0)
                {
                    var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
                    if (!allowedTypes.Contains(model.ProfileImage.ContentType.ToLower()))
                        return (false, "Only JPEG, PNG, GIF, and WEBP images are allowed.");

                    if (model.ProfileImage.Length > 2 * 1024 * 1024)
                        return (false, "Profile image must be under 2MB.");

                    var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "profiles");
                    Directory.CreateDirectory(uploadsFolder);

                    var fileName = $"{userId}_{DateTime.UtcNow.Ticks}{Path.GetExtension(model.ProfileImage.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                        await model.ProfileImage.CopyToAsync(stream);

                    // Delete old image
                    if (!string.IsNullOrEmpty(candidate.User.ProfileImagePath))
                    {
                        var oldPath = Path.Combine(_env.WebRootPath,
                            candidate.User.ProfileImagePath.TrimStart('/'));
                        if (File.Exists(oldPath)) File.Delete(oldPath);
                    }

                    candidate.User.ProfileImagePath = $"/uploads/profiles/{fileName}";
                }

                // Update User fields
                candidate.User.FirstName = model.FirstName;
                candidate.User.LastName = model.LastName;
                candidate.User.PhoneNumber = model.PhoneNumber;
                candidate.User.UpdatedAt = DateTime.UtcNow;

                // Update Candidate fields
                candidate.Headline = model.Headline;
                candidate.Summary = model.Summary;
                candidate.CurrentJobTitle = model.CurrentJobTitle;
                candidate.CurrentCompany = model.CurrentCompany;
                candidate.TotalExperienceYears = model.TotalExperienceYears;
                candidate.Location = model.Location;
                candidate.LinkedInUrl = model.LinkedInUrl;
                candidate.GitHubUrl = model.GitHubUrl;
                candidate.PortfolioUrl = model.PortfolioUrl;
                candidate.HighestEducation = model.HighestEducation;
                candidate.University = model.University;
                candidate.GraduationYear = model.GraduationYear;
                candidate.IsAvailable = model.IsAvailable;
                candidate.UpdatedAt = DateTime.UtcNow;

                _candidateRepo.Update(candidate);
                await _candidateRepo.SaveChangesAsync();

                _logger.LogInformation("Profile updated for user: {UserId}", userId);
                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile for {UserId}", userId);
                return (false, "An error occurred while updating your profile.");
            }
        }

        public async Task<(bool Success, string Error)> ToggleAvailabilityAsync(string userId)
        {
            var candidate = await _candidateRepo.GetCandidateByUserIdAsync(userId);
            if (candidate == null) return (false, "Candidate not found.");

            candidate.IsAvailable = !candidate.IsAvailable;
            candidate.UpdatedAt = DateTime.UtcNow;
            _candidateRepo.Update(candidate);
            await _candidateRepo.SaveChangesAsync();
            return (true, string.Empty);
        }

        // ─── Skills ────────────────────────────────────────────────────────

        public async Task<List<CandidateSkillViewModel>> GetCandidateSkillsAsync(int candidateId)
        {
            var skills = await _candidateRepo.GetCandidateSkillsAsync(candidateId);
            return skills.Select(MapSkillToViewModel).ToList();
        }

        public async Task<(bool Success, string Error)> AddSkillAsync(
            string userId, AddSkillViewModel model)
        {
            try
            {
                var candidate = await _candidateRepo.GetCandidateByUserIdAsync(userId);
                if (candidate == null) return (false, "Candidate not found.");

                int skillId = model.SkillId;

                // Handle new skill creation
                if (!string.IsNullOrWhiteSpace(model.NewSkillName) && model.SkillId == 0)
                {
                    var normalizedName = model.NewSkillName.Trim();
                    var existingSkill = await _context.Skills
                        .FirstOrDefaultAsync(s =>
                            s.Name.ToLower() == normalizedName.ToLower());

                    if (existingSkill != null)
                    {
                        skillId = existingSkill.Id;
                    }
                    else
                    {
                        var newSkill = new Skill
                        {
                            Name = normalizedName,
                            Category = model.NewSkillCategory,
                            CreatedAt = DateTime.UtcNow
                        };
                        await _context.Skills.AddAsync(newSkill);
                        await _context.SaveChangesAsync();
                        skillId = newSkill.Id;
                    }
                }

                if (skillId == 0) return (false, "Please select or enter a skill.");

                // Check duplicate
                bool exists = await _context.CandidateSkills
                    .AnyAsync(cs => cs.CandidateId == candidate.Id && cs.SkillId == skillId);
                if (exists) return (false, "This skill is already in your profile.");

                var candidateSkill = new CandidateSkill
                {
                    CandidateId = candidate.Id,
                    SkillId = skillId,
                    ProficiencyLevel = model.ProficiencyLevel,
                    YearsOfExperience = model.YearsOfExperience
                };

                await _context.CandidateSkills.AddAsync(candidateSkill);
                await _context.SaveChangesAsync();
                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding skill for {UserId}", userId);
                return (false, "An error occurred while adding the skill.");
            }
        }

        public async Task<(bool Success, string Error)> RemoveSkillAsync(
            string userId, int skillId)
        {
            var candidate = await _candidateRepo.GetCandidateByUserIdAsync(userId);
            if (candidate == null) return (false, "Candidate not found.");

            bool removed = await _candidateRepo.RemoveCandidateSkillAsync(candidate.Id, skillId);
            return removed ? (true, string.Empty) : (false, "Skill not found.");
        }

        public async Task<(bool Success, string Error)> UpdateSkillAsync(
            string userId, int candidateSkillId, ProficiencyLevel level, int years)
        {
            var candidate = await _candidateRepo.GetCandidateByUserIdAsync(userId);
            if (candidate == null) return (false, "Candidate not found.");

            bool updated = await _candidateRepo.UpdateCandidateSkillAsync(
                candidateSkillId, level, years);
            return updated ? (true, string.Empty) : (false, "Skill not found.");
        }

        // ─── Listing ───────────────────────────────────────────────────────

        public async Task<CandidateListViewModel> GetCandidatesAsync(CandidateSearchViewModel search)
        {
            var (candidates, totalCount) = await _candidateRepo.GetPagedCandidatesAsync(search);
            var stats = await _candidateRepo.GetCandidateStatsAsync();

            return new CandidateListViewModel
            {
                Candidates = candidates.Select(MapToViewModel).ToList(),
                Search = search,
                CurrentPage = search.Page,
                TotalCount = totalCount,
                PageSize = search.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)search.PageSize),
                TotalCandidates = stats.GetValueOrDefault("Total"),
                AvailableCandidates = stats.GetValueOrDefault("Available"),
                NewThisMonthCount = stats.GetValueOrDefault("NewThisMonth")
            };
        }

        // ─── Dashboard ─────────────────────────────────────────────────────

        public async Task<CandidateDashboardViewModel> GetDashboardAsync(string userId)
        {
            var candidate = await _candidateRepo.GetCandidateWithDetailsByUserIdAsync(userId);
            if (candidate == null)
                return new CandidateDashboardViewModel { FullName = "Unknown" };

            var profileCompletion = await _candidateRepo
                .GetProfileCompletionPercentAsync(candidate.Id);

            var fullName = $"{candidate.User.FirstName} {candidate.User.LastName}";

            var recentApps = candidate.Applications
                .OrderByDescending(a => a.AppliedAt)
                .Take(5)
                .Select(a => new RecentApplicationItem
                {
                    ApplicationId = a.Id,
                    JobId = a.JobId,
                    JobTitle = a.Job?.Title ?? "Unknown",
                    Company = a.Job?.Company ?? "Unknown",
                    Status = a.Status,
                    AIMatchScore = a.AIMatchScore,
                    AppliedAt = a.AppliedAt
                }).ToList();

            // Recommend jobs based on candidate skills
            var candidateSkillIds = candidate.CandidateSkills.Select(cs => cs.SkillId).ToList();
            var recommendedJobs = await _context.Jobs
                .Include(j => j.JobSkills).ThenInclude(js => js.Skill)
                .Where(j => j.Status == JobStatus.Active &&
                            j.JobSkills.Any(js => candidateSkillIds.Contains(js.SkillId)) &&
                            !candidate.Applications.Select(a => a.JobId).Contains(j.Id))
                .Take(5)
                .ToListAsync();

            var recommended = recommendedJobs.Select(j =>
            {
                var matchingSkills = j.JobSkills
                    .Where(js => candidateSkillIds.Contains(js.SkillId))
                    .Select(js => js.Skill.Name).ToList();
                int matchPct = j.JobSkills.Any()
                    ? (int)Math.Round((double)matchingSkills.Count / j.JobSkills.Count * 100)
                    : 0;
                return new RecommendedJobItem
                {
                    JobId = j.Id,
                    Title = j.Title,
                    Company = j.Company,
                    Location = j.Location,
                    SalaryRange = j.SalaryMin.HasValue
                        ? $"${j.SalaryMin:N0}–${j.SalaryMax:N0}" : "Negotiable",
                    MatchPercent = matchPct,
                    MatchingSkills = matchingSkills
                };
            }).OrderByDescending(r => r.MatchPercent).ToList();

            // Build chart data (last 5 applications by score)
            var chartApps = candidate.Applications
                .Where(a => a.AIMatchScore.HasValue)
                .OrderBy(a => a.AppliedAt)
                .TakeLast(6)
                .ToList();

            return new CandidateDashboardViewModel
            {
                CandidateId = candidate.Id,
                FullName = fullName,
                Headline = candidate.Headline,
                ProfileImagePath = candidate.User.ProfileImagePath,
                ProfileCompletionPercent = profileCompletion,
                IsAvailable = candidate.IsAvailable,
                InitialsAvatar = fullName.Length >= 2
                    ? $"{fullName.Split(' ').First()[0]}{fullName.Split(' ').Last()[0]}".ToUpper()
                    : fullName[..1].ToUpper(),

                TotalApplications = candidate.Applications.Count,
                PendingApplications = candidate.Applications
                    .Count(a => a.Status == ApplicationStatus.Submitted ||
                                a.Status == ApplicationStatus.UnderReview),
                ShortlistedApplications = candidate.Applications
                    .Count(a => a.Status == ApplicationStatus.Shortlisted),
                RejectedApplications = candidate.Applications
                    .Count(a => a.Status == ApplicationStatus.Rejected),
                ApprovedApplications = candidate.Applications
                    .Count(a => a.Status == ApplicationStatus.Approved),

                TotalResumes = candidate.Resumes.Count,
                HasActiveResume = candidate.Resumes.Any(),
                TotalSkills = candidate.CandidateSkills.Count,

                RecentApplications = recentApps,
                RecommendedJobs = recommended,

                ChartLabels = chartApps.Select(a => a.Job?.Title[..Math.Min(15, a.Job.Title.Length)] ?? "Job").ToList(),
                ChartScores = chartApps.Select(a => a.AIMatchScore ?? 0).ToList()
            };
        }

        // ─── Helpers ───────────────────────────────────────────────────────

        public async Task<int?> GetCandidateIdByUserIdAsync(string userId)
        {
            var candidate = await _candidateRepo.GetCandidateByUserIdAsync(userId);
            return candidate?.Id;
        }

        public async Task<bool> CandidateExistsAsync(string userId)
        {
            return await _candidateRepo.AnyAsync(c => c.UserId == userId);
        }

        // ─── Private Mappers ───────────────────────────────────────────────

        private static CandidateViewModel MapToViewModel(Candidate c) => new()
        {
            Id = c.Id,
            UserId = c.UserId,
            FullName = $"{c.User.FirstName} {c.User.LastName}",
            Email = c.User.Email ?? string.Empty,
            PhoneNumber = c.User.PhoneNumber,
            Headline = c.Headline,
            Summary = c.Summary,
            CurrentJobTitle = c.CurrentJobTitle,
            CurrentCompany = c.CurrentCompany,
            TotalExperienceYears = c.TotalExperienceYears,
            Location = c.Location,
            LinkedInUrl = c.LinkedInUrl,
            GitHubUrl = c.GitHubUrl,
            PortfolioUrl = c.PortfolioUrl,
            HighestEducation = c.HighestEducation,
            University = c.University,
            GraduationYear = c.GraduationYear,
            IsAvailable = c.IsAvailable,
            IsActive = c.User.IsActive,
            ProfileImagePath = c.User.ProfileImagePath,
            CreatedAt = c.CreatedAt,
            Skills = c.CandidateSkills?.Select(MapSkillToViewModel).ToList() ?? new(),
            TotalApplications = c.Applications?.Count ?? 0,
            TotalResumes = c.Resumes?.Count ?? 0,
            ShortlistedCount = c.Applications?
                .Count(a => a.Status == ApplicationStatus.Shortlisted) ?? 0
        };

        private static CandidateSkillViewModel MapSkillToViewModel(CandidateSkill cs) => new()
        {
            Id = cs.Id,
            SkillId = cs.SkillId,
            SkillName = cs.Skill?.Name ?? string.Empty,
            Category = cs.Skill?.Category ?? SkillCategory.Other,
            ProficiencyLevel = cs.ProficiencyLevel,
            YearsOfExperience = cs.YearsOfExperience,
            IsVerified = cs.IsVerified
        };
    }
}