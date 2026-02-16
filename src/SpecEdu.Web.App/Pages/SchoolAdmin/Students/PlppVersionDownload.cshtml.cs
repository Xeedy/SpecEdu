using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Infrastructure.Authorization;

namespace SpecEdu.Web.App.Pages.SchoolAdmin.Students;

[Authorize(Policy = AuthorizationPolicies.RequireSchoolAdmin)]
public class PlppVersionDownloadModel : PageModel
{
    private readonly IPlppVersionService _versionService;
    private readonly IPlppService _plppService;
    private readonly IPdfService _pdfService;
    private readonly IStudentService _studentService;
    private readonly ISchoolService _schoolService;
    private readonly IStudentAccessService _studentAccessService;
    private readonly ICurrentUserService _currentUserService;

    public PlppVersionDownloadModel(
        IPlppVersionService versionService,
        IPlppService plppService,
        IPdfService pdfService,
        IStudentService studentService,
        ISchoolService schoolService,
        IStudentAccessService studentAccessService,
        ICurrentUserService currentUserService)
    {
        _versionService = versionService;
        _plppService = plppService;
        _pdfService = pdfService;
        _studentService = studentService;
        _schoolService = schoolService;
        _studentAccessService = studentAccessService;
        _currentUserService = currentUserService;
    }

    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool IncludeInternalNotes { get; set; } = false;

    public async Task<IActionResult> OnGetAsync()
    {
        var version = await _versionService.GetVersionByIdAsync(Id);
        if (version?.Snapshot == null)
        {
            return NotFound();
        }

        var plpp = await _plppService.GetByIdAsync(version.PlppId);
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

        // Get school info for header
        var school = await _schoolService.GetByIdAsync(student.SchoolId);
        var schoolName = school?.Name ?? "Skola";
        var schoolAddress = school?.Address;
        var schoolIco = school?.Ico;

        // Generate PDF from version snapshot
        var pdfBytes = await _pdfService.GeneratePlppVersionPdfAsync(
            version.Snapshot,
            version.VersionNumber,
            schoolName,
            schoolAddress,
            schoolIco,
            IncludeInternalNotes);

        var filename = $"PLPP_{student.LastName}_v{version.VersionNumber}_{DateTime.UtcNow:yyyyMMdd}.pdf";

        return File(pdfBytes, "application/pdf", filename);
    }
}
