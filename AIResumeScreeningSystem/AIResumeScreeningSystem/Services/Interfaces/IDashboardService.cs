using AIResumeScreeningSystem.ViewModels.Dashboard;

namespace AIResumeScreeningSystem.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<AdminDashboardViewModel> GetAdminDashboardAsync();
        Task<RecruiterDashboardViewModel> GetRecruiterDashboardAsync(string recruiterId);
    }
}