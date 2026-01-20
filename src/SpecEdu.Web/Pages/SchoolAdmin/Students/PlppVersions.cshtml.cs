using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Application.Common.Models;
using SpecEdu.Domain.Enums;
using SpecEdu.Infrastructure.Authorization;

namespace SpecEdu.Web.Pages.SchoolAdmin.Students;

[Authorize(Policy = AuthorizationPolicies.RequireSchoolAdmin)]
public class PlppVersionsModel : PageModel
{
    private readonly IPlppService _plppService;
    private readonly IPlppVersionService _versionService;
    private readonly IStudentService _studentService;
    private readonly IStudentAccessService _studentAccessService;
    private readonly ICurrentUserService _currentUserService;

    public PlppVersionsModel(
        IPlppService plppService,
        IPlppVersionService versionService,
        IStudentService studentService,
        IStudentAccessService studentAccessService,
        ICurrentUserService currentUserService)
    {
        _plppService = plppService;
        _versionService = versionService;
        _studentService = studentService;
        _studentAccessService = studentAccessService;
        _currentUserService = currentUserService;
    }

    public PlppDto Plpp { get; set; } = null!;
    public StudentDto Student { get; set; } = null!;
    public IList<PlppVersionListItemDto> Versions { get; set; } = new List<PlppVersionListItemDto>();

    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [TempData]
    public string? SuccessMessage { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

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
        Versions = await _versionService.GetVersionsAsync(Id);

        return Page();
    }

    public async Task<IActionResult> OnPostRestoreAsync(Guid versionId)
    {
        var userId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToPage("/Account/AccessDenied");
        }

        try
        {
            await _versionService.RestoreVersionAsync(versionId);
            SuccessMessage = "PLPP byl obnoven do vybrane verze.";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Nepodarilo se obnovit verzi: {ex.Message}";
        }

        return RedirectToPage(new { Id });
    }

    public static string GetSourceName(VersionSource source)
    {
        return source switch
        {
            VersionSource.ManualSave => "Ulozeni",
            VersionSource.Activation => "Aktivace",
            VersionSource.Archive => "Archivace",
            VersionSource.GoalChange => "Zmena cile",
            VersionSource.EvaluationChange => "Zmena hodnoceni",
            VersionSource.Restore => "Obnoveni",
            _ => source.ToString()
        };
    }

    public static string GetSourceBadgeClass(VersionSource source)
    {
        return source switch
        {
            VersionSource.ManualSave => "bg-primary",
            VersionSource.Activation => "bg-success",
            VersionSource.Archive => "bg-dark",
            VersionSource.GoalChange => "bg-info",
            VersionSource.EvaluationChange => "bg-info",
            VersionSource.Restore => "bg-warning text-dark",
            _ => "bg-secondary"
        };
    }

    public static string GetStatusName(PlppStatus? status)
    {
        if (!status.HasValue)
        {
            return "-";
        }

        return status.Value switch
        {
            PlppStatus.Draft => "Koncept",
            PlppStatus.Active => "Aktivni",
            PlppStatus.UnderReview => "K revizi",
            PlppStatus.Archived => "Archivovano",
            _ => status.Value.ToString()
        };
    }
}
