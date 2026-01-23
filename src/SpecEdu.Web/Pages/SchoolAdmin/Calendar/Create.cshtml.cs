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
public class CreateModel : PageModel
{
    private readonly IConsultationService _consultationService;
    private readonly IStudentService _studentService;
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUserService;

    public CreateModel(
        IConsultationService consultationService,
        IStudentService studentService,
        IIdentityService identityService,
        ICurrentUserService currentUserService)
    {
        _consultationService = consultationService;
        _studentService = studentService;
        _identityService = identityService;
        _currentUserService = currentUserService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public List<SelectListItem> TypeOptions { get; set; } = new();
    public List<SelectListItem> StudentOptions { get; set; } = new();

    public Guid SchoolId { get; set; }

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
        public ConsultationType Type { get; set; } = ConsultationType.IndividualConsultation;

        [Display(Name = "Zak (volitelne)")]
        public Guid? StudentId { get; set; }

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
        public bool IsVisibleToParents { get; set; } = true;

        [Display(Name = "Umoznit odpovedi")]
        public bool AllowResponses { get; set; } = true;

        [Display(Name = "Pripomenout pred (minuty)")]
        public int? ReminderMinutesBefore { get; set; } = 1440;

        [Display(Name = "Pozvat rodice zaka")]
        public bool InviteGuardians { get; set; } = true;

        [Display(Name = "Pozvat prirazene ucitele")]
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
            ModelState.AddModelError("Input.EndTime", "Cas konce musi byt po casu zacatku.");
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

        if (Input.InviteGuardians && Input.StudentId.HasValue)
        {
            await _consultationService.InviteStudentGuardiansAsync(evt.Id, Input.StudentId.Value);
        }

        if (Input.InviteStaff && Input.StudentId.HasValue)
        {
            await _consultationService.InviteStudentStaffAsync(evt.Id, Input.StudentId.Value);
        }

        TempData["SuccessMessage"] = "Konzultace byla vytvorena.";
        return RedirectToPage("Details", new { id = evt.Id });
    }

    private async Task PopulateOptions()
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

        var students = await _studentService.GetBySchoolAsync(SchoolId);
        StudentOptions = new List<SelectListItem>
        {
            new SelectListItem("-- Bez vazby na zaka --", "")
        };
        StudentOptions.AddRange(students
            .OrderBy(s => s.LastName)
            .Select(s => new SelectListItem($"{s.LastName} {s.FirstName}", s.Id.ToString())));
    }
}
