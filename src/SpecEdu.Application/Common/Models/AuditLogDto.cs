namespace SpecEdu.Application.Common.Models;

public class AuditLogDto
{
    public Guid Id { get; set; }

    public string? UserId { get; set; }

    public string? UserName { get; set; }

    public string Action { get; set; } = string.Empty;

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

    public string EntityType { get; set; } = string.Empty;

    public string? EntityId { get; set; }

    public Guid? StudentId { get; set; }

    public string? StudentName { get; set; }

    public Guid? SchoolId { get; set; }

    public string? SchoolName { get; set; }

    public DateTime Timestamp { get; set; }

    public string TimestampFormatted => Timestamp.ToLocalTime().ToString("dd.MM.yyyy HH:mm:ss");

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public string? Details { get; set; }
}
