using AIResumeScreeningSystem.Data;
using AIResumeScreeningSystem.Models;
using AIResumeScreeningSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AIResumeScreeningSystem.Repositories.Implementations
{
    public class NotificationRepository
        : GenericRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(AppDbContext context) : base(context) { }

        public async Task<List<Notification>> GetUserNotificationsAsync(
            string userId, bool unreadOnly = false)
        {
            var query = _context.Notifications
                .Where(n => n.UserId == userId);

            if (unreadOnly)
                query = query.Where(n => !n.IsRead);

            return await query
                .OrderByDescending(n => n.CreatedAt)
                .Take(100)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            return await _context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            var notification = await _context.Notifications
                .FindAsync(notificationId);

            if (notification != null && !notification.IsRead)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
                _context.Notifications.Update(notification);
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAllAsReadAsync(string userId)
        {
            var unread = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var n in unread)
            {
                n.IsRead = true;
                n.ReadAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteOldNotificationsAsync(
            string userId, int keepCount = 50)
        {
            var toDelete = await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Skip(keepCount)
                .ToListAsync();

            if (toDelete.Any())
            {
                _context.Notifications.RemoveRange(toDelete);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Notification?> GetLatestUnreadAsync(string userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .FirstOrDefaultAsync();
        }
    }
}