using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Application.Common.Models;
using SpecEdu.Domain.Enums;
using SpecEdu.Infrastructure.Authorization;
using System.Text.Json;

namespace SpecEdu.Web.Pages.SchoolAdmin.Calendar;

[Authorize(Policy = AuthorizationPolicies.RequireSchoolAdmin)]
public class IndexModel : PageModel
{
    private readonly IConsultationService _consultationService;
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public IndexModel(
        IConsultationService consultationService,
        IIdentityService identityService,
        ICurrentUserService currentUserService,
        IStringLocalizer<SharedResource> localizer)
    {
        _consultationService = consultationService;
        _identityService = identityService;
        _currentUserService = currentUserService;
        _localizer = localizer;
    }

    public Guid SchoolId { get; set; }
    public IList<ConsultationEventListItemDto> UpcomingEvents { get; set; } = new List<ConsultationEventListItemDto>();

    [TempData]
    public string? SuccessMessage { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToPage("/Account/AccessDenied");
        }

        var user = await _identityService.GetUserByIdAsync(userId);
        if (user?.SchoolId == null)
        {
            return RedirectToPage("/Account/AccessDenied");
        }

        SchoolId = user.SchoolId.Value;

        var now = DateTime.UtcNow;
        UpcomingEvents = await _consultationService.GetEventsForSchoolAsync(
            SchoolId,
            now,
            now.AddDays(30),
            statusFilter: null,
            typeFilter: null);

        UpcomingEvents = UpcomingEvents
            .Where(e => e.Status != ConsultationEventStatus.Cancelled)
            .Take(10)
            .ToList();

        return Page();
    }

    public async Task<IActionResult> OnGetEventsAsync(DateTime start, DateTime end)
    {
        var userId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId))
        {
            return new JsonResult(new List<CalendarEventDto>());
        }

        var user = await _identityService.GetUserByIdAsync(userId);
        if (user?.SchoolId == null)
        {
            return new JsonResult(new List<CalendarEventDto>());
        }

        var events = await _consultationService.GetCalendarEventsAsync(
            user.SchoolId.Value,
            start,
            end);

        return new JsonResult(events);
    }

    public string GetTypeName(ConsultationType type)
    {
        return type switch
        {
            ConsultationType.ParentTeacherMeeting => _localizer["CalendarPage.TypeParentTeacher"],
            ConsultationType.IndividualConsultation => _localizer["CalendarPage.TypeIndividual"],
            ConsultationType.PlppReview => _localizer["CalendarPage.TypePlppReview"],
            ConsultationType.IvpReview => _localizer["CalendarPage.TypeIvpReview"],
            ConsultationType.CounselorMeeting => _localizer["CalendarPage.TypeCounselor"],
            ConsultationType.ExternalSpecialistMeeting => _localizer["CalendarPage.TypeExternal"],
            ConsultationType.SchoolEvent => _localizer["CalendarPage.TypeSchoolEvent"],
            ConsultationType.Other => _localizer["CalendarPage.TypeOther"],
            _ => type.ToString()
        };
    }

    public string GetStatusName(ConsultationEventStatus status)
    {
        return status switch
        {
            ConsultationEventStatus.Scheduled => _localizer["CalendarPage.StatusScheduled"],
            ConsultationEventStatus.Confirmed => _localizer["CalendarPage.StatusConfirmed"],
            ConsultationEventStatus.Completed => _localizer["CalendarPage.StatusCompleted"],
            ConsultationEventStatus.Cancelled => _localizer["CalendarPage.StatusCancelled"],
            ConsultationEventStatus.Rescheduled => _localizer["CalendarPage.StatusRescheduled"],
            _ => status.ToString()
        };
    }

    public static string GetStatusBadgeClass(ConsultationEventStatus status)
    {
        return status switch
        {
            ConsultationEventStatus.Scheduled => "bg-warning text-dark",
            ConsultationEventStatus.Confirmed => "bg-success",
            ConsultationEventStatus.Completed => "bg-secondary",
            ConsultationEventStatus.Cancelled => "bg-danger",
            ConsultationEventStatus.Rescheduled => "bg-info",
            _ => "bg-primary"
        };
    }
}
