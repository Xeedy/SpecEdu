namespace SpecEdu.Application.Common.Models;

public class AuthResult
{
    public bool Succeeded { get; set; }

    public string? Token { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public ApplicationUserDto? User { get; set; }

    public IList<string> Errors { get; set; } = new List<string>();

    public bool IsLockedOut { get; set; }

    public bool RequiresEmailConfirmation { get; set; }

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

    public static AuthResult Failure(params string[] errors)
    {
        return new AuthResult
        {
            Succeeded = false,
            Errors = errors.ToList()
        };
    }

    public static AuthResult LockedOut()
    {
        return new AuthResult
        {
            Succeeded = false,
            IsLockedOut = true,
            Errors = new List<string> { "Účet je dočasně zablokován kvůli příliš mnoha neúspěšným pokusům o přihlášení." }
        };
    }

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
