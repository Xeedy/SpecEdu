using SpecEdu.Application.Common.Models;

namespace SpecEdu.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<AuthResult> AuthenticateAsync(string email, string password);

    Task<ApplicationUserDto?> GetUserByIdAsync(string userId);

    Task<ApplicationUserDto?> GetUserByEmailAsync(string email);

    Task<IList<ApplicationUserDto>> GetUsersBySchoolAsync(Guid schoolId);

    Task<(bool Succeeded, ApplicationUserDto? User, IList<string> Errors)> CreateUserAsync(
        string email,
        string password,
        string firstName,
        string lastName,
        Guid? schoolId,
        IEnumerable<string> roles);

    Task<bool> UpdateUserAsync(string userId, string firstName, string lastName, bool isActive);

    Task<bool> DeleteUserAsync(string userId);

    Task<bool> AddToRoleAsync(string userId, string role);

    Task<bool> RemoveFromRoleAsync(string userId, string role);

    Task<IList<string>> GetRolesAsync(string userId);

    Task<bool> IsInRoleAsync(string userId, string role);

    Task<(bool Succeeded, IList<string> Errors)> ChangePasswordAsync(
        string userId,
        string currentPassword,
        string newPassword);

    Task<(bool Succeeded, IList<string> Errors)> ResetPasswordAsync(string userId, string newPassword);
}
