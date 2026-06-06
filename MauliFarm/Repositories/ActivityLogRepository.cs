using Microsoft.EntityFrameworkCore;
using MauliFarm.Data;
using MauliFarm.Models;
using MauliFarm.Repositories.Interfaces;

namespace MauliFarm.Repositories
{
    /// <summary>
    /// Concrete implementation of IActivityLogRepository.
    /// Writes/reads directly to MF_UserActivityLogs table via EF Core.
    /// </summary>
    public class ActivityLogRepository : IActivityLogRepository
    {
        private readonly ApplicationDbContext    _context;
        private readonly ILogger<ActivityLogRepository> _logger;

        public ActivityLogRepository(
            ApplicationDbContext context,
            ILogger<ActivityLogRepository> logger)
        {
            _context = context;
            _logger  = logger;
        }

        // ─────────────────────────────────────────────────────────────────
        //  WRITE
        // ─────────────────────────────────────────────────────────────────

        public async Task LogAsync(
            string userId,
            string activityType,
            string? description = null,
            string? ipAddress   = null,
            string? userAgent   = null,
            bool   isSuccess    = true)
        {
            try
            {
                var log = new UserActivityLog
                {
                    UserId       = userId,
                    ActivityType = activityType,
                    Description  = description,
                    IpAddress    = TruncateSafe(ipAddress, 50),
                    UserAgent    = TruncateSafe(userAgent, 300),
                    Timestamp    = DateTime.UtcNow,
                    IsSuccess    = isSuccess
                };

                _context.UserActivityLogs.Add(log);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Never let audit logging crash the application
                _logger.LogWarning(ex, "Failed to write activity log for user {UserId} — activity: {Activity}",
                    userId, activityType);
            }
        }

        // ─────────────────────────────────────────────────────────────────
        //  READ
        // ─────────────────────────────────────────────────────────────────

        public async Task<IEnumerable<UserActivityLog>> GetByUserIdAsync(string userId, int take = 50)
            => await _context.UserActivityLogs
                .Where(l => l.UserId == userId)
                .OrderByDescending(l => l.Timestamp)
                .Take(take)
                .Include(l => l.User)
                .ToListAsync();

        public async Task<IEnumerable<UserActivityLog>> GetRecentAsync(int take = 100)
            => await _context.UserActivityLogs
                .OrderByDescending(l => l.Timestamp)
                .Take(take)
                .Include(l => l.User)
                .ToListAsync();

        public async Task<IEnumerable<UserActivityLog>> GetByActivityTypeAsync(
            string activityType, int take = 100)
            => await _context.UserActivityLogs
                .Where(l => l.ActivityType == activityType)
                .OrderByDescending(l => l.Timestamp)
                .Take(take)
                .Include(l => l.User)
                .ToListAsync();

        public async Task<IEnumerable<UserActivityLog>> GetByDateRangeAsync(
            DateTime from, DateTime to)
            => await _context.UserActivityLogs
                .Where(l => l.Timestamp >= from && l.Timestamp <= to)
                .OrderByDescending(l => l.Timestamp)
                .Include(l => l.User)
                .ToListAsync();

        public async Task<int> GetFailedLoginCountAsync(string userId, DateTime since)
            => await _context.UserActivityLogs
                .CountAsync(l => l.UserId       == userId
                              && l.ActivityType == ActivityTypes.LoginFailed
                              && l.Timestamp    >= since
                              && !l.IsSuccess);

        // ─────────────────────────────────────────────────────────────────
        //  MAINTENANCE
        // ─────────────────────────────────────────────────────────────────

        public async Task<bool> DeleteOlderThanAsync(DateTime cutoffDate)
        {
            try
            {
                var deleted = await _context.UserActivityLogs
                    .Where(l => l.Timestamp < cutoffDate)
                    .ExecuteDeleteAsync();

                _logger.LogInformation("Activity log purge: {Count} records deleted before {Date}",
                    deleted, cutoffDate.ToShortDateString());
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to purge activity logs older than {Date}", cutoffDate);
                return false;
            }
        }

        // ─────────────────────────────────────────────────────────────────
        //  HELPERS
        // ─────────────────────────────────────────────────────────────────

        private static string? TruncateSafe(string? value, int maxLength)
            => value?.Length > maxLength ? value[..maxLength] : value;
    }
}
