using AIResumeScreeningSystem.Data;
using AIResumeScreeningSystem.Models;
using AIResumeScreeningSystem.Repositories.Interfaces;
using AIResumeScreeningSystem.ViewModels.Candidate;
using Microsoft.EntityFrameworkCore;

namespace AIResumeScreeningSystem.Repositories.Implementations
{
    public class CandidateRepository : GenericRepository<Candidate>, ICandidateRepository
    {
        public CandidateRepository(AppDbContext context) : base(context) { }

        public async Task<Candidate?> GetCandidateByUserIdAsync(string userId)
        {
            return await _context.Candidates
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<Candidate?> GetCandidateWithDetailsAsync(int candidateId)
        {
            return await _context.Candidates
                .Include(c => c.User)
                .Include(c => c.CandidateSkills)
                    .ThenInclude(cs => cs.Skill)
                .Include(c => c.Resumes.Where(r => r.IsActive))
                .Include(c => c.Applications)
                    .ThenInclude(a => a.Job)
                .FirstOrDefaultAsync(c => c.Id == candidateId);
        }

        public async Task<Candidate?> GetCandidateWithDetailsByUserIdAsync(string userId)
        {
            return await _context.Candidates
                .Include(c => c.User)
                .Include(c => c.CandidateSkills)
                    .ThenInclude(cs => cs.Skill)
                .Include(c => c.Resumes.Where(r => r.IsActive))
                .Include(c => c.Applications)
                    .ThenInclude(a => a.Job)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<(IEnumerable<Candidate> Candidates, int TotalCount)> GetPagedCandidatesAsync(
            CandidateSearchViewModel search)
        {
            var query = _context.Candidates
                .Include(c => c.User)
                .Include(c => c.CandidateSkills).ThenInclude(cs => cs.Skill)
                .Include(c => c.Resumes.Where(r => r.IsActive))
                .Include(c => c.Applications)
                .AsQueryable();

            // Keyword filter
            if (!string.IsNullOrWhiteSpace(search.Keyword))
            {
                var kw = search.Keyword.ToLower();
                query = query.Where(c =>
                    (c.User.FirstName + " " + c.User.LastName).ToLower().Contains(kw) ||
                    (c.Headline != null && c.Headline.ToLower().Contains(kw)) ||
                    (c.CurrentJobTitle != null && c.CurrentJobTitle.ToLower().Contains(kw)) ||
                    (c.CurrentCompany != null && c.CurrentCompany.ToLower().Contains(kw)) ||
                    c.CandidateSkills.Any(cs => cs.Skill.Name.ToLower().Contains(kw)));
            }

            // Location filter
            if (!string.IsNullOrWhiteSpace(search.Location))
            {
                var loc = search.Location.ToLower();
                query = query.Where(c => c.Location != null &&
                    c.Location.ToLower().Contains(loc));
            }

            // Skill filter
            if (!string.IsNullOrWhiteSpace(search.Skill))
            {
                var sk = search.Skill.ToLower();
                query = query.Where(c =>
                    c.CandidateSkills.Any(cs => cs.Skill.Name.ToLower().Contains(sk)));
            }

            // Experience filters
            if (search.MinExperience.HasValue)
                query = query.Where(c => c.TotalExperienceYears >= search.MinExperience.Value);
            if (search.MaxExperience.HasValue)
                query = query.Where(c => c.TotalExperienceYears <= search.MaxExperience.Value);

            // Education filter
            if (!string.IsNullOrWhiteSpace(search.Education))
            {
                var edu = search.Education.ToLower();
                query = query.Where(c =>
                    c.HighestEducation != null && c.HighestEducation.ToLower().Contains(edu));
            }

            // Availability filter
            if (search.IsAvailable.HasValue)
                query = query.Where(c => c.IsAvailable == search.IsAvailable.Value);

            // Only active users
            query = query.Where(c => c.User.IsActive);

            int totalCount = await query.CountAsync();

            // Sorting
            query = (search.SortBy?.ToLower(), search.SortDirection?.ToLower()) switch
            {
                ("experience", "asc") => query.OrderBy(c => c.TotalExperienceYears),
                ("experience", _) => query.OrderByDescending(c => c.TotalExperienceYears),
                ("name", "asc") => query.OrderBy(c => c.User.FirstName),
                ("name", _) => query.OrderByDescending(c => c.User.FirstName),
                (_, "asc") => query.OrderBy(c => c.CreatedAt),
                _ => query.OrderByDescending(c => c.CreatedAt)
            };

            var candidates = await query
                .Skip((search.Page - 1) * search.PageSize)
                .Take(search.PageSize)
                .ToListAsync();

            return (candidates, totalCount);
        }

        public async Task<IEnumerable<Candidate>> GetCandidatesWithSkillsAsync()
        {
            return await _context.Candidates
                .Include(c => c.User)
                .Include(c => c.CandidateSkills).ThenInclude(cs => cs.Skill)
                .Where(c => c.User.IsActive)
                .ToListAsync();
        }

        public async Task<bool> AddCandidateSkillAsync(CandidateSkill skill)
        {
            bool exists = await _context.CandidateSkills
                .AnyAsync(cs => cs.CandidateId == skill.CandidateId &&
                                cs.SkillId == skill.SkillId);
            if (exists) return false;

            await _context.CandidateSkills.AddAsync(skill);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveCandidateSkillAsync(int candidateId, int skillId)
        {
            var skill = await _context.CandidateSkills
                .FirstOrDefaultAsync(cs => cs.CandidateId == candidateId &&
                                           cs.SkillId == skillId);
            if (skill == null) return false;

            _context.CandidateSkills.Remove(skill);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateCandidateSkillAsync(
            int candidateSkillId, ProficiencyLevel level, int years)
        {
            var skill = await _context.CandidateSkills.FindAsync(candidateSkillId);
            if (skill == null) return false;

            skill.ProficiencyLevel = level;
            skill.YearsOfExperience = years;
            _context.CandidateSkills.Update(skill);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<CandidateSkill?> GetCandidateSkillAsync(int candidateId, int skillId)
        {
            return await _context.CandidateSkills
                .Include(cs => cs.Skill)
                .FirstOrDefaultAsync(cs => cs.CandidateId == candidateId &&
                                           cs.SkillId == skillId);
        }

        public async Task<List<CandidateSkill>> GetCandidateSkillsAsync(int candidateId)
        {
            return await _context.CandidateSkills
                .Include(cs => cs.Skill)
                .Where(cs => cs.CandidateId == candidateId)
                .OrderBy(cs => cs.Skill.Name)
                .ToListAsync();
        }

        public async Task<Dictionary<string, int>> GetCandidateStatsAsync()
        {
            var candidates = await _context.Candidates
                .Include(c => c.User)
                .ToListAsync();

            return new Dictionary<string, int>
            {
                { "Total", candidates.Count },
                { "Available", candidates.Count(c => c.IsAvailable) },
                { "Active", candidates.Count(c => c.User.IsActive) },
                {
                    "NewThisMonth",
                    candidates.Count(c =>
                        c.CreatedAt.Month == DateTime.UtcNow.Month &&
                        c.CreatedAt.Year == DateTime.UtcNow.Year)
                }
            };
        }

        public async Task<int> GetProfileCompletionPercentAsync(int candidateId)
        {
            var candidate = await _context.Candidates
                .Include(c => c.User)
                .Include(c => c.CandidateSkills)
                .Include(c => c.Resumes.Where(r => r.IsActive))
                .FirstOrDefaultAsync(c => c.Id == candidateId);

            if (candidate == null) return 0;

            int score = 0;
            if (!string.IsNullOrEmpty(candidate.User.FirstName)) score += 10;
            if (!string.IsNullOrEmpty(candidate.User.Email)) score += 10;
            if (!string.IsNullOrEmpty(candidate.User.PhoneNumber)) score += 10;
            if (!string.IsNullOrEmpty(candidate.Headline)) score += 10;
            if (!string.IsNullOrEmpty(candidate.Summary)) score += 10;
            if (!string.IsNullOrEmpty(candidate.CurrentJobTitle)) score += 10;
            if (!string.IsNullOrEmpty(candidate.Location)) score += 5;
            if (!string.IsNullOrEmpty(candidate.HighestEducation)) score += 10;
            if (candidate.CandidateSkills.Any()) score += 15;
            if (candidate.Resumes.Any()) score += 10;

            return Math.Min(score, 100);
        }
    }
}