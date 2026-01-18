using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SpecEdu.Pages
{
    public class TestModel(ILogger<TestModel> Log) : PageModel
    {
        public void OnGet()
        {
            Log.LogInformation("Test page accessed");
        }
    }
}
