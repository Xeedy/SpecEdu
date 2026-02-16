using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpecEdu.Infrastructure.Identity;

namespace SpecEdu.Web.App.Pages.Account;

public class LogoutModel(
    SignInManager<ApplicationUser> signInManager,
    ILogger<LogoutModel> logger,
    IConfiguration configuration) : PageModel
{

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await signInManager.SignOutAsync();
        logger.LogInformation("User logged out");
        var publicUrl = configuration["PublicUrl"] ?? "https://localhost:5000";
        return Redirect(publicUrl);
    }
}
