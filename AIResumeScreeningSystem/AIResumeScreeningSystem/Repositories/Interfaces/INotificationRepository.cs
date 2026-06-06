using AIResumeScreeningSystem.Models;

namespace AIResumeScreeningSystem.Repositories.Interfaces
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        Task<List<Notification>> GetUserNotificationsAsync(
            string userId, bool unreadOnly = false);
        Task<int> GetUnreadCountAsync(string userId);
        Task MarkAsReadAsync(int notificationId);
        Task MarkAllAsReadAsync(string userId);
        Task DeleteOldNotificationsAsync(string userId, int keepCount = 50);
        Task<Notification?> GetLatestUnreadAsync(string userId);
    }
}