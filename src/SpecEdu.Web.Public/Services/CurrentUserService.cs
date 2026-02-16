using System.Security.Claims;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Infrastructure.Identity;

namespace SpecEdu.Web.Public.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public string? UserId => User?.FindFirstValue(ClaimTypes.NameIdentifier);

    public Guid? SchoolId
    {
        get
        {
            var schoolIdClaim = User?.FindFirstValue(JwtTokenService.SchoolIdClaimType);
            if (string.IsNullOrEmpty(schoolIdClaim)) return null;
            return Guid.TryParse(schoolIdClaim, out var schoolId) ? schoolId : null;
        }
    }

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    public IReadOnlyList<string> Roles =>
        User?.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList()
            .AsReadOnly()
        ?? new List<string>().AsReadOnly();

    public bool IsInRole(string role) => User?.IsInRole(role) ?? false;
}
