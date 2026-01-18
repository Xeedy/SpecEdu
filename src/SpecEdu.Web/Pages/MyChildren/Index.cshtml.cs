using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Application.Common.Models;

namespace SpecEdu.Web.Pages.MyChildren;

[Authorize(Roles = "Parent")]
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

    public IList<StudentDto> Children { get; set; } = new List<StudentDto>();

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToPage("/Account/Login");
        }

        Children = await _studentAccessService.GetStudentsForParentAsync(userId);

        return Page();
    }
}
