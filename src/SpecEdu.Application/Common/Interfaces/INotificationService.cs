using SpecEdu.Application.Common.Models;
using SpecEdu.Domain.Enums;

namespace SpecEdu.Application.Common.Interfaces;

public interface INotificationService
{
    Task CreateAsync(string userId, string title, string message, NotificationType type,
        string? link = null, string? relatedEntityId = null, string? relatedEntityType = null);

    Task<IReadOnlyList<NotificationDto>> GetUnreadAsync(string userId, int count = 5);

    Task<IReadOnlyList<NotificationDto>> GetAllAsync(string userId, int page = 1, int pageSize = 20);

    Task<int> GetUnreadCountAsync(string userId);

    Task MarkAsReadAsync(Guid notificationId, string userId);

    Task MarkAllAsReadAsync(string userId);
}
