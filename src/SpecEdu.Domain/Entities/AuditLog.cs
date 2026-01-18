using SpecEdu.Domain.Common;

namespace SpecEdu.Domain.Entities;

public class AuditLog : BaseEntity
{
    public string? UserId { get; set; }

    public string? UserName { get; set; }

    public string Action { get; set; } = string.Empty;

    public string EntityType { get; set; } = string.Empty;

    public string? EntityId { get; set; }

    public Guid? StudentId { get; set; }

    public Guid? SchoolId { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public string? Details { get; set; }
}
