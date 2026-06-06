using AIResumeScreeningSystem.Models;
using AIResumeScreeningSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AIResumeScreeningSystem.Controllers
{
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotificationController(
            INotificationService notificationService,
            UserManager<ApplicationUser> userManager)
        {
            _notificationService = notificationService;
            _userManager = userManager;
        }

        // ─── Notification Centre ───────────────────────────────────────────
        public async Task<IActionResult> Index(bool unreadOnly = false)
        {
            var user = await _userManager.GetUserAsync(User);
            var vm = await _notificationService
                .GetUserNotificationsAsync(user!.Id, unreadOnly);
            return View(vm);
        }

        // ─── Mark Single as Read ───────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkRead(int id)
        {
            await _notificationService.MarkAsReadAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // ─── Mark All as Read ──────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAllRead()
        {
            var user = await _userManager.GetUserAsync(User);
            await _notificationService.MarkAllAsReadAsync(user!.Id);
            TempData["SuccessMessage"] = "All notifications marked as read.";
            return RedirectToAction(nameof(Index));
        }

        // ─── API: Unread Count (for navbar badge) ─────────────────────────
        [HttpGet]
        public async Task<IActionResult> UnreadCount()
        {
            var user = await _userManager.GetUserAsync(User);
            var count = await _notificationService.GetUnreadCountAsync(user!.Id);
            return Ok(new { count });
        }
    }
}