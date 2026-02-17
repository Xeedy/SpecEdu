using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Application.Common.Models;
using SpecEdu.Infrastructure.Authorization;

namespace SpecEdu.Web.App.Pages.Admin.AuditLog;

[Authorize(Policy = AuthorizationPolicies.RequireAdmin)]
public class IndexModel(IAuditService auditService) : PageModel
{
    public IList<AuditLogDto> Logs { get; set; } = new List<AuditLogDto>();

    public int TotalCount { get; set; }

    public int CurrentPage { get; set; } = 1;

    public int PageSize { get; set; } = 50;

    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    [BindProperty(SupportsGet = true)]
    public DateTime? FromDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? ToDate { get; set; }

    public async Task OnGetAsync(int? page)
    {
        CurrentPage = page ?? 1;
        if (CurrentPage < 1) CurrentPage = 1;

        var (logs, totalCount) = await auditService.GetLogsAsync(
            page: CurrentPage,
            pageSize: PageSize,
            fromDate: FromDate,
            toDate: ToDate);

        Logs = logs;
        TotalCount = totalCount;
    }
}
