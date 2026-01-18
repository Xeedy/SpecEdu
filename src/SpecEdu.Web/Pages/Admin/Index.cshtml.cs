using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Infrastructure.Authorization;
using SpecEdu.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace SpecEdu.Web.Pages.Admin;

[Authorize(Policy = AuthorizationPolicies.RequireAdmin)]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IWebHostEnvironment _environment;

    public IndexModel(ApplicationDbContext dbContext, IWebHostEnvironment environment)
    {
        _dbContext = dbContext;
        _environment = environment;
    }

    public int SchoolCount { get; set; }
    public int UserCount { get; set; }
    public string Environment => _environment.EnvironmentName;

    public async Task OnGetAsync()
    {
        SchoolCount = await _dbContext.Schools.CountAsync(s => s.IsActive);
        UserCount = await _dbContext.Users.CountAsync(u => u.IsActive);
    }
}
