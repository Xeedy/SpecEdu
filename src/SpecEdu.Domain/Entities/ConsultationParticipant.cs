using SpecEdu.Domain.Common;
using SpecEdu.Domain.Enums;

namespace SpecEdu.Domain.Entities;

/// <summary>
/// Represents a participant in a consultation event.
/// Can be a user (staff/parent) or external person.
/// Czech: Účastník konzultace
/// </summary>
public class ConsultationParticipant : AuditableEntity
{
    /// <summary>
    /// The event this participant belongs to.
    /// Czech: ID konzultace
    /// </summary>
    public Guid ConsultationEventId { get; set; }

    /// <summary>
    /// User ID if participant is a registered user.
    /// Null for external participants.
    /// Czech: ID uživatele
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Name of external participant (if not a registered user).
    /// Czech: Jméno externího účastníka
    /// </summary>
    public string? ExternalName { get; set; }

    /// <summary>
    /// Email of external participant (for notifications).
    /// Czech: Email externího účastníka
    /// </summary>
    public string? ExternalEmail { get; set; }

    /// <summary>
    /// Role description of the participant (e.g., "Třídní učitel", "Matka").
    /// Czech: Role účastníka
    /// </summary>
    public string? RoleDescription { get; set; }

    /// <summary>
    /// Response status of the participant.
    /// Czech: Stav odpovědi
    /// </summary>
    public ParticipantResponseStatus ResponseStatus { get; set; } = ParticipantResponseStatus.Pending;

    /// <summary>
    /// When the participant responded.
    /// Czech: Datum odpovědi
    /// </summary>
    public DateTime? RespondedAt { get; set; }

    /// <summary>
    /// Note from the participant (reason for decline, etc.).
    /// Czech: Poznámka účastníka
    /// </summary>
    public string? ResponseNote { get; set; }

    /// <summary>
    /// Whether this participant is the organizer.
    /// Czech: Je organizátorem
    /// </summary>
    public bool IsOrganizer { get; set; } = false;

    /// <summary>
    /// Whether this participant is required (vs optional).
    /// Czech: Povinná účast
    /// </summary>
    public bool IsRequired { get; set; } = true;

    /// <summary>
    /// Whether notification was sent to this participant.
    /// Czech: Notifikace odeslána
    /// </summary>
    public bool NotificationSent { get; set; } = false;

    /// <summary>
    /// When notification was sent.
    /// Czech: Datum odeslání notifikace
    /// </summary>
    public DateTime? NotificationSentAt { get; set; }

    /// <summary>
    /// Whether the participant attended (filled after event).
    /// Czech: Zúčastnil se
    /// </summary>
    public bool? Attended { get; set; }

    // Navigation properties

    /// <summary>
    /// The event this participant belongs to.
    /// </summary>
    public ConsultationEvent? ConsultationEvent { get; set; }
}
