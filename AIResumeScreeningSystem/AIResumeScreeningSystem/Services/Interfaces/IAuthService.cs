using AIResumeScreeningSystem.ViewModels.Account;
using Microsoft.AspNetCore.Identity;

namespace AIResumeScreeningSystem.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Success, IEnumerable<string> Errors)> RegisterAsync(RegisterViewModel model);
        Task<(bool Success, string Error)> LoginAsync(LoginViewModel model);
        Task LogoutAsync();
        Task<(bool Success, string Error)> ForgotPasswordAsync(ForgotPasswordViewModel model);
        Task<(bool Success, IEnumerable<string> Errors)> ResetPasswordAsync(ResetPasswordViewModel model);
        Task<string> GetUserRoleAsync(string userId);
        Task<string> GetDashboardUrlByRoleAsync(string userId);
    }
}