using System.Security.Claims;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Infrastructure.Identity;

namespace SpecEdu.Web.Services;

/// <summary>
/// Provides access to the current authenticated user's information.
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    /// <summary>
    /// Gets the current user's ID from the ClaimsPrincipal.
    /// Returns null if no user is authenticated.
    /// </summary>
    public string? UserId => User?.FindFirstValue(ClaimTypes.NameIdentifier);

    /// <summary>
    /// Gets the school ID of the currently authenticated user.
    /// Returns null if user has no school or is not authenticated.
    /// </summary>
    public Guid? SchoolId
    {
        get
        {
            var schoolIdClaim = User?.FindFirstValue(JwtTokenService.SchoolIdClaimType);
            if (string.IsNullOrEmpty(schoolIdClaim)) return null;
            return Guid.TryParse(schoolIdClaim, out var schoolId) ? schoolId : null;
        }
    }

    /// <summary>
    /// Gets whether the current user is authenticated.
    /// </summary>
    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    /// <summary>
    /// Gets the roles of the currently authenticated user.
    /// </summary>
    public IReadOnlyList<string> Roles =>
        User?.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList()
            .AsReadOnly()
        ?? new List<string>().AsReadOnly();

    /// <summary>
    /// Checks if the current user is in the specified role.
    /// </summary>
    public bool IsInRole(string role) => User?.IsInRole(role) ?? false;
}
