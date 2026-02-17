using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Application.Common.Models;

namespace SpecEdu.Web.App.Pages.Notifications;

[Authorize]
public class IndexModel(
    INotificationService notificationService,
    ICurrentUserService currentUserService) : PageModel
{
    public IReadOnlyList<NotificationDto> Notifications { get; set; } = [];
    public int CurrentPage { get; set; } = 1;
    public bool HasMore { get; set; }

    public async Task<IActionResult> OnGetAsync(int page = 1)
    {
        var userId = currentUserService.UserId;
        if (userId == null) return RedirectToPage("/Account/Login");

        CurrentPage = page;
        var items = await notificationService.GetAllAsync(userId, page, 20);
        Notifications = items;
        HasMore = items.Count == 20;

        return Page();
    }

    public async Task<IActionResult> OnPostMarkReadAsync(Guid id)
    {
        var userId = currentUserService.UserId;
        if (userId == null) return RedirectToPage("/Account/Login");

        await notificationService.MarkAsReadAsync(id, userId);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostMarkAllReadAsync()
    {
        var userId = currentUserService.UserId;
        if (userId == null) return RedirectToPage("/Account/Login");

        await notificationService.MarkAllAsReadAsync(userId);
        return RedirectToPage();
    }
}
