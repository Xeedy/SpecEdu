namespace SpecEdu.Domain.Common;

/// <summary>
/// Base class for all domain entities.
/// Provides a unique identifier using GUID.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Unique identifier for the entity.
    /// Generated automatically on creation.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
}
