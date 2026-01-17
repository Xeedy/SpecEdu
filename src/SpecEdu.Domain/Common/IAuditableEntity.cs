namespace SpecEdu.Domain.Common;

/// <summary>
/// Interface for entities that require audit tracking.
/// Implemented by entities that need to track creation and modification metadata.
/// </summary>
public interface IAuditableEntity
{
    /// <summary>
    /// UTC timestamp when the entity was created.
    /// </summary>
    DateTime CreatedAt { get; set; }

    /// <summary>
    /// Identifier of the user who created the entity.
    /// Null if created by system process.
    /// </summary>
    string? CreatedBy { get; set; }

    /// <summary>
    /// UTC timestamp when the entity was last modified.
    /// Null if never modified after creation.
    /// </summary>
    DateTime? ModifiedAt { get; set; }

    /// <summary>
    /// Identifier of the user who last modified the entity.
    /// Null if never modified or modified by system process.
    /// </summary>
    string? ModifiedBy { get; set; }
}
