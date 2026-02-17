using Microsoft.EntityFrameworkCore;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Application.Common.Models;
using SpecEdu.Domain.Entities;
using SpecEdu.Domain.Enums;
using SpecEdu.Infrastructure.Data;

namespace SpecEdu.Infrastructure.Services;

public class NotificationService(ApplicationDbContext context) : INotificationService
{
    public async Task CreateAsync(string userId, string title, string message, NotificationType type,
        string? link = null, string? relatedEntityId = null, string? relatedEntityType = null)
    {
        var notification = new Notification
        {
            UserId = userId,
            Title = title,
            Message = message,
            Type = type,
            Link = link,
            RelatedEntityId = relatedEntityId,
            RelatedEntityType = relatedEntityType
        };

        context.Notifications.Add(notification);
        await context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<NotificationDto>> GetUnreadAsync(string userId, int count = 5)
    {
        return await context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .OrderByDescending(n => n.CreatedAt)
            .Take(count)
            .Select(n => new NotificationDto
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                Link = n.Link,
                Type = n.Type,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt,
                ReadAt = n.ReadAt
            })
            .ToListAsync();
    }

    public async Task<IReadOnlyList<NotificationDto>> GetAllAsync(string userId, int page = 1, int pageSize = 20)
    {
        return await context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(n => new NotificationDto
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                Link = n.Link,
                Type = n.Type,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt,
                ReadAt = n.ReadAt
            })
            .ToListAsync();
    }

    public async Task<int> GetUnreadCountAsync(string userId)
    {
        return await context.Notifications
            .CountAsync(n => n.UserId == userId && !n.IsRead);
    }

    public async Task MarkAsReadAsync(Guid notificationId, string userId)
    {
        var notification = await context.Notifications
            .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

        if (notification is { IsRead: false })
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            await context.SaveChangesAsync();
        }
    }

    public async Task MarkAllAsReadAsync(string userId)
    {
        await context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ExecuteUpdateAsync(s => s
                .SetProperty(n => n.IsRead, true)
                .SetProperty(n => n.ReadAt, DateTime.UtcNow));
    }
}
