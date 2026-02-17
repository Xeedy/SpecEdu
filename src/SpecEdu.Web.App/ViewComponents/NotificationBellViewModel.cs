using SpecEdu.Domain.Enums;

namespace SpecEdu.Web.App.ViewComponents;

public class NotificationBellViewModel
{
    public int UnreadCount { get; set; }
    public List<NotificationBellItem> RecentNotifications { get; set; } = [];
}

public class NotificationBellItem
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Link { get; set; }
    public NotificationType Type { get; set; }
    public DateTime CreatedAt { get; set; }
}
