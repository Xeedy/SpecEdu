using SpecEdu.Domain.Common;
using SpecEdu.Domain.Enums;

namespace SpecEdu.Domain.Entities;

/// <summary>
/// Represents a link between a staff member and a student with specific access level.
/// This is the core entity for staff access control.
/// Czech: Vazba zaměstnance na žáka
/// </summary>
public class StudentStaffLink : AuditableEntity
{
    /// <summary>
    /// Foreign key to the student.
    /// </summary>
    public Guid StudentId { get; set; }

    /// <summary>
    /// ID of the staff user (AspNetUsers.Id).
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Type of staff link (Teacher, Assistant, SPP, PPP, SPC).
    /// Czech: Typ vazby
    /// </summary>
    public StaffLinkType LinkType { get; set; }

    /// <summary>
    /// Level of access granted (Read or Edit).
    /// Czech: Úroveň přístupu
    /// </summary>
    public AccessLevel AccessLevel { get; set; } = AccessLevel.Read;

    /// <summary>
    /// Indicates whether this staff link is active.
    /// Inactive links do not grant access.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Navigation property to the student.
    /// </summary>
    public Student? Student { get; set; }
}
