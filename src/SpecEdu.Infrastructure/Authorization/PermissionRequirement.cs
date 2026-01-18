using Microsoft.AspNetCore.Authorization;

namespace SpecEdu.Infrastructure.Authorization;

/// <summary>
/// Authorization requirement that checks if a user has a specific permission.
/// Used with policy-based authorization.
/// </summary>
public class PermissionRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// The permission that is required.
    /// </summary>
    public string Permission { get; }

    /// <summary>
    /// Creates a new permission requirement.
    /// </summary>
    /// <param name="permission">The required permission.</param>
    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }
}
