using AIResumeScreeningSystem.Data;
using AIResumeScreeningSystem.Models;
using AIResumeScreeningSystem.Services.Interfaces;
using AIResumeScreeningSystem.ViewModels.Candidate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AIResumeScreeningSystem.Controllers
{
    [Authorize]
    public class CandidateController : Controller
    {
        private readonly ICandidateService _candidateService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;
        private readonly ILogger<CandidateController> _logger;

        public CandidateController(
            ICandidateService candidateService,
            UserManager<ApplicationUser> userManager,
            AppDbContext context,
            ILogger<CandidateController> logger)
        {
            _candidateService = candidateService;
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        // ─── Dashboard (Candidate) ─────────────────────────────────────────
        [Authorize(Policy = "CandidateOnly")]
        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            var dashboard = await _candidateService.GetDashboardAsync(user!.Id);
            ViewData["ActivePage"] = "CandidateDashboard";
            return View(dashboard);
        }

        // ─── My Profile (Candidate) ────────────────────────────────────────
        [Authorize(Policy = "CandidateOnly")]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            var viewModel = await _candidateService.GetCandidateViewModelByUserIdAsync(user!.Id);
            if (viewModel == null)
            {
                TempData["ErrorMessage"] = "Profile not found. Please contact support.";
                return RedirectToAction("Dashboard");
            }
            return View(viewModel);
        }

        // ─── Edit Profile ──────────────────────────────────────────────────
        [Authorize(Policy = "CandidateOnly")]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            var model = await _candidateService.GetProfileForEditAsync(user!.Id);
            if (model == null)
            {
                TempData["ErrorMessage"] = "Profile not found.";
                return RedirectToAction("Dashboard");
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = "CandidateOnly")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(CandidateProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            var (success, error) = await _candidateService.UpdateProfileAsync(user!.Id, model);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, error);
                return View(model);
            }

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction(nameof(Profile));
        }

        // ─── Toggle Availability ───────────────────────────────────────────
        [HttpPost]
        [Authorize(Policy = "CandidateOnly")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleAvailability()
        {
            var user = await _userManager.GetUserAsync(User);
            await _candidateService.ToggleAvailabilityAsync(user!.Id);
            TempData["SuccessMessage"] = "Availability status updated.";
            return RedirectToAction(nameof(Profile));
        }

        // ─── Skills Management ─────────────────────────────────────────────
        [Authorize(Policy = "CandidateOnly")]
        public async Task<IActionResult> Skills()
        {
            var user = await _userManager.GetUserAsync(User);
            var candidateId = await _candidateService.GetCandidateIdByUserIdAsync(user!.Id);
            if (candidateId == null) return RedirectToAction("Dashboard");

            var skills = await _candidateService.GetCandidateSkillsAsync(candidateId.Value);

            var allSkills = await _context.Skills.OrderBy(s => s.Name).ToListAsync();
            var addModel = new AddSkillViewModel
            {
                AvailableSkills = allSkills.Select(s =>
                    new SelectListItem(s.Name, s.Id.ToString())).ToList()
            };

            ViewBag.AddSkillModel = addModel;
            ViewBag.CandidateId = candidateId;
            return View(skills);
        }

        [HttpPost]
        [Authorize(Policy = "CandidateOnly")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSkill(AddSkillViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            var (success, error) = await _candidateService.AddSkillAsync(user!.Id, model);

            if (!success)
                TempData["ErrorMessage"] = error;
            else
                TempData["SuccessMessage"] = "Skill added successfully!";

            return RedirectToAction(nameof(Skills));
        }

        [HttpPost]
        [Authorize(Policy = "CandidateOnly")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveSkill(int skillId)
        {
            var user = await _userManager.GetUserAsync(User);
            var (success, error) = await _candidateService.RemoveSkillAsync(user!.Id, skillId);

            if (!success)
                TempData["ErrorMessage"] = error;
            else
                TempData["SuccessMessage"] = "Skill removed.";

            return RedirectToAction(nameof(Skills));
        }

        [HttpPost]
        [Authorize(Policy = "CandidateOnly")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSkill(
            int candidateSkillId, ProficiencyLevel proficiencyLevel, int yearsOfExperience)
        {
            var user = await _userManager.GetUserAsync(User);
            await _candidateService.UpdateSkillAsync(
                user!.Id, candidateSkillId, proficiencyLevel, yearsOfExperience);

            TempData["SuccessMessage"] = "Skill updated.";
            return RedirectToAction(nameof(Skills));
        }

        // ─── Candidate List (Admin / Recruiter) ────────────────────────────
        [Authorize(Policy = "AdminOrRecruiter")]
        public async Task<IActionResult> Index(CandidateSearchViewModel? search)
        {
            search ??= new CandidateSearchViewModel();
            var viewModel = await _candidateService.GetCandidatesAsync(search);
            return View(viewModel);
        }

        // ─── Candidate Details (Admin / Recruiter) ─────────────────────────
        [Authorize(Policy = "AdminOrRecruiter")]
        public async Task<IActionResult> Details(int id)
        {
            var viewModel = await _candidateService.GetCandidateViewModelByIdAsync(id);
            if (viewModel == null)
            {
                TempData["ErrorMessage"] = "Candidate not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // ─── Toggle Active Status (Admin) ──────────────────────────────────
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction(nameof(Index));
            }

            user.IsActive = !user.IsActive;
            user.UpdatedAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            TempData["SuccessMessage"] = $"Candidate {(user.IsActive ? "activated" : "deactivated")}.";
            return RedirectToAction(nameof(Index));
        }
    }
}