using SpecEdu.Domain.Enums;

namespace SpecEdu.Application.Common.Models;

/// <summary>
/// Data transfer object for StudentStaffLink entity.
/// Represents the link between a staff member and a student.
/// </summary>
public class StudentStaffLinkDto
{
    /// <summary>
    /// Unique identifier of the staff link.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// ID of the student.
    /// </summary>
    public Guid StudentId { get; set; }

    /// <summary>
    /// Name of the student (populated when needed).
    /// </summary>
    public string? StudentName { get; set; }

    /// <summary>
    /// ID of the staff user.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Name of the staff member (populated when needed).
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// Email of the staff member (populated when needed).
    /// </summary>
    public string? UserEmail { get; set; }

    /// <summary>
    /// Type of staff link (Teacher, Assistant, SPP, PPP, SPC).
    /// </summary>
    public StaffLinkType LinkType { get; set; }

    /// <summary>
    /// Display name for the link type.
    /// </summary>
    public string LinkTypeName => LinkType switch
    {
        StaffLinkType.Teacher => "Učitel",
        StaffLinkType.Assistant => "Asistent",
        StaffLinkType.SPP => "ŠPP",
        StaffLinkType.PPP => "PPP",
        StaffLinkType.SPC => "SPC",
        _ => "Neznámý"
    };

    /// <summary>
    /// Level of access granted (Read or Edit).
    /// </summary>
    public AccessLevel AccessLevel { get; set; }

    /// <summary>
    /// Display name for the access level.
    /// </summary>
    public string AccessLevelName => AccessLevel switch
    {
        AccessLevel.Read => "Pouze čtení",
        AccessLevel.Edit => "Čtení a zápis",
        _ => "Neznámý"
    };

    /// <summary>
    /// Indicates whether the link is active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// UTC timestamp when the link was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
