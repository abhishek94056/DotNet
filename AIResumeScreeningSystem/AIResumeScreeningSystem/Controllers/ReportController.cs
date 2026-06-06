using AIResumeScreeningSystem.Models;
using AIResumeScreeningSystem.Services.Interfaces;
using AIResumeScreeningSystem.ViewModels.Report;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AIResumeScreeningSystem.Data;

namespace AIResumeScreeningSystem.Controllers
{
    [Authorize(Policy = "AdminOrRecruiter")]
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;
        private readonly ILogger<ReportController> _logger;

        public ReportController(
            IReportService reportService,
            UserManager<ApplicationUser> userManager,
            AppDbContext context,
            ILogger<ReportController> logger)
        {
            _reportService = reportService;
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        // ─── Report List ───────────────────────────────────────────────────
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var vm = await _reportService.GetReportsAsync(user!.Id);
            return View(vm);
        }

        // ─── Generate Form ─────────────────────────────────────────────────
        public async Task<IActionResult> Generate()
        {
            var jobs = await _context.Jobs
                .Where(j => j.Status == JobStatus.Active ||
                            j.Status == JobStatus.Closed)
                .OrderByDescending(j => j.PostedDate)
                .ToListAsync();

            var model = new GenerateReportViewModel
            {
                AvailableJobs = jobs
                    .Select(j => new SelectListItem(
                        $"{j.Title} — {j.Company}", j.Id.ToString()))
                    .ToList()
            };

            return View(model);
        }

        // ─── Generate POST ─────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Generate(GenerateReportViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var jobs = await _context.Jobs
                    .OrderByDescending(j => j.PostedDate).ToListAsync();
                model.AvailableJobs = jobs
                    .Select(j => new SelectListItem(
                        $"{j.Title} — {j.Company}", j.Id.ToString()))
                    .ToList();
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            var (success, filePath, fileName, error) =
                await _reportService.GenerateReportAsync(model, user!.Id);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, error);
                return View(model);
            }

            TempData["SuccessMessage"] =
                $"Report generated: {fileName}. You can download it below.";
            return RedirectToAction(nameof(Index));
        }

        // ─── Download ──────────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Download(int id)
        {
            var report = await _context.Reports.FindAsync(id);
            if (report == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (report.GeneratedByUserId != user!.Id && !User.IsInRole("Admin"))
                return Forbid();

            if (string.IsNullOrEmpty(report.FilePath))
                return NotFound("Report file path missing.");

            var bytes = _reportService.GetReportBytes(report.FilePath);
            if (bytes == null)
                return NotFound("Report file not found on server.");

            var contentType = _reportService.GetContentType(report.FilePath);
            var ext = Path.GetExtension(report.FilePath);
            var downloadName = $"{report.Name.Replace(" ", "_")}{ext}";

            return File(bytes, contentType, downloadName);
        }

        // ─── Delete ────────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var (success, error) = await _reportService.DeleteReportAsync(id, user!.Id);

            if (!success)
                TempData["ErrorMessage"] = error;
            else
                TempData["SuccessMessage"] = "Report deleted.";

            return RedirectToAction(nameof(Index));
        }
    }
}