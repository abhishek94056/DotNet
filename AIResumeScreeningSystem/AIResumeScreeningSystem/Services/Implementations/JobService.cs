using AIResumeScreeningSystem.Data;
using AIResumeScreeningSystem.Models;
using AIResumeScreeningSystem.Repositories.Interfaces;
using AIResumeScreeningSystem.Services.Interfaces;
using AIResumeScreeningSystem.ViewModels.Job;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AIResumeScreeningSystem.Services.Implementations
{
    public class JobService : IJobService
    {
        private readonly IJobRepository _jobRepository;
        private readonly AppDbContext _context;
        private readonly ILogger<JobService> _logger;

        public JobService(
            IJobRepository jobRepository,
            AppDbContext context,
            ILogger<JobService> logger)
        {
            _jobRepository = jobRepository;
            _context = context;
            _logger = logger;
        }

        public async Task<JobListViewModel> GetJobsAsync(JobSearchViewModel search, string? postedByUserId = null)
        {
            try
            {
                var (jobs, totalCount) = await _jobRepository.GetPagedJobsAsync(search, postedByUserId);
                var jobViewModels = jobs.Select(MapToViewModel).ToList();

                var allUserJobs = postedByUserId != null
                    ? await _context.Jobs.Where(j => j.PostedByUserId == postedByUserId).ToListAsync()
                    : await _context.Jobs.ToListAsync();

                return new JobListViewModel
                {
                    Jobs = jobViewModels,
                    Search = search,
                    CurrentPage = search.Page,
                    TotalCount = totalCount,
                    PageSize = search.PageSize,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)search.PageSize),
                    ActiveJobsCount = allUserJobs.Count(j => j.Status == JobStatus.Active),
                    DraftJobsCount = allUserJobs.Count(j => j.Status == JobStatus.Draft),
                    ClosedJobsCount = allUserJobs.Count(j => j.Status == JobStatus.Closed),
                    TotalApplicationsCount = await _context.Applications
                        .CountAsync(a => postedByUserId == null || a.Job.PostedByUserId == postedByUserId)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving jobs list");
                throw;
            }
        }

        public async Task<JobViewModel?> GetJobByIdAsync(int jobId)
        {
            var job = await _jobRepository.GetJobWithDetailsAsync(jobId);
            return job == null ? null : MapToViewModel(job);
        }

        public async Task<(bool Success, int JobId, string Error)> CreateJobAsync(
            CreateJobViewModel model, string postedByUserId)
        {
            try
            {
                var job = new Job
                {
                    Title = model.Title,
                    Company = model.Company,
                    Description = model.Description,
                    Requirements = model.Requirements,
                    Location = model.Location,
                    SalaryMin = model.SalaryMin,
                    SalaryMax = model.SalaryMax,
                    JobType = model.JobType,
                    Status = model.Status,
                    ExpiryDate = model.ExpiryDate,
                    Department = model.Department,
                    ExperienceYearsMin = model.ExperienceYearsMin,
                    ExperienceYearsMax = model.ExperienceYearsMax,
                    PostedByUserId = postedByUserId,
                    PostedDate = DateTime.UtcNow
                };

                await _jobRepository.AddAsync(job);
                await _jobRepository.SaveChangesAsync();

                // Save skills
                if (model.RequiredSkillIds.Any() || model.OptionalSkillIds.Any())
                {
                    await _jobRepository.UpdateJobSkillsAsync(
                        job.Id, model.RequiredSkillIds, model.OptionalSkillIds);
                }

                _logger.LogInformation("Job created: {Title} by {UserId}", model.Title, postedByUserId);
                return (true, job.Id, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating job: {Title}", model.Title);
                return (false, 0, "An error occurred while creating the job.");
            }
        }

        public async Task<EditJobViewModel?> GetJobForEditAsync(int jobId)
        {
            var job = await _jobRepository.GetJobWithDetailsAsync(jobId);
            if (job == null) return null;

            var allSkills = await _context.Skills.OrderBy(s => s.Name).ToListAsync();

            return new EditJobViewModel
            {
                Id = job.Id,
                Title = job.Title,
                Company = job.Company,
                Description = job.Description,
                Requirements = job.Requirements,
                Location = job.Location,
                SalaryMin = job.SalaryMin,
                SalaryMax = job.SalaryMax,
                JobType = job.JobType,
                Status = job.Status,
                ExpiryDate = job.ExpiryDate,
                Department = job.Department,
                ExperienceYearsMin = job.ExperienceYearsMin,
                ExperienceYearsMax = job.ExperienceYearsMax,
                RequiredSkillIds = job.JobSkills
                    .Where(js => js.IsRequired).Select(js => js.SkillId).ToList(),
                OptionalSkillIds = job.JobSkills
                    .Where(js => !js.IsRequired).Select(js => js.SkillId).ToList(),
                AvailableSkills = allSkills.Select(s =>
                    new SelectListItem(s.Name, s.Id.ToString())).ToList()
            };
        }

        public async Task<(bool Success, string Error)> UpdateJobAsync(
            EditJobViewModel model, string currentUserId)
        {
            try
            {
                var job = await _jobRepository.GetJobWithDetailsAsync(model.Id);
                if (job == null)
                    return (false, "Job not found.");

                if (job.PostedByUserId != currentUserId)
                    return (false, "You are not authorized to edit this job.");

                job.Title = model.Title;
                job.Company = model.Company;
                job.Description = model.Description;
                job.Requirements = model.Requirements;
                job.Location = model.Location;
                job.SalaryMin = model.SalaryMin;
                job.SalaryMax = model.SalaryMax;
                job.JobType = model.JobType;
                job.Status = model.Status;
                job.ExpiryDate = model.ExpiryDate;
                job.Department = model.Department;
                job.ExperienceYearsMin = model.ExperienceYearsMin;
                job.ExperienceYearsMax = model.ExperienceYearsMax;
                job.UpdatedAt = DateTime.UtcNow;

                _jobRepository.Update(job);
                await _jobRepository.SaveChangesAsync();

                await _jobRepository.UpdateJobSkillsAsync(
                    job.Id, model.RequiredSkillIds, model.OptionalSkillIds);

                _logger.LogInformation("Job updated: {JobId} by {UserId}", model.Id, currentUserId);
                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating job {JobId}", model.Id);
                return (false, "An error occurred while updating the job.");
            }
        }

        public async Task<(bool Success, string Error)> DeleteJobAsync(int jobId, string currentUserId)
        {
            try
            {
                var job = await _jobRepository.GetByIdAsync(jobId);
                if (job == null)
                    return (false, "Job not found.");

                if (job.PostedByUserId != currentUserId)
                    return (false, "You are not authorized to delete this job.");

                _jobRepository.Remove(job);
                await _jobRepository.SaveChangesAsync();

                _logger.LogInformation("Job deleted: {JobId} by {UserId}", jobId, currentUserId);
                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting job {JobId}", jobId);
                return (false, "An error occurred while deleting the job.");
            }
        }

        public async Task<(bool Success, string Error)> ToggleJobStatusAsync(int jobId, string currentUserId)
        {
            var job = await _jobRepository.GetByIdAsync(jobId);
            if (job == null) return (false, "Job not found.");
            if (job.PostedByUserId != currentUserId) return (false, "Unauthorized.");

            job.Status = job.Status == JobStatus.Active ? JobStatus.Closed : JobStatus.Active;
            job.UpdatedAt = DateTime.UtcNow;
            _jobRepository.Update(job);
            await _jobRepository.SaveChangesAsync();
            return (true, string.Empty);
        }

        public async Task<List<JobViewModel>> GetActiveJobsAsync()
        {
            var jobs = await _jobRepository.GetActiveJobsAsync();
            return jobs.Select(MapToViewModel).ToList();
        }

        public async Task<JobListViewModel> GetPublicJobsAsync(JobSearchViewModel search)
        {
            search.Status = JobStatus.Active;
            return await GetJobsAsync(search);
        }

        public async Task<bool> IsJobOwnedByUserAsync(int jobId, string userId)
        {
            return await _jobRepository.AnyAsync(j => j.Id == jobId && j.PostedByUserId == userId);
        }

        public async Task<Dictionary<string, int>> GetJobStatsByUserAsync(string userId)
        {
            return await _jobRepository.GetJobStatsByUserAsync(userId);
        }

        // ─── Private Mapper ────────────────────────────────────────────────
        private static JobViewModel MapToViewModel(Job job) => new()
        {
            Id = job.Id,
            Title = job.Title,
            Company = job.Company,
            Description = job.Description,
            Requirements = job.Requirements,
            Location = job.Location,
            SalaryMin = job.SalaryMin,
            SalaryMax = job.SalaryMax,
            JobType = job.JobType,
            Status = job.Status,
            PostedDate = job.PostedDate,
            ExpiryDate = job.ExpiryDate,
            Department = job.Department,
            ExperienceYearsMin = job.ExperienceYearsMin,
            ExperienceYearsMax = job.ExperienceYearsMax,
            PostedByUserId = job.PostedByUserId,
            PostedByName = job.PostedBy != null
                ? $"{job.PostedBy.FirstName} {job.PostedBy.LastName}" : "Unknown",
            TotalApplications = job.Applications?.Count ?? 0,
            RequiredSkills = job.JobSkills?
                .Where(js => js.IsRequired).Select(js => js.Skill.Name).ToList() ?? new(),
            OptionalSkills = job.JobSkills?
                .Where(js => !js.IsRequired).Select(js => js.Skill.Name).ToList() ?? new()
        };
    }
}