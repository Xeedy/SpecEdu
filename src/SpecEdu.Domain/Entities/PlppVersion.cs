using SpecEdu.Domain.Common;
using SpecEdu.Domain.Enums;

namespace SpecEdu.Domain.Entities;

/// <summary>
/// Represents a version/snapshot of a PLPP document.
/// Stores the complete PLPP state as JSON for historical tracking.
/// Czech: Verze/snímek PLPP dokumentu
/// </summary>
public class PlppVersion : AuditableEntity
{
    /// <summary>
    /// The PLPP this version belongs to.
    /// Czech: ID plánu
    /// </summary>
    public Guid PlppId { get; set; }

    /// <summary>
    /// Sequential version number (1, 2, 3...).
    /// Czech: Číslo verze
    /// </summary>
    public int VersionNumber { get; set; }

    /// <summary>
    /// Complete PLPP state serialized as JSON.
    /// Czech: JSON snapshot plánu
    /// </summary>
    public string Snapshot { get; set; } = string.Empty;

    /// <summary>
    /// Optional summary of what changed in this version.
    /// Czech: Popis změny
    /// </summary>
    public string? ChangeSummary { get; set; }

    /// <summary>
    /// Source/reason for creating this version.
    /// Czech: Zdroj vytvoření verze
    /// </summary>
    public VersionSource Source { get; set; }

    // Navigation properties

    /// <summary>
    /// The PLPP this version belongs to.
    /// </summary>
    public Plpp? Plpp { get; set; }
}
