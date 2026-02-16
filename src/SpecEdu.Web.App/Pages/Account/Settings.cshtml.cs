using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SpecEdu.Web.App.Pages.Account;

[Authorize]
public class SettingsModel : PageModel
{
    public void OnGet()
    {
    }
}
