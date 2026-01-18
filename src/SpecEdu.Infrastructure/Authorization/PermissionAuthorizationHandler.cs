using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace SpecEdu.Infrastructure.Authorization;

/// <summary>
/// Authorization handler that evaluates permission requirements.
/// Checks if the user's roles grant them the required permission.
/// </summary>
public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var roles = context.User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        foreach (var role in roles)
        {
            if (RolePermissions.RoleHasPermission(role, requirement.Permission))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
        }

        return Task.CompletedTask;
    }
}
