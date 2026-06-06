using AIResumeScreeningSystem.Data;
using AIResumeScreeningSystem.Helpers;
using AIResumeScreeningSystem.Models;
using AIResumeScreeningSystem.Repositories.Interfaces;
using AIResumeScreeningSystem.Services.Interfaces;
using AIResumeScreeningSystem.ViewModels.Resume;
using Microsoft.EntityFrameworkCore;

namespace AIResumeScreeningSystem.Services.Implementations
{
    public class ResumeService : IResumeService
    {
        private readonly IResumeRepository _resumeRepo;
        private readonly ICandidateRepository _candidateRepo;
        private readonly IResumeParserService _parserService;
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ResumeService> _logger;

        public ResumeService(
            IResumeRepository resumeRepo,
            ICandidateRepository candidateRepo,
            IResumeParserService parserService,
            AppDbContext context,
            IWebHostEnvironment env,
            IConfiguration configuration,
            ILogger<ResumeService> logger)
        {
            _resumeRepo = resumeRepo;
            _candidateRepo = candidateRepo;
            _parserService = parserService;
            _context = context;
            _env = env;
            _configuration = configuration;
            _logger = logger;
        }

        // ─── Upload View Model ─────────────────────────────────────────────

        public async Task<ResumeUploadViewModel> GetUploadViewModelAsync(string userId)
        {
            var candidate = await _candidateRepo.GetCandidateByUserIdAsync(userId);
            if (candidate == null)
                return new ResumeUploadViewModel();

            var resumes = await _resumeRepo.GetResumesByCandidateAsync(candidate.Id);
            var maxMB = _configuration.GetValue<int>("FileStorage:MaxFileSizeMB", 5);

            return new ResumeUploadViewModel
            {
                ExistingResumes = resumes.Select(MapToViewModel).ToList(),
                MaxFileSizeMB = maxMB,
                SetAsActive = true
            };
        }

        // ─── Upload ────────────────────────────────────────────────────────

        public async Task<(bool Success, int ResumeId, string Error)> UploadResumeAsync(
            string userId, ResumeUploadViewModel model)
        {
            try
            {
                if (model.ResumeFile == null || model.ResumeFile.Length == 0)
                    return (false, 0, "No file selected.");

                var maxMB = _configuration.GetValue<int>("FileStorage:MaxFileSizeMB", 5);
                var validationError = FileHelper.GetValidationError(model.ResumeFile, maxMB);
                if (!string.IsNullOrEmpty(validationError))
                    return (false, 0, validationError);

                var candidate = await _candidateRepo.GetCandidateByUserIdAsync(userId);
                if (candidate == null)
                    return (false, 0, "Candidate profile not found.");

                // Check max resumes (limit to 10 per candidate)
                var existingCount = await _context.Resumes
                    .CountAsync(r => r.CandidateId == candidate.Id);
                if (existingCount >= 10)
                    return (false, 0, "Maximum 10 resumes allowed. Please delete an old one first.");

                // Build storage path
                var uploadsFolder = Path.Combine(
                    _env.WebRootPath, "uploads", "resumes", candidate.Id.ToString());
                Directory.CreateDirectory(uploadsFolder);

                var ext = Path.GetExtension(model.ResumeFile.FileName).ToLower();
                var safeOriginalName = FileHelper.SanitizeFileName(
                    Path.GetFileNameWithoutExtension(model.ResumeFile.FileName));
                var uniqueFileName = $"{safeOriginalName}_{DateTime.UtcNow.Ticks}{ext}";
                var fullPath = Path.Combine(uploadsFolder, uniqueFileName);
                var relativePath = $"/uploads/resumes/{candidate.Id}/{uniqueFileName}";

                // Save the file
                await using (var stream = new FileStream(fullPath, FileMode.Create))
                    await model.ResumeFile.CopyToAsync(stream);

                // Deactivate old resumes if set as active
                if (model.SetAsActive)
                    await _resumeRepo.DeactivateAllResumesAsync(candidate.Id);

                // Create Resume record
                var resume = new Resume
                {
                    CandidateId = candidate.Id,
                    FileName = model.ResumeFile.FileName,
                    FilePath = relativePath,
                    FileExtension = ext,
                    FileSizeBytes = model.ResumeFile.Length,
                    Status = ResumeStatus.Uploaded,
                    UploadedAt = DateTime.UtcNow,
                    IsActive = model.SetAsActive
                };

                await _resumeRepo.AddAsync(resume);
                await _resumeRepo.SaveChangesAsync();

                _logger.LogInformation(
                    "Resume uploaded: {FileName} for candidate {CandidateId}",
                    model.ResumeFile.FileName, candidate.Id);

                // Auto-parse in background
                _ = Task.Run(async () =>
                {
                    try { await ParseResumeAsync(resume.Id); }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Background parse failed for resume {ResumeId}", resume.Id);
                    }
                });

                return (true, resume.Id, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading resume for user {UserId}", userId);
                return (false, 0, "An error occurred while uploading the file.");
            }
        }

        // ─── Parse ─────────────────────────────────────────────────────────

        public async Task<(bool Success, string Error)> ParseResumeAsync(int resumeId)
        {
            Resume? resume = null;
            try
            {
                resume = await _resumeRepo.GetResumeWithDetailsAsync(resumeId);
                if (resume == null)
                    return (false, "Resume not found.");

                resume.Status = ResumeStatus.Parsing;
                _resumeRepo.Update(resume);
                await _resumeRepo.SaveChangesAsync();

                var fullPath = Path.Combine(_env.WebRootPath,
                    resume.FilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

                if (!File.Exists(fullPath))
                {
                    resume.Status = ResumeStatus.Failed;
                    _resumeRepo.Update(resume);
                    await _resumeRepo.SaveChangesAsync();
                    return (false, "Resume file not found on disk.");
                }

                var parseResult = await _parserService.ParseAsync(fullPath, resume.FileExtension);

                if (!parseResult.Success)
                {
                    resume.Status = ResumeStatus.Failed;
                    _resumeRepo.Update(resume);
                    await _resumeRepo.SaveChangesAsync();
                    return (false, parseResult.Error ?? "Parsing failed.");
                }

                // Store parsed results
                resume.ParsedName = parseResult.Name;
                resume.ParsedEmail = parseResult.Email;
                resume.ParsedPhone = parseResult.Phone;
                resume.ParsedSummary = parseResult.Summary;
                resume.ParsedSkills = string.Join(", ", parseResult.ExtractedSkills);
                resume.ParsedEducation = parseResult.EducationSection?.Length > 2000
                    ? parseResult.EducationSection[..2000]
                    : parseResult.EducationSection;
                resume.ParsedExperience = parseResult.ExperienceSection?.Length > 2000
                    ? parseResult.ExperienceSection[..2000]
                    : parseResult.ExperienceSection;
                resume.RawText = parseResult.RawText?.Length > 10000
                    ? parseResult.RawText[..10000]
                    : parseResult.RawText;
                resume.Status = ResumeStatus.Parsed;
                resume.ParsedAt = DateTime.UtcNow;

                _resumeRepo.Update(resume);
                await _resumeRepo.SaveChangesAsync();

                // Auto-update candidate profile from parsed data
                await UpdateCandidateFromParsedData(resume, parseResult);

                _logger.LogInformation(
                    "Resume parsed successfully: {ResumeId}, Skills found: {Count}",
                    resumeId, parseResult.ExtractedSkills.Count);

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing resume {ResumeId}", resumeId);

                if (resume != null)
                {
                    resume.Status = ResumeStatus.Failed;
                    _resumeRepo.Update(resume);
                    await _resumeRepo.SaveChangesAsync();
                }

                return (false, "An error occurred during parsing.");
            }
        }

        // ─── Auto-Update Candidate Profile ────────────────────────────────

        private async Task UpdateCandidateFromParsedData(
            Resume resume, DTOs.ResumeParseResultDto parseResult)
        {
            try
            {
                var candidate = await _candidateRepo.GetCandidateWithDetailsAsync(resume.CandidateId);
                if (candidate == null) return;

                bool updated = false;

                // Only fill in blank fields
                if (string.IsNullOrEmpty(candidate.User.PhoneNumber) &&
                    !string.IsNullOrEmpty(parseResult.Phone))
                {
                    candidate.User.PhoneNumber = parseResult.Phone;
                    updated = true;
                }

                if (string.IsNullOrEmpty(candidate.Summary) &&
                    !string.IsNullOrEmpty(parseResult.Summary))
                {
                    candidate.Summary = parseResult.Summary;
                    updated = true;
                }

                if (string.IsNullOrEmpty(candidate.HighestEducation) &&
                    !string.IsNullOrEmpty(parseResult.HighestEducation))
                {
                    candidate.HighestEducation = parseResult.HighestEducation;
                    updated = true;
                }

                if (candidate.TotalExperienceYears == 0 &&
                    parseResult.EstimatedExperienceYears > 0)
                {
                    candidate.TotalExperienceYears = parseResult.EstimatedExperienceYears;
                    updated = true;
                }

                if (updated)
                {
                    candidate.UpdatedAt = DateTime.UtcNow;
                    _candidateRepo.Update(candidate);
                }

                // Auto-add extracted skills not already in profile
                if (parseResult.ExtractedSkills.Any())
                {
                    var allSkills = await _context.Skills.ToListAsync();
                    var existingSkillIds = candidate.CandidateSkills.Select(cs => cs.SkillId).ToHashSet();

                    foreach (var skillName in parseResult.ExtractedSkills)
                    {
                        var skill = allSkills.FirstOrDefault(s =>
                            string.Equals(s.Name, skillName, StringComparison.OrdinalIgnoreCase));

                        // Create skill if it doesn't exist
                        if (skill == null)
                        {
                            skill = new Skill
                            {
                                Name = skillName,
                                Category = InferSkillCategory(skillName),
                                CreatedAt = DateTime.UtcNow
                            };
                            await _context.Skills.AddAsync(skill);
                            await _context.SaveChangesAsync();
                            allSkills.Add(skill);
                        }

                        if (!existingSkillIds.Contains(skill.Id))
                        {
                            await _context.CandidateSkills.AddAsync(new CandidateSkill
                            {
                                CandidateId = candidate.Id,
                                SkillId = skill.Id,
                                ProficiencyLevel = ProficiencyLevel.Intermediate,
                                YearsOfExperience = 0
                            });
                            existingSkillIds.Add(skill.Id);
                        }
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating candidate from parsed data for resume {ResumeId}", resume.Id);
            }
        }

        private static SkillCategory InferSkillCategory(string skillName)
        {
            var lower = skillName.ToLower();
            if (lower.Contains("azure") || lower.Contains("aws") ||
                lower.Contains("docker") || lower.Contains("git") ||
                lower.Contains("jira") || lower.Contains("jenkins"))
                return SkillCategory.Tool;

            if (lower.Contains("react") || lower.Contains("angular") ||
                lower.Contains("vue") || lower.Contains(".net") ||
                lower.Contains("spring") || lower.Contains("django"))
                return SkillCategory.Framework;

            if (new[] { "communication", "leadership", "teamwork",
                        "problem solving", "agile", "scrum" }.Contains(lower))
                return SkillCategory.Soft;

            return SkillCategory.Technical;
        }

        // ─── Getters ───────────────────────────────────────────────────────

        public async Task<ResumeViewModel?> GetResumeByIdAsync(int resumeId)
        {
            var resume = await _resumeRepo.GetResumeWithDetailsAsync(resumeId);
            return resume == null ? null : MapToViewModel(resume);
        }

        public async Task<ResumeListViewModel> GetCandidateResumesAsync(int candidateId)
        {
            var candidate = await _candidateRepo.GetCandidateWithDetailsAsync(candidateId);
            var resumes = await _resumeRepo.GetResumesByCandidateAsync(candidateId);

            return new ResumeListViewModel
            {
                CandidateId = candidateId,
                CandidateName = candidate != null
                    ? $"{candidate.User.FirstName} {candidate.User.LastName}"
                    : "Unknown",
                Resumes = resumes.Select(MapToViewModel).ToList()
            };
        }

        public async Task<ResumeListViewModel> GetCandidateResumesByUserIdAsync(string userId)
        {
            var candidate = await _candidateRepo.GetCandidateByUserIdAsync(userId);
            if (candidate == null) return new ResumeListViewModel();
            return await GetCandidateResumesAsync(candidate.Id);
        }

        public async Task<(bool Success, string Error)> SetActiveResumeAsync(
            string userId, int resumeId)
        {
            var candidate = await _candidateRepo.GetCandidateByUserIdAsync(userId);
            if (candidate == null) return (false, "Candidate not found.");

            var resume = await _resumeRepo.GetByIdAsync(resumeId);
            if (resume == null || resume.CandidateId != candidate.Id)
                return (false, "Resume not found or access denied.");

            await _resumeRepo.DeactivateAllResumesAsync(candidate.Id);

            resume.IsActive = true;
            _resumeRepo.Update(resume);
            await _resumeRepo.SaveChangesAsync();

            return (true, string.Empty);
        }

        public async Task<(bool Success, string Error)> DeleteResumeAsync(
            string userId, int resumeId)
        {
            try
            {
                var candidate = await _candidateRepo.GetCandidateByUserIdAsync(userId);
                if (candidate == null) return (false, "Candidate not found.");

                var resume = await _resumeRepo.GetByIdAsync(resumeId);
                if (resume == null || resume.CandidateId != candidate.Id)
                    return (false, "Resume not found or access denied.");

                // Check if resume is used in any application
                bool isUsedInApplication = await _context.Applications
                    .AnyAsync(a => a.ResumeId == resumeId);
                if (isUsedInApplication)
                    return (false, "Cannot delete a resume that has been used in an application.");

                // Delete physical file
                var fullPath = Path.Combine(_env.WebRootPath,
                    resume.FilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                FileHelper.DeleteFile(fullPath);

                _resumeRepo.Remove(resume);
                await _resumeRepo.SaveChangesAsync();

                _logger.LogInformation(
                    "Resume deleted: {ResumeId} for candidate {CandidateId}",
                    resumeId, candidate.Id);

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting resume {ResumeId}", resumeId);
                return (false, "An error occurred while deleting the resume.");
            }
        }

        public async Task<string?> GetActiveResumePathAsync(int candidateId)
        {
            var resume = await _resumeRepo.GetActiveResumeAsync(candidateId);
            return resume?.FilePath;
        }

        public async Task<int?> GetActiveResumeIdAsync(int candidateId)
        {
            var resume = await _resumeRepo.GetActiveResumeAsync(candidateId);
            return resume?.Id;
        }

        // ─── Mapper ────────────────────────────────────────────────────────

        private static ResumeViewModel MapToViewModel(Resume r) => new()
        {
            Id = r.Id,
            CandidateId = r.CandidateId,
            CandidateName = r.Candidate?.User != null
                ? $"{r.Candidate.User.FirstName} {r.Candidate.User.LastName}" : string.Empty,
            FileName = r.FileName,
            FilePath = r.FilePath,
            FileExtension = r.FileExtension,
            FileSizeBytes = r.FileSizeBytes,
            Status = r.Status,
            UploadedAt = r.UploadedAt,
            ParsedAt = r.ParsedAt,
            IsActive = r.IsActive,
            ParsedName = r.ParsedName,
            ParsedEmail = r.ParsedEmail,
            ParsedPhone = r.ParsedPhone,
            ParsedSkills = r.ParsedSkills,
            ParsedEducation = r.ParsedEducation,
            ParsedExperience = r.ParsedExperience,
            ParsedSummary = r.ParsedSummary
        };
    }
}