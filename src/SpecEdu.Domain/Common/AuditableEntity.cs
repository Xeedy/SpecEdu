namespace SpecEdu.Domain.Common;

/// <summary>
/// Base class for entities that require audit tracking.
/// Inherits from BaseEntity (Id) and implements IAuditableEntity (audit fields).
/// Most domain entities in SpecEdu should inherit from this class.
/// </summary>
public abstract class AuditableEntity : BaseEntity, IAuditableEntity
{
    /// <summary>
    /// UTC timestamp when the entity was created.
    /// Set automatically by the DbContext on SaveChanges.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Identifier of the user who created the entity.
    /// Set automatically by the DbContext based on current user.
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// UTC timestamp when the entity was last modified.
    /// Set automatically by the DbContext on SaveChanges.
    /// </summary>
    public DateTime? ModifiedAt { get; set; }

    /// <summary>
    /// Identifier of the user who last modified the entity.
    /// Set automatically by the DbContext based on current user.
    /// </summary>
    public string? ModifiedBy { get; set; }
}
