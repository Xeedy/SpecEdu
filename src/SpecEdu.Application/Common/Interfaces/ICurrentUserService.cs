namespace SpecEdu.Application.Common.Interfaces;

/// <summary>
/// Service for accessing current user information.
/// Implemented in Web layer, used by Infrastructure for audit fields.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Gets the unique identifier of the currently authenticated user.
    /// Returns null if no user is authenticated.
    /// </summary>
    string? UserId { get; }

    /// <summary>
    /// Gets the school identifier of the currently authenticated user.
    /// Returns null if user has no school (global admin) or not authenticated.
    /// </summary>
    Guid? SchoolId { get; }

    /// <summary>
    /// Gets whether the current user is authenticated.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Gets the roles of the currently authenticated user.
    /// Returns empty collection if not authenticated.
    /// </summary>
    IReadOnlyList<string> Roles { get; }

    /// <summary>
    /// Checks if the current user is in the specified role.
    /// </summary>
    bool IsInRole(string role);
}
