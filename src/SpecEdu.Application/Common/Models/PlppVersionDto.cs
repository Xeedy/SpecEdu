using SpecEdu.Domain.Enums;

namespace SpecEdu.Application.Common.Models;

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

    public PlppVersionSnapshot? Snapshot { get; set; }
}

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

    public PlppStatus? PlppStatus { get; set; }
}

public class CreatePlppVersionDto
{
    public Guid PlppId { get; set; }
    public VersionSource Source { get; set; }
    public string? ChangeSummary { get; set; }
}
