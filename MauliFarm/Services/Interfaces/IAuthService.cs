using MauliFarm.Models;
using MauliFarm.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;

namespace MauliFarm.Services.Interfaces
{
    /// <summary>
    /// Business logic contract for authentication and user management.
    /// Controllers talk only to this interface — never directly to repositories.
    /// </summary>
    public interface IAuthService
    {
        // ── Authentication ────────────────────────────────────────────────
        Task<ServiceResult> LoginAsync(LoginViewModel model, HttpContext httpContext);
        Task LogoutAsync(HttpContext httpContext);

        // ── Registration / User Management ────────────────────────────────
        Task<ServiceResult> RegisterUserAsync(RegisterViewModel model, string createdByUserId);
        Task<ServiceResult> UpdateProfileAsync(EditProfileViewModel model, IFormFile? picture, IWebHostEnvironment env);
        Task<ServiceResult> ChangePasswordAsync(string userId, ChangePasswordViewModel model);
        Task<ServiceResult> ToggleUserStatusAsync(string targetUserId, string adminUserId);
        Task<ServiceResult> DeleteUserAsync(string targetUserId, string adminUserId);
        Task<ServiceResult> ChangeUserRoleAsync(string targetUserId, string newRole, string adminUserId);

        // ── Password Reset ────────────────────────────────────────────────
        Task<ServiceResult<string>> GeneratePasswordResetLinkAsync(ForgotPasswordViewModel model, string baseUrl);
        Task<ServiceResult>         ResetPasswordAsync(ResetPasswordViewModel model);

        // ── Queries ───────────────────────────────────────────────────────
        Task<ApplicationUser?>              GetCurrentUserAsync(HttpContext httpContext);
        Task<IEnumerable<UserListViewModel>> GetAllUsersAsync();
        Task<EditProfileViewModel?>          GetProfileViewModelAsync(string userId);
        Task<IEnumerable<string>>            GetAvailableRolesAsync();
    }
}
