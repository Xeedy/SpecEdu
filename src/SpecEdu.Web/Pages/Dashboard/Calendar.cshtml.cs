using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Application.Common.Models;
using SpecEdu.Domain.Enums;

namespace SpecEdu.Web.Pages.Dashboard;

[Authorize(Policy = "CanViewStudent")]
public class CalendarModel : PageModel
{
    private readonly IConsultationService _consultationService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IIdentityService _identityService;

    public CalendarModel(
        IConsultationService consultationService,
        ICurrentUserService currentUserService,
        IIdentityService identityService)
    {
        _consultationService = consultationService;
        _currentUserService = currentUserService;
        _identityService = identityService;
    }

    public Guid? SchoolId { get; set; }
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

        var user = await _identityService.GetUserByIdAsync(userId);
        SchoolId = user?.SchoolId;

        // Get upcoming events where this teacher is a participant
        UpcomingEvents = await _consultationService.GetUpcomingEventsAsync(userId, 10);

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

        // Get events where teacher is participant
        var events = await _consultationService.GetCalendarEventsAsync(
            user.SchoolId.Value,
            start,
            end,
            userId: userId);

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
                ParticipantResponseStatus.Accepted => "Pozvanka prijata.",
                ParticipantResponseStatus.Declined => "Pozvanka odmitnuta.",
                ParticipantResponseStatus.Tentative => "Odpoved zaznamenana jako 'mozna'.",
                _ => "Odpoved zaznamenana."
            };
        }
        else
        {
            ErrorMessage = "Nepodarilo se zaznamenat odpoved.";
        }

        return RedirectToPage();
    }

    public static string GetTypeName(ConsultationType type)
    {
        return type switch
        {
            ConsultationType.ParentTeacherMeeting => "Tridni schuzka",
            ConsultationType.IndividualConsultation => "Individualni konzultace",
            ConsultationType.PlppReview => "Vyhodnoceni PLPP",
            ConsultationType.IvpReview => "Vyhodnoceni IVP",
            ConsultationType.CounselorMeeting => "Schuzka s poradcem",
            ConsultationType.ExternalSpecialistMeeting => "Externi specialista",
            ConsultationType.SchoolEvent => "Skolni akce",
            ConsultationType.Other => "Jine",
            _ => type.ToString()
        };
    }

    public static string GetStatusName(ConsultationEventStatus status)
    {
        return status switch
        {
            ConsultationEventStatus.Scheduled => "Naplanovano",
            ConsultationEventStatus.Confirmed => "Potvrzeno",
            ConsultationEventStatus.Completed => "Probehlo",
            ConsultationEventStatus.Cancelled => "Zruseno",
            ConsultationEventStatus.Rescheduled => "Presunuto",
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
