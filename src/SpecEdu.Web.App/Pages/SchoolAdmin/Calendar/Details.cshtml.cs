using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Application.Common.Models;
using SpecEdu.Domain.Enums;
using SpecEdu.Infrastructure.Authorization;

namespace SpecEdu.Web.App.Pages.SchoolAdmin.Calendar;

[Authorize(Policy = AuthorizationPolicies.RequireSchoolAdmin)]
public class DetailsModel : PageModel
{
    private readonly IConsultationService _consultationService;
    private readonly IStudentAccessService _studentAccessService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public DetailsModel(
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

    public ConsultationEventDto Event { get; set; } = null!;

    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [TempData]
    public string? SuccessMessage { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var evt = await _consultationService.GetEventByIdAsync(Id);
        if (evt == null)
        {
            return NotFound();
        }

        Event = evt;
        return Page();
    }

    public async Task<IActionResult> OnPostCancelAsync()
    {
        var success = await _consultationService.CancelEventAsync(Id, _localizer["CalendarPage.MsgCancelledByOrganizer"]);
        if (success)
        {
            SuccessMessage = _localizer["CalendarPage.MsgEventCancelled"];
        }
        else
        {
            ErrorMessage = _localizer["CalendarPage.MsgEventCancelFailed"];
        }

        return RedirectToPage(new { Id });
    }

    public async Task<IActionResult> OnPostConfirmAsync()
    {
        var success = await _consultationService.ChangeEventStatusAsync(Id, ConsultationEventStatus.Confirmed);
        if (success)
        {
            SuccessMessage = _localizer["CalendarPage.MsgEventConfirmed"];
        }
        else
        {
            ErrorMessage = _localizer["CalendarPage.MsgEventConfirmFailed"];
        }

        return RedirectToPage(new { Id });
    }

    public async Task<IActionResult> OnPostCompleteAsync()
    {
        var success = await _consultationService.CompleteEventAsync(Id);
        if (success)
        {
            SuccessMessage = _localizer["CalendarPage.MsgEventCompleted"];
        }
        else
        {
            ErrorMessage = _localizer["CalendarPage.MsgEventCompleteFailed"];
        }

        return RedirectToPage(new { Id });
    }

    public async Task<IActionResult> OnPostDeleteAsync()
    {
        var success = await _consultationService.DeleteEventAsync(Id);
        if (success)
        {
            TempData["SuccessMessage"] = _localizer["CalendarPage.MsgEventDeleted"].Value;
            return RedirectToPage("Index");
        }
        else
        {
            ErrorMessage = _localizer["CalendarPage.MsgEventDeleteFailed"];
            return RedirectToPage(new { Id });
        }
    }

    public async Task<IActionResult> OnPostRemoveParticipantAsync(Guid participantId)
    {
        var success = await _consultationService.RemoveParticipantAsync(participantId);
        if (success)
        {
            SuccessMessage = _localizer["CalendarPage.MsgParticipantRemoved"];
        }
        else
        {
            ErrorMessage = _localizer["CalendarPage.MsgParticipantRemoveFailed"];
        }

        return RedirectToPage(new { Id });
    }

    public async Task<IActionResult> OnPostInviteGuardiansAsync()
    {
        var evt = await _consultationService.GetEventByIdAsync(Id);
        if (evt?.StudentId == null)
        {
            ErrorMessage = _localizer["CalendarPage.MsgNoStudentForEvent"];
            return RedirectToPage(new { Id });
        }

        var count = await _consultationService.InviteStudentGuardiansAsync(Id, evt.StudentId.Value);
        if (count > 0)
        {
            SuccessMessage = string.Format(_localizer["CalendarPage.MsgGuardiansInvited"], count);
        }
        else
        {
            ErrorMessage = _localizer["CalendarPage.MsgNoGuardiansToInvite"];
        }

        return RedirectToPage(new { Id });
    }

    public async Task<IActionResult> OnPostInviteStaffAsync()
    {
        var evt = await _consultationService.GetEventByIdAsync(Id);
        if (evt?.StudentId == null)
        {
            ErrorMessage = _localizer["CalendarPage.MsgNoStudentForEvent"];
            return RedirectToPage(new { Id });
        }

        var count = await _consultationService.InviteStudentStaffAsync(Id, evt.StudentId.Value);
        if (count > 0)
        {
            SuccessMessage = string.Format(_localizer["CalendarPage.MsgStaffInvited"], count);
        }
        else
        {
            ErrorMessage = _localizer["CalendarPage.MsgNoStaffToInvite"];
        }

        return RedirectToPage(new { Id });
    }

    public async Task<IActionResult> OnPostSendNotificationsAsync()
    {
        var count = await _consultationService.SendInvitationNotificationsAsync(Id);
        if (count > 0)
        {
            SuccessMessage = string.Format(_localizer["CalendarPage.MsgNotificationsSent"], count);
        }
        else
        {
            ErrorMessage = _localizer["CalendarPage.MsgNotificationsSendFailed"];
        }

        return RedirectToPage(new { Id });
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

    public string GetResponseStatusName(ParticipantResponseStatus status)
    {
        return status switch
        {
            ParticipantResponseStatus.Pending => _localizer["CalendarPage.ResponsePending"],
            ParticipantResponseStatus.Accepted => _localizer["CalendarPage.ResponseAccepted"],
            ParticipantResponseStatus.Declined => _localizer["CalendarPage.ResponseDeclined"],
            ParticipantResponseStatus.Tentative => _localizer["CalendarPage.ResponseTentative"],
            _ => status.ToString()
        };
    }

    public static string GetResponseStatusBadgeClass(ParticipantResponseStatus status)
    {
        return status switch
        {
            ParticipantResponseStatus.Pending => "bg-secondary",
            ParticipantResponseStatus.Accepted => "bg-success",
            ParticipantResponseStatus.Declined => "bg-danger",
            ParticipantResponseStatus.Tentative => "bg-warning text-dark",
            _ => "bg-primary"
        };
    }
}
