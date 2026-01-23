using SpecEdu.Domain.Enums;

namespace SpecEdu.Application.Common.Models;

/// <summary>
/// DTO for a consultation participant.
/// Czech: DTO účastníka konzultace
/// </summary>
public class ConsultationParticipantDto
{
    public Guid Id { get; set; }
    public Guid ConsultationEventId { get; set; }
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? UserEmail { get; set; }
    public string? ExternalName { get; set; }
    public string? ExternalEmail { get; set; }
    public string? RoleDescription { get; set; }
    public ParticipantResponseStatus ResponseStatus { get; set; }
    public DateTime? RespondedAt { get; set; }
    public string? ResponseNote { get; set; }
    public bool IsOrganizer { get; set; }
    public bool IsRequired { get; set; }
    public bool NotificationSent { get; set; }
    public DateTime? NotificationSentAt { get; set; }
    public bool? Attended { get; set; }
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Display name (user name or external name).
    /// </summary>
    public string DisplayName => !string.IsNullOrEmpty(UserName) ? UserName : ExternalName ?? "Neznámý";

    /// <summary>
    /// Display email (user email or external email).
    /// </summary>
    public string? DisplayEmail => !string.IsNullOrEmpty(UserEmail) ? UserEmail : ExternalEmail;

    /// <summary>
    /// Whether this is an external (non-registered) participant.
    /// </summary>
    public bool IsExternal => string.IsNullOrEmpty(UserId);
}

/// <summary>
/// DTO for adding a participant to an existing event.
/// Czech: DTO pro přidání účastníka
/// </summary>
public class AddParticipantDto
{
    public Guid ConsultationEventId { get; set; }
    public string? UserId { get; set; }
    public string? ExternalName { get; set; }
    public string? ExternalEmail { get; set; }
    public string? RoleDescription { get; set; }
    public bool IsRequired { get; set; } = true;
}

/// <summary>
/// DTO for participant response.
/// Czech: DTO pro odpověď účastníka
/// </summary>
public class ParticipantResponseDto
{
    public Guid ParticipantId { get; set; }
    public ParticipantResponseStatus ResponseStatus { get; set; }
    public string? ResponseNote { get; set; }
}

/// <summary>
/// DTO for marking attendance after event.
/// Czech: DTO pro zaznamenání účasti
/// </summary>
public class MarkAttendanceDto
{
    public Guid ParticipantId { get; set; }
    public bool Attended { get; set; }
}
