using SpecEdu.Domain.Common;
using SpecEdu.Domain.Enums;

namespace SpecEdu.Domain.Entities;

/// <summary>
/// Represents a relationship between a parent/guardian and a student.
/// This is the core entity for parent access control.
/// Czech: Vazba zákonného zástupce na žáka
/// </summary>
public class StudentGuardian : AuditableEntity
{
    /// <summary>
    /// Foreign key to the student.
    /// </summary>
    public Guid StudentId { get; set; }

    /// <summary>
    /// ID of the parent/guardian user (AspNetUsers.Id).
    /// </summary>
    public string ParentUserId { get; set; } = string.Empty;

    /// <summary>
    /// Type of relationship (Mother, Father, LegalGuardian, Other).
    /// Czech: Typ vztahu
    /// </summary>
    public RelationshipType RelationshipType { get; set; }

    /// <summary>
    /// Indicates whether this guardian relationship is active.
    /// Inactive relationships do not grant access.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Navigation property to the student.
    /// </summary>
    public Student? Student { get; set; }
}
