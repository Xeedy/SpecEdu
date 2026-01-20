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
public class PlppEditModel : PageModel
{
    private readonly IPlppService _plppService;
    private readonly IStudentService _studentService;
    private readonly IStudentAccessService _studentAccessService;
    private readonly ICurrentUserService _currentUserService;

    public PlppEditModel(
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

    public PlppDto Plpp { get; set; } = null!;
    public StudentDto Student { get; set; } = null!;

    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty]
    public PlppInputModel Input { get; set; } = new();

    [BindProperty]
    public GoalInputModel GoalInput { get; set; } = new();

    [BindProperty]
    public EvaluationInputModel EvaluationInput { get; set; } = new();

    public List<SelectListItem> SupportLevelOptions { get; set; } = new();
    public List<SelectListItem> GoalStatusOptions { get; set; } = new();

    [TempData]
    public string? SuccessMessage { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public class PlppInputModel
    {
        [Required(ErrorMessage = "Skolni rok je povinny.")]
        [Display(Name = "Skolni rok")]
        public string SchoolYear { get; set; } = string.Empty;

        [Required(ErrorMessage = "Stupen podpory je povinny.")]
        [Display(Name = "Stupen podpurnych opatreni")]
        public SupportMeasureLevel SupportLevel { get; set; }

        [Required(ErrorMessage = "Datum zacatku je povinne.")]
        [Display(Name = "Platnost od")]
        [DataType(DataType.Date)]
        public DateTime ValidFrom { get; set; }

        [Required(ErrorMessage = "Datum konce je povinne.")]
        [Display(Name = "Platnost do")]
        [DataType(DataType.Date)]
        public DateTime ValidTo { get; set; }

        [Display(Name = "Silne stranky zaka")]
        public string? Strengths { get; set; }

        [Display(Name = "Oblasti vyzadujici podporu")]
        public string? AreasNeedingSupport { get; set; }

        [Display(Name = "Doporucene metody a pristupy")]
        public string? RecommendedMethods { get; set; }

        [Display(Name = "Organizacni upravy")]
        public string? OrganizationalAdjustments { get; set; }

        [Display(Name = "Obsahove upravy")]
        public string? ContentAdjustments { get; set; }

        [Display(Name = "Zpusob hodnoceni")]
        public string? AssessmentMethods { get; set; }

        [Display(Name = "Spoluprace s rodici")]
        public string? ParentCollaboration { get; set; }

        [Display(Name = "Interni poznamky")]
        public string? InternalNotes { get; set; }

        [Display(Name = "Viditelne pro rodice")]
        public bool IsVisibleToParents { get; set; }
    }

    public class GoalInputModel
    {
        public Guid? GoalId { get; set; }

        [Display(Name = "Predmet/Oblast")]
        public string? Subject { get; set; }

        [Required(ErrorMessage = "Popis cile je povinny.")]
        [Display(Name = "Popis cile")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Kriteria uspechu")]
        public string? SuccessCriteria { get; set; }

        [Display(Name = "Metody a strategie")]
        public string? Methods { get; set; }

        [Display(Name = "Odpovedna osoba")]
        public string? ResponsiblePerson { get; set; }

        [Display(Name = "Cilove datum")]
        [DataType(DataType.Date)]
        public DateTime? TargetDate { get; set; }

        [Display(Name = "Stav")]
        public GoalStatus Status { get; set; } = GoalStatus.NotStarted;

        [Display(Name = "Poznamky k pokroku")]
        public string? ProgressNotes { get; set; }
    }

    public class EvaluationInputModel
    {
        public Guid? EvaluationId { get; set; }

        [Required(ErrorMessage = "Mesic hodnoceni je povinny.")]
        [Display(Name = "Mesic hodnoceni")]
        [DataType(DataType.Date)]
        public DateTime EvaluationMonth { get; set; }

        [Display(Name = "Co zak zvlada")]
        public string? WhatStudentManages { get; set; }

        [Display(Name = "Co zak nezvlada / potrebuje podporu")]
        public string? WhatNeedsImprovement { get; set; }

        [Display(Name = "Doporucene upravy")]
        public string? RecommendedAdjustments { get; set; }

        [Display(Name = "Poznamky z konzultace s rodici")]
        public string? ParentConsultationNotes { get; set; }

        [Display(Name = "Celkove hodnoceni pokroku (1-5)")]
        [Range(1, 5, ErrorMessage = "Hodnoceni musi byt v rozmezi 1-5.")]
        public int? ProgressRating { get; set; }

        [Display(Name = "Dalsi poznamky")]
        public string? Notes { get; set; }

        [Display(Name = "Rodice seznameni")]
        public bool ParentsNotified { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var plpp = await _plppService.GetByIdAsync(Id);
        if (plpp == null)
        {
            return NotFound();
        }

        var student = await _studentService.GetByIdAsync(plpp.StudentId);
        if (student == null)
        {
            return NotFound();
        }

        var userId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToPage("/Account/AccessDenied");
        }

        var accessResult = await _studentAccessService.CanAccessStudentAsync(userId, plpp.StudentId);
        if (!accessResult.CanAccess)
        {
            return RedirectToPage("/Account/AccessDenied");
        }

        Plpp = plpp;
        Student = student;
        PopulateOptions();
        PopulateInputFromPlpp(plpp);
        SetDefaultEvaluationMonth();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var plpp = await _plppService.GetByIdAsync(Id);
        if (plpp == null)
        {
            return NotFound();
        }

        var student = await _studentService.GetByIdAsync(plpp.StudentId);
        if (student == null)
        {
            return NotFound();
        }

        Plpp = plpp;
        Student = student;
        PopulateOptions();

        // Clear validation for goal and evaluation inputs when saving PLPP
        ModelState.Remove("GoalInput.Description");
        ModelState.Remove("EvaluationInput.EvaluationMonth");

        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (Input.ValidFrom >= Input.ValidTo)
        {
            ModelState.AddModelError("Input.ValidTo", "Datum konce musi byt po datu zacatku.");
            return Page();
        }

        var dto = new UpdatePlppDto
        {
            Id = Id,
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

        await _plppService.UpdateAsync(dto);
        SuccessMessage = "PLPP byl uspesne ulozen.";

        return RedirectToPage(new { Id });
    }

    public async Task<IActionResult> OnPostAddGoalAsync()
    {
        var plpp = await _plppService.GetByIdAsync(Id);
        if (plpp == null)
        {
            return NotFound();
        }

        // Clear validation for PLPP and evaluation inputs
        foreach (var key in ModelState.Keys.Where(k => k.StartsWith("Input.") || k.StartsWith("EvaluationInput.")).ToList())
        {
            ModelState.Remove(key);
        }

        if (!ModelState.IsValid)
        {
            var student = await _studentService.GetByIdAsync(plpp.StudentId);
            Plpp = plpp;
            Student = student!;
            PopulateOptions();
            PopulateInputFromPlpp(plpp);
            return Page();
        }

        var goalDto = new CreatePlppGoalDto
        {
            PlppId = Id,
            Order = plpp.Goals.Count + 1,
            Subject = GoalInput.Subject,
            Description = GoalInput.Description,
            SuccessCriteria = GoalInput.SuccessCriteria,
            Methods = GoalInput.Methods,
            ResponsiblePerson = GoalInput.ResponsiblePerson,
            TargetDate = GoalInput.TargetDate
        };

        await _plppService.AddGoalAsync(goalDto);
        SuccessMessage = "Cil byl pridan.";

        return RedirectToPage(new { Id });
    }

    public async Task<IActionResult> OnPostUpdateGoalStatusAsync(Guid goalId, GoalStatus status)
    {
        await _plppService.UpdateGoalStatusAsync(goalId, status);
        SuccessMessage = "Stav cile byl aktualizovan.";
        return RedirectToPage(new { Id });
    }

    public async Task<IActionResult> OnPostDeleteGoalAsync(Guid goalId)
    {
        await _plppService.DeleteGoalAsync(goalId);
        SuccessMessage = "Cil byl smazan.";
        return RedirectToPage(new { Id });
    }

    public async Task<IActionResult> OnPostAddEvaluationAsync()
    {
        var plpp = await _plppService.GetByIdAsync(Id);
        if (plpp == null)
        {
            return NotFound();
        }

        // Clear validation for PLPP and goal inputs
        foreach (var key in ModelState.Keys.Where(k => k.StartsWith("Input.") || k.StartsWith("GoalInput.")).ToList())
        {
            ModelState.Remove(key);
        }

        if (!ModelState.IsValid)
        {
            var student = await _studentService.GetByIdAsync(plpp.StudentId);
            Plpp = plpp;
            Student = student!;
            PopulateOptions();
            PopulateInputFromPlpp(plpp);
            return Page();
        }

        var evalDto = new CreatePlppEvaluationDto
        {
            PlppId = Id,
            EvaluationMonth = EvaluationInput.EvaluationMonth,
            WhatStudentManages = EvaluationInput.WhatStudentManages,
            WhatNeedsImprovement = EvaluationInput.WhatNeedsImprovement,
            RecommendedAdjustments = EvaluationInput.RecommendedAdjustments,
            ParentConsultationNotes = EvaluationInput.ParentConsultationNotes,
            ProgressRating = EvaluationInput.ProgressRating,
            Notes = EvaluationInput.Notes
        };

        await _plppService.AddEvaluationAsync(evalDto);
        SuccessMessage = "Hodnoceni bylo pridano.";

        return RedirectToPage(new { Id });
    }

    public async Task<IActionResult> OnPostMarkParentsNotifiedAsync(Guid evaluationId)
    {
        await _plppService.MarkParentsNotifiedAsync(evaluationId);
        SuccessMessage = "Rodice byli oznaceni jako seznameni.";
        return RedirectToPage(new { Id });
    }

    public async Task<IActionResult> OnPostDeleteEvaluationAsync(Guid evaluationId)
    {
        await _plppService.DeleteEvaluationAsync(evaluationId);
        SuccessMessage = "Hodnoceni bylo smazano.";
        return RedirectToPage(new { Id });
    }

    private void PopulateOptions()
    {
        SupportLevelOptions = new List<SelectListItem>
        {
            new SelectListItem("1. stupen", ((int)SupportMeasureLevel.Level1).ToString()),
            new SelectListItem("2. stupen", ((int)SupportMeasureLevel.Level2).ToString()),
            new SelectListItem("3. stupen", ((int)SupportMeasureLevel.Level3).ToString()),
            new SelectListItem("4. stupen", ((int)SupportMeasureLevel.Level4).ToString()),
            new SelectListItem("5. stupen", ((int)SupportMeasureLevel.Level5).ToString())
        };

        GoalStatusOptions = new List<SelectListItem>
        {
            new SelectListItem("Nezahajeno", ((int)GoalStatus.NotStarted).ToString()),
            new SelectListItem("Probiha", ((int)GoalStatus.InProgress).ToString()),
            new SelectListItem("Splneno", ((int)GoalStatus.Achieved).ToString()),
            new SelectListItem("Castecne splneno", ((int)GoalStatus.PartiallyAchieved).ToString()),
            new SelectListItem("Nesplneno", ((int)GoalStatus.NotAchieved).ToString())
        };
    }

    private void PopulateInputFromPlpp(PlppDto plpp)
    {
        Input = new PlppInputModel
        {
            SchoolYear = plpp.SchoolYear,
            SupportLevel = plpp.SupportLevel,
            ValidFrom = plpp.ValidFrom,
            ValidTo = plpp.ValidTo,
            Strengths = plpp.Strengths,
            AreasNeedingSupport = plpp.AreasNeedingSupport,
            RecommendedMethods = plpp.RecommendedMethods,
            OrganizationalAdjustments = plpp.OrganizationalAdjustments,
            ContentAdjustments = plpp.ContentAdjustments,
            AssessmentMethods = plpp.AssessmentMethods,
            ParentCollaboration = plpp.ParentCollaboration,
            InternalNotes = plpp.InternalNotes,
            IsVisibleToParents = plpp.IsVisibleToParents
        };
    }

    private void SetDefaultEvaluationMonth()
    {
        var now = DateTime.UtcNow;
        EvaluationInput.EvaluationMonth = new DateTime(now.Year, now.Month, 1);
    }

    public static string GetGoalStatusName(GoalStatus status)
    {
        return status switch
        {
            GoalStatus.NotStarted => "Nezahajeno",
            GoalStatus.InProgress => "Probiha",
            GoalStatus.Achieved => "Splneno",
            GoalStatus.PartiallyAchieved => "Castecne splneno",
            GoalStatus.NotAchieved => "Nesplneno",
            _ => status.ToString()
        };
    }

    public static string GetGoalStatusBadgeClass(GoalStatus status)
    {
        return status switch
        {
            GoalStatus.NotStarted => "bg-secondary",
            GoalStatus.InProgress => "bg-primary",
            GoalStatus.Achieved => "bg-success",
            GoalStatus.PartiallyAchieved => "bg-warning text-dark",
            GoalStatus.NotAchieved => "bg-danger",
            _ => "bg-secondary"
        };
    }

    public static string GetStatusName(PlppStatus status)
    {
        return status switch
        {
            PlppStatus.Draft => "Koncept",
            PlppStatus.Active => "Aktivni",
            PlppStatus.UnderReview => "K revizi",
            PlppStatus.Archived => "Archivovano",
            _ => status.ToString()
        };
    }

    public static string GetStatusBadgeClass(PlppStatus status)
    {
        return status switch
        {
            PlppStatus.Draft => "bg-secondary",
            PlppStatus.Active => "bg-success",
            PlppStatus.UnderReview => "bg-warning text-dark",
            PlppStatus.Archived => "bg-dark",
            _ => "bg-primary"
        };
    }
}
