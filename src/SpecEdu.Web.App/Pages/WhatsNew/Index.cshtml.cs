using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpecEdu.Application.Common;

namespace SpecEdu.Web.App.Pages.WhatsNew;

[Authorize]
public class IndexModel : PageModel
{
    public IReadOnlyList<ChangelogEntry> Entries => ChangelogEntries.All;

    public void OnGet()
    {
    }
}
