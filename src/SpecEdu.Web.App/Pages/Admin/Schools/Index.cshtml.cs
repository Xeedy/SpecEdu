using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Application.Common.Models;
using SpecEdu.Infrastructure.Authorization;

namespace SpecEdu.Web.App.Pages.Admin.Schools;

[Authorize(Policy = AuthorizationPolicies.RequireAdmin)]
public class IndexModel(ISchoolService schoolService) : PageModel
{
    public IList<SchoolDto> Schools { get; set; } = new List<SchoolDto>();

    [TempData]
    public string? SuccessMessage { get; set; }

    public async Task OnGetAsync()
    {
        Schools = await schoolService.GetAllAsync(includeInactive: true);
    }
}
