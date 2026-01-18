using SpecEdu.Domain.Enums;

namespace SpecEdu.Application.Common.Models;

public class StudentStaffLinkDto
{
    public Guid Id { get; set; }

    public Guid StudentId { get; set; }

    public string? StudentName { get; set; }

    public string UserId { get; set; } = string.Empty;

    public string? UserName { get; set; }

    public string? UserEmail { get; set; }

    public StaffLinkType LinkType { get; set; }

    public string LinkTypeName => LinkType switch
    {
        StaffLinkType.Teacher => "Učitel",
        StaffLinkType.Assistant => "Asistent",
        StaffLinkType.SPP => "ŠPP",
        StaffLinkType.PPP => "PPP",
        StaffLinkType.SPC => "SPC",
        _ => "Neznámý"
    };

    public AccessLevel AccessLevel { get; set; }

    public string AccessLevelName => AccessLevel switch
    {
        AccessLevel.Read => "Pouze čtení",
        AccessLevel.Edit => "Čtení a zápis",
        _ => "Neznámý"
    };

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
}
