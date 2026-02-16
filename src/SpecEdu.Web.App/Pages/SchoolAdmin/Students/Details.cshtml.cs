using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Application.Common.Models;
using SpecEdu.Infrastructure.Authorization;

namespace SpecEdu.Web.App.Pages.SchoolAdmin.Students;

[Authorize(Policy = AuthorizationPolicies.RequireSchoolAdmin)]
public class DetailsModel : PageModel
{
    private readonly IStudentService _studentService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuditService _auditService;

    public DetailsModel(
        IStudentService studentService,
        ICurrentUserService currentUserService,
        IAuditService auditService)
    {
        _studentService = studentService;
        _currentUserService = currentUserService;
        _auditService = auditService;
    }

    public StudentDto Student { get; set; } = null!;
    public IList<StudentGuardianDto> Guardians { get; set; } = new List<StudentGuardianDto>();
    public IList<StudentStaffLinkDto> StaffLinks { get; set; } = new List<StudentStaffLinkDto>();

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var student = await _studentService.GetByIdAsync(id);
        if (student == null)
        {
            return NotFound();
        }

        if (_currentUserService.SchoolId != student.SchoolId)
        {
            return RedirectToPage("/Account/AccessDenied");
        }

        Student = student;
        Guardians = await _studentService.GetGuardiansAsync(id);
        StaffLinks = await _studentService.GetStaffLinksAsync(id);

        await _auditService.LogStudentActionAsync("View", id, "Viewed student details");

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        var student = await _studentService.GetByIdAsync(id);
        if (student == null)
        {
            return NotFound();
        }

        if (_currentUserService.SchoolId != student.SchoolId)
        {
            return RedirectToPage("/Account/AccessDenied");
        }

        await _studentService.DeleteAsync(id);
        await _auditService.LogStudentActionAsync("Delete", id, $"Deleted student {student.FullName}");

        TempData["SuccessMessage"] = $"Žák {student.FullName} byl úspěšně deaktivován.";

        return RedirectToPage("Index");
    }
}
