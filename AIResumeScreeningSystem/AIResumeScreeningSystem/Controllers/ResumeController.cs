using AIResumeScreeningSystem.Models;
using AIResumeScreeningSystem.Services.Interfaces;
using AIResumeScreeningSystem.ViewModels.Resume;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AIResumeScreeningSystem.Controllers
{
    [Authorize]
    public class ResumeController : Controller
    {
        private readonly IResumeService _resumeService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ResumeController> _logger;

        public ResumeController(
            IResumeService resumeService,
            UserManager<ApplicationUser> userManager,
            ILogger<ResumeController> logger)
        {
            _resumeService = resumeService;
            _userManager = userManager;
            _logger = logger;
        }

        // ─── Upload Page (GET) ─────────────────────────────────────────────
        [Authorize(Policy = "CandidateOnly")]
        public async Task<IActionResult> Upload()
        {
            var user = await _userManager.GetUserAsync(User);
            var model = await _resumeService.GetUploadViewModelAsync(user!.Id);
            return View(model);
        }

        // ─── Upload (POST) ─────────────────────────────────────────────────
        [HttpPost]
        [Authorize(Policy = "CandidateOnly")]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(10 * 1024 * 1024)] // 10 MB limit
        public async Task<IActionResult> Upload(ResumeUploadViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var user0 = await _userManager.GetUserAsync(User);
                var vm = await _resumeService.GetUploadViewModelAsync(user0!.Id);
                vm.SetAsActive = model.SetAsActive;
                return View(vm);
            }

            var user = await _userManager.GetUserAsync(User);
            var (success, resumeId, error) = await _resumeService.UploadResumeAsync(user!.Id, model);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, error);
                var vm = await _resumeService.GetUploadViewModelAsync(user.Id);
                return View(vm);
            }

            TempData["SuccessMessage"] =
                "Resume uploaded successfully! AI parsing is running in the background.";
            return RedirectToAction(nameof(Details), new { id = resumeId });
        }

        // ─── Resume Details ────────────────────────────────────────────────
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var resume = await _resumeService.GetResumeByIdAsync(id);
            if (resume == null)
            {
                TempData["ErrorMessage"] = "Resume not found.";
                return RedirectToAction(nameof(Upload));
            }

            // Security: candidates can only see their own resumes
            if (User.IsInRole("Candidate"))
            {
                var user = await _userManager.GetUserAsync(User);
                var userResumes = await _resumeService.GetCandidateResumesByUserIdAsync(user!.Id);
                if (!userResumes.Resumes.Any(r => r.Id == id))
                {
                    TempData["ErrorMessage"] = "Access denied.";
                    return RedirectToAction(nameof(Upload));
                }
            }

            return View(resume);
        }

        // ─── Candidate Resume History ──────────────────────────────────────
        [Authorize(Policy = "CandidateOnly")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var model = await _resumeService.GetCandidateResumesByUserIdAsync(user!.Id);
            return View(model);
        }

        // ─── Recruiter: View Candidate Resumes ─────────────────────────────
        [Authorize(Policy = "AdminOrRecruiter")]
        public async Task<IActionResult> CandidateResumes(int candidateId)
        {
            var model = await _resumeService.GetCandidateResumesAsync(candidateId);
            return View(model);
        }

        // ─── Set Active Resume ─────────────────────────────────────────────
        [HttpPost]
        [Authorize(Policy = "CandidateOnly")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetActive(int resumeId)
        {
            var user = await _userManager.GetUserAsync(User);
            var (success, error) = await _resumeService.SetActiveResumeAsync(user!.Id, resumeId);

            if (!success)
                TempData["ErrorMessage"] = error;
            else
                TempData["SuccessMessage"] = "Active resume updated.";

            return RedirectToAction(nameof(Index));
        }

        // ─── Re-parse Resume ───────────────────────────────────────────────
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reparse(int resumeId)
        {
            var (success, error) = await _resumeService.ParseResumeAsync(resumeId);

            if (!success)
                TempData["ErrorMessage"] = $"Parsing failed: {error}";
            else
                TempData["SuccessMessage"] = "Resume re-parsed successfully!";

            return RedirectToAction(nameof(Details), new { id = resumeId });
        }

        // ─── Delete Resume ─────────────────────────────────────────────────
        [HttpPost]
        [Authorize(Policy = "CandidateOnly")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int resumeId)
        {
            var user = await _userManager.GetUserAsync(User);
            var (success, error) = await _resumeService.DeleteResumeAsync(user!.Id, resumeId);

            if (!success)
                TempData["ErrorMessage"] = error;
            else
                TempData["SuccessMessage"] = "Resume deleted.";

            return RedirectToAction(nameof(Index));
        }

        // ─── Download Resume ───────────────────────────────────────────────
        [Authorize]
        public async Task<IActionResult> Download(int id)
        {
            var resume = await _resumeService.GetResumeByIdAsync(id);
            if (resume == null) return NotFound();

            // Security check for candidates
            if (User.IsInRole("Candidate"))
            {
                var user = await _userManager.GetUserAsync(User);
                var userResumes = await _resumeService.GetCandidateResumesByUserIdAsync(user!.Id);
                if (!userResumes.Resumes.Any(r => r.Id == id))
                    return Forbid();
            }

            var env = HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>();
            var fullPath = Path.Combine(env.WebRootPath,
                resume.FilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

            if (!System.IO.File.Exists(fullPath))
                return NotFound("File not found on server.");

            var contentType = resume.FileExtension.ToLower() switch
            {
                ".pdf" => "application/pdf",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".doc" => "application/msword",
                _ => "application/octet-stream"
            };

            return PhysicalFile(fullPath, contentType, resume.FileName);
        }
    }
}