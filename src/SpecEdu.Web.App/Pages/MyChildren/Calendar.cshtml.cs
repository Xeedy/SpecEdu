using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Application.Common.Models;
using SpecEdu.Domain.Enums;
namespace SpecEdu.Web.App.Pages.MyChildren;

[Authorize(Roles = "Parent")]
public class CalendarModel : PageModel
{
    private readonly IConsultationService _consultationService;
    private readonly IStudentAccessService _studentAccessService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public CalendarModel(
        IConsultationService consultationService,
        IStudentAccessService studentAccessService,
        ICurrentUserService currentUserService,
        IStringLocalizer<SharedResource> localizer)
    {
        _consultationService = consultationService;
        _studentAccessService = studentAccessService;
        _currentUserService = currentUserService;
        _localizer = localizer;
    }

    public IList<StudentDto> Children { get; set; } = new List<StudentDto>();
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
            return RedirectToPage("/Account/Login");
        }

        Children = await _studentAccessService.GetStudentsForParentAsync(userId);

        UpcomingEvents = await _consultationService.GetUpcomingEventsForParentAsync(userId, 10);

        return Page();
    }

    public async Task<IActionResult> OnGetEventsAsync(DateTime start, DateTime end)
    {
        var userId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId))
        {
            return new JsonResult(new List<CalendarEventDto>());
        }

        var events = await _consultationService.GetCalendarEventsForParentAsync(userId, start, end);

        return new JsonResult(events);
    }

    public async Task<IActionResult> OnPostRespondAsync(Guid participantId, ParticipantResponseStatus response)
    {
        var userId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToPage("/Account/Login");
        }

        var dto = new ParticipantResponseDto
        {
            ParticipantId = participantId,
            ResponseStatus = response
        };

        var success = await _consultationService.RespondToInvitationAsync(dto);
        if (success)
        {
            SuccessMessage = response switch
            {
                ParticipantResponseStatus.Accepted => _localizer["CalendarPage.MsgResponseAccepted"],
                ParticipantResponseStatus.Declined => _localizer["CalendarPage.MsgResponseDeclined"],
                ParticipantResponseStatus.Tentative => _localizer["CalendarPage.MsgResponseTentative"],
                _ => _localizer["CalendarPage.MsgResponseRecorded"]
            };
        }
        else
        {
            ErrorMessage = _localizer["CalendarPage.MsgResponseFailed"];
        }

        return RedirectToPage();
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
