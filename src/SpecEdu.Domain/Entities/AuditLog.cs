using SpecEdu.Domain.Common;

namespace SpecEdu.Domain.Entities;

/// <summary>
/// Represents an audit log entry for compliance and security tracking.
/// Note: This entity inherits from BaseEntity (not AuditableEntity)
/// because we don't want audit fields on the audit log itself.
/// Czech: Záznam auditu pro bezpečnost a compliance
/// </summary>
public class AuditLog : BaseEntity
{
    /// <summary>
    /// ID of the user who performed the action.
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Display name of the user at the time of action.
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// The action that was performed (e.g., "Create", "Update", "Delete", "View").
    /// </summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// Type of entity that was affected (e.g., "Student", "Document").
    /// </summary>
    public string EntityType { get; set; } = string.Empty;

    /// <summary>
    /// ID of the entity that was affected.
    /// </summary>
    public string? EntityId { get; set; }

    /// <summary>
    /// ID of the student if the action was related to a student.
    /// Used for filtering audit logs by student.
    /// </summary>
    public Guid? StudentId { get; set; }

    /// <summary>
    /// ID of the school where the action occurred.
    /// Used for filtering audit logs by school.
    /// </summary>
    public Guid? SchoolId { get; set; }

    /// <summary>
    /// UTC timestamp when the action occurred.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// IP address of the user who performed the action.
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// User agent string of the client.
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// Additional details about the action in JSON format.
    /// </summary>
    public string? Details { get; set; }
}
