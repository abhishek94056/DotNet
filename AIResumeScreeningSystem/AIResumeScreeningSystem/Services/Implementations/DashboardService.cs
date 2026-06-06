using AIResumeScreeningSystem.Data;
using AIResumeScreeningSystem.Models;
using AIResumeScreeningSystem.Services.Interfaces;
using AIResumeScreeningSystem.ViewModels.Dashboard;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AIResumeScreeningSystem.Services.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(
            AppDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<DashboardService> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // ──────────────────────────────────────────────────────────────────
        // ADMIN DASHBOARD
        // ──────────────────────────────────────────────────────────────────

        public async Task<AdminDashboardViewModel> GetAdminDashboardAsync()
        {
            var vm = new AdminDashboardViewModel();
            var now = DateTime.UtcNow;
            var monthStart = new DateTime(now.Year, now.Month, 1);

            try
            {
                // ── User Counts ────────────────────────────────────────────
                var allUsers = await _userManager.Users.ToListAsync();
                vm.TotalUsers = allUsers.Count;
                vm.NewUsersThisMonth = allUsers
                    .Count(u => u.CreatedAt >= monthStart);

                var candidateRole = await _context.Roles
                    .FirstOrDefaultAsync(r => r.Name == "Candidate");
                var recruiterRole = await _context.Roles
                    .FirstOrDefaultAsync(r => r.Name == "Recruiter");

                if (candidateRole != null)
                    vm.TotalCandidates = await _context.UserRoles
                        .CountAsync(ur => ur.RoleId == candidateRole.Id);
                if (recruiterRole != null)
                    vm.TotalRecruiters = await _context.UserRoles
                        .CountAsync(ur => ur.RoleId == recruiterRole.Id);

                // ── Job Stats ──────────────────────────────────────────────
                var jobs = await _context.Jobs.ToListAsync();
                vm.TotalJobs = jobs.Count;
                vm.ActiveJobs = jobs.Count(j => j.Status == JobStatus.Active);
                vm.NewJobsThisMonth = jobs.Count(j => j.PostedDate >= monthStart);

                // ── Application Stats ──────────────────────────────────────
                var applications = await _context.Applications.ToListAsync();
                vm.TotalApplications = applications.Count;
                vm.ShortlistedApplications = applications
                    .Count(a => a.Status == ApplicationStatus.Shortlisted);
                vm.ApprovedApplications = applications
                    .Count(a => a.Status == ApplicationStatus.Approved);
                vm.NewApplicationsThisMonth = applications
                    .Count(a => a.AppliedAt >= monthStart);
                vm.AverageMatchScore = applications.Any(a => a.AIMatchScore.HasValue)
                    ? Math.Round(applications
                        .Where(a => a.AIMatchScore.HasValue)
                        .Average(a => a.AIMatchScore!.Value), 1)
                    : 0;

                // ── Resume Stats ───────────────────────────────────────────
                var resumes = await _context.Resumes.ToListAsync();
                vm.TotalResumes = resumes.Count;
                vm.ParsedResumes = resumes.Count(r => r.Status == ResumeStatus.Parsed);
                vm.PendingParseQueue = resumes.Count(r =>
                    r.Status == ResumeStatus.Uploaded ||
                    r.Status == ResumeStatus.Parsing);
                vm.FailedParseCount = resumes.Count(r => r.Status == ResumeStatus.Failed);

                // ── AI Stats ───────────────────────────────────────────────
                vm.TotalInterviewQuestions = await _context.InterviewQuestions.CountAsync();
                vm.AIEvaluationsRun = applications.Count(a =>
                    !string.IsNullOrEmpty(a.AIEvaluation));
                vm.AIAccuracyRate = 94.2m; // Placeholder metric

                // ── Chart: Applications by Status ──────────────────────────
                vm.ApplicationsByStatus = new ChartDataSet
                {
                    Labels = new List<string>
                        { "Submitted", "Under Review", "Shortlisted", "Approved", "Rejected" },
                    Values = new List<decimal>
                    {
                        applications.Count(a => a.Status == ApplicationStatus.Submitted),
                        applications.Count(a => a.Status == ApplicationStatus.UnderReview),
                        applications.Count(a => a.Status == ApplicationStatus.Shortlisted),
                        applications.Count(a => a.Status == ApplicationStatus.Approved),
                        applications.Count(a => a.Status == ApplicationStatus.Rejected)
                    },
                    BackgroundColors = new List<string>
                    {
                        "rgba(107,114,128,0.8)",
                        "rgba(14,165,233,0.8)",
                        "rgba(234,179,8,0.8)",
                        "rgba(34,197,94,0.8)",
                        "rgba(239,68,68,0.8)"
                    },
                    DatasetLabel = "Applications"
                };

                // ── Chart: Applications Trend (last 6 months) ─────────────
                vm.ApplicationsTrend = await BuildApplicationsTrendAsync(6);

                // ── Chart: User Growth (last 6 months) ────────────────────
                vm.UserGrowth = BuildUserGrowthChart(allUsers, 6);

                // ── Chart: Top Skills Distribution ────────────────────────
                vm.TopSkillsDistribution = await BuildTopSkillsChartAsync(8);

                // ── Chart: Jobs by Type ────────────────────────────────────
                vm.JobsByType = new ChartDataSet
                {
                    Labels = Enum.GetNames<JobType>().ToList(),
                    Values = Enum.GetValues<JobType>()
                        .Select(jt => (decimal)jobs.Count(j => j.JobType == jt))
                        .ToList(),
                    DatasetLabel = "Jobs"
                };

                // ── Top Jobs ───────────────────────────────────────────────
                vm.TopJobs = await BuildTopJobsAsync(5);

                // ── Top Candidates ─────────────────────────────────────────
                vm.TopCandidates = await BuildTopCandidatesAsync(5);

                // ── Recent Activity ────────────────────────────────────────
                vm.RecentActivity = await BuildRecentActivityAsync(10);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error building admin dashboard");
            }

            return vm;
        }

        // ──────────────────────────────────────────────────────────────────
        // RECRUITER DASHBOARD
        // ──────────────────────────────────────────────────────────────────

        public async Task<RecruiterDashboardViewModel> GetRecruiterDashboardAsync(
            string recruiterId)
        {
            var vm = new RecruiterDashboardViewModel();
            var now = DateTime.UtcNow;

            try
            {
                var user = await _userManager.FindByIdAsync(recruiterId);
                if (user == null) return vm;

                vm.RecruiterName = $"{user.FirstName} {user.LastName}";
                vm.RecruiterEmail = user.Email ?? string.Empty;
                vm.InitialsAvatar = vm.RecruiterName.Length >= 2
                    ? $"{vm.RecruiterName.Split(' ').First()[0]}{vm.RecruiterName.Split(' ').Last()[0]}".ToUpper()
                    : "?";

                // ── Jobs owned by this recruiter ───────────────────────────
                var myJobs = await _context.Jobs
                    .Where(j => j.PostedByUserId == recruiterId)
                    .ToListAsync();

                vm.TotalJobs = myJobs.Count;
                vm.ActiveJobs = myJobs.Count(j => j.Status == JobStatus.Active);
                vm.DraftJobs = myJobs.Count(j => j.Status == JobStatus.Draft);
                vm.ClosedJobs = myJobs.Count(j => j.Status == JobStatus.Closed);

                var myJobIds = myJobs.Select(j => j.Id).ToHashSet();

                // ── Applications on recruiter's jobs ───────────────────────
                var myApplications = await _context.Applications
                    .Include(a => a.Candidate).ThenInclude(c => c.User)
                    .Include(a => a.Job)
                    .Where(a => myJobIds.Contains(a.JobId))
                    .ToListAsync();

                vm.TotalApplications = myApplications.Count;
                vm.PendingReview = myApplications.Count(a =>
                    a.Status == ApplicationStatus.Submitted ||
                    a.Status == ApplicationStatus.UnderReview);
                vm.Shortlisted = myApplications
                    .Count(a => a.Status == ApplicationStatus.Shortlisted);
                vm.Approved = myApplications
                    .Count(a => a.Status == ApplicationStatus.Approved);
                vm.Rejected = myApplications
                    .Count(a => a.Status == ApplicationStatus.Rejected);
                vm.AverageMatchScore = myApplications.Any(a => a.AIMatchScore.HasValue)
                    ? Math.Round(myApplications
                        .Where(a => a.AIMatchScore.HasValue)
                        .Average(a => a.AIMatchScore!.Value), 1)
                    : 0;

                // ── Chart: Applications by Status (doughnut) ──────────────
                vm.ApplicationsByStatus = new ChartDataSet
                {
                    Labels = new List<string>
                        { "Pending", "Shortlisted", "Approved", "Rejected" },
                    Values = new List<decimal>
                    {
                        vm.PendingReview, vm.Shortlisted, vm.Approved, vm.Rejected
                    },
                    BackgroundColors = new List<string>
                    {
                        "rgba(107,114,128,0.8)",
                        "rgba(234,179,8,0.8)",
                        "rgba(34,197,94,0.8)",
                        "rgba(239,68,68,0.8)"
                    },
                    DatasetLabel = "Applications"
                };

                // ── Chart: Applications Trend (last 8 weeks) ──────────────
                vm.ApplicationsTrend = BuildWeeklyTrend(myApplications, 8);

                // ── Chart: Score Distribution ──────────────────────────────
                vm.ScoreDistribution = BuildScoreDistributionChart(myApplications);

                // ── Chart: Job Performance ─────────────────────────────────
                vm.JobPerformance = await BuildJobPerformanceChartAsync(
                    myJobs.Take(6).ToList());

                // ── My Top Jobs ────────────────────────────────────────────
                vm.MyTopJobs = myJobs
                    .Select(j => new TopJobItem
                    {
                        JobId = j.Id,
                        Title = j.Title,
                        Company = j.Company,
                        ApplicationCount = myApplications.Count(a => a.JobId == j.Id),
                        AverageMatchScore = myApplications
                            .Where(a => a.JobId == j.Id && a.AIMatchScore.HasValue)
                            .Select(a => a.AIMatchScore!.Value)
                            .DefaultIfEmpty(0)
                            .Average(),
                        ShortlistedCount = myApplications
                            .Count(a => a.JobId == j.Id &&
                                        a.Status == ApplicationStatus.Shortlisted),
                        Status = j.Status.ToString()
                    })
                    .OrderByDescending(j => j.ApplicationCount)
                    .Take(5)
                    .ToList();

                // ── Top Candidates ─────────────────────────────────────────
                vm.TopCandidates = myApplications
                    .Where(a => a.AIMatchScore.HasValue)
                    .GroupBy(a => a.CandidateId)
                    .Select(g =>
                    {
                        var best = g.OrderByDescending(a => a.AIMatchScore).First();
                        var name = $"{best.Candidate.User.FirstName} {best.Candidate.User.LastName}";
                        return new TopCandidateItem
                        {
                            CandidateId = g.Key,
                            Name = name,
                            Headline = best.Candidate.Headline,
                            HighestMatchScore = best.AIMatchScore ?? 0,
                            ApplicationCount = g.Count(),
                            Location = best.Candidate.Location,
                            InitialsAvatar = name.Length >= 2
                                ? $"{name.Split(' ').First()[0]}{name.Split(' ').Last()[0]}".ToUpper()
                                : "?",
                            ProfileImagePath = best.Candidate.User.ProfileImagePath
                        };
                    })
                    .OrderByDescending(c => c.HighestMatchScore)
                    .Take(5)
                    .ToList();

                // ── Pending Actions ────────────────────────────────────────
                vm.PendingActions = myApplications
                    .Where(a => a.Status == ApplicationStatus.Submitted ||
                                a.Status == ApplicationStatus.UnderReview)
                    .OrderByDescending(a => a.AIMatchScore)
                    .Take(8)
                    .Select(a =>
                    {
                        var name = $"{a.Candidate.User.FirstName} {a.Candidate.User.LastName}";
                        return new PendingActionItem
                        {
                            ApplicationId = a.Id,
                            JobId = a.JobId,
                            CandidateName = name,
                            JobTitle = a.Job?.Title ?? string.Empty,
                            MatchScore = a.AIMatchScore ?? 0,
                            Status = a.Status.ToString(),
                            AppliedAt = a.AppliedAt,
                            InitialsAvatar = name.Length >= 2
                                ? $"{name.Split(' ').First()[0]}{name.Split(' ').Last()[0]}".ToUpper()
                                : "?"
                        };
                    })
                    .ToList();

                // ── Recent Activity ────────────────────────────────────────
                vm.RecentActivity = myApplications
                    .OrderByDescending(a => a.UpdatedAt ?? a.AppliedAt)
                    .Take(8)
                    .Select(a => new ActivityFeedItem
                    {
                        Title = $"{a.Candidate.User.FirstName} {a.Candidate.User.LastName}",
                        Description = $"applied for {a.Job?.Title ?? "a job"} · " +
                                      $"{(a.AIMatchScore.HasValue ? a.AIMatchScore.Value.ToString("F0") + "% match" : "Calculating...")}",
                        Icon = "bi-person-plus",
                        IconColorClass = "text-primary",
                        Timestamp = a.AppliedAt,
                        ActionUrl = $"/Application/Details/{a.Id}"
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error building recruiter dashboard for {RecruiterId}", recruiterId);
            }

            return vm;
        }

        // ──────────────────────────────────────────────────────────────────
        // Private chart builders
        // ──────────────────────────────────────────────────────────────────

        private async Task<ChartDataSet> BuildApplicationsTrendAsync(int months)
        {
            var labels = new List<string>();
            var values = new List<decimal>();

            for (int i = months - 1; i >= 0; i--)
            {
                var date = DateTime.UtcNow.AddMonths(-i);
                var start = new DateTime(date.Year, date.Month, 1);
                var end = start.AddMonths(1);

                labels.Add(date.ToString("MMM yy"));
                var count = await _context.Applications
                    .CountAsync(a => a.AppliedAt >= start && a.AppliedAt < end);
                values.Add(count);
            }

            return new ChartDataSet
            {
                Labels = labels,
                Values = values,
                DatasetLabel = "Applications"
            };
        }

        private static ChartDataSet BuildUserGrowthChart(
            List<ApplicationUser> users, int months)
        {
            var labels = new List<string>();
            var values = new List<decimal>();

            for (int i = months - 1; i >= 0; i--)
            {
                var date = DateTime.UtcNow.AddMonths(-i);
                var start = new DateTime(date.Year, date.Month, 1);
                var end = start.AddMonths(1);

                labels.Add(date.ToString("MMM yy"));
                values.Add(users.Count(u => u.CreatedAt >= start && u.CreatedAt < end));
            }

            return new ChartDataSet
            {
                Labels = labels,
                Values = values,
                DatasetLabel = "New Users"
            };
        }

        private async Task<ChartDataSet> BuildTopSkillsChartAsync(int topN)
        {
            var topSkills = await _context.CandidateSkills
                .Include(cs => cs.Skill)
                .GroupBy(cs => cs.Skill.Name)
                .OrderByDescending(g => g.Count())
                .Take(topN)
                .Select(g => new { Name = g.Key, Count = g.Count() })
                .ToListAsync();

            return new ChartDataSet
            {
                Labels = topSkills.Select(s => s.Name).ToList(),
                Values = topSkills.Select(s => (decimal)s.Count).ToList(),
                DatasetLabel = "Candidates"
            };
        }

        private async Task<List<TopJobItem>> BuildTopJobsAsync(int topN)
        {
            return await _context.Jobs
                .Include(j => j.Applications)
                .OrderByDescending(j => j.Applications.Count)
                .Take(topN)
                .Select(j => new TopJobItem
                {
                    JobId = j.Id,
                    Title = j.Title,
                    Company = j.Company,
                    ApplicationCount = j.Applications.Count,
                    AverageMatchScore = j.Applications.Any(a => a.AIMatchScore.HasValue)
                        ? j.Applications
                            .Where(a => a.AIMatchScore.HasValue)
                            .Average(a => a.AIMatchScore!.Value)
                        : 0,
                    ShortlistedCount = j.Applications
                        .Count(a => a.Status == ApplicationStatus.Shortlisted),
                    Status = j.Status.ToString()
                })
                .ToListAsync();
        }

        private async Task<List<TopCandidateItem>> BuildTopCandidatesAsync(int topN)
        {
            return await _context.Applications
                .Where(a => a.AIMatchScore.HasValue)
                .Include(a => a.Candidate).ThenInclude(c => c.User)
                .Include(a => a.Candidate).ThenInclude(c => c.CandidateSkills)
                    .ThenInclude(cs => cs.Skill)
                .GroupBy(a => a.CandidateId)
                .OrderByDescending(g => g.Max(a => a.AIMatchScore))
                .Take(topN)
                .Select(g => new TopCandidateItem
                {
                    CandidateId = g.Key,
                    Name = g.First().Candidate.User.FirstName + " " +
                           g.First().Candidate.User.LastName,
                    Headline = g.First().Candidate.Headline,
                    HighestMatchScore = g.Max(a => a.AIMatchScore ?? 0),
                    ApplicationCount = g.Count(),
                    Location = g.First().Candidate.Location,
                    ProfileImagePath = g.First().Candidate.User.ProfileImagePath,
                    InitialsAvatar =
                        (g.First().Candidate.User.FirstName + " " +
                         g.First().Candidate.User.LastName).Length >= 2
                            ? (g.First().Candidate.User.FirstName[0].ToString() +
                               g.First().Candidate.User.LastName[0].ToString()).ToUpper()
                            : "?",
                    TopSkills = g.First().Candidate.CandidateSkills
                        .Take(3).Select(cs => cs.Skill.Name).ToList()
                })
                .ToListAsync();
        }

        private async Task<List<ActivityFeedItem>> BuildRecentActivityAsync(int count)
        {
            var feed = new List<ActivityFeedItem>();

            var recentApps = await _context.Applications
                .Include(a => a.Candidate).ThenInclude(c => c.User)
                .Include(a => a.Job)
                .OrderByDescending(a => a.AppliedAt)
                .Take(count / 2)
                .ToListAsync();

            foreach (var app in recentApps)
            {
                feed.Add(new ActivityFeedItem
                {
                    Title = $"{app.Candidate.User.FirstName} applied",
                    Description = $"for {app.Job?.Title ?? "a job"}",
                    Icon = "bi-person-plus",
                    IconColorClass = "text-primary",
                    Timestamp = app.AppliedAt,
                    ActionUrl = $"/Application/Details/{app.Id}"
                });
            }

            var recentJobs = await _context.Jobs
                .OrderByDescending(j => j.PostedDate)
                .Take(count / 2)
                .ToListAsync();

            foreach (var job in recentJobs)
            {
                feed.Add(new ActivityFeedItem
                {
                    Title = "New job posted",
                    Description = job.Title,
                    Icon = "bi-briefcase",
                    IconColorClass = "text-success",
                    Timestamp = job.PostedDate,
                    ActionUrl = $"/Job/Details/{job.Id}"
                });
            }

            return feed.OrderByDescending(f => f.Timestamp).Take(count).ToList();
        }

        private static ChartDataSet BuildWeeklyTrend(
            List<Application> applications, int weeks)
        {
            var labels = new List<string>();
            var values = new List<decimal>();

            for (int i = weeks - 1; i >= 0; i--)
            {
                var start = DateTime.UtcNow.AddDays(-i * 7 - 6).Date;
                var end = start.AddDays(7);
                labels.Add($"Wk {weeks - i}");
                values.Add(applications.Count(a =>
                    a.AppliedAt >= start && a.AppliedAt < end));
            }

            return new ChartDataSet
            {
                Labels = labels,
                Values = values,
                DatasetLabel = "Applications"
            };
        }

        private static ChartDataSet BuildScoreDistributionChart(
            List<Application> applications)
        {
            var scored = applications.Where(a => a.AIMatchScore.HasValue).ToList();
            return new ChartDataSet
            {
                Labels = new List<string>
                    { "0-25%", "26-50%", "51-74%", "75-89%", "90-100%" },
                Values = new List<decimal>
                {
                    scored.Count(a => a.AIMatchScore < 26),
                    scored.Count(a => a.AIMatchScore >= 26 && a.AIMatchScore < 51),
                    scored.Count(a => a.AIMatchScore >= 51 && a.AIMatchScore < 75),
                    scored.Count(a => a.AIMatchScore >= 75 && a.AIMatchScore < 90),
                    scored.Count(a => a.AIMatchScore >= 90)
                },
                BackgroundColors = new List<string>
                {
                    "rgba(239,68,68,0.8)",
                    "rgba(249,115,22,0.8)",
                    "rgba(234,179,8,0.8)",
                    "rgba(34,197,94,0.8)",
                    "rgba(16,185,129,0.8)"
                },
                DatasetLabel = "Candidates"
            };
        }

        private async Task<ChartDataSet> BuildJobPerformanceChartAsync(List<Job> jobs)
        {
            var labels = new List<string>();
            var values = new List<decimal>();

            foreach (var job in jobs)
            {
                labels.Add(job.Title.Length > 15
                    ? job.Title[..15] + "…" : job.Title);
                var count = await _context.Applications
                    .CountAsync(a => a.JobId == job.Id);
                values.Add(count);
            }

            return new ChartDataSet
            {
                Labels = labels,
                Values = values,
                DatasetLabel = "Applications"
            };
        }
    }
}