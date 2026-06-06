using AIResumeScreeningSystem.Models;
using AIResumeScreeningSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AIResumeScreeningSystem.Controllers
{
    [Authorize(Policy = "RecruiterOnly")]
    public class RecruiterController : Controller
    {
        private readonly IDashboardService _dashboardService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RecruiterController> _logger;

        public RecruiterController(
            IDashboardService dashboardService,
            UserManager<ApplicationUser> userManager,
            ILogger<RecruiterController> logger)
        {
            _dashboardService = dashboardService;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Dashboard()
        {
            ViewData["ActivePage"] = "RecruiterDashboard";
            var user = await _userManager.GetUserAsync(User);
            var vm = await _dashboardService.GetRecruiterDashboardAsync(user!.Id);
            return View(vm);
        }
    }
}