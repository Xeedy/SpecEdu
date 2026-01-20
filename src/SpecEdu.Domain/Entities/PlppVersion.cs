using SpecEdu.Domain.Common;
using SpecEdu.Domain.Enums;

namespace SpecEdu.Domain.Entities;

public class PlppVersion : AuditableEntity
{
    public Guid PlppId { get; set; }

    public int VersionNumber { get; set; }

    public string Snapshot { get; set; } = string.Empty;

    public string? ChangeSummary { get; set; }

    public VersionSource Source { get; set; }

    public Plpp? Plpp { get; set; }
}
