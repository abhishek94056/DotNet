using Microsoft.AspNetCore.Identity;
using MauliFarm.Models;
using MauliFarm.Models.ViewModels;
using MauliFarm.Repositories.Interfaces;
using MauliFarm.Services.Interfaces;

namespace MauliFarm.Services
{
    /// <summary>
    /// Core authentication and user management service for Mauli Farm.
    /// Orchestrates repository calls, file uploads, activity logging,
    /// and Identity sign-in — keeping controllers clean.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository         _userRepo;
        private readonly IRoleRepository         _roleRepo;
        private readonly IActivityLogRepository  _activityLog;
        private readonly IFileUploadService      _fileUpload;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AuthService>    _logger;
        private readonly IWebHostEnvironment     _env;

        public AuthService(
            IUserRepository                userRepo,
            IRoleRepository                roleRepo,
            IActivityLogRepository         activityLog,
            IFileUploadService             fileUpload,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AuthService>           logger,
            IWebHostEnvironment            env)
        {
            _userRepo      = userRepo;
            _roleRepo      = roleRepo;
            _activityLog   = activityLog;
            _fileUpload    = fileUpload;
            _signInManager = signInManager;
            _logger        = logger;
            _env           = env;
        }

        // ═════════════════════════════════════════════════════════════════
        //  AUTHENTICATION
        // ═════════════════════════════════════════════════════════════════

        public async Task<ServiceResult> LoginAsync(LoginViewModel model, HttpContext httpContext)
        {
            var ipAddress = GetIpAddress(httpContext);
            var userAgent = httpContext.Request.Headers.UserAgent.ToString();

            // Find user by email OR username
            var user = await _userRepo.GetByEmailOrUserNameAsync(model.UsernameOrEmail);

            if (user == null)
            {
                _logger.LogWarning("Login failed — user not found: {Input}", model.UsernameOrEmail);
                return ServiceResult.Failure("Invalid username / email or password.");
            }

            if (!user.IsActive)
            {
                await _activityLog.LogAsync(user.Id, ActivityTypes.LoginFailed,
                    "Login attempt on deactivated account.", ipAddress, userAgent, false);
                return ServiceResult.Failure("Your account has been deactivated. Please contact the administrator.");
            }

            var result = await _signInManager.PasswordSignInAsync(
                user,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: true);

            if (result.Succeeded)
            {
                await _userRepo.UpdateLastLoginAsync(user.Id);
                await _activityLog.LogAsync(user.Id, ActivityTypes.Login,
                    $"Login successful from {ipAddress}", ipAddress, userAgent);

                _logger.LogInformation("User {UserName} logged in.", user.UserName);
                return ServiceResult.Success("Login successful. Welcome back!");
            }

            if (result.IsLockedOut)
            {
                await _activityLog.LogAsync(user.Id, ActivityTypes.LoginFailed,
                    "Account locked out.", ipAddress, userAgent, false);
                return ServiceResult.Failure(
                    "Your account has been temporarily locked due to multiple failed attempts. " +
                    "Please try again after 15 minutes.");
            }

            await _activityLog.LogAsync(user.Id, ActivityTypes.LoginFailed,
                "Invalid password.", ipAddress, userAgent, false);

            return ServiceResult.Failure("Invalid username / email or password.");
        }

        public async Task LogoutAsync(HttpContext httpContext)
        {
            var user = await GetCurrentUserAsync(httpContext);

            if (user != null)
            {
                var ip = GetIpAddress(httpContext);
                await _activityLog.LogAsync(user.Id, ActivityTypes.Logout,
                    $"User logged out from {ip}", ip);
            }

            await _signInManager.SignOutAsync();
        }

        // ═════════════════════════════════════════════════════════════════
        //  REGISTRATION / USER MANAGEMENT
        // ═════════════════════════════════════════════════════════════════

        public async Task<ServiceResult> RegisterUserAsync(
            RegisterViewModel model, string createdByUserId)
        {
            // ── Duplicate checks ──────────────────────────────────────────
            if (await _userRepo.EmailExistsAsync(model.Email))
                return ServiceResult.Failure("A user with this email address already exists.");

            if (await _userRepo.UserNameExistsAsync(model.UserName))
                return ServiceResult.Failure("This username is already taken.");

            if (!string.IsNullOrWhiteSpace(model.EmployeeCode) &&
                await _userRepo.EmployeeCodeExistsAsync(model.EmployeeCode))
                return ServiceResult.Failure("This employee code is already assigned to another user.");

            // ── Validate role ─────────────────────────────────────────────
            if (!await _roleRepo.RoleExistsAsync(model.Role))
                return ServiceResult.Failure($"Role '{model.Role}' does not exist.");

            // ── Build entity ──────────────────────────────────────────────
            var newUser = new ApplicationUser
            {
                UserName      = model.UserName.Trim(),
                Email         = model.Email.Trim(),
                FullName      = model.FullName.Trim(),
                PhoneNumber   = model.PhoneNumber?.Trim(),
                EmployeeCode  = model.EmployeeCode?.Trim(),
                Designation   = model.Designation?.Trim(),
                Address       = model.Address?.Trim(),
                IsActive      = true,
                CreatedOn     = DateTime.UtcNow,
                EmailConfirmed = true  // Admin-created users are pre-confirmed
            };

            var (success, errors) = await _userRepo.CreateAsync(newUser, model.Password, model.Role);

            if (!success)
                return ServiceResult.Failure("Failed to create the user account.", errors);

            // ── Audit ─────────────────────────────────────────────────────
            await _activityLog.LogAsync(createdByUserId, ActivityTypes.UserCreated,
                $"Created user '{newUser.UserName}' with role '{model.Role}'");

            _logger.LogInformation("User {UserName} created by {AdminId}", newUser.UserName, createdByUserId);
            return ServiceResult.Success($"User '{model.FullName}' created successfully.");
        }

        public async Task<ServiceResult> UpdateProfileAsync(
            EditProfileViewModel model, IFormFile? picture, IWebHostEnvironment env)
        {
            var user = await _userRepo.GetByIdAsync(model.Id);
            if (user == null)
                return ServiceResult.Failure("User not found.");

            // ── Email uniqueness check ────────────────────────────────────
            if (!string.IsNullOrWhiteSpace(model.Email) &&
                await _userRepo.EmailExistsAsync(model.Email, model.Id))
                return ServiceResult.Failure("This email address is already in use by another user.");

            // ── Profile picture ───────────────────────────────────────────
            if (picture != null && picture.Length > 0)
            {
                // Delete old picture if exists
                if (!string.IsNullOrWhiteSpace(user.ProfilePicturePath))
                    await _fileUpload.DeleteFileAsync(user.ProfilePicturePath, env);

                var uploadResult = await _fileUpload.SaveProfilePictureAsync(picture, user.Id, env);
                if (!uploadResult.IsSuccess)
                    return ServiceResult.Failure(uploadResult.Message);

                user.ProfilePicturePath = uploadResult.Data;
            }

            // ── Update fields ─────────────────────────────────────────────
            user.FullName      = model.FullName.Trim();
            user.Email         = model.Email?.Trim() ?? user.Email;
            user.PhoneNumber   = model.PhoneNumber?.Trim();
            user.Designation   = model.Designation?.Trim();
            user.Address       = model.Address?.Trim();
            user.Notes         = model.Notes?.Trim();

            var (success, errors) = await _userRepo.UpdateAsync(user);
            if (!success)
                return ServiceResult.Failure("Failed to update the profile.", errors);

            await _activityLog.LogAsync(user.Id, ActivityTypes.ProfileUpdate,
                "Profile updated successfully.");

            return ServiceResult.Success("Profile updated successfully.");
        }

        public async Task<ServiceResult> ChangePasswordAsync(
            string userId, ChangePasswordViewModel model)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null)
                return ServiceResult.Failure("User not found.");

            var (success, errors) = await _userRepo.ChangePasswordAsync(
                userId, model.CurrentPassword, model.NewPassword);

            if (!success)
                return ServiceResult.Failure("Password change failed.", errors);

            await _activityLog.LogAsync(userId, ActivityTypes.PasswordChange,
                "Password changed successfully.");

            return ServiceResult.Success("Password changed successfully. Please log in again.");
        }

        public async Task<ServiceResult> ToggleUserStatusAsync(
            string targetUserId, string adminUserId)
        {
            var user = await _userRepo.GetByIdAsync(targetUserId);
            if (user == null)
                return ServiceResult.Failure("User not found.");

            if (targetUserId == adminUserId)
                return ServiceResult.Failure("You cannot deactivate your own account.");

            var newStatus = !user.IsActive;
            var success   = await _userRepo.SetActiveStatusAsync(targetUserId, newStatus);

            if (!success)
                return ServiceResult.Failure("Failed to update user status.");

            var action = newStatus ? "activated" : "deactivated";
            await _activityLog.LogAsync(adminUserId, ActivityTypes.UserDeactivated,
                $"User '{user.FullName}' ({user.UserName}) was {action}.");

            return ServiceResult.Success($"User '{user.FullName}' has been {action}.");
        }

        public async Task<ServiceResult> DeleteUserAsync(
            string targetUserId, string adminUserId)
        {
            if (targetUserId == adminUserId)
                return ServiceResult.Failure("You cannot delete your own account.");

            var user = await _userRepo.GetByIdAsync(targetUserId);
            if (user == null)
                return ServiceResult.Failure("User not found.");

            // Delete profile picture from disk
            if (!string.IsNullOrWhiteSpace(user.ProfilePicturePath))
                await _fileUpload.DeleteFileAsync(user.ProfilePicturePath, _env);

            var (success, errors) = await _userRepo.DeleteAsync(targetUserId);
            if (!success)
                return ServiceResult.Failure("Failed to delete the user.", errors);

            await _activityLog.LogAsync(adminUserId, ActivityTypes.UserDeactivated,
                $"User '{user.FullName}' ({user.UserName}) was permanently deleted.");

            return ServiceResult.Success($"User '{user.FullName}' has been deleted.");
        }

        public async Task<ServiceResult> ChangeUserRoleAsync(
            string targetUserId, string newRole, string adminUserId)
        {
            if (!await _roleRepo.RoleExistsAsync(newRole))
                return ServiceResult.Failure($"Role '{newRole}' does not exist.");

            var (success, errors) = await _userRepo.AssignRoleAsync(targetUserId, newRole);

            if (!success)
                return ServiceResult.Failure("Failed to update user role.", errors);

            await _activityLog.LogAsync(adminUserId, ActivityTypes.RoleAssigned,
                $"Assigned role '{newRole}' to user ID '{targetUserId}'.");

            return ServiceResult.Success($"Role updated to '{newRole}' successfully.");
        }

        // ═════════════════════════════════════════════════════════════════
        //  PASSWORD RESET
        // ═════════════════════════════════════════════════════════════════

        public async Task<ServiceResult<string>> GeneratePasswordResetLinkAsync(
            ForgotPasswordViewModel model, string baseUrl)
        {
            var user = await _userRepo.GetByEmailAsync(model.Email);

            // Always return success message — never confirm whether email exists (security)
            if (user == null || !user.IsActive)
                return ServiceResult<string>.Success(
                    string.Empty,
                    "If this email is registered, a reset link has been sent.");

            var token = await _userRepo.GeneratePasswordResetTokenAsync(user.Id);
            var encodedToken = Uri.EscapeDataString(token);
            var resetLink = $"{baseUrl}/Auth/ResetPassword?email={Uri.EscapeDataString(model.Email)}&token={encodedToken}";

            // In production: send this link via email (SMTP service)
            // For now: return it so the controller can display/log it in dev
            _logger.LogInformation("Password reset link for {Email}: {Link}", model.Email, resetLink);

            return ServiceResult<string>.Success(resetLink,
                "If this email is registered, a reset link has been sent.");
        }

        public async Task<ServiceResult> ResetPasswordAsync(ResetPasswordViewModel model)
        {
            var (success, errors) = await _userRepo.ResetPasswordAsync(
                model.Email, model.Token, model.NewPassword);

            if (!success)
                return ServiceResult.Failure(
                    "Password reset failed. The link may have expired. Please request a new one.", errors);

            var user = await _userRepo.GetByEmailAsync(model.Email);
            if (user != null)
                await _activityLog.LogAsync(user.Id, ActivityTypes.PasswordChange,
                    "Password reset via email link.");

            return ServiceResult.Success("Password reset successfully. Please log in with your new password.");
        }

        // ═════════════════════════════════════════════════════════════════
        //  QUERIES
        // ═════════════════════════════════════════════════════════════════

        public async Task<ApplicationUser?> GetCurrentUserAsync(HttpContext httpContext)
        {
            var userId = _signInManager.UserManager.GetUserId(httpContext.User);
            if (string.IsNullOrEmpty(userId)) return null;
            return await _userRepo.GetByIdAsync(userId);
        }

        public async Task<IEnumerable<UserListViewModel>> GetAllUsersAsync()
            => await _userRepo.GetUserListAsync();

        public async Task<EditProfileViewModel?> GetProfileViewModelAsync(string userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null) return null;

            return new EditProfileViewModel
            {
                Id                    = user.Id,
                FullName              = user.FullName,
                Email                 = user.Email,
                PhoneNumber           = user.PhoneNumber,
                Designation           = user.Designation,
                Address               = user.Address,
                Notes                 = user.Notes,
                ExistingProfilePicture = user.ProfilePicturePath
            };
        }

        public async Task<IEnumerable<string>> GetAvailableRolesAsync()
        {
            var roles = await _roleRepo.GetAllActiveAsync();
            return roles.Select(r => r.Name ?? string.Empty)
                        .Where(n => !string.IsNullOrEmpty(n));
        }

        // ═════════════════════════════════════════════════════════════════
        //  HELPERS
        // ═════════════════════════════════════════════════════════════════

        private static string GetIpAddress(HttpContext httpContext)
        {
            var forwarded = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwarded))
                return forwarded.Split(',')[0].Trim();

            return httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }
    }
}
