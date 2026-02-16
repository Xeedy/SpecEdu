using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Application.Common.Models;
using SpecEdu.Domain.Enums;
using SpecEdu.Infrastructure.Authorization;

namespace SpecEdu.Web.App.Pages.SchoolAdmin.Students;

[Authorize(Policy = AuthorizationPolicies.RequireSchoolAdmin)]
public class PlppsModel : PageModel
{
    private readonly IPlppService _plppService;
    private readonly IStudentService _studentService;
    private readonly IStudentAccessService _studentAccessService;
    private readonly ICurrentUserService _currentUserService;

    public PlppsModel(
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
    public IList<PlppListItemDto> Plpps { get; set; } = new List<PlppListItemDto>();

    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [TempData]
    public string? SuccessMessage { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var student = await _studentService.GetByIdAsync(Id);
        if (student == null)
        {
            return NotFound();
        }

        var userId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToPage("/Account/AccessDenied");
        }

        var accessResult = await _studentAccessService.CanAccessStudentAsync(userId, Id);
        if (!accessResult.CanAccess)
        {
            return RedirectToPage("/Account/AccessDenied");
        }

        Student = student;
        Plpps = await _plppService.GetByStudentIdAsync(Id);

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid plppId)
    {
        var userId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToPage("/Account/AccessDenied");
        }

        var success = await _plppService.DeleteAsync(plppId);
        if (success)
        {
            SuccessMessage = "PLPP byl smazan.";
        }
        else
        {
            ErrorMessage = "Nepodařilo se smazat PLPP.";
        }

        return RedirectToPage(new { Id });
    }

    public async Task<IActionResult> OnPostArchiveAsync(Guid plppId)
    {
        var success = await _plppService.ArchiveAsync(plppId);
        if (success)
        {
            SuccessMessage = "PLPP byl archivovan.";
        }
        else
        {
            ErrorMessage = "Nepodařilo se archivovat PLPP.";
        }

        return RedirectToPage(new { Id });
    }

    public async Task<IActionResult> OnPostActivateAsync(Guid plppId)
    {
        var userId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToPage("/Account/AccessDenied");
        }

        var success = await _plppService.ActivateAsync(plppId, userId);
        if (success)
        {
            SuccessMessage = "PLPP byl aktivovan.";
        }
        else
        {
            ErrorMessage = "Nepodařilo se aktivovat PLPP. PLPP musi byt ve stavu Koncept.";
        }

        return RedirectToPage(new { Id });
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

    public static string GetSupportLevelName(SupportMeasureLevel level)
    {
        return level switch
        {
            SupportMeasureLevel.Level1 => "Stupen 1",
            SupportMeasureLevel.Level2 => "Stupen 2",
            SupportMeasureLevel.Level3 => "Stupen 3",
            SupportMeasureLevel.Level4 => "Stupen 4",
            SupportMeasureLevel.Level5 => "Stupen 5",
            _ => level.ToString()
        };
    }
}
