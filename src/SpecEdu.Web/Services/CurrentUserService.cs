using System.Security.Claims;
using SpecEdu.Application.Common.Interfaces;

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

    /// <summary>
    /// Gets the current user's ID from the ClaimsPrincipal.
    /// Returns null if no user is authenticated.
    /// </summary>
    public string? UserId =>
        _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
}
