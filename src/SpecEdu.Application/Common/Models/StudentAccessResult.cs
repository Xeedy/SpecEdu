using SpecEdu.Domain.Enums;

namespace SpecEdu.Application.Common.Models;

/// <summary>
/// Result of checking a user's access to a student.
/// Used by the authorization system to determine permissions.
/// </summary>
public class StudentAccessResult
{
    /// <summary>
    /// Indicates whether the user can access the student (view).
    /// </summary>
    public bool CanAccess { get; set; }

    /// <summary>
    /// Indicates whether the user can edit student data.
    /// </summary>
    public bool CanEdit { get; set; }

    /// <summary>
    /// The reason or source of the access grant.
    /// </summary>
    public AccessReason AccessReason { get; set; }

    /// <summary>
    /// Additional message explaining the access decision.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Creates a result indicating no access.
    /// </summary>
    public static StudentAccessResult NoAccess(string? message = null) => new()
    {
        CanAccess = false,
        CanEdit = false,
        AccessReason = AccessReason.NoAccess,
        Message = message ?? "Nemáte oprávnění k přístupu k tomuto žákovi."
    };

    /// <summary>
    /// Creates a result indicating full access (Admin).
    /// </summary>
    public static StudentAccessResult FullAccess(AccessReason reason, string? message = null) => new()
    {
        CanAccess = true,
        CanEdit = true,
        AccessReason = reason,
        Message = message
    };

    /// <summary>
    /// Creates a result indicating read-only access (Parent).
    /// </summary>
    public static StudentAccessResult ReadOnly(AccessReason reason, string? message = null) => new()
    {
        CanAccess = true,
        CanEdit = false,
        AccessReason = reason,
        Message = message
    };

    /// <summary>
    /// Creates a result based on a specific access level.
    /// </summary>
    public static StudentAccessResult FromAccessLevel(AccessLevel level, AccessReason reason, string? message = null) => new()
    {
        CanAccess = true,
        CanEdit = level == AccessLevel.Edit,
        AccessReason = reason,
        Message = message
    };
}

/// <summary>
/// Reason for access being granted or denied.
/// </summary>
public enum AccessReason
{
    /// <summary>
    /// No access granted.
    /// </summary>
    NoAccess = 0,

    /// <summary>
    /// Access granted because user is a global admin.
    /// </summary>
    Admin = 1,

    /// <summary>
    /// Access granted because user is a school admin of the student's school.
    /// </summary>
    SchoolAdmin = 2,

    /// <summary>
    /// Access granted via guardian relationship.
    /// </summary>
    Guardian = 3,

    /// <summary>
    /// Access granted via staff link.
    /// </summary>
    StaffLink = 4,

    /// <summary>
    /// Access denied because user is from a different school.
    /// </summary>
    DifferentSchool = 5
}
