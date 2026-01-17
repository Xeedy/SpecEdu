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
}
