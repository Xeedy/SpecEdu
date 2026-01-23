using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Application.Common.Models;
using SpecEdu.Domain.Enums;
using SpecEdu.Infrastructure.Authorization;

namespace SpecEdu.Web.Pages.SchoolAdmin.Calendar;

[Authorize(Policy = AuthorizationPolicies.RequireSchoolAdmin)]
public class DetailsModel : PageModel
{
    private readonly IConsultationService _consultationService;
    private readonly IStudentAccessService _studentAccessService;
    private readonly ICurrentUserService _currentUserService;

    public DetailsModel(
        IConsultationService consultationService,
        IStudentAccessService studentAccessService,
        ICurrentUserService currentUserService)
    {
        _consultationService = consultationService;
        _studentAccessService = studentAccessService;
        _currentUserService = currentUserService;
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
        var success = await _consultationService.CancelEventAsync(Id, "Zruseno organizatorem");
        if (success)
        {
            SuccessMessage = "Konzultace byla zrusena.";
        }
        else
        {
            ErrorMessage = "Nepodarilo se zrusit konzultaci.";
        }

        return RedirectToPage(new { Id });
    }

    public async Task<IActionResult> OnPostConfirmAsync()
    {
        var success = await _consultationService.ChangeEventStatusAsync(Id, ConsultationEventStatus.Confirmed);
        if (success)
        {
            SuccessMessage = "Konzultace byla potvrzena.";
        }
        else
        {
            ErrorMessage = "Nepodarilo se potvrdit konzultaci.";
        }

        return RedirectToPage(new { Id });
    }

    public async Task<IActionResult> OnPostCompleteAsync()
    {
        var success = await _consultationService.CompleteEventAsync(Id);
        if (success)
        {
            SuccessMessage = "Konzultace byla oznacena jako probehla.";
        }
        else
        {
            ErrorMessage = "Nepodarilo se oznacit konzultaci jako probehlou.";
        }

        return RedirectToPage(new { Id });
    }

    public async Task<IActionResult> OnPostDeleteAsync()
    {
        var success = await _consultationService.DeleteEventAsync(Id);
        if (success)
        {
            TempData["SuccessMessage"] = "Konzultace byla smazana.";
            return RedirectToPage("Index");
        }
        else
        {
            ErrorMessage = "Nepodarilo se smazat konzultaci.";
            return RedirectToPage(new { Id });
        }
    }

    public async Task<IActionResult> OnPostRemoveParticipantAsync(Guid participantId)
    {
        var success = await _consultationService.RemoveParticipantAsync(participantId);
        if (success)
        {
            SuccessMessage = "Ucastnik byl odebran.";
        }
        else
        {
            ErrorMessage = "Nepodarilo se odebrat ucastnika.";
        }

        return RedirectToPage(new { Id });
    }

    public async Task<IActionResult> OnPostInviteGuardiansAsync()
    {
        var evt = await _consultationService.GetEventByIdAsync(Id);
        if (evt?.StudentId == null)
        {
            ErrorMessage = "Konzultace neni prirazena zadnemu zakovi.";
            return RedirectToPage(new { Id });
        }

        var count = await _consultationService.InviteStudentGuardiansAsync(Id, evt.StudentId.Value);
        if (count > 0)
        {
            SuccessMessage = $"Pozváno {count} zakonnych zastupcu.";
        }
        else
        {
            ErrorMessage = "Zadni dalsi zakonni zastupci k pozvani.";
        }

        return RedirectToPage(new { Id });
    }

    public async Task<IActionResult> OnPostInviteStaffAsync()
    {
        var evt = await _consultationService.GetEventByIdAsync(Id);
        if (evt?.StudentId == null)
        {
            ErrorMessage = "Konzultace neni prirazena zadnemu zakovi.";
            return RedirectToPage(new { Id });
        }

        var count = await _consultationService.InviteStudentStaffAsync(Id, evt.StudentId.Value);
        if (count > 0)
        {
            SuccessMessage = $"Pozváno {count} ucitelu.";
        }
        else
        {
            ErrorMessage = "Zadni dalsi ucitele k pozvani.";
        }

        return RedirectToPage(new { Id });
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

    public static string GetResponseStatusName(ParticipantResponseStatus status)
    {
        return status switch
        {
            ParticipantResponseStatus.Pending => "Ceka na odpoved",
            ParticipantResponseStatus.Accepted => "Prijato",
            ParticipantResponseStatus.Declined => "Odmitnuto",
            ParticipantResponseStatus.Tentative => "Mozna",
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
