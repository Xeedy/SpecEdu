using SpecEdu.Domain.Constants;

namespace SpecEdu.Infrastructure.Authorization;

/// <summary>
/// Defines the mapping between roles and their permissions.
/// This is the central location for role-based permission assignments.
/// </summary>
public static class RolePermissions
{
    /// <summary>
    /// Gets the permissions for a specific role.
    /// </summary>
    /// <param name="role">The role name.</param>
    /// <returns>Array of permission strings for the role.</returns>
    public static string[] GetPermissionsForRole(string role)
    {
        return role switch
        {
            Roles.Admin => AdminPermissions,
            Roles.SchoolAdmin => SchoolAdminPermissions,
            Roles.Teacher => TeacherPermissions,
            Roles.Parent => ParentPermissions,
            Roles.SPP => SppPermissions,
            Roles.PPP => PppPermissions,
            Roles.SPC => SpcPermissions,
            Roles.Assistant => AssistantPermissions,
            _ => Array.Empty<string>()
        };
    }

    /// <summary>
    /// Checks if a role has a specific permission.
    /// </summary>
    public static bool RoleHasPermission(string role, string permission)
    {
        var permissions = GetPermissionsForRole(role);
        return permissions.Contains(permission);
    }

    /// <summary>
    /// Admin (global) - full system access.
    /// </summary>
    private static readonly string[] AdminPermissions =
    [
        Permissions.System.SystemAdmin,
        Permissions.System.ManageSchools,

        Permissions.Administration.ManageSchool,
        Permissions.Administration.ManageUsers,
        Permissions.Administration.ManageRoles,
        Permissions.Administration.ViewAuditLog,

        Permissions.Student.View,
        Permissions.Student.Edit,
        Permissions.Student.Create,
        Permissions.Student.Delete,

        Permissions.Document.View,
        Permissions.Document.Edit,
        Permissions.Document.Create,
        Permissions.Document.Delete
    ];

    /// <summary>
    /// SchoolAdmin - full access within their school.
    /// </summary>
    private static readonly string[] SchoolAdminPermissions =
    [
        Permissions.Administration.ManageSchool,
        Permissions.Administration.ManageUsers,
        Permissions.Administration.ManageRoles,
        Permissions.Administration.ViewAuditLog,

        Permissions.Student.View,
        Permissions.Student.Edit,
        Permissions.Student.Create,
        Permissions.Student.Delete,

        Permissions.Document.View,
        Permissions.Document.Edit,
        Permissions.Document.Create,
        Permissions.Document.Delete
    ];

    /// <summary>
    /// Teacher - manages students and documentation.
    /// </summary>
    private static readonly string[] TeacherPermissions =
    [
        Permissions.Student.View,
        Permissions.Student.Edit,
        Permissions.Student.Create,

        Permissions.Document.View,
        Permissions.Document.Edit,
        Permissions.Document.Create
    ];

    /// <summary>
    /// Parent - view-only access to their child's data.
    /// </summary>
    private static readonly string[] ParentPermissions =
    [
        Permissions.Student.View,
        Permissions.Document.View
    ];

    /// <summary>
    /// SPP (Školní poradenské pracoviště) - school counseling.
    /// </summary>
    private static readonly string[] SppPermissions =
    [
        Permissions.Student.View,
        Permissions.Student.Edit,

        Permissions.Document.View,
        Permissions.Document.Edit,
        Permissions.Document.Create
    ];

    /// <summary>
    /// PPP (Pedagogicko-psychologická poradna) - external counseling.
    /// </summary>
    private static readonly string[] PppPermissions =
    [
        Permissions.Student.View,
        Permissions.Student.Edit,

        Permissions.Document.View,
        Permissions.Document.Edit,
        Permissions.Document.Create
    ];

    /// <summary>
    /// SPC (Speciálně pedagogické centrum) - special education center.
    /// </summary>
    private static readonly string[] SpcPermissions =
    [
        Permissions.Student.View,
        Permissions.Student.Edit,

        Permissions.Document.View,
        Permissions.Document.Edit,
        Permissions.Document.Create
    ];

    /// <summary>
    /// Assistant - limited access to assigned students.
    /// </summary>
    private static readonly string[] AssistantPermissions =
    [
        Permissions.Student.View,
        Permissions.Document.View
    ];
}
