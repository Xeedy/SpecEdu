using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Application.Common.Models;
using SpecEdu.Domain.Enums;

namespace SpecEdu.Web.Pages.MyChildren;

[Authorize(Roles = "Parent")]
public class ChildDetailsModel : PageModel
{
    private readonly IStudentService _studentService;
    private readonly IStudentAccessService _studentAccessService;
    private readonly IDiaryService _diaryService;
    private readonly ICurrentUserService _currentUserService;

    public ChildDetailsModel(
        IStudentService studentService,
        IStudentAccessService studentAccessService,
        IDiaryService diaryService,
        ICurrentUserService currentUserService)
    {
        _studentService = studentService;
        _studentAccessService = studentAccessService;
        _diaryService = diaryService;
        _currentUserService = currentUserService;
    }

    public StudentDto Child { get; set; } = null!;
    public IList<DiaryEntryDto> RecentDiaryEntries { get; set; } = new List<DiaryEntryDto>();

    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToPage("/Account/Login");
        }

        var accessResult = await _studentAccessService.CanAccessStudentAsync(userId, Id);
        if (!accessResult.CanAccess)
        {
            return RedirectToPage("/Account/AccessDenied");
        }

        var student = await _studentService.GetByIdAsync(Id);
        if (student == null)
        {
            return NotFound();
        }

        Child = student;

        var (entries, _) = await _diaryService.GetEntriesAsync(
            Id,
            page: 1,
            pageSize: 5,
            visibilityFilter: DiaryVisibility.ParentVisible);

        RecentDiaryEntries = entries;

        return Page();
    }
}
