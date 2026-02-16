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

namespace SpecEdu.Web.App.Pages.SchoolAdmin.Calendar;

[Authorize(Policy = AuthorizationPolicies.RequireSchoolAdmin)]
public class CreateModel : PageModel
{
    private readonly IConsultationService _consultationService;
    private readonly IStudentService _studentService;
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public CreateModel(
        IConsultationService consultationService,
        IStudentService studentService,
        IIdentityService identityService,
        ICurrentUserService currentUserService,
        IStringLocalizer<SharedResource> localizer)
    {
        _consultationService = consultationService;
        _studentService = studentService;
        _identityService = identityService;
        _currentUserService = currentUserService;
        _localizer = localizer;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public List<SelectListItem> TypeOptions { get; set; } = new();
    public List<SelectListItem> StudentOptions { get; set; } = new();

    public Guid SchoolId { get; set; }

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
        public ConsultationType Type { get; set; } = ConsultationType.IndividualConsultation;

        [Display(Name = "CalendarPage.LabelStudent")]
        public Guid? StudentId { get; set; }

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
        public bool IsVisibleToParents { get; set; } = true;

        [Display(Name = "CalendarPage.LabelAllowResponses")]
        public bool AllowResponses { get; set; } = true;

        [Display(Name = "CalendarPage.LabelReminder")]
        public int? ReminderMinutesBefore { get; set; } = 1440;

        [Display(Name = "CalendarPage.LabelInviteGuardians")]
        public bool InviteGuardians { get; set; } = true;

        [Display(Name = "CalendarPage.LabelInviteStaff")]
        public bool InviteStaff { get; set; } = false;
    }

    public async Task<IActionResult> OnGetAsync(Guid? studentId = null)
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
        await PopulateOptions();

        // Set defaults
        var now = DateTime.UtcNow;
        Input.StartTime = new DateTime(now.Year, now.Month, now.Day, 14, 0, 0).AddDays(1);
        Input.EndTime = Input.StartTime.AddHours(1);

        if (studentId.HasValue)
        {
            Input.StudentId = studentId;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
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

        if (!ModelState.IsValid)
        {
            await PopulateOptions();
            return Page();
        }

        if (Input.StartTime >= Input.EndTime)
        {
            ModelState.AddModelError("Input.EndTime", _localizer["CalendarPage.MsgEndAfterStart"]);
            await PopulateOptions();
            return Page();
        }

        var dto = new CreateConsultationEventDto
        {
            SchoolId = SchoolId,
            StudentId = Input.StudentId,
            Title = Input.Title,
            Description = Input.Description,
            Type = Input.Type,
            StartTime = Input.StartTime,
            EndTime = Input.EndTime,
            Location = Input.Location,
            OnlineMeetingLink = Input.OnlineMeetingLink,
            IsVisibleToParents = Input.IsVisibleToParents,
            AllowResponses = Input.AllowResponses,
            ReminderMinutesBefore = Input.ReminderMinutesBefore
        };

        var evt = await _consultationService.CreateEventAsync(dto, userId);

        // Invite guardians if requested and student is selected
        if (Input.InviteGuardians && Input.StudentId.HasValue)
        {
            await _consultationService.InviteStudentGuardiansAsync(evt.Id, Input.StudentId.Value);
        }

        // Invite staff if requested and student is selected
        if (Input.InviteStaff && Input.StudentId.HasValue)
        {
            await _consultationService.InviteStudentStaffAsync(evt.Id, Input.StudentId.Value);
        }

        TempData["SuccessMessage"] = _localizer["CalendarPage.MsgEventCreated"].Value;
        return RedirectToPage("Details", new { id = evt.Id });
    }

    private async Task PopulateOptions()
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

        var students = await _studentService.GetBySchoolAsync(SchoolId);
        StudentOptions = new List<SelectListItem>
        {
            new SelectListItem(_localizer["CalendarPage.NoStudentBinding"], "")
        };
        StudentOptions.AddRange(students
            .OrderBy(s => s.LastName)
            .Select(s => new SelectListItem($"{s.LastName} {s.FirstName}", s.Id.ToString())));
    }
}
