// Controllers/AuthController.cs
using InvoiceGenerator.Filters;
using InvoiceGenerator.Helper;
using InvoiceGenerator.Interfaces;
using InvoiceGenerator.Models;
using InvoiceGenerator.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceGenerator.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth)
        {
            _auth = auth;
        }

        // ── GET /Auth/Login ──
        [HttpGet]
        public IActionResult Login()
        {
            if (SessionHelper.IsLoggedIn(HttpContext.Session))
                return RedirectToAction("Dashboard");

            LoginViewModel vm = new LoginViewModel();

            if (Request.Cookies.ContainsKey("RememberEmail"))
            {
                vm.Email = Request.Cookies["RememberEmail"];
                vm.RememberMe = true;
            }

            return View(vm);
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

            // Block Operator at login
            if (user!.Role == "Operator")
            {
                ModelState.AddModelError(string.Empty,
                    "Operator accounts do not have system access.");
                return View(vm);
            }

            SessionHelper.SetUser(HttpContext.Session, user!);

            if (vm.RememberMe)
            {
                CookieOptions options = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(30),
                    HttpOnly = true,
                    IsEssential = true
                };

                Response.Cookies.Append("RememberEmail", vm.Email, options);
            }
            else
            {
                Response.Cookies.Delete("RememberEmail");
            }
            return RedirectToAction("Dashboard");
        }
        [HttpGet]
        public IActionResult GetUser(int userId)
        {
            try
            {
                var user = _auth.GetUserById(userId);
                if (user == null)
                    return Json(new { success = false, message = "User not found" });

                return Json(new
                {
                    success = true,
                    user = new
                    {
                        user.UserId,
                        user.Name,
                        user.Email,
                        user.Department,
                        user.Designation,
                        user.Role,
                        user.IsActive
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message   // 👈 IMPORTANT for debugging
                });
            }
        }

        [RequireAdmin]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditUser(UserModel user)
        {
            var (success, message) = _auth.UpdateUser(user); // ✅ get result

            return Json(new
            {
                success,
                message
            });
        }
        // ── GET /Auth/Dashboard ──
        [RequireLogin]
        public IActionResult Dashboard()
        {
            ViewBag.UserName = SessionHelper.GetUserName(HttpContext.Session);
            ViewBag.UserRole = SessionHelper.GetUserRole(HttpContext.Session);
            ViewBag.IsAdmin = SessionHelper.IsAdmin(HttpContext.Session);
            ViewBag.HasInvoice = SessionHelper.HasInvoiceAccess(HttpContext.Session);
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
        [ValidateAntiForgeryToken]
        public IActionResult ToggleActive(int userId, bool isActive)
        {
            _auth.ToggleActive(userId, isActive);
            return Json(new { success = true });
        }



        // ── POST /Auth/RegisterAjax (modal from User Master) ──
        [RequireAdmin]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RegisterAjax(RegisterViewModel vm)
        {
            if (!ModelState.IsValid)
                return Json(new
                {
                    success = false,
                    message = string.Join(" | ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage))
                });

            var (success, message) = _auth.Register(vm);
            return Json(new { success, message });
        }
    }
}