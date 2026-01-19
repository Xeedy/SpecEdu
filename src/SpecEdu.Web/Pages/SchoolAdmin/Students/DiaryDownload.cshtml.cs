using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Infrastructure.Authorization;

namespace SpecEdu.Web.Pages.SchoolAdmin.Students;

[Authorize(Policy = AuthorizationPolicies.RequireSchoolAdmin)]
public class DiaryDownloadModel : PageModel
{
    private readonly IDiaryService _diaryService;
    private readonly IStudentAccessService _studentAccessService;
    private readonly ICurrentUserService _currentUserService;

    public DiaryDownloadModel(
        IDiaryService diaryService,
        IStudentAccessService studentAccessService,
        ICurrentUserService currentUserService)
    {
        _diaryService = diaryService;
        _studentAccessService = studentAccessService;
        _currentUserService = currentUserService;
    }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var result = await _diaryService.GetAttachmentDataAsync(id);
        if (result == null)
        {
            return NotFound();
        }

        var (fileData, contentType, fileName) = result.Value;

        return File(fileData, contentType, fileName);
    }
}
