using AIResumeScreeningSystem.Models;
using AIResumeScreeningSystem.Services.Interfaces;
using AIResumeScreeningSystem.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AIResumeScreeningSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            IAuthService authService,
            UserManager<ApplicationUser> userManager,
            ILogger<AccountController> logger)
        {
            _authService = authService;
            _userManager = userManager;
            _logger = logger;
        }

        // ─── Register ──────────────────────────────────────────────────────
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            return View(new RegisterViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var (success, errors) = await _authService.RegisterAsync(model);
            if (!success)
            {
                foreach (var error in errors)
                    ModelState.AddModelError(string.Empty, error);
                return View(model);
            }

            TempData["SuccessMessage"] = "Registration successful! Please log in.";
            return RedirectToAction(nameof(Login));
        }

        // ─── Login ─────────────────────────────────────────────────────────
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var (success, error) = await _authService.LoginAsync(model);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, error);
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            var dashboardUrl = await _authService.GetDashboardUrlByRoleAsync(user!.Id);

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                return Redirect(model.ReturnUrl);

            return Redirect(dashboardUrl);
        }

        // ─── Logout ────────────────────────────────────────────────────────
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            return RedirectToAction(nameof(Login));
        }

        // ─── Forgot
        // ───────────────────────────────────────────────
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
            => View(new ForgotPasswordViewModel());

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _authService.ForgotPasswordAsync(model);
            TempData["SuccessMessage"] = "If an account with that email exists, a reset link has been sent.";
            return RedirectToAction(nameof(Login));
        }

        // ─── Reset Password ────────────────────────────────────────────────
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token, string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
                return RedirectToAction(nameof(Login));

            return View(new ResetPasswordViewModel
            {
                Token = token,
                Email = email
            });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var (success, errors) = await _authService.ResetPasswordAsync(model);
            if (!success)
            {
                foreach (var error in errors)
                    ModelState.AddModelError(string.Empty, error);
                return View(model);
            }

            TempData["SuccessMessage"] = "Password reset successfully. Please log in.";
            return RedirectToAction(nameof(Login));
        }

        // ─── Access Denied ─────────────────────────────────────────────────
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
            => View();
    }
}