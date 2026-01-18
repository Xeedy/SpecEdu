namespace SpecEdu.Application.Common.Models;

/// <summary>
/// Data transfer object for AuditLog entity.
/// Used to display audit log entries.
/// </summary>
public class AuditLogDto
{
    /// <summary>
    /// Unique identifier of the audit log entry.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// ID of the user who performed the action.
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Display name of the user at the time of action.
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// The action that was performed.
    /// </summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// Display name for the action (localized).
    /// </summary>
    public string ActionName => Action switch
    {
        "Create" => "Vytvoření",
        "Update" => "Úprava",
        "Delete" => "Smazání",
        "View" => "Zobrazení",
        "Login" => "Přihlášení",
        "Logout" => "Odhlášení",
        "AccessDenied" => "Odepřený přístup",
        _ => Action
    };

    /// <summary>
    /// Type of entity that was affected.
    /// </summary>
    public string EntityType { get; set; } = string.Empty;

    /// <summary>
    /// ID of the entity that was affected.
    /// </summary>
    public string? EntityId { get; set; }

    /// <summary>
    /// ID of the student if the action was related to a student.
    /// </summary>
    public Guid? StudentId { get; set; }

    /// <summary>
    /// Name of the student (populated when needed).
    /// </summary>
    public string? StudentName { get; set; }

    /// <summary>
    /// ID of the school where the action occurred.
    /// </summary>
    public Guid? SchoolId { get; set; }

    /// <summary>
    /// Name of the school (populated when needed).
    /// </summary>
    public string? SchoolName { get; set; }

    /// <summary>
    /// UTC timestamp when the action occurred.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Formatted timestamp for display.
    /// </summary>
    public string TimestampFormatted => Timestamp.ToLocalTime().ToString("dd.MM.yyyy HH:mm:ss");

    /// <summary>
    /// IP address of the user who performed the action.
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// User agent string of the client.
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// Additional details about the action.
    /// </summary>
    public string? Details { get; set; }
}
