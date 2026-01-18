namespace SpecEdu.Application.Common.Models;

/// <summary>
/// Data transfer object for School entity.
/// Used to transfer school data without exposing internal implementation.
/// </summary>
public class SchoolDto
{
    /// <summary>
    /// Unique identifier of the school.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Name of the school/institution.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Czech business identification number (IČO).
    /// </summary>
    public string? Ico { get; set; }

    /// <summary>
    /// Street address of the institution.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// City where the institution is located.
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Postal code (PSČ).
    /// </summary>
    public string? PostalCode { get; set; }

    /// <summary>
    /// Full formatted address.
    /// </summary>
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

    /// <summary>
    /// Primary contact email for the institution.
    /// </summary>
    public string? ContactEmail { get; set; }

    /// <summary>
    /// Primary contact phone number for the institution.
    /// </summary>
    public string? ContactPhone { get; set; }

    /// <summary>
    /// Type of educational institution.
    /// </summary>
    public string InstitutionType { get; set; } = "Škola";

    /// <summary>
    /// Indicates whether the school account is active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Date when the school's license expires.
    /// </summary>
    public DateTime? LicenseExpiresAt { get; set; }

    /// <summary>
    /// UTC timestamp when the school was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Number of users in the school (optional, populated when needed).
    /// </summary>
    public int? UserCount { get; set; }
}
