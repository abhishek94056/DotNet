using AIResumeScreeningSystem.Data;
using AIResumeScreeningSystem.Models;
using AIResumeScreeningSystem.Repositories.Interfaces;
using AIResumeScreeningSystem.ViewModels.Job;
using Microsoft.EntityFrameworkCore;

namespace AIResumeScreeningSystem.Repositories.Implementations
{
    public class JobRepository : GenericRepository<Job>, IJobRepository
    {
        public JobRepository(AppDbContext context) : base(context) { }

        public async Task<Job?> GetJobWithDetailsAsync(int jobId)
        {
            return await _context.Jobs
                .Include(j => j.PostedBy)
                .Include(j => j.JobSkills)
                    .ThenInclude(js => js.Skill)
                .Include(j => j.Applications)
                .FirstOrDefaultAsync(j => j.Id == jobId);
        }

        public async Task<(IEnumerable<Job> Jobs, int TotalCount)> GetPagedJobsAsync(
            JobSearchViewModel search,
            string? postedByUserId = null)
        {
            var query = _context.Jobs
                .Include(j => j.PostedBy)
                .Include(j => j.JobSkills).ThenInclude(js => js.Skill)
                .Include(j => j.Applications)
                .AsQueryable();

            // Filter by recruiter if provided
            if (!string.IsNullOrEmpty(postedByUserId))
                query = query.Where(j => j.PostedByUserId == postedByUserId);

            // Keyword search
            if (!string.IsNullOrWhiteSpace(search.Keyword))
            {
                var kw = search.Keyword.ToLower();
                query = query.Where(j =>
                    j.Title.ToLower().Contains(kw) ||
                    j.Company.ToLower().Contains(kw) ||
                    j.Description.ToLower().Contains(kw) ||
                    (j.Department != null && j.Department.ToLower().Contains(kw)));
            }

            // Location filter
            if (!string.IsNullOrWhiteSpace(search.Location))
            {
                var loc = search.Location.ToLower();
                query = query.Where(j => j.Location != null && j.Location.ToLower().Contains(loc));
            }

            // Job type filter
            if (search.JobType.HasValue)
                query = query.Where(j => j.JobType == search.JobType.Value);

            // Status filter
            if (search.Status.HasValue)
                query = query.Where(j => j.Status == search.Status.Value);

            // Department filter
            if (!string.IsNullOrWhiteSpace(search.Department))
                query = query.Where(j => j.Department != null &&
                    j.Department.ToLower().Contains(search.Department.ToLower()));

            // Salary filters
            if (search.SalaryMin.HasValue)
                query = query.Where(j => j.SalaryMin >= search.SalaryMin.Value);
            if (search.SalaryMax.HasValue)
                query = query.Where(j => j.SalaryMax <= search.SalaryMax.Value);

            // Experience filter
            if (search.ExperienceYears.HasValue)
                query = query.Where(j =>
                    j.ExperienceYearsMin <= search.ExperienceYears.Value &&
                    (j.ExperienceYearsMax == 0 || j.ExperienceYearsMax >= search.ExperienceYears.Value));

            int totalCount = await query.CountAsync();

            // Sorting
            query = (search.SortBy?.ToLower(), search.SortDirection?.ToLower()) switch
            {
                ("title", "asc") => query.OrderBy(j => j.Title),
                ("title", _) => query.OrderByDescending(j => j.Title),
                ("company", "asc") => query.OrderBy(j => j.Company),
                ("company", _) => query.OrderByDescending(j => j.Company),
                ("salary", "asc") => query.OrderBy(j => j.SalaryMin),
                ("salary", _) => query.OrderByDescending(j => j.SalaryMin),
                ("applications", "asc") => query.OrderBy(j => j.Applications.Count),
                ("applications", _) => query.OrderByDescending(j => j.Applications.Count),
                (_, "asc") => query.OrderBy(j => j.PostedDate),
                _ => query.OrderByDescending(j => j.PostedDate)
            };

            var jobs = await query
                .Skip((search.Page - 1) * search.PageSize)
                .Take(search.PageSize)
                .ToListAsync();

            return (jobs, totalCount);
        }

        public async Task<IEnumerable<Job>> GetActiveJobsAsync()
        {
            return await _context.Jobs
                .Include(j => j.PostedBy)
                .Include(j => j.JobSkills).ThenInclude(js => js.Skill)
                .Where(j => j.Status == JobStatus.Active &&
                            (!j.ExpiryDate.HasValue || j.ExpiryDate > DateTime.UtcNow))
                .OrderByDescending(j => j.PostedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Job>> GetJobsByUserAsync(string userId)
        {
            return await _context.Jobs
                .Include(j => j.JobSkills).ThenInclude(js => js.Skill)
                .Include(j => j.Applications)
                .Where(j => j.PostedByUserId == userId)
                .OrderByDescending(j => j.PostedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Job>> GetJobsWithSkillsAsync()
        {
            return await _context.Jobs
                .Include(j => j.JobSkills).ThenInclude(js => js.Skill)
                .Where(j => j.Status == JobStatus.Active)
                .ToListAsync();
        }

        public async Task UpdateJobSkillsAsync(int jobId, List<int> requiredSkillIds, List<int> optionalSkillIds)
        {
            // Remove existing job skills
            var existingSkills = await _context.JobSkills
                .Where(js => js.JobId == jobId)
                .ToListAsync();
            _context.JobSkills.RemoveRange(existingSkills);

            // Add required skills
            foreach (var skillId in requiredSkillIds.Distinct())
            {
                await _context.JobSkills.AddAsync(new JobSkill
                {
                    JobId = jobId,
                    SkillId = skillId,
                    IsRequired = true,
                    WeightagePercent = 10
                });
            }

            // Add optional skills (avoid duplicates with required)
            foreach (var skillId in optionalSkillIds.Distinct().Where(id => !requiredSkillIds.Contains(id)))
            {
                await _context.JobSkills.AddAsync(new JobSkill
                {
                    JobId = jobId,
                    SkillId = skillId,
                    IsRequired = false,
                    WeightagePercent = 5
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task<Dictionary<string, int>> GetJobStatsByUserAsync(string userId)
        {
            var jobs = await _context.Jobs
                .Where(j => j.PostedByUserId == userId)
                .ToListAsync();

            return new Dictionary<string, int>
            {
                { "Total", jobs.Count },
                { "Active", jobs.Count(j => j.Status == JobStatus.Active) },
                { "Draft", jobs.Count(j => j.Status == JobStatus.Draft) },
                { "Closed", jobs.Count(j => j.Status == JobStatus.Closed) },
                { "Expired", jobs.Count(j => j.Status == JobStatus.Expired) }
            };
        }

        public async Task<int> GetTotalApplicationsForJobAsync(int jobId)
        {
            return await _context.Applications.CountAsync(a => a.JobId == jobId);
        }
    }
}