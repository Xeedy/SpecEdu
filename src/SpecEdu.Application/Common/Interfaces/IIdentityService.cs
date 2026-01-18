using SpecEdu.Application.Common.Models;

namespace SpecEdu.Application.Common.Interfaces;

/// <summary>
/// Service for user identity management operations.
/// Implements user authentication, registration, and management.
/// </summary>
public interface IIdentityService
{
    /// <summary>
    /// Authenticates a user with email and password.
    /// </summary>
    /// <param name="email">User's email address.</param>
    /// <param name="password">User's password.</param>
    /// <returns>Authentication result with token if successful.</returns>
    Task<AuthResult> AuthenticateAsync(string email, string password);

    /// <summary>
    /// Gets a user by their unique identifier.
    /// </summary>
    /// <param name="userId">The user's ID.</param>
    /// <returns>User DTO or null if not found.</returns>
    Task<ApplicationUserDto?> GetUserByIdAsync(string userId);

    /// <summary>
    /// Gets a user by their email address.
    /// </summary>
    /// <param name="email">The user's email.</param>
    /// <returns>User DTO or null if not found.</returns>
    Task<ApplicationUserDto?> GetUserByEmailAsync(string email);

    /// <summary>
    /// Gets all users in a specific school.
    /// </summary>
    /// <param name="schoolId">The school's ID.</param>
    /// <returns>List of users in the school.</returns>
    Task<IList<ApplicationUserDto>> GetUsersBySchoolAsync(Guid schoolId);

    /// <summary>
    /// Creates a new user account.
    /// </summary>
    /// <param name="email">User's email address.</param>
    /// <param name="password">User's password.</param>
    /// <param name="firstName">User's first name.</param>
    /// <param name="lastName">User's last name.</param>
    /// <param name="schoolId">Optional school ID for the user.</param>
    /// <param name="roles">Roles to assign to the user.</param>
    /// <returns>Result containing user info or errors.</returns>
    Task<(bool Succeeded, ApplicationUserDto? User, IList<string> Errors)> CreateUserAsync(
        string email,
        string password,
        string firstName,
        string lastName,
        Guid? schoolId,
        IEnumerable<string> roles);

    /// <summary>
    /// Updates an existing user's information.
    /// </summary>
    /// <param name="userId">The user's ID.</param>
    /// <param name="firstName">New first name.</param>
    /// <param name="lastName">New last name.</param>
    /// <param name="isActive">Whether the user is active.</param>
    /// <returns>True if update was successful.</returns>
    Task<bool> UpdateUserAsync(string userId, string firstName, string lastName, bool isActive);

    /// <summary>
    /// Deletes a user account.
    /// </summary>
    /// <param name="userId">The user's ID.</param>
    /// <returns>True if deletion was successful.</returns>
    Task<bool> DeleteUserAsync(string userId);

    /// <summary>
    /// Adds a user to a role.
    /// </summary>
    /// <param name="userId">The user's ID.</param>
    /// <param name="role">The role to add.</param>
    /// <returns>True if successful.</returns>
    Task<bool> AddToRoleAsync(string userId, string role);

    /// <summary>
    /// Removes a user from a role.
    /// </summary>
    /// <param name="userId">The user's ID.</param>
    /// <param name="role">The role to remove.</param>
    /// <returns>True if successful.</returns>
    Task<bool> RemoveFromRoleAsync(string userId, string role);

    /// <summary>
    /// Gets all roles assigned to a user.
    /// </summary>
    /// <param name="userId">The user's ID.</param>
    /// <returns>List of role names.</returns>
    Task<IList<string>> GetRolesAsync(string userId);

    /// <summary>
    /// Checks if a user is in a specific role.
    /// </summary>
    /// <param name="userId">The user's ID.</param>
    /// <param name="role">The role to check.</param>
    /// <returns>True if user is in the role.</returns>
    Task<bool> IsInRoleAsync(string userId, string role);

    /// <summary>
    /// Changes a user's password.
    /// </summary>
    /// <param name="userId">The user's ID.</param>
    /// <param name="currentPassword">Current password.</param>
    /// <param name="newPassword">New password.</param>
    /// <returns>Result indicating success or errors.</returns>
    Task<(bool Succeeded, IList<string> Errors)> ChangePasswordAsync(
        string userId,
        string currentPassword,
        string newPassword);

    /// <summary>
    /// Resets a user's password (admin operation).
    /// </summary>
    /// <param name="userId">The user's ID.</param>
    /// <param name="newPassword">New password.</param>
    /// <returns>Result indicating success or errors.</returns>
    Task<(bool Succeeded, IList<string> Errors)> ResetPasswordAsync(string userId, string newPassword);
}
