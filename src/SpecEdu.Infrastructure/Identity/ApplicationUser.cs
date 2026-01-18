using Microsoft.AspNetCore.Identity;
using SpecEdu.Domain.Entities;

namespace SpecEdu.Infrastructure.Identity;

/// <summary>
/// Custom user entity extending IdentityUser with additional properties.
/// Used for authentication and user management in SpecEdu.
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// User's first name.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// User's last name.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// UTC timestamp when the user was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// UTC timestamp when the user was last modified.
    /// </summary>
    public DateTime? ModifiedAt { get; set; }

    /// <summary>
    /// Indicates whether the user account is active.
    /// Inactive users cannot log in.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Identifier of the school this user belongs to.
    /// Null for global administrators who are not tied to a specific school.
    /// </summary>
    public Guid? SchoolId { get; set; }

    /// <summary>
    /// Navigation property to the user's school.
    /// </summary>
    public School? School { get; set; }

    /// <summary>
    /// Gets the user's full name.
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();
}
