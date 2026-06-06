using MauliFarm.Models;

namespace MauliFarm.Repositories.Interfaces
{
    /// <summary>
    /// Contract for user activity log data access.
    /// </summary>
    public interface IActivityLogRepository
    {
        Task LogAsync(string userId, string activityType, string? description = null,
                      string? ipAddress = null, string? userAgent = null, bool isSuccess = true);

        Task<IEnumerable<UserActivityLog>> GetByUserIdAsync(string userId, int take = 50);
        Task<IEnumerable<UserActivityLog>> GetRecentAsync(int take = 100);
        Task<IEnumerable<UserActivityLog>> GetByActivityTypeAsync(string activityType, int take = 100);
        Task<IEnumerable<UserActivityLog>> GetByDateRangeAsync(DateTime from, DateTime to);
        Task<int> GetFailedLoginCountAsync(string userId, DateTime since);
        Task<bool> DeleteOlderThanAsync(DateTime cutoffDate);
    }
}
