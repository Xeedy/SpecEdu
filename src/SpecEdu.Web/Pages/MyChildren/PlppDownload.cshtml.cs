using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpecEdu.Application.Common.Interfaces;

namespace SpecEdu.Web.Pages.MyChildren;

/// <summary>
/// Parent PDF download endpoint for PLPP.
/// Respects IsVisibleToParents flag and excludes internal notes.
/// Czech: Stažení PDF PLPP pro rodiče
/// </summary>
[Authorize(Roles = "Parent")]
public class PlppDownloadModel : PageModel
{
    private readonly IPlppService _plppService;
    private readonly IPdfService _pdfService;
    private readonly IStudentService _studentService;
    private readonly ISchoolService _schoolService;
    private readonly IStudentAccessService _studentAccessService;
    private readonly ICurrentUserService _currentUserService;

    public PlppDownloadModel(
        IPlppService plppService,
        IPdfService pdfService,
        IStudentService studentService,
        ISchoolService schoolService,
        IStudentAccessService studentAccessService,
        ICurrentUserService currentUserService)
    {
        _plppService = plppService;
        _pdfService = pdfService;
        _studentService = studentService;
        _schoolService = schoolService;
        _studentAccessService = studentAccessService;
        _currentUserService = currentUserService;
    }

    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToPage("/Account/Login");
        }

        var plpp = await _plppService.GetByIdAsync(Id);
        if (plpp == null)
        {
            return NotFound();
        }

        // Check if PLPP is visible to parents
        if (!plpp.IsVisibleToParents)
        {
            return RedirectToPage("/Account/AccessDenied");
        }

        var student = await _studentService.GetByIdAsync(plpp.StudentId);
        if (student == null)
        {
            return NotFound();
        }

        // Check if user (parent) has access to this student
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

        // Create a copy of PLPP DTO without internal notes for parents
        var parentPlpp = new Application.Common.Models.PlppDto
        {
            Id = plpp.Id,
            StudentId = plpp.StudentId,
            StudentName = plpp.StudentName,
            SchoolYear = plpp.SchoolYear,
            Status = plpp.Status,
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
            InternalNotes = null, // Never include internal notes for parents
            IsVisibleToParents = plpp.IsVisibleToParents,
            ActivatedAt = plpp.ActivatedAt,
            ActivatedBy = plpp.ActivatedBy,
            CreatedAt = plpp.CreatedAt,
            ModifiedAt = plpp.ModifiedAt,
            IsActive = plpp.IsActive,
            Goals = plpp.Goals,
            Evaluations = plpp.Evaluations
        };

        // Generate PDF without internal notes
        var pdfBytes = await _pdfService.GeneratePlppPdfAsync(
            parentPlpp,
            schoolName,
            schoolAddress,
            schoolIco,
            includeInternalNotes: false);

        var filename = _pdfService.GetPlppPdfFilename(student.LastName, plpp.SchoolYear);

        return File(pdfBytes, "application/pdf", filename);
    }
}
