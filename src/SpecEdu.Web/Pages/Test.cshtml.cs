using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SpecEdu.Pages
{
    public class TestModel : PageModel
    {
        private readonly ILogger<TestModel> _logger;

        public TestModel(ILogger<TestModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            _logger.LogInformation("Test page accessed");
        }
    }
}
