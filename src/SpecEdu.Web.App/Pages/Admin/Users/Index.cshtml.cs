using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Application.Common.Models;
using SpecEdu.Infrastructure.Authorization;
using SpecEdu.Infrastructure.Data;

namespace SpecEdu.Web.App.Pages.Admin.Users;

[Authorize(Policy = AuthorizationPolicies.RequireAdmin)]
public class IndexModel(ApplicationDbContext dbContext, IIdentityService identityService) : PageModel
{
    public IList<ApplicationUserDto> Users { get; set; } = new List<ApplicationUserDto>();

    [TempData]
    public string? SuccessMessage { get; set; }

    public async Task OnGetAsync()
    {
        var appUsers = await dbContext.Users
            .Include(u => u.School)
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .ToListAsync();

        foreach (var user in appUsers)
        {
            var roles = await identityService.GetRolesAsync(user.Id);
            Users.Add(new ApplicationUserDto
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                FirstName = user.FirstName,
                LastName = user.LastName,
                SchoolId = user.SchoolId,
                SchoolName = user.School?.Name,
                Roles = roles,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            });
        }
    }
}
