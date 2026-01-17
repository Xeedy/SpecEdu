namespace SpecEdu.Application.Common.Interfaces;

/// <summary>
/// Service for generating and validating JWT tokens.
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Generates a JWT token for the specified user.
    /// </summary>
    /// <param name="userId">The user's unique identifier.</param>
    /// <param name="email">The user's email address.</param>
    /// <param name="roles">The user's roles.</param>
    /// <returns>A JWT token string.</returns>
    string GenerateToken(string userId, string email, IEnumerable<string> roles);

    /// <summary>
    /// Generates a refresh token.
    /// </summary>
    /// <returns>A refresh token string.</returns>
    string GenerateRefreshToken();
}
