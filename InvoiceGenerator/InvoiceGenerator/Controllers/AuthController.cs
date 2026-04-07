// Controllers/AuthController.cs
using InvoiceGenerator.Filters;
using InvoiceGenerator.Helper;
using InvoiceGenerator.ViewModels;
using InvoiceGenerator.Interfaces;
using Microsoft.AspNetCore.Mvc;//Controller functionality

namespace InvoiceGenerator.Controllers
{
    public class AuthController : Controller //(C) Controller -> Base class for MVC controllers
    {
        //Dependency Injection (Service)
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth) => _auth = auth;

        // ── GET /Auth/Register ──
        [HttpGet]
        public IActionResult Register() //(I) IActionResult -> Represents action result
        {
            if (SessionHelper.IsLoggedIn(HttpContext.Session))
                return RedirectToAction("Dashboard");
            return View();  //(M) View()	-> Returns UI view
        }

        // ── POST /Auth/Register ──
        [HttpPost]  // form submit
        [ValidateAntiForgeryToken] // security (CSRF protection)
        public IActionResult Register(RegisterViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var (success, message) = _auth.Register(vm);
            if (!success)
            {
                ModelState.AddModelError("Email", message);
                return View(vm);
            }

            TempData["Success"] = "Registration successful! Please login.";
            return RedirectToAction("Login");
        }

        // ── GET /Auth/Login ──
        [HttpGet]
        public IActionResult Login()
        {
            if (SessionHelper.IsLoggedIn(HttpContext.Session))
                return RedirectToAction("Dashboard");
            return View();
        }

        // ── POST /Auth/Login ──
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var (success, user, message) = _auth.Login(vm);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, message);
                return View(vm);
            }

            SessionHelper.SetUser(HttpContext.Session, user!);
            return RedirectToAction("Dashboard");
        }

        // ── GET /Auth/Dashboard ──
        [RequireLogin]
        public IActionResult Dashboard()
        {
            ViewBag.UserName = SessionHelper.GetUserName(HttpContext.Session);
            ViewBag.UserRole = SessionHelper.GetUserRole(HttpContext.Session);
            return View();
        }

        // ── GET /Auth/Logout ──
        public IActionResult Logout()
        {
            SessionHelper.Clear(HttpContext.Session);
            return RedirectToAction("Login");
        }

        // ── GET /Auth/AccessDenied ──
        public IActionResult AccessDenied()
            => View();

        // ── GET /Auth/Users (Admin only) ──
        [RequireAdmin]
        public IActionResult Users()
        {
            var list = _auth.GetAllUsers();
            return View(list);
        }

        // ── POST /Auth/ToggleActive ──
        [RequireAdmin]
        [HttpPost]
        public IActionResult ToggleActive(int userId, bool isActive)
        {
            _auth.ToggleActive(userId, isActive);
            return Json(new { success = true });
        }
    }
}