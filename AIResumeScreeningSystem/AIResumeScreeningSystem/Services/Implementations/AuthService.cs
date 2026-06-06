using AIResumeScreeningSystem.Data;
using AIResumeScreeningSystem.Models;
using AIResumeScreeningSystem.Services.Interfaces;
using AIResumeScreeningSystem.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AIResumeScreeningSystem.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<AuthService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            AppDbContext context,
            IEmailService emailService,
            ILogger<AuthService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
            _emailService = emailService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<(bool Success, IEnumerable<string> Errors)> RegisterAsync(RegisterViewModel model)
        {
            try
            {
                // Check if email already exists
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                    return (false, new[] { "An account with this email already exists." });

                // Validate role
                var allowedRoles = new[] { "Recruiter", "Candidate" };
                if (!allowedRoles.Contains(model.Role))
                    return (false, new[] { "Invalid role selected." });

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    IsActive = true,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                    return (false, result.Errors.Select(e => e.Description));

                // Assign role
                await _userManager.AddToRoleAsync(user, model.Role);

                // Create candidate profile automatically
                if (model.Role == "Candidate")
                {
                    var candidate = new Candidate
                    {
                        UserId = user.Id,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _context.Candidates.AddAsync(candidate);
                    await _context.SaveChangesAsync();
                }

                // Send welcome email (non-blocking)
                _ = Task.Run(() => _emailService.SendWelcomeEmailAsync(user.Email!, user.FirstName, model.Role));

                _logger.LogInformation("New user registered: {Email} as {Role}", model.Email, model.Role);
                return (true, Enumerable.Empty<string>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for {Email}", model.Email);
                return (false, new[] { "An unexpected error occurred. Please try again." });
            }
        }

        public async Task<(bool Success, string Error)> LoginAsync(LoginViewModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                    return (false, "Invalid email or password.");

                if (!user.IsActive)
                    return (false, "Your account has been deactivated. Please contact support.");

                var result = await _signInManager.PasswordSignInAsync(
                    user, model.Password, model.RememberMe, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    // Update last login
                    user.UpdatedAt = DateTime.UtcNow;
                    await _userManager.UpdateAsync(user);

                    _logger.LogInformation("User logged in: {Email}", model.Email);
                    return (true, string.Empty);
                }

                if (result.IsLockedOut)
                    return (false, "Account locked due to multiple failed attempts. Try again in 15 minutes.");

                return (false, "Invalid email or password.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for {Email}", model.Email);
                return (false, "An unexpected error occurred. Please try again.");
            }
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
        }

        public async Task<(bool Success, string Error)> ForgotPasswordAsync(ForgotPasswordViewModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !user.IsActive)
                {
                    // Return success regardless — don't reveal if email exists
                    return (true, string.Empty);
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var request = _httpContextAccessor.HttpContext?.Request;
                var baseUrl = $"{request?.Scheme}://{request?.Host}";
                var encodedToken = Uri.EscapeDataString(token);
                var resetLink = $"{baseUrl}/Account/ResetPassword?token={encodedToken}&email={Uri.EscapeDataString(model.Email)}";

                _ = Task.Run(() => _emailService.SendPasswordResetEmailAsync(user.Email!, user.FirstName, resetLink));

                _logger.LogInformation("Password reset requested for: {Email}", model.Email);
                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ForgotPassword for {Email}", model.Email);
                return (false, "An unexpected error occurred. Please try again.");
            }
        }

        public async Task<(bool Success, IEnumerable<string> Errors)> ResetPasswordAsync(ResetPasswordViewModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                    return (false, new[] { "Invalid request." });

                var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
                if (!result.Succeeded)
                    return (false, result.Errors.Select(e => e.Description));

                _logger.LogInformation("Password reset successful for: {Email}", model.Email);
                return (true, Enumerable.Empty<string>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ResetPassword for {Email}", model.Email);
                return (false, new[] { "An unexpected error occurred. Please try again." });
            }
        }

        public async Task<string> GetUserRoleAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return string.Empty;
            var roles = await _userManager.GetRolesAsync(user);
            return roles.FirstOrDefault() ?? string.Empty;
        }

        public async Task<string> GetDashboardUrlByRoleAsync(string userId)
        {
            var role = await GetUserRoleAsync(userId);
            return role switch
            {
                "Admin" => "/Admin/Dashboard",
                "Recruiter" => "/Recruiter/Dashboard",
                "Candidate" => "/Candidate/Dashboard",
                _ => "/"
            };
        }
    }
}