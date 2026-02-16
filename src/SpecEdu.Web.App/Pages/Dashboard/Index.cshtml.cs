using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Application.Common.Models;

namespace SpecEdu.Web.App.Pages.Dashboard;

[Authorize(Policy = "CanViewStudent")]
public class IndexModel : PageModel
{
    private readonly IStudentAccessService _studentAccessService;
    private readonly ICurrentUserService _currentUserService;

    public IndexModel(
        IStudentAccessService studentAccessService,
        ICurrentUserService currentUserService)
    {
        _studentAccessService = studentAccessService;
        _currentUserService = currentUserService;
    }

    public IList<StudentDto> Students { get; set; } = new List<StudentDto>();
    public int StudentCount => Students.Count;

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToPage("/Account/Login");
        }

        Students = await _studentAccessService.GetAccessibleStudentsAsync(userId);

        return Page();
    }
}
