using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
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
    private readonly IStringLocalizer<SharedResource> _localizer;

    public EditModel(
        IConsultationService consultationService,
        ICurrentUserService currentUserService,
        IStringLocalizer<SharedResource> localizer)
    {
        _consultationService = consultationService;
        _currentUserService = currentUserService;
        _localizer = localizer;
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
        [Required(ErrorMessage = "CalendarPage.TitleRequired")]
        [Display(Name = "CalendarPage.LabelTitle")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "CalendarPage.LabelDescription")]
        [StringLength(4000)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "CalendarPage.TypeRequired")]
        [Display(Name = "CalendarPage.LabelType")]
        public ConsultationType Type { get; set; }

        [Required(ErrorMessage = "CalendarPage.StartRequired")]
        [Display(Name = "CalendarPage.LabelStart")]
        [DataType(DataType.DateTime)]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "CalendarPage.EndRequired")]
        [Display(Name = "CalendarPage.LabelEnd")]
        [DataType(DataType.DateTime)]
        public DateTime EndTime { get; set; }

        [Display(Name = "CalendarPage.LabelLocation")]
        [StringLength(500)]
        public string? Location { get; set; }

        [Display(Name = "CalendarPage.LabelOnlineLink")]
        [StringLength(1000)]
        [Url(ErrorMessage = "CalendarPage.InvalidUrl")]
        public string? OnlineMeetingLink { get; set; }

        [Display(Name = "CalendarPage.LabelVisibleToParents")]
        public bool IsVisibleToParents { get; set; }

        [Display(Name = "CalendarPage.LabelAllowResponses")]
        public bool AllowResponses { get; set; }

        [Display(Name = "CalendarPage.LabelNotes")]
        [StringLength(4000)]
        public string? Notes { get; set; }

        [Display(Name = "CalendarPage.LabelReminder")]
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
            ModelState.AddModelError("Input.EndTime", _localizer["CalendarPage.MsgEndAfterStart"]);
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

        TempData["SuccessMessage"] = _localizer["CalendarPage.MsgEventUpdated"].Value;
        return RedirectToPage("Details", new { Id });
    }

    private void PopulateOptions()
    {
        TypeOptions = new List<SelectListItem>
        {
            new SelectListItem(_localizer["CalendarPage.TypeParentTeacher"], ((int)ConsultationType.ParentTeacherMeeting).ToString()),
            new SelectListItem(_localizer["CalendarPage.TypeIndividual"], ((int)ConsultationType.IndividualConsultation).ToString()),
            new SelectListItem(_localizer["CalendarPage.TypePlppReview"], ((int)ConsultationType.PlppReview).ToString()),
            new SelectListItem(_localizer["CalendarPage.TypeIvpReview"], ((int)ConsultationType.IvpReview).ToString()),
            new SelectListItem(_localizer["CalendarPage.TypeCounselor"], ((int)ConsultationType.CounselorMeeting).ToString()),
            new SelectListItem(_localizer["CalendarPage.TypeExternal"], ((int)ConsultationType.ExternalSpecialistMeeting).ToString()),
            new SelectListItem(_localizer["CalendarPage.TypeSchoolEvent"], ((int)ConsultationType.SchoolEvent).ToString()),
            new SelectListItem(_localizer["CalendarPage.TypeOther"], ((int)ConsultationType.Other).ToString())
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
