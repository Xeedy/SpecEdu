using SpecEdu.Domain.Common;

namespace SpecEdu.Domain.Entities;

/// <summary>
/// Represents a school or educational institution (tenant).
/// Each school is an isolated tenant in the multi-tenant architecture.
/// Czech: Škola / Vzdělávací instituce
/// </summary>
public class School : AuditableEntity
{
    /// <summary>
    /// Name of the school/institution.
    /// Czech: Název instituce
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Czech business identification number (IČO).
    /// 8-digit unique identifier for Czech legal entities.
    /// </summary>
    public string? Ico { get; set; }

    /// <summary>
    /// Street address of the institution.
    /// Czech: Adresa
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// City where the institution is located.
    /// Czech: Město
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Postal code (PSČ in Czech).
    /// Format: XXXXX (5 digits)
    /// </summary>
    public string? PostalCode { get; set; }

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
    /// Values: Škola, PPP, SPC, ŠPP, Jiné
    /// </summary>
    public string InstitutionType { get; set; } = "Škola";

    /// <summary>
    /// Indicates whether the school account is active.
    /// Inactive schools cannot be accessed by their users.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Date when the school's license expires.
    /// After this date, access may be restricted.
    /// </summary>
    public DateTime? LicenseExpiresAt { get; set; }
}
