using SpecEdu.Domain.Common;

namespace SpecEdu.Domain.Entities;

public class School : AuditableEntity
{
    public string Name { get; set; } = string.Empty;

    public string? Ico { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? PostalCode { get; set; }

    public string? ContactEmail { get; set; }

    public string? ContactPhone { get; set; }

    public string InstitutionType { get; set; } = "Škola";

    public bool IsActive { get; set; } = true;

    public DateTime? LicenseExpiresAt { get; set; }
}
