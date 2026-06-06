using AIResumeScreeningSystem.Models;
using AIResumeScreeningSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AIResumeScreeningSystem.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class AdminController : Controller
    {
        private readonly IDashboardService _dashboardService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            IDashboardService dashboardService,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<AdminController> logger)
        {
            _dashboardService = dashboardService;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        // ─── Admin Dashboard ───────────────────────────────────────────────
        public async Task<IActionResult> Dashboard()
        {
            ViewData["ActivePage"] = "AdminDashboard";
            var vm = await _dashboardService.GetAdminDashboardAsync();
            return View(vm);
        }

        // ─── Users List ────────────────────────────────────────────────────
        public async Task<IActionResult> Users(
            string? role = null, string? keyword = null, int page = 1)
        {
            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                var kw = keyword.ToLower();
                query = query.Where(u =>
                    u.FirstName.ToLower().Contains(kw) ||
                    u.LastName.ToLower().Contains(kw) ||
                    (u.Email != null && u.Email.ToLower().Contains(kw)));
            }

            var users = await query
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();

            // Filter by role
            var userWithRoles = new List<(ApplicationUser User, List<string> Roles)>();
            foreach (var user in users)
            {
                var roles = (await _userManager.GetRolesAsync(user)).ToList();
                if (string.IsNullOrEmpty(role) || roles.Contains(role))
                    userWithRoles.Add((user, roles));
            }

            int pageSize = 20;
            var paged = userWithRoles
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.Users = paged;
            ViewBag.Total = userWithRoles.Count;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(userWithRoles.Count / (double)pageSize);
            ViewBag.RoleFilter = role;
            ViewBag.Keyword = keyword;
            ViewData["ActivePage"] = "Users";
            return View();
        }

        // ─── Toggle User Active ────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUserActive(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction(nameof(Users));
            }

            user.IsActive = !user.IsActive;
            user.UpdatedAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            TempData["SuccessMessage"] =
                $"User {user.Email} has been {(user.IsActive ? "activated" : "deactivated")}.";
            return RedirectToAction(nameof(Users));
        }

        // ─── Change User Role ──────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUserRole(string userId, string newRole)
        {
            var allowedRoles = new[] { "Admin", "Recruiter", "Candidate" };
            if (!allowedRoles.Contains(newRole))
            {
                TempData["ErrorMessage"] = "Invalid role.";
                return RedirectToAction(nameof(Users));
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction(nameof(Users));
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, newRole);

            TempData["SuccessMessage"] =
                $"Role updated to {newRole} for {user.Email}.";
            return RedirectToAction(nameof(Users));
        }

        // ─── Roles Page ────────────────────────────────────────────────────
        public async Task<IActionResult> Roles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = roles;
            ViewData["ActivePage"] = "Roles";
            return View();
        }
    }
}