namespace SpecEdu.Application.Common.Models;

/// <summary>
/// Data transfer object for ApplicationUser.
/// Used to transfer user data without exposing internal implementation.
/// </summary>
public class ApplicationUserDto
{
    /// <summary>
    /// Unique identifier of the user.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// User's email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's first name.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// User's last name.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// User's full name (FirstName + LastName).
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();

    /// <summary>
    /// Identifier of the school this user belongs to.
    /// Null for global administrators.
    /// </summary>
    public Guid? SchoolId { get; set; }

    /// <summary>
    /// Name of the school this user belongs to.
    /// </summary>
    public string? SchoolName { get; set; }

    /// <summary>
    /// List of roles assigned to this user.
    /// </summary>
    public IList<string> Roles { get; set; } = new List<string>();

    /// <summary>
    /// Indicates whether the user account is active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// UTC timestamp when the user was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
