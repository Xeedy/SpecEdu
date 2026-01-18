using SpecEdu.Domain.Enums;

namespace SpecEdu.Application.Common.Models;

/// <summary>
/// Data transfer object for StudentGuardian entity.
/// Represents the relationship between a guardian and a student.
/// </summary>
public class StudentGuardianDto
{
    /// <summary>
    /// Unique identifier of the guardian relationship.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// ID of the student.
    /// </summary>
    public Guid StudentId { get; set; }

    /// <summary>
    /// Name of the student (populated when needed).
    /// </summary>
    public string? StudentName { get; set; }

    /// <summary>
    /// ID of the parent/guardian user.
    /// </summary>
    public string ParentUserId { get; set; } = string.Empty;

    /// <summary>
    /// Name of the parent/guardian (populated when needed).
    /// </summary>
    public string? ParentName { get; set; }

    /// <summary>
    /// Email of the parent/guardian (populated when needed).
    /// </summary>
    public string? ParentEmail { get; set; }

    /// <summary>
    /// Type of relationship (Mother, Father, LegalGuardian, Other).
    /// </summary>
    public RelationshipType RelationshipType { get; set; }

    /// <summary>
    /// Display name for the relationship type.
    /// </summary>
    public string RelationshipTypeName => RelationshipType switch
    {
        RelationshipType.Mother => "Matka",
        RelationshipType.Father => "Otec",
        RelationshipType.LegalGuardian => "Zákonný zástupce",
        RelationshipType.Other => "Jiný",
        _ => "Neznámý"
    };

    /// <summary>
    /// Indicates whether the relationship is active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// UTC timestamp when the relationship was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
