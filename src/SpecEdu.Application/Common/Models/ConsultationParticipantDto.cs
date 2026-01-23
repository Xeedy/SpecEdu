using SpecEdu.Domain.Enums;

namespace SpecEdu.Application.Common.Models;

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

    public string DisplayName => !string.IsNullOrEmpty(UserName) ? UserName : ExternalName ?? "Neznámý";

    public string? DisplayEmail => !string.IsNullOrEmpty(UserEmail) ? UserEmail : ExternalEmail;

    public bool IsExternal => string.IsNullOrEmpty(UserId);
}

public class AddParticipantDto
{
    public Guid ConsultationEventId { get; set; }
    public string? UserId { get; set; }
    public string? ExternalName { get; set; }
    public string? ExternalEmail { get; set; }
    public string? RoleDescription { get; set; }
    public bool IsRequired { get; set; } = true;
}

public class ParticipantResponseDto
{
    public Guid ParticipantId { get; set; }
    public ParticipantResponseStatus ResponseStatus { get; set; }
    public string? ResponseNote { get; set; }
}

public class MarkAttendanceDto
{
    public Guid ParticipantId { get; set; }
    public bool Attended { get; set; }
}
