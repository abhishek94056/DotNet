using AIResumeScreeningSystem.Data;
using AIResumeScreeningSystem.Models;
using AIResumeScreeningSystem.Services.Interfaces;
using AIResumeScreeningSystem.ViewModels.Job;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AIResumeScreeningSystem.Controllers
{
    public class JobController : Controller
    {
        private readonly IJobService _jobService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;
        private readonly ILogger<JobController> _logger;

        public JobController(
            IJobService jobService,
            UserManager<ApplicationUser> userManager,
            AppDbContext context,
            ILogger<JobController> logger)
        {
            _jobService = jobService;
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        // ─── Index: Recruiter / Admin job list ─────────────────────────────
        [Authorize(Policy = "AdminOrRecruiter")]
        public async Task<IActionResult> Index(JobSearchViewModel? search)
        {
            search ??= new JobSearchViewModel();
            var user = await _userManager.GetUserAsync(User);
            string? filterByUser = User.IsInRole("Admin") ? null : user!.Id;

            var viewModel = await _jobService.GetJobsAsync(search, filterByUser);
            ViewData["ActivePage"] = "MyJobs";
            return View(viewModel);
        }

        // ─── Browse: Public job board for Candidates ───────────────────────
        [AllowAnonymous]
        public async Task<IActionResult> Browse(JobSearchViewModel? search)
        {
            search ??= new JobSearchViewModel();
            var viewModel = await _jobService.GetPublicJobsAsync(search);
            return View(viewModel);
        }

        // ─── Details ───────────────────────────────────────────────────────
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var job = await _jobService.GetJobByIdAsync(id);
            if (job == null)
            {
                TempData["ErrorMessage"] = "Job not found.";
                return RedirectToAction(nameof(Browse));
            }

            // Check if current candidate already applied
            if (User.Identity?.IsAuthenticated == true && User.IsInRole("Candidate"))
            {
                var user = await _userManager.GetUserAsync(User);
                var candidate = await _context.Candidates.FirstOrDefaultAsync(c => c.UserId == user!.Id);
                if (candidate != null)
                    ViewBag.AlreadyApplied = await _context.Applications
                        .AnyAsync(a => a.JobId == id && a.CandidateId == candidate.Id);
            }

            return View(job);
        }

        // ─── Create ────────────────────────────────────────────────────────
        [Authorize(Policy = "AdminOrRecruiter")]
        public async Task<IActionResult> Create()
        {
            var skills = await _context.Skills.OrderBy(s => s.Name).ToListAsync();
            var model = new CreateJobViewModel
            {
                AvailableSkills = skills.Select(s =>
                    new SelectListItem(s.Name, s.Id.ToString())).ToList()
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOrRecruiter")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateJobViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var skills = await _context.Skills.OrderBy(s => s.Name).ToListAsync();
                model.AvailableSkills = skills.Select(s =>
                    new SelectListItem(s.Name, s.Id.ToString())).ToList();
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            var (success, jobId, error) = await _jobService.CreateJobAsync(model, user!.Id);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, error);
                var skills = await _context.Skills.OrderBy(s => s.Name).ToListAsync();
                model.AvailableSkills = skills.Select(s =>
                    new SelectListItem(s.Name, s.Id.ToString())).ToList();
                return View(model);
            }

            TempData["SuccessMessage"] = "Job posted successfully!";
            return RedirectToAction(nameof(Details), new { id = jobId });
        }

        // ─── Edit ──────────────────────────────────────────────────────────
        [Authorize(Policy = "AdminOrRecruiter")]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _jobService.GetJobForEditAsync(id);
            if (model == null)
            {
                TempData["ErrorMessage"] = "Job not found.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.GetUserAsync(User);
            if (!User.IsInRole("Admin") && !await _jobService.IsJobOwnedByUserAsync(id, user!.Id))
            {
                TempData["ErrorMessage"] = "You are not authorized to edit this job.";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOrRecruiter")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditJobViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var skills = await _context.Skills.OrderBy(s => s.Name).ToListAsync();
                model.AvailableSkills = skills.Select(s =>
                    new SelectListItem(s.Name, s.Id.ToString())).ToList();
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            var (success, error) = await _jobService.UpdateJobAsync(model, user!.Id);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, error);
                var skills = await _context.Skills.OrderBy(s => s.Name).ToListAsync();
                model.AvailableSkills = skills.Select(s =>
                    new SelectListItem(s.Name, s.Id.ToString())).ToList();
                return View(model);
            }

            TempData["SuccessMessage"] = "Job updated successfully!";
            return RedirectToAction(nameof(Details), new { id = model.Id });
        }

        // ─── Delete ────────────────────────────────────────────────────────
        [Authorize(Policy = "AdminOrRecruiter")]
        public async Task<IActionResult> Delete(int id)
        {
            var job = await _jobService.GetJobByIdAsync(id);
            if (job == null)
            {
                TempData["ErrorMessage"] = "Job not found.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.GetUserAsync(User);
            if (!User.IsInRole("Admin") && !await _jobService.IsJobOwnedByUserAsync(id, user!.Id))
            {
                TempData["ErrorMessage"] = "Unauthorized.";
                return RedirectToAction(nameof(Index));
            }

            return View(job);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Policy = "AdminOrRecruiter")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = User.IsInRole("Admin") ? string.Empty : user!.Id;

            // Admin can delete any job — pass a known owner check bypass
            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
            {
                TempData["ErrorMessage"] = "Job not found.";
                return RedirectToAction(nameof(Index));
            }

            var (success, error) = await _jobService.DeleteJobAsync(id,
                User.IsInRole("Admin") ? job.PostedByUserId : user!.Id);

            if (!success)
            {
                TempData["ErrorMessage"] = error;
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "Job deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        // ─── Toggle Status (AJAX-friendly) ─────────────────────────────────
        [HttpPost]
        [Authorize(Policy = "AdminOrRecruiter")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var job = await _context.Jobs.FindAsync(id);
            if (job == null) return NotFound();

            var (success, error) = await _jobService.ToggleJobStatusAsync(id,
                User.IsInRole("Admin") ? job.PostedByUserId : user!.Id);

            if (!success)
                TempData["ErrorMessage"] = error;
            else
                TempData["SuccessMessage"] = "Job status updated.";

            return RedirectToAction(nameof(Index));
        }
    }
}