using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SpecEdu.Web.Pages.Account;

public class AccessDeniedModel : PageModel
{
    private readonly ILogger<AccessDeniedModel> _logger;

    public AccessDeniedModel(ILogger<AccessDeniedModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        _logger.LogWarning(
            "Access denied for user {UserId} attempting to access {Path}",
            User.Identity?.Name ?? "anonymous",
            HttpContext.Request.Path);
    }
}
