using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Application.Common.Models;
using SpecEdu.Domain.Enums;
using SpecEdu.Infrastructure.Authorization;
using System.ComponentModel.DataAnnotations;

namespace SpecEdu.Web.Pages.SchoolAdmin.Students;

[Authorize(Policy = AuthorizationPolicies.RequireSchoolAdmin)]
public class PlppCreateModel : PageModel
{
    private readonly IPlppService _plppService;
    private readonly IStudentService _studentService;
    private readonly IStudentAccessService _studentAccessService;
    private readonly ICurrentUserService _currentUserService;

    public PlppCreateModel(
        IPlppService plppService,
        IStudentService studentService,
        IStudentAccessService studentAccessService,
        ICurrentUserService currentUserService)
    {
        _plppService = plppService;
        _studentService = studentService;
        _studentAccessService = studentAccessService;
        _currentUserService = currentUserService;
    }

    public StudentDto Student { get; set; } = null!;

    [BindProperty(SupportsGet = true)]
    public Guid StudentId { get; set; }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public List<SelectListItem> SupportLevelOptions { get; set; } = new();

    public class InputModel
    {
        [Required(ErrorMessage = "Skolni rok je povinny.")]
        [Display(Name = "Skolni rok")]
        public string SchoolYear { get; set; } = string.Empty;

        [Required(ErrorMessage = "Stupen podpory je povinny.")]
        [Display(Name = "Stupen podpurnych opatreni")]
        public SupportMeasureLevel SupportLevel { get; set; } = SupportMeasureLevel.Level2;

        [Required(ErrorMessage = "Datum zacatku je povinne.")]
        [Display(Name = "Platnost od")]
        [DataType(DataType.Date)]
        public DateTime ValidFrom { get; set; }

        [Required(ErrorMessage = "Datum konce je povinne.")]
        [Display(Name = "Platnost do")]
        [DataType(DataType.Date)]
        public DateTime ValidTo { get; set; }

        [Display(Name = "Silne stranky zaka")]
        [MaxLength(4000)]
        public string? Strengths { get; set; }

        [Display(Name = "Oblasti vyzadujici podporu")]
        [MaxLength(4000)]
        public string? AreasNeedingSupport { get; set; }

        [Display(Name = "Doporucene metody a pristupy")]
        [MaxLength(4000)]
        public string? RecommendedMethods { get; set; }

        [Display(Name = "Organizacni upravy")]
        [MaxLength(4000)]
        public string? OrganizationalAdjustments { get; set; }

        [Display(Name = "Obsahove upravy")]
        [MaxLength(4000)]
        public string? ContentAdjustments { get; set; }

        [Display(Name = "Zpusob hodnoceni")]
        [MaxLength(4000)]
        public string? AssessmentMethods { get; set; }

        [Display(Name = "Spoluprace s rodici")]
        [MaxLength(4000)]
        public string? ParentCollaboration { get; set; }

        [Display(Name = "Interni poznamky")]
        [MaxLength(4000)]
        public string? InternalNotes { get; set; }

        [Display(Name = "Viditelne pro rodice")]
        public bool IsVisibleToParents { get; set; } = true;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var student = await _studentService.GetByIdAsync(StudentId);
        if (student == null)
        {
            return NotFound();
        }

        var userId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToPage("/Account/AccessDenied");
        }

        var accessResult = await _studentAccessService.CanAccessStudentAsync(userId, StudentId);
        if (!accessResult.CanAccess)
        {
            return RedirectToPage("/Account/AccessDenied");
        }

        Student = student;
        PopulateSupportLevelOptions();
        SetDefaultValues();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var student = await _studentService.GetByIdAsync(StudentId);
        if (student == null)
        {
            return NotFound();
        }

        var userId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToPage("/Account/AccessDenied");
        }

        Student = student;
        PopulateSupportLevelOptions();

        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (Input.ValidFrom >= Input.ValidTo)
        {
            ModelState.AddModelError("Input.ValidTo", "Datum konce musi byt po datu zacatku.");
            return Page();
        }

        var dto = new CreatePlppDto
        {
            StudentId = StudentId,
            SchoolYear = Input.SchoolYear,
            SupportLevel = Input.SupportLevel,
            ValidFrom = Input.ValidFrom,
            ValidTo = Input.ValidTo,
            Strengths = Input.Strengths,
            AreasNeedingSupport = Input.AreasNeedingSupport,
            RecommendedMethods = Input.RecommendedMethods,
            OrganizationalAdjustments = Input.OrganizationalAdjustments,
            ContentAdjustments = Input.ContentAdjustments,
            AssessmentMethods = Input.AssessmentMethods,
            ParentCollaboration = Input.ParentCollaboration,
            InternalNotes = Input.InternalNotes,
            IsVisibleToParents = Input.IsVisibleToParents
        };

        var plpp = await _plppService.CreateAsync(dto);

        TempData["SuccessMessage"] = "PLPP byl uspesne vytvoren.";
        return RedirectToPage("PlppEdit", new { id = plpp.Id });
    }

    private void PopulateSupportLevelOptions()
    {
        SupportLevelOptions = new List<SelectListItem>
        {
            new SelectListItem("1. stupen - bez doporuceni PPP/SPC", ((int)SupportMeasureLevel.Level1).ToString()),
            new SelectListItem("2. stupen - s doporucenim PPP/SPC", ((int)SupportMeasureLevel.Level2).ToString()),
            new SelectListItem("3. stupen - s doporucenim PPP/SPC", ((int)SupportMeasureLevel.Level3).ToString()),
            new SelectListItem("4. stupen - s doporucenim PPP/SPC", ((int)SupportMeasureLevel.Level4).ToString()),
            new SelectListItem("5. stupen - s doporucenim PPP/SPC", ((int)SupportMeasureLevel.Level5).ToString())
        };
    }

    private void SetDefaultValues()
    {
        Input.SchoolYear = _plppService.GetCurrentSchoolYear();

        var now = DateTime.UtcNow;
        var year = now.Month >= 9 ? now.Year : now.Year - 1;
        Input.ValidFrom = new DateTime(year, 9, 1);
        Input.ValidTo = new DateTime(year + 1, 8, 31);
    }
}
