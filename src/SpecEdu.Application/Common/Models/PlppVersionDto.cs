using SpecEdu.Domain.Enums;

namespace SpecEdu.Application.Common.Models;

/// <summary>
/// Full DTO for a PLPP version including snapshot data.
/// Czech: Kompletní DTO verze PLPP včetně snímku
/// </summary>
public class PlppVersionDto
{
    public Guid Id { get; set; }
    public Guid PlppId { get; set; }
    public int VersionNumber { get; set; }
    public string? ChangeSummary { get; set; }
    public VersionSource Source { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? CreatedByName { get; set; }

    /// <summary>
    /// The deserialized snapshot data.
    /// </summary>
    public PlppVersionSnapshot? Snapshot { get; set; }
}

/// <summary>
/// Lightweight DTO for version list views.
/// Czech: Lehký DTO pro seznamy verzí
/// </summary>
public class PlppVersionListItemDto
{
    public Guid Id { get; set; }
    public Guid PlppId { get; set; }
    public int VersionNumber { get; set; }
    public string? ChangeSummary { get; set; }
    public VersionSource Source { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? CreatedByName { get; set; }

    /// <summary>
    /// PLPP status at time of this version.
    /// </summary>
    public PlppStatus? PlppStatus { get; set; }
}

/// <summary>
/// Request DTO for creating a version.
/// Czech: DTO pro vytvoření verze
/// </summary>
public class CreatePlppVersionDto
{
    public Guid PlppId { get; set; }
    public VersionSource Source { get; set; }
    public string? ChangeSummary { get; set; }
}
