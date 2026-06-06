using MauliFarm.Models;
using MauliFarm.Models.ViewModels;

namespace MauliFarm.Repositories.Interfaces
{
    /// <summary>
    /// Contract for user data access operations.
    /// Wraps UserManager with farm-specific queries.
    /// </summary>
    public interface IUserRepository
    {
        // ── Read ──────────────────────────────────────────────────────────
        Task<ApplicationUser?>          GetByIdAsync(string userId);
        Task<ApplicationUser?>          GetByEmailAsync(string email);
        Task<ApplicationUser?>          GetByUserNameAsync(string userName);
        Task<ApplicationUser?>          GetByEmailOrUserNameAsync(string input);
        Task<IEnumerable<ApplicationUser>> GetAllAsync();
        Task<IEnumerable<ApplicationUser>> GetAllActiveAsync();
        Task<IEnumerable<UserListViewModel>> GetUserListAsync();

        // ── Write ─────────────────────────────────────────────────────────
        Task<(bool Success, IEnumerable<string> Errors)> CreateAsync(ApplicationUser user, string password, string role);
        Task<(bool Success, IEnumerable<string> Errors)> UpdateAsync(ApplicationUser user);
        Task<(bool Success, IEnumerable<string> Errors)> DeleteAsync(string userId);
        Task<bool> SetActiveStatusAsync(string userId, bool isActive);
        Task<bool> UpdateLastLoginAsync(string userId);
        Task<bool> UpdateProfilePictureAsync(string userId, string picturePath);

        // ── Role ──────────────────────────────────────────────────────────
        Task<string?> GetUserRoleAsync(string userId);
        Task<IList<string>> GetUserRolesAsync(string userId);
        Task<(bool Success, IEnumerable<string> Errors)> AssignRoleAsync(string userId, string role);
        Task<(bool Success, IEnumerable<string> Errors)> RemoveRoleAsync(string userId, string role);

        // ── Password ──────────────────────────────────────────────────────
        Task<(bool Success, IEnumerable<string> Errors)> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        Task<string> GeneratePasswordResetTokenAsync(string userId);
        Task<(bool Success, IEnumerable<string> Errors)> ResetPasswordAsync(string email, string token, string newPassword);
        Task<bool> CheckPasswordAsync(string userId, string password);

        // ── Exists ────────────────────────────────────────────────────────
        Task<bool> EmailExistsAsync(string email, string? excludeUserId = null);
        Task<bool> UserNameExistsAsync(string userName, string? excludeUserId = null);
        Task<bool> EmployeeCodeExistsAsync(string code, string? excludeUserId = null);
    }
}
