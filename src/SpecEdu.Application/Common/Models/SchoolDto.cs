namespace SpecEdu.Application.Common.Models;

public class SchoolDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Ico { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? PostalCode { get; set; }

    public string FullAddress
    {
        get
        {
            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(Address)) parts.Add(Address);
            if (!string.IsNullOrWhiteSpace(City)) parts.Add(City);
            if (!string.IsNullOrWhiteSpace(PostalCode)) parts.Add(PostalCode);
            return string.Join(", ", parts);
        }
    }

    public string? ContactEmail { get; set; }

    public string? ContactPhone { get; set; }

    public string InstitutionType { get; set; } = "Škola";

    public bool IsActive { get; set; }

    public DateTime? LicenseExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? UserCount { get; set; }
}
