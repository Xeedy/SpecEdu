using SpecEdu.Domain.Enums;

namespace SpecEdu.Application.Common.Models;

public class StudentAccessResult
{
    public bool CanAccess { get; set; }

    public bool CanEdit { get; set; }

    public AccessReason AccessReason { get; set; }

    public string? Message { get; set; }

    public static StudentAccessResult NoAccess(string? message = null) => new()
    {
        CanAccess = false,
        CanEdit = false,
        AccessReason = AccessReason.NoAccess,
        Message = message ?? "Nemáte oprávnění k přístupu k tomuto žákovi."
    };

    public static StudentAccessResult FullAccess(AccessReason reason, string? message = null) => new()
    {
        CanAccess = true,
        CanEdit = true,
        AccessReason = reason,
        Message = message
    };

    public static StudentAccessResult ReadOnly(AccessReason reason, string? message = null) => new()
    {
        CanAccess = true,
        CanEdit = false,
        AccessReason = reason,
        Message = message
    };

    public static StudentAccessResult FromAccessLevel(AccessLevel level, AccessReason reason, string? message = null) => new()
    {
        CanAccess = true,
        CanEdit = level == AccessLevel.Edit,
        AccessReason = reason,
        Message = message
    };
}

public enum AccessReason
{
    NoAccess = 0,

    Admin = 1,

    SchoolAdmin = 2,

    Guardian = 3,

    StaffLink = 4,

    DifferentSchool = 5
}
