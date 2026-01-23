using SpecEdu.Domain.Common;
using SpecEdu.Domain.Enums;

namespace SpecEdu.Domain.Entities;

public class ConsultationParticipant : AuditableEntity
{
    public Guid ConsultationEventId { get; set; }

    public string? UserId { get; set; }

    public string? ExternalName { get; set; }

    public string? ExternalEmail { get; set; }

    public string? RoleDescription { get; set; }

    public ParticipantResponseStatus ResponseStatus { get; set; } = ParticipantResponseStatus.Pending;

    public DateTime? RespondedAt { get; set; }

    public string? ResponseNote { get; set; }

    public bool IsOrganizer { get; set; } = false;

    public bool IsRequired { get; set; } = true;

    public bool NotificationSent { get; set; } = false;

    public DateTime? NotificationSentAt { get; set; }

    public bool? Attended { get; set; }

    public ConsultationEvent? ConsultationEvent { get; set; }
}
