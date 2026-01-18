using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Application.Common.Models;
using SpecEdu.Infrastructure.Data;

namespace SpecEdu.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly JwtSettings _jwtSettings;
    private readonly ApplicationDbContext _dbContext;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtTokenService jwtTokenService,
        IOptions<JwtSettings> jwtSettings,
        ApplicationDbContext dbContext)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenService = jwtTokenService;
        _jwtSettings = jwtSettings.Value;
        _dbContext = dbContext;
    }

    public async Task<AuthResult> AuthenticateAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            return AuthResult.Failure("Neplatný e-mail nebo heslo.");
        }

        if (!user.IsActive)
        {
            return AuthResult.Failure("Účet je deaktivován. Kontaktujte správce.");
        }

        if (await _userManager.IsLockedOutAsync(user))
        {
            return AuthResult.LockedOut();
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);

        if (result.IsLockedOut)
        {
            return AuthResult.LockedOut();
        }

        if (!result.Succeeded)
        {
            return AuthResult.Failure("Neplatný e-mail nebo heslo.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtTokenService.GenerateToken(user.Id, user.Email!, roles, user.SchoolId);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes);

        var userDto = await MapToUserDto(user);

        return AuthResult.Success(token, refreshToken, expiresAt, userDto);
    }

    public async Task<ApplicationUserDto?> GetUserByIdAsync(string userId)
    {
        var user = await _userManager.Users
            .Include(u => u.School)
            .FirstOrDefaultAsync(u => u.Id == userId);

        return user == null ? null : await MapToUserDto(user);
    }

    public async Task<ApplicationUserDto?> GetUserByEmailAsync(string email)
    {
        var user = await _userManager.Users
            .Include(u => u.School)
            .FirstOrDefaultAsync(u => u.Email == email);

        return user == null ? null : await MapToUserDto(user);
    }

    public async Task<IList<ApplicationUserDto>> GetUsersBySchoolAsync(Guid schoolId)
    {
        var users = await _userManager.Users
            .Include(u => u.School)
            .Where(u => u.SchoolId == schoolId)
            .ToListAsync();

        var result = new List<ApplicationUserDto>();
        foreach (var user in users)
        {
            result.Add(await MapToUserDto(user));
        }
        return result;
    }

    public async Task<(bool Succeeded, ApplicationUserDto? User, IList<string> Errors)> CreateUserAsync(
        string email,
        string password,
        string firstName,
        string lastName,
        Guid? schoolId,
        IEnumerable<string> roles)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            SchoolId = schoolId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            return (false, null, result.Errors.Select(e => e.Description).ToList());
        }

        var rolesList = roles.ToList();
        if (rolesList.Any())
        {
            var roleResult = await _userManager.AddToRolesAsync(user, rolesList);
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                return (false, null, roleResult.Errors.Select(e => e.Description).ToList());
            }
        }

        var userDto = await MapToUserDto(user);
        return (true, userDto, new List<string>());
    }

    public async Task<bool> UpdateUserAsync(string userId, string firstName, string lastName, bool isActive)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        user.FirstName = firstName;
        user.LastName = lastName;
        user.IsActive = isActive;
        user.ModifiedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> AddToRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        var result = await _userManager.AddToRoleAsync(user, role);
        return result.Succeeded;
    }

    public async Task<bool> RemoveFromRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        var result = await _userManager.RemoveFromRoleAsync(user, role);
        return result.Succeeded;
    }

    public async Task<IList<string>> GetRolesAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return new List<string>();

        return await _userManager.GetRolesAsync(user);
    }

    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        return await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<(bool Succeeded, IList<string> Errors)> ChangePasswordAsync(
        string userId,
        string currentPassword,
        string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return (false, new List<string> { "Uživatel nebyl nalezen." });
        }

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

        if (!result.Succeeded)
        {
            return (false, result.Errors.Select(e => e.Description).ToList());
        }

        user.ModifiedAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        return (true, new List<string>());
    }

    public async Task<(bool Succeeded, IList<string> Errors)> ResetPasswordAsync(string userId, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return (false, new List<string> { "Uživatel nebyl nalezen." });
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

        if (!result.Succeeded)
        {
            return (false, result.Errors.Select(e => e.Description).ToList());
        }

        user.ModifiedAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        return (true, new List<string>());
    }

    private async Task<ApplicationUserDto> MapToUserDto(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);

        return new ApplicationUserDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            SchoolId = user.SchoolId,
            SchoolName = user.School?.Name,
            Roles = roles,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };
    }
}
