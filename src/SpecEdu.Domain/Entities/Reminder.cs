using SpecEdu.Domain.Common;
using SpecEdu.Domain.Enums;

namespace SpecEdu.Domain.Entities;

/// <summary>
/// Represents a reminder notification for a student (e.g., upcoming control examination).
/// Czech: Připomínka notifikace pro studenta (např. blížící se kontrolní vyšetření)
/// </summary>
public class Reminder : AuditableEntity
{
    /// <summary>
    /// The ID of the student this reminder relates to.
    /// Czech: ID studenta
    /// </summary>
    public Guid StudentId { get; set; }

    /// <summary>
    /// Short title for the reminder (e.g., "Kontrolní vyšetření").
    /// Czech: Název připomínky
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Optional detailed description of the reminder.
    /// Czech: Podrobný popis
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The due date for the event being reminded about (e.g., examination date).
    /// Czech: Datum události (např. datum vyšetření)
    /// </summary>
    public DateTime DueDate { get; set; }

    /// <summary>
    /// The date/time when the notification should be sent.
    /// Typically calculated as DueDate minus 2 months.
    /// Czech: Datum odeslání notifikace
    /// </summary>
    public DateTime NotifyAt { get; set; }

    /// <summary>
    /// The channel through which the notification will be sent.
    /// Czech: Kanál pro odeslání
    /// </summary>
    public NotificationChannel Channel { get; set; } = NotificationChannel.Email;

    /// <summary>
    /// Current status of the reminder.
    /// Czech: Stav připomínky
    /// </summary>
    public ReminderStatus Status { get; set; } = ReminderStatus.Pending;

    /// <summary>
    /// The date/time when the notification was actually sent.
    /// Null if not yet sent.
    /// Czech: Datum skutečného odeslání
    /// </summary>
    public DateTime? SentAt { get; set; }

    /// <summary>
    /// Error message if the notification failed to send.
    /// Czech: Chybová zpráva
    /// </summary>
    public string? LastError { get; set; }

    /// <summary>
    /// Number of retry attempts made for failed notifications.
    /// Czech: Počet pokusů o opětovné odeslání
    /// </summary>
    public int RetryCount { get; set; } = 0;

    /// <summary>
    /// Whether the reminder is active (soft delete support).
    /// Czech: Zda je připomínka aktivní
    /// </summary>
    public bool IsActive { get; set; } = true;

    // Navigation properties

    /// <summary>
    /// The student this reminder relates to.
    /// Czech: Student
    /// </summary>
    public Student? Student { get; set; }
}
