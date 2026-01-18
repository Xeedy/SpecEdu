namespace SpecEdu.Application.Common.Models;

/// <summary>
/// Result of an authentication operation.
/// Contains success status, tokens, and error information.
/// </summary>
public class AuthResult
{
    /// <summary>
    /// Indicates whether the authentication was successful.
    /// </summary>
    public bool Succeeded { get; set; }

    /// <summary>
    /// JWT access token (only populated on success).
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    /// Refresh token for obtaining new access tokens (only populated on success).
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Token expiration time in UTC (only populated on success).
    /// </summary>
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// User information (only populated on success).
    /// </summary>
    public ApplicationUserDto? User { get; set; }

    /// <summary>
    /// Error messages if authentication failed.
    /// </summary>
    public IList<string> Errors { get; set; } = new List<string>();

    /// <summary>
    /// Indicates if the account is locked out.
    /// </summary>
    public bool IsLockedOut { get; set; }

    /// <summary>
    /// Indicates if email confirmation is required.
    /// </summary>
    public bool RequiresEmailConfirmation { get; set; }

    /// <summary>
    /// Creates a successful authentication result.
    /// </summary>
    public static AuthResult Success(string token, string refreshToken, DateTime expiresAt, ApplicationUserDto user)
    {
        return new AuthResult
        {
            Succeeded = true,
            Token = token,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt,
            User = user
        };
    }

    /// <summary>
    /// Creates a failed authentication result.
    /// </summary>
    public static AuthResult Failure(params string[] errors)
    {
        return new AuthResult
        {
            Succeeded = false,
            Errors = errors.ToList()
        };
    }

    /// <summary>
    /// Creates a locked out authentication result.
    /// </summary>
    public static AuthResult LockedOut()
    {
        return new AuthResult
        {
            Succeeded = false,
            IsLockedOut = true,
            Errors = new List<string> { "Účet je dočasně zablokován kvůli příliš mnoha neúspěšným pokusům o přihlášení." }
        };
    }

    /// <summary>
    /// Creates a result requiring email confirmation.
    /// </summary>
    public static AuthResult EmailNotConfirmed()
    {
        return new AuthResult
        {
            Succeeded = false,
            RequiresEmailConfirmation = true,
            Errors = new List<string> { "E-mailová adresa nebyla potvrzena." }
        };
    }
}
