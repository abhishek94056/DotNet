namespace AIResumeScreeningSystem.ViewModels.Notification
{
    public class NotificationListViewModel
    {
        public List<NotificationViewModel> Notifications { get; set; } = new();
        public int UnreadCount { get; set; }
        public int TotalCount { get; set; }
        public bool ShowOnlyUnread { get; set; }
    }
}