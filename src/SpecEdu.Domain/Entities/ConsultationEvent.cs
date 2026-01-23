using SpecEdu.Domain.Common;
using SpecEdu.Domain.Enums;

namespace SpecEdu.Domain.Entities;

/// <summary>
/// Represents a consultation event (meeting, consultation, school event).
/// Can be linked to a specific student, school, or be school-wide.
/// Czech: Konzultace / schůzka
/// </summary>
public class ConsultationEvent : AuditableEntity
{
    /// <summary>
    /// School this event belongs to.
    /// Czech: ID školy
    /// </summary>
    public Guid SchoolId { get; set; }

    /// <summary>
    /// Optional: Student this event is about.
    /// Null for school-wide events.
    /// Czech: ID žáka (volitelné)
    /// </summary>
    public Guid? StudentId { get; set; }

    /// <summary>
    /// Title of the event.
    /// Czech: Název
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Description or agenda of the event.
    /// Czech: Popis / program
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Type of consultation.
    /// Czech: Typ konzultace
    /// </summary>
    public ConsultationType Type { get; set; } = ConsultationType.IndividualConsultation;

    /// <summary>
    /// Current status of the event.
    /// Czech: Stav
    /// </summary>
    public ConsultationEventStatus Status { get; set; } = ConsultationEventStatus.Scheduled;

    /// <summary>
    /// Start date and time.
    /// Czech: Začátek
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// End date and time.
    /// Czech: Konec
    /// </summary>
    public DateTime EndTime { get; set; }

    /// <summary>
    /// Location of the event (room number, address, or "online").
    /// Czech: Místo konání
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// Online meeting link (if virtual).
    /// Czech: Odkaz na online schůzku
    /// </summary>
    public string? OnlineMeetingLink { get; set; }

    /// <summary>
    /// Whether the event is visible to parents.
    /// Czech: Viditelné pro rodiče
    /// </summary>
    public bool IsVisibleToParents { get; set; } = true;

    /// <summary>
    /// Whether participants can respond to the invitation.
    /// Czech: Účastníci mohou odpovídat
    /// </summary>
    public bool AllowResponses { get; set; } = true;

    /// <summary>
    /// Notes after the event (summary, outcomes).
    /// Czech: Poznámky z průběhu
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Reference to related PLPP (if applicable).
    /// Czech: Odkaz na PLPP
    /// </summary>
    public Guid? PlppId { get; set; }

    /// <summary>
    /// User ID of the event organizer.
    /// Czech: Organizátor
    /// </summary>
    public string? OrganizerId { get; set; }

    /// <summary>
    /// How many minutes before to send reminder notification.
    /// Null = no reminder. Default: 1440 (24 hours).
    /// Czech: Připomenout (minuty před)
    /// </summary>
    public int? ReminderMinutesBefore { get; set; } = 1440;

    /// <summary>
    /// Whether reminder notification was sent.
    /// Czech: Připomínka odeslána
    /// </summary>
    public bool ReminderSent { get; set; } = false;

    /// <summary>
    /// Soft delete flag.
    /// </summary>
    public bool IsActive { get; set; } = true;

    // Navigation properties

    /// <summary>
    /// School this event belongs to.
    /// </summary>
    public School? School { get; set; }

    /// <summary>
    /// Student this event is about (optional).
    /// </summary>
    public Student? Student { get; set; }

    /// <summary>
    /// Related PLPP (optional).
    /// </summary>
    public Plpp? Plpp { get; set; }

    /// <summary>
    /// Participants of this event.
    /// Czech: Účastníci
    /// </summary>
    public ICollection<ConsultationParticipant> Participants { get; set; } = new List<ConsultationParticipant>();
}
