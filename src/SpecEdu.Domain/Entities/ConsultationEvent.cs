using SpecEdu.Domain.Common;
using SpecEdu.Domain.Enums;

namespace SpecEdu.Domain.Entities;

public class ConsultationEvent : AuditableEntity
{
    public Guid SchoolId { get; set; }

    public Guid? StudentId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public ConsultationType Type { get; set; } = ConsultationType.IndividualConsultation;

    public ConsultationEventStatus Status { get; set; } = ConsultationEventStatus.Scheduled;

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public string? Location { get; set; }

    public string? OnlineMeetingLink { get; set; }

    public bool IsVisibleToParents { get; set; } = true;

    public bool AllowResponses { get; set; } = true;

    public string? Notes { get; set; }

    public Guid? PlppId { get; set; }

    public string? OrganizerId { get; set; }

    public int? ReminderMinutesBefore { get; set; } = 1440;

    public bool ReminderSent { get; set; } = false;

    public bool IsActive { get; set; } = true;

    public School? School { get; set; }

    public Student? Student { get; set; }

    public Plpp? Plpp { get; set; }

    public ICollection<ConsultationParticipant> Participants { get; set; } = new List<ConsultationParticipant>();
}
