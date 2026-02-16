using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SpecEdu.Web.App.Pages.Account;

[Authorize]
public class ProfileModel : PageModel
{
    public void OnGet()
    {
    }
}
