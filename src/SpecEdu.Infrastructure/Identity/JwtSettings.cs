namespace SpecEdu.Infrastructure.Identity;

/// <summary>
/// Configuration settings for JWT token generation and validation.
/// Mapped from appsettings.json "JwtSettings" section.
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// Configuration section name in appsettings.json.
    /// </summary>
    public const string SectionName = "JwtSettings";

    /// <summary>
    /// Secret key used to sign JWT tokens.
    /// Must be at least 32 characters for security.
    /// </summary>
    public string Secret { get; set; } = string.Empty;

    /// <summary>
    /// Token issuer (typically your application URL).
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Token audience (typically your application URL).
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Token expiration time in minutes.
    /// Default: 60 minutes (1 hour).
    /// </summary>
    public int ExpirationInMinutes { get; set; } = 60;

    /// <summary>
    /// Refresh token expiration time in days.
    /// Default: 7 days.
    /// </summary>
    public int RefreshTokenExpirationInDays { get; set; } = 7;
}
