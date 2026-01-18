namespace SpecEdu.Domain.Constants;

/// <summary>
/// Defines all permission constants used for policy-based authorization.
/// Permissions are granular access controls that can be assigned to roles.
/// </summary>
public static class Permissions
{
    /// <summary>
    /// Student-related permissions.
    /// Czech: Oprávnění pro žáky
    /// </summary>
    public static class Student
    {
        public const string View = "Permissions.Student.View";
        public const string Edit = "Permissions.Student.Edit";
        public const string Create = "Permissions.Student.Create";
        public const string Delete = "Permissions.Student.Delete";
    }

    /// <summary>
    /// Document-related permissions.
    /// Czech: Oprávnění pro dokumenty
    /// </summary>
    public static class Document
    {
        public const string View = "Permissions.Document.View";
        public const string Edit = "Permissions.Document.Edit";
        public const string Create = "Permissions.Document.Create";
        public const string Delete = "Permissions.Document.Delete";
    }

    /// <summary>
    /// Administration permissions for school-level management.
    /// Czech: Administrační oprávnění
    /// </summary>
    public static class Administration
    {
        public const string ManageSchool = "Permissions.Administration.ManageSchool";
        public const string ManageUsers = "Permissions.Administration.ManageUsers";
        public const string ManageRoles = "Permissions.Administration.ManageRoles";
        public const string ViewAuditLog = "Permissions.Administration.ViewAuditLog";
    }

    /// <summary>
    /// System-level permissions for global administrators.
    /// Czech: Systémová oprávnění
    /// </summary>
    public static class System
    {
        public const string SystemAdmin = "Permissions.System.SystemAdmin";
        public const string ManageSchools = "Permissions.System.ManageSchools";
    }

    /// <summary>
    /// Gets all available permissions as an array.
    /// Useful for seeding and validation.
    /// </summary>
    public static readonly string[] All =
    [
        // Student permissions
        Student.View,
        Student.Edit,
        Student.Create,
        Student.Delete,

        // Document permissions
        Document.View,
        Document.Edit,
        Document.Create,
        Document.Delete,

        // Administration permissions
        Administration.ManageSchool,
        Administration.ManageUsers,
        Administration.ManageRoles,
        Administration.ViewAuditLog,

        // System permissions
        System.SystemAdmin,
        System.ManageSchools
    ];
}
