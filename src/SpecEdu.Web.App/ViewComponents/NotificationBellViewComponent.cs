using Microsoft.AspNetCore.Mvc;
using SpecEdu.Application.Common.Interfaces;

namespace SpecEdu.Web.App.ViewComponents;

public class NotificationBellViewComponent(
    INotificationService notificationService,
    ICurrentUserService currentUserService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var userId = currentUserService.UserId;
        if (userId == null)
        {
            return View(new NotificationBellViewModel());
        }

        var unreadCount = await notificationService.GetUnreadCountAsync(userId);
        var recent = await notificationService.GetUnreadAsync(userId, 5);

        var model = new NotificationBellViewModel
        {
            UnreadCount = unreadCount,
            RecentNotifications = recent.Select(n => new NotificationBellItem
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                Link = n.Link,
                Type = n.Type,
                CreatedAt = n.CreatedAt
            }).ToList()
        };

        return View(model);
    }
}
