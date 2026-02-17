using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpecEdu.Application.Common.Interfaces;

namespace SpecEdu.Web.App.Pages.Account;

[Authorize]
public class DataExportModel(
    IGdprService gdprService,
    ICurrentUserService currentUserService) : PageModel
{
    public IDictionary<string, string> RetentionPolicy { get; set; } = new Dictionary<string, string>();

    public void OnGet()
    {
        RetentionPolicy = gdprService.GetDataRetentionPolicy();
    }

    public async Task<IActionResult> OnPostDownloadAsync()
    {
        var userId = currentUserService.UserId;
        if (userId == null) return RedirectToPage("/Account/Login");

        var json = await gdprService.ExportUserDataAsync(userId);
        var bytes = Encoding.UTF8.GetBytes(json);

        return File(bytes, "application/json", $"specedu-data-export-{DateTime.UtcNow:yyyyMMdd}.json");
    }
}
