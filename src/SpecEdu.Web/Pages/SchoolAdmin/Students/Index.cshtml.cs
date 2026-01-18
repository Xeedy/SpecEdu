using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Application.Common.Models;
using SpecEdu.Infrastructure.Authorization;

namespace SpecEdu.Web.Pages.SchoolAdmin.Students;

[Authorize(Policy = AuthorizationPolicies.RequireSchoolAdmin)]
public class IndexModel : PageModel
{
    private readonly IStudentService _studentService;
    private readonly ICurrentUserService _currentUserService;

    public IndexModel(
        IStudentService studentService,
        ICurrentUserService currentUserService)
    {
        _studentService = studentService;
        _currentUserService = currentUserService;
    }

    public IList<StudentDto> Students { get; set; } = new List<StudentDto>();

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    [BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; } = 1;

    public int TotalCount { get; set; }
    public int PageSize { get; set; } = 20;
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    [TempData]
    public string? SuccessMessage { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var schoolId = _currentUserService.SchoolId;
        if (!schoolId.HasValue)
        {
            return RedirectToPage("/Account/AccessDenied");
        }

        var (students, totalCount) = await _studentService.GetPagedAsync(
            schoolId.Value,
            CurrentPage,
            PageSize,
            SearchTerm,
            includeInactive: true);

        Students = students;
        TotalCount = totalCount;

        return Page();
    }
}
