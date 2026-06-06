using AIResumeScreeningSystem.Models;
using AIResumeScreeningSystem.ViewModels.Notification;

namespace AIResumeScreeningSystem.Services.Interfaces
{
    public interface INotificationService
    {
        Task<NotificationListViewModel> GetUserNotificationsAsync(
            string userId, bool unreadOnly = false);
        Task<int> GetUnreadCountAsync(string userId);
        Task CreateNotificationAsync(
            string userId,
            string title,
            string message,
            NotificationType type = NotificationType.Info,
            string? actionUrl = null);
        Task MarkAsReadAsync(int notificationId);
        Task MarkAllAsReadAsync(string userId);
        Task DeleteOldNotificationsAsync(string userId);

        // Domain-specific notification senders
        Task NotifyApplicationSubmittedAsync(int applicationId);
        Task NotifyApplicationStatusChangedAsync(int applicationId, string newStatus);
        Task NotifyCandidateShortlistedAsync(int applicationId);
        Task NotifyCandidateRejectedAsync(int applicationId);
        Task NotifyNewJobPostedAsync(int jobId);
        Task NotifyResumeParseCompleteAsync(int resumeId);
    }
}