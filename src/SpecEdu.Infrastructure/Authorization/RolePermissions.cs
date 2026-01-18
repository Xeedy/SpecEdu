using SpecEdu.Domain.Constants;

namespace SpecEdu.Infrastructure.Authorization;

public static class RolePermissions
{
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

    public static bool RoleHasPermission(string role, string permission)
    {
        var permissions = GetPermissionsForRole(role);
        return permissions.Contains(permission);
    }

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

    private static readonly string[] TeacherPermissions =
    [
        Permissions.Student.View,
        Permissions.Student.Edit,
        Permissions.Student.Create,

        Permissions.Document.View,
        Permissions.Document.Edit,
        Permissions.Document.Create
    ];

    private static readonly string[] ParentPermissions =
    [
        Permissions.Student.View,
        Permissions.Document.View
    ];

    private static readonly string[] SppPermissions =
    [
        Permissions.Student.View,
        Permissions.Student.Edit,

        Permissions.Document.View,
        Permissions.Document.Edit,
        Permissions.Document.Create
    ];

    private static readonly string[] PppPermissions =
    [
        Permissions.Student.View,
        Permissions.Student.Edit,

        Permissions.Document.View,
        Permissions.Document.Edit,
        Permissions.Document.Create
    ];

    private static readonly string[] SpcPermissions =
    [
        Permissions.Student.View,
        Permissions.Student.Edit,

        Permissions.Document.View,
        Permissions.Document.Edit,
        Permissions.Document.Create
    ];

    private static readonly string[] AssistantPermissions =
    [
        Permissions.Student.View,
        Permissions.Document.View
    ];
}
