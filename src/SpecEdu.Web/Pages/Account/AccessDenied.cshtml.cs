using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SpecEdu.Web.Pages.Account;

public class AccessDeniedModel(ILogger<AccessDeniedModel> Log) : PageModel
{

    public void OnGet()
    {
        Log.LogWarning(
            "Access denied for user {UserId} attempting to access {Path}",
            User.Identity?.Name ?? "anonymous",
            HttpContext.Request.Path);
    }
}
