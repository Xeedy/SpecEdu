using Microsoft.AspNetCore.Authorization;
using SpecEdu.Domain.Constants;

namespace SpecEdu.Infrastructure.Authorization;

public static class AuthorizationPolicies
{
    public const string RequireAdmin = "RequireAdmin";
    public const string RequireSchoolAdmin = "RequireSchoolAdmin";
    public const string CanViewStudent = "CanViewStudent";
    public const string CanEditStudent = "CanEditStudent";
    public const string CanAccessStudent = "CanAccessStudent";
    public const string CanEditStudentData = "CanEditStudentData";
    public const string CanCreateStudent = "CanCreateStudent";
    public const string CanDeleteStudent = "CanDeleteStudent";
    public const string CanViewDocument = "CanViewDocument";
    public const string CanEditDocument = "CanEditDocument";
    public const string CanCreateDocument = "CanCreateDocument";
    public const string CanDeleteDocument = "CanDeleteDocument";
    public const string CanManageUsers = "CanManageUsers";
    public const string CanManageSchool = "CanManageSchool";
    public const string CanManageSchools = "CanManageSchools";
    public const string CanViewAuditLog = "CanViewAuditLog";

    public static void AddPolicies(AuthorizationOptions options)
    {
        options.AddPolicy(RequireAdmin, policy =>
            policy.RequireRole(Roles.Admin));

        options.AddPolicy(RequireSchoolAdmin, policy =>
            policy.RequireRole(Roles.Admin, Roles.SchoolAdmin));

        options.AddPolicy(CanViewStudent, policy =>
            policy.Requirements.Add(new PermissionRequirement(Permissions.Student.View)));

        options.AddPolicy(CanEditStudent, policy =>
            policy.Requirements.Add(new PermissionRequirement(Permissions.Student.Edit)));

        options.AddPolicy(CanCreateStudent, policy =>
            policy.Requirements.Add(new PermissionRequirement(Permissions.Student.Create)));

        options.AddPolicy(CanDeleteStudent, policy =>
            policy.Requirements.Add(new PermissionRequirement(Permissions.Student.Delete)));

        options.AddPolicy(CanViewDocument, policy =>
            policy.Requirements.Add(new PermissionRequirement(Permissions.Document.View)));

        options.AddPolicy(CanEditDocument, policy =>
            policy.Requirements.Add(new PermissionRequirement(Permissions.Document.Edit)));

        options.AddPolicy(CanCreateDocument, policy =>
            policy.Requirements.Add(new PermissionRequirement(Permissions.Document.Create)));

        options.AddPolicy(CanDeleteDocument, policy =>
            policy.Requirements.Add(new PermissionRequirement(Permissions.Document.Delete)));

        options.AddPolicy(CanManageUsers, policy =>
            policy.Requirements.Add(new PermissionRequirement(Permissions.Administration.ManageUsers)));

        options.AddPolicy(CanManageSchool, policy =>
            policy.Requirements.Add(new PermissionRequirement(Permissions.Administration.ManageSchool)));

        options.AddPolicy(CanManageSchools, policy =>
            policy.Requirements.Add(new PermissionRequirement(Permissions.System.ManageSchools)));

        options.AddPolicy(CanViewAuditLog, policy =>
            policy.Requirements.Add(new PermissionRequirement(Permissions.Administration.ViewAuditLog)));

        options.AddPolicy(CanAccessStudent, policy =>
            policy.Requirements.Add(new StudentAccessRequirement(requireEditAccess: false)));

        options.AddPolicy(CanEditStudentData, policy =>
            policy.Requirements.Add(new StudentAccessRequirement(requireEditAccess: true)));
    }
}
