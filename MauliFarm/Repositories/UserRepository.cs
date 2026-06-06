using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MauliFarm.Data;
using MauliFarm.Models;
using MauliFarm.Models.ViewModels;
using MauliFarm.Repositories.Interfaces;

namespace MauliFarm.Repositories
{
    /// <summary>
    /// Concrete implementation of IUserRepository.
    /// Uses ASP.NET Identity UserManager + direct EF Core queries for performance.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser>  _userManager;
        private readonly ApplicationDbContext          _context;
        private readonly ILogger<UserRepository>       _logger;

        public UserRepository(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            ILogger<UserRepository> logger)
        {
            _userManager = userManager;
            _context     = context;
            _logger      = logger;
        }

        // ─────────────────────────────────────────────────────────────────
        //  READ
        // ─────────────────────────────────────────────────────────────────

        public async Task<ApplicationUser?> GetByIdAsync(string userId)
            => await _userManager.FindByIdAsync(userId);

        public async Task<ApplicationUser?> GetByEmailAsync(string email)
            => await _userManager.FindByEmailAsync(email);

        public async Task<ApplicationUser?> GetByUserNameAsync(string userName)
            => await _userManager.FindByNameAsync(userName);

        public async Task<ApplicationUser?> GetByEmailOrUserNameAsync(string input)
        {
            var user = await _userManager.FindByEmailAsync(input);
            if (user == null)
                user = await _userManager.FindByNameAsync(input);
            return user;
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllAsync()
            => await _context.Users
                             .OrderBy(u => u.FullName)
                             .ToListAsync();

        public async Task<IEnumerable<ApplicationUser>> GetAllActiveAsync()
            => await _context.Users
                             .Where(u => u.IsActive)
                             .OrderBy(u => u.FullName)
                             .ToListAsync();

        public async Task<IEnumerable<UserListViewModel>> GetUserListAsync()
        {
            // Single query using a join to UserRoles + Roles for efficiency
            var users = await _context.Users
                .OrderBy(u => u.FullName)
                .ToListAsync();

            var result = new List<UserListViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                result.Add(new UserListViewModel
                {
                    Id                 = user.Id,
                    FullName           = user.FullName,
                    UserName           = user.UserName ?? string.Empty,
                    Email              = user.Email ?? string.Empty,
                    PhoneNumber        = user.PhoneNumber,
                    Designation        = user.Designation,
                    EmployeeCode       = user.EmployeeCode,
                    Role               = roles.FirstOrDefault(),
                    IsActive           = user.IsActive,
                    CreatedOn          = user.CreatedOn,
                    LastLogin          = user.LastLogin,
                    ProfilePicturePath = user.ProfilePicturePath
                });
            }

            return result;
        }

        // ─────────────────────────────────────────────────────────────────
        //  WRITE
        // ─────────────────────────────────────────────────────────────────

        public async Task<(bool Success, IEnumerable<string> Errors)> CreateAsync(
            ApplicationUser user, string password, string role)
        {
            try
            {
                var createResult = await _userManager.CreateAsync(user, password);
                if (!createResult.Succeeded)
                    return (false, createResult.Errors.Select(e => e.Description));

                if (!string.IsNullOrWhiteSpace(role))
                {
                    var roleResult = await _userManager.AddToRoleAsync(user, role);
                    if (!roleResult.Succeeded)
                        return (false, roleResult.Errors.Select(e => e.Description));
                }

                return (true, Enumerable.Empty<string>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user {UserName}", user.UserName);
                return (false, new[] { "An unexpected error occurred while creating the user." });
            }
        }

        public async Task<(bool Success, IEnumerable<string> Errors)> UpdateAsync(ApplicationUser user)
        {
            try
            {
                var result = await _userManager.UpdateAsync(user);
                return result.Succeeded
                    ? (true, Enumerable.Empty<string>())
                    : (false, result.Errors.Select(e => e.Description));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {UserId}", user.Id);
                return (false, new[] { "An unexpected error occurred while updating the user." });
            }
        }

        public async Task<(bool Success, IEnumerable<string> Errors)> DeleteAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return (false, new[] { "User not found." });

                var result = await _userManager.DeleteAsync(user);
                return result.Succeeded
                    ? (true, Enumerable.Empty<string>())
                    : (false, result.Errors.Select(e => e.Description));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}", userId);
                return (false, new[] { "An unexpected error occurred while deleting the user." });
            }
        }

        public async Task<bool> SetActiveStatusAsync(string userId, bool isActive)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return false;

                user.IsActive = isActive;
                var result = await _userManager.UpdateAsync(user);
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting active status for user {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> UpdateLastLoginAsync(string userId)
        {
            try
            {
                // Use direct EF for performance — avoid full UserManager overhead
                await _context.Users
                    .Where(u => u.Id == userId)
                    .ExecuteUpdateAsync(s => s.SetProperty(u => u.LastLogin, DateTime.UtcNow));
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating last login for user {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> UpdateProfilePictureAsync(string userId, string picturePath)
        {
            try
            {
                await _context.Users
                    .Where(u => u.Id == userId)
                    .ExecuteUpdateAsync(s => s.SetProperty(u => u.ProfilePicturePath, picturePath));
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile picture for user {UserId}", userId);
                return false;
            }
        }

        // ─────────────────────────────────────────────────────────────────
        //  ROLE
        // ─────────────────────────────────────────────────────────────────

        public async Task<string?> GetUserRoleAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;
            var roles = await _userManager.GetRolesAsync(user);
            return roles.FirstOrDefault();
        }

        public async Task<IList<string>> GetUserRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return new List<string>();
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<(bool Success, IEnumerable<string> Errors)> AssignRoleAsync(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return (false, new[] { "User not found." });

            // Remove existing roles first (one role per user policy)
            var existing = await _userManager.GetRolesAsync(user);
            if (existing.Any())
                await _userManager.RemoveFromRolesAsync(user, existing);

            var result = await _userManager.AddToRoleAsync(user, role);
            return result.Succeeded
                ? (true, Enumerable.Empty<string>())
                : (false, result.Errors.Select(e => e.Description));
        }

        public async Task<(bool Success, IEnumerable<string> Errors)> RemoveRoleAsync(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return (false, new[] { "User not found." });

            var result = await _userManager.RemoveFromRoleAsync(user, role);
            return result.Succeeded
                ? (true, Enumerable.Empty<string>())
                : (false, result.Errors.Select(e => e.Description));
        }

        // ─────────────────────────────────────────────────────────────────
        //  PASSWORD
        // ─────────────────────────────────────────────────────────────────

        public async Task<(bool Success, IEnumerable<string> Errors)> ChangePasswordAsync(
            string userId, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return (false, new[] { "User not found." });

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            return result.Succeeded
                ? (true, Enumerable.Empty<string>())
                : (false, result.Errors.Select(e => e.Description));
        }

        public async Task<string> GeneratePasswordResetTokenAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new InvalidOperationException($"User {userId} not found.");
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<(bool Success, IEnumerable<string> Errors)> ResetPasswordAsync(
            string email, string token, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return (false, new[] { "User not found." });

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            return result.Succeeded
                ? (true, Enumerable.Empty<string>())
                : (false, result.Errors.Select(e => e.Description));
        }

        public async Task<bool> CheckPasswordAsync(string userId, string password)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;
            return await _userManager.CheckPasswordAsync(user, password);
        }

        // ─────────────────────────────────────────────────────────────────
        //  EXISTS CHECKS
        // ─────────────────────────────────────────────────────────────────

        public async Task<bool> EmailExistsAsync(string email, string? excludeUserId = null)
            => await _context.Users
                .AnyAsync(u => u.NormalizedEmail == email.ToUpper()
                            && (excludeUserId == null || u.Id != excludeUserId));

        public async Task<bool> UserNameExistsAsync(string userName, string? excludeUserId = null)
            => await _context.Users
                .AnyAsync(u => u.NormalizedUserName == userName.ToUpper()
                            && (excludeUserId == null || u.Id != excludeUserId));

        public async Task<bool> EmployeeCodeExistsAsync(string code, string? excludeUserId = null)
            => await _context.Users
                .AnyAsync(u => u.EmployeeCode == code
                            && (excludeUserId == null || u.Id != excludeUserId));
    }
}
