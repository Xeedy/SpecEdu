using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Application.Common.Models;
using SpecEdu.Domain.Enums;
using SpecEdu.Infrastructure.Authorization;
using System.ComponentModel.DataAnnotations;

namespace SpecEdu.Web.Pages.SchoolAdmin.Calendar;

[Authorize(Policy = AuthorizationPolicies.RequireSchoolAdmin)]
public class EditModel : PageModel
{
    private readonly IConsultationService _consultationService;
    private readonly ICurrentUserService _currentUserService;

    public EditModel(
        IConsultationService consultationService,
        ICurrentUserService currentUserService)
    {
        _consultationService = consultationService;
        _currentUserService = currentUserService;
    }

    public ConsultationEventDto Event { get; set; } = null!;

    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public List<SelectListItem> TypeOptions { get; set; } = new();

    [TempData]
    public string? ErrorMessage { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Nazev je povinny.")]
        [Display(Name = "Nazev")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Popis / program")]
        [StringLength(4000)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Typ je povinny.")]
        [Display(Name = "Typ konzultace")]
        public ConsultationType Type { get; set; }

        [Required(ErrorMessage = "Datum a cas zacatku je povinny.")]
        [Display(Name = "Zacatek")]
        [DataType(DataType.DateTime)]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "Datum a cas konce je povinny.")]
        [Display(Name = "Konec")]
        [DataType(DataType.DateTime)]
        public DateTime EndTime { get; set; }

        [Display(Name = "Misto konani")]
        [StringLength(500)]
        public string? Location { get; set; }

        [Display(Name = "Odkaz na online schuzku")]
        [StringLength(1000)]
        [Url(ErrorMessage = "Zadejte platnou URL adresu.")]
        public string? OnlineMeetingLink { get; set; }

        [Display(Name = "Viditelne pro rodice")]
        public bool IsVisibleToParents { get; set; }

        [Display(Name = "Umoznit odpovedi")]
        public bool AllowResponses { get; set; }

        [Display(Name = "Poznamky z prubehu")]
        [StringLength(4000)]
        public string? Notes { get; set; }

        [Display(Name = "Pripomenout pred (minuty)")]
        public int? ReminderMinutesBefore { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var evt = await _consultationService.GetEventByIdAsync(Id);
        if (evt == null)
        {
            return NotFound();
        }

        Event = evt;
        PopulateOptions();
        PopulateInputFromEvent(evt);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var evt = await _consultationService.GetEventByIdAsync(Id);
        if (evt == null)
        {
            return NotFound();
        }

        Event = evt;

        if (!ModelState.IsValid)
        {
            PopulateOptions();
            return Page();
        }

        if (Input.StartTime >= Input.EndTime)
        {
            ModelState.AddModelError("Input.EndTime", "Cas konce musi byt po casu zacatku.");
            PopulateOptions();
            return Page();
        }

        var dto = new UpdateConsultationEventDto
        {
            Id = Id,
            Title = Input.Title,
            Description = Input.Description,
            Type = Input.Type,
            StartTime = Input.StartTime,
            EndTime = Input.EndTime,
            Location = Input.Location,
            OnlineMeetingLink = Input.OnlineMeetingLink,
            IsVisibleToParents = Input.IsVisibleToParents,
            AllowResponses = Input.AllowResponses,
            Notes = Input.Notes,
            ReminderMinutesBefore = Input.ReminderMinutesBefore
        };

        await _consultationService.UpdateEventAsync(dto);

        TempData["SuccessMessage"] = "Konzultace byla upravena.";
        return RedirectToPage("Details", new { Id });
    }

    private void PopulateOptions()
    {
        TypeOptions = new List<SelectListItem>
        {
            new SelectListItem("Tridni schuzka", ((int)ConsultationType.ParentTeacherMeeting).ToString()),
            new SelectListItem("Individualni konzultace", ((int)ConsultationType.IndividualConsultation).ToString()),
            new SelectListItem("Vyhodnoceni PLPP", ((int)ConsultationType.PlppReview).ToString()),
            new SelectListItem("Vyhodnoceni IVP", ((int)ConsultationType.IvpReview).ToString()),
            new SelectListItem("Schuzka s poradcem", ((int)ConsultationType.CounselorMeeting).ToString()),
            new SelectListItem("Externi specialista", ((int)ConsultationType.ExternalSpecialistMeeting).ToString()),
            new SelectListItem("Skolni akce", ((int)ConsultationType.SchoolEvent).ToString()),
            new SelectListItem("Jine", ((int)ConsultationType.Other).ToString())
        };
    }

    private void PopulateInputFromEvent(ConsultationEventDto evt)
    {
        Input = new InputModel
        {
            Title = evt.Title,
            Description = evt.Description,
            Type = evt.Type,
            StartTime = evt.StartTime,
            EndTime = evt.EndTime,
            Location = evt.Location,
            OnlineMeetingLink = evt.OnlineMeetingLink,
            IsVisibleToParents = evt.IsVisibleToParents,
            AllowResponses = evt.AllowResponses,
            Notes = evt.Notes,
            ReminderMinutesBefore = evt.ReminderMinutesBefore
        };
    }
}
