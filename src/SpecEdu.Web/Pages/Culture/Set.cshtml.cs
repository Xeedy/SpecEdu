using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SpecEdu.Web.Pages.Culture;

public class SetModel : PageModel
{
    public IActionResult OnGet(string culture, string returnUrl = "/")
    {
        if (string.IsNullOrWhiteSpace(culture))
            culture = "cs";

        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true,
                Path = "/"
            });

        if (!Url.IsLocalUrl(returnUrl))
            returnUrl = "/";

        return LocalRedirect(returnUrl);
    }
}
