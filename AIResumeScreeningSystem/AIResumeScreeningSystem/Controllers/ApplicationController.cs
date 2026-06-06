using AIResumeScreeningSystem.Data;
using AIResumeScreeningSystem.Models;
using AIResumeScreeningSystem.Repositories.Interfaces;
using AIResumeScreeningSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AIResumeScreeningSystem.Controllers
{
    [Authorize]
    public class ApplicationController : Controller
    {
        private readonly INotificationService _notificationService;
        private readonly IApplicationRepository _applicationRepo;
        private readonly ICandidateRankingService _rankingService;
        private readonly ISkillMatchingService _skillMatchingService;
        private readonly IResumeService _resumeService;
        private readonly ICandidateService _candidateService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;
        private readonly ILogger<ApplicationController> _logger;

        public ApplicationController(
            IApplicationRepository applicationRepo,
            ICandidateRankingService rankingService,
            ISkillMatchingService skillMatchingService,
            IResumeService resumeService,
            ICandidateService candidateService,
            UserManager<ApplicationUser> userManager,
            AppDbContext context,
            ILogger<ApplicationController> logger,
             INotificationService notificationService)
        {
            _applicationRepo = applicationRepo;
            _rankingService = rankingService;
            _skillMatchingService = skillMatchingService;
            _resumeService = resumeService;
            _candidateService = candidateService;
            _userManager = userManager;
            _context = context;
            _logger = logger;
            _notificationService = notificationService;
        }

        // ─── Recruiter / Admin: Applications for a Job ─────────────────────
        [Authorize(Policy = "AdminOrRecruiter")]
        public async Task<IActionResult> Index(
            int jobId,
            string? statusFilter = null,
            decimal? minScore = null)
        {
            var rankingVm = await _rankingService.GetRankedCandidatesAsync(
                jobId, statusFilter, minScore);

            ViewBag.JobId = jobId;
            ViewBag.StatusFilter = statusFilter;
            ViewBag.MinScore = minScore;

            // Recalculate scores on demand if any are null
            bool hasUnscored = rankingVm.RankedCandidates.Any(r => r.OverallMatchScore == 0);
            if (hasUnscored)
            {
                _ = Task.Run(() =>
                    _skillMatchingService.RecalculateAllApplicationScoresAsync(jobId));
            }

            return View(rankingVm);
        }

        // ─── Ranking Dashboard ─────────────────────────────────────────────
        [Authorize(Policy = "AdminOrRecruiter")]
        public async Task<IActionResult> Ranking(int jobId)
        {
            var rankingVm = await _rankingService.GetRankedCandidatesAsync(jobId);
            return View(rankingVm);
        }

        // ─── Application Details ───────────────────────────────────────────
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var matchResult = await _skillMatchingService.GetApplicationMatchResultAsync(id);
            if (matchResult == null || matchResult.ApplicationId == 0)
            {
                TempData["ErrorMessage"] = "Application not found.";
                return RedirectToAction(nameof(MyApplications));
            }

            // Security: candidates can only see their own applications
            if (User.IsInRole("Candidate"))
            {
                var user = await _userManager.GetUserAsync(User);
                var candidateId = await _candidateService.GetCandidateIdByUserIdAsync(user!.Id);
                if (matchResult.CandidateId != candidateId)
                    return Forbid();
            }

            return View(matchResult);
        }

        // ─── Candidate: Apply for a Job ────────────────────────────────────
        [Authorize(Policy = "CandidateOnly")]
        public async Task<IActionResult> Apply(int jobId)
        {
            var user = await _userManager.GetUserAsync(User);
            var candidateId = await _candidateService.GetCandidateIdByUserIdAsync(user!.Id);

            if (candidateId == null)
            {
                TempData["ErrorMessage"] = "Candidate profile not found.";
                return RedirectToAction("Dashboard", "Candidate");
            }

            // Check already applied
            bool alreadyApplied = await _applicationRepo
                .HasAlreadyAppliedAsync(jobId, candidateId.Value);
            if (alreadyApplied)
            {
                TempData["ErrorMessage"] = "You have already applied for this job.";
                return RedirectToAction("Details", "Job", new { id = jobId });
            }

            // Check job is active
            var job = await _context.Jobs
                .Include(j => j.JobSkills).ThenInclude(js => js.Skill)
                .FirstOrDefaultAsync(j => j.Id == jobId);

            if (job == null || job.Status != JobStatus.Active)
            {
                TempData["ErrorMessage"] = "This job is no longer accepting applications.";
                return RedirectToAction("Browse", "Job");
            }

            // Get candidate's active resume
            var activeResumeId = await _resumeService.GetActiveResumeIdAsync(candidateId.Value);
            var resumes = await _resumeService.GetCandidateResumesAsync(candidateId.Value);

            ViewBag.Job = job;
            ViewBag.ActiveResumeId = activeResumeId;
            ViewBag.Resumes = resumes.Resumes;
            ViewBag.CandidateId = candidateId.Value;

            // Quick match score preview
            var quickScore = await _skillMatchingService
                .GetQuickMatchScoreAsync(jobId, candidateId.Value);
            ViewBag.QuickMatchScore = quickScore;

            return View();
        }

        [HttpPost]
        [Authorize(Policy = "CandidateOnly")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(int jobId, int resumeId)
        {
            var user = await _userManager.GetUserAsync(User);
            var candidateId = await _candidateService.GetCandidateIdByUserIdAsync(user!.Id);

            if (candidateId == null)
                return RedirectToAction("Dashboard", "Candidate");

            // Validate resume belongs to candidate
            var resume = await _context.Resumes
                .FirstOrDefaultAsync(r => r.Id == resumeId &&
                                          r.CandidateId == candidateId.Value);
            if (resume == null)
            {
                TempData["ErrorMessage"] = "Invalid resume selected.";
                return RedirectToAction(nameof(Apply), new { jobId });
            }

            // Duplicate check
            bool alreadyApplied = await _applicationRepo
                .HasAlreadyAppliedAsync(jobId, candidateId.Value);
            if (alreadyApplied)
            {
                TempData["ErrorMessage"] = "You have already applied for this job.";
                return RedirectToAction("Details", "Job", new { id = jobId });
            }

            // Create application
            var application = new Application
            {
                JobId = jobId,
                CandidateId = candidateId.Value,
                ResumeId = resumeId,
                Status = ApplicationStatus.Submitted,
                AppliedAt = DateTime.UtcNow
            };

            await _applicationRepo.AddAsync(application);
            await _applicationRepo.SaveChangesAsync();
            // Send notification
            _ = Task.Run(() =>
                _notificationService.NotifyApplicationSubmittedAsync(application.Id));
            // Calculate AI match score in background
            _ = Task.Run(async () =>
            {
                try
                {
                    var match = await _skillMatchingService
                        .CalculateMatchForApplicationAsync(application.Id);

                    await _applicationRepo.UpdateApplicationScoresAsync(
                        application.Id,
                        match.OverallMatchScore,
                        match.RequiredSkillScore,
                        0,
                        null,
                        null,
                        string.Join(", ", match.MissingRequiredSkills));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error calculating AI score for Application {AppId}", application.Id);
                }
            });

            TempData["SuccessMessage"] =
                "Application submitted! AI is calculating your match score.";
            return RedirectToAction(nameof(Details), new { id = application.Id });
        }

        // ─── Candidate: My Applications ────────────────────────────────────
        [Authorize(Policy = "CandidateOnly")]
        public async Task<IActionResult> MyApplications()
        {
            var user = await _userManager.GetUserAsync(User);
            var candidateId = await _candidateService.GetCandidateIdByUserIdAsync(user!.Id);

            if (candidateId == null)
                return RedirectToAction("Dashboard", "Candidate");

            var applications = await _applicationRepo
                .GetApplicationsByCandidateAsync(candidateId.Value);

            var viewModels = applications.Select(a => new
            {
                a.Id,
                JobTitle = a.Job?.Title ?? "Unknown",
                Company = a.Job?.Company ?? "Unknown",
                a.Status,
                a.AIMatchScore,
                a.SkillMatchPercentage,
                a.AppliedAt,
                a.UpdatedAt
            }).ToList();

            ViewBag.Applications = viewModels;
            return View();
        }

        // ─── Recruiter: Shortlisted ────────────────────────────────────────
        [Authorize(Policy = "AdminOrRecruiter")]
        public async Task<IActionResult> Shortlisted(int? jobId = null)
        {
            var user = await _userManager.GetUserAsync(User);

            IQueryable<Application> query = _context.Applications
                .Include(a => a.Job)
                .Include(a => a.Candidate).ThenInclude(c => c.User)
                .Where(a => a.Status == ApplicationStatus.Shortlisted);

            if (!User.IsInRole("Admin"))
                query = query.Where(a => a.Job.PostedByUserId == user!.Id);

            if (jobId.HasValue)
                query = query.Where(a => a.JobId == jobId.Value);

            var applications = await query
                .OrderByDescending(a => a.AIMatchScore)
                .ToListAsync();

            ViewBag.JobId = jobId;
            return View(applications);
        }

        // ─── Update Application Status (Recruiter) ─────────────────────────
        [HttpPost]
        [Authorize(Policy = "AdminOrRecruiter")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(
            int applicationId,
            string status,
            string? notes = null,
            int? jobId = null)
        {
            var user = await _userManager.GetUserAsync(User);
            var (success, error) = await _rankingService.UpdateApplicationStatusAsync(
                applicationId, status, user!.Id, notes);

            if (!success)
            {
                TempData["ErrorMessage"] = error;
            }
            else
            {
                TempData["SuccessMessage"] =
                    $"Application status updated to {status}.";

                if (status == "Shortlisted")
                {
                    _ = Task.Run(() =>
                        _notificationService.NotifyCandidateShortlistedAsync(applicationId));
                }
                else if (status == "Rejected")
                {
                    _ = Task.Run(() =>
                        _notificationService.NotifyCandidateRejectedAsync(applicationId));
                }
                else
                {
                    _ = Task.Run(() =>
                        _notificationService.NotifyApplicationStatusChangedAsync(
                            applicationId,
                            status));
                }
            }

            if (jobId.HasValue)
                return RedirectToAction(nameof(Index), new { jobId });

            return RedirectToAction(nameof(Details), new { id = applicationId });
        }

        // ─── Recalculate Scores ────────────────────────────────────────────
        [HttpPost]
        [Authorize(Policy = "AdminOrRecruiter")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecalculateScores(int jobId)
        {
            await _rankingService.RankAllApplicationsAsync(jobId);
            TempData["SuccessMessage"] = "AI match scores recalculated and candidates re-ranked.";
            return RedirectToAction(nameof(Ranking), new { jobId });
        }

        // ─── Bulk Shortlist ────────────────────────────────────────────────
        [HttpPost]
        [Authorize(Policy = "AdminOrRecruiter")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkShortlist(int jobId, int topN = 5)
        {
            var user = await _userManager.GetUserAsync(User);
            var (success, error) = await _rankingService
                .BulkShortlistTopCandidatesAsync(jobId, topN, user!.Id);

            if (!success)
                TempData["ErrorMessage"] = error;
            else
                TempData["SuccessMessage"] = $"Top {topN} candidates shortlisted successfully.";

            return RedirectToAction(nameof(Ranking), new { jobId });
        }

        // ─── Skill Gap View ────────────────────────────────────────────────
        [Authorize]
        public async Task<IActionResult> SkillGap(int id)
        {
            var gapVm = await _skillMatchingService.GetSkillGapAsync(id);
            return View(gapVm);
        }
    }
}