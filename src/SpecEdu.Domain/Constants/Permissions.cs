namespace SpecEdu.Domain.Constants;

public static class Permissions
{
    public static class Student
    {
        public const string View = "Permissions.Student.View";
        public const string Edit = "Permissions.Student.Edit";
        public const string Create = "Permissions.Student.Create";
        public const string Delete = "Permissions.Student.Delete";
    }

    public static class Document
    {
        public const string View = "Permissions.Document.View";
        public const string Edit = "Permissions.Document.Edit";
        public const string Create = "Permissions.Document.Create";
        public const string Delete = "Permissions.Document.Delete";
    }

    public static class Administration
    {
        public const string ManageSchool = "Permissions.Administration.ManageSchool";
        public const string ManageUsers = "Permissions.Administration.ManageUsers";
        public const string ManageRoles = "Permissions.Administration.ManageRoles";
        public const string ViewAuditLog = "Permissions.Administration.ViewAuditLog";
    }

    public static class System
    {
        public const string SystemAdmin = "Permissions.System.SystemAdmin";
        public const string ManageSchools = "Permissions.System.ManageSchools";
    }

    public static readonly string[] All =
    [

        Student.View,
        Student.Edit,
        Student.Create,
        Student.Delete,

        Document.View,
        Document.Edit,
        Document.Create,
        Document.Delete,

        Administration.ManageSchool,
        Administration.ManageUsers,
        Administration.ManageRoles,
        Administration.ViewAuditLog,

        System.SystemAdmin,
        System.ManageSchools
    ];
}
