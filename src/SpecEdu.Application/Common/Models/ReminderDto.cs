using SpecEdu.Domain.Enums;

namespace SpecEdu.Application.Common.Models;

/// <summary>
/// Data transfer object for Reminder entity.
/// Used to display reminders in management views.
/// </summary>
public class ReminderDto
{
    /// <summary>
    /// Unique identifier of the reminder.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// ID of the student this reminder relates to.
    /// </summary>
    public Guid StudentId { get; set; }

    /// <summary>
    /// Name of the student (populated when needed).
    /// </summary>
    public string? StudentName { get; set; }

    /// <summary>
    /// Short title for the reminder.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Optional detailed description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The due date for the event being reminded about.
    /// </summary>
    public DateTime DueDate { get; set; }

    /// <summary>
    /// The date/time when the notification will be sent.
    /// </summary>
    public DateTime NotifyAt { get; set; }

    /// <summary>
    /// The channel through which the notification is sent.
    /// </summary>
    public NotificationChannel Channel { get; set; }

    /// <summary>
    /// Current status of the reminder.
    /// </summary>
    public ReminderStatus Status { get; set; }

    /// <summary>
    /// The date/time when the notification was actually sent.
    /// </summary>
    public DateTime? SentAt { get; set; }

    /// <summary>
    /// Error message if the notification failed to send.
    /// </summary>
    public string? LastError { get; set; }

    /// <summary>
    /// Number of retry attempts made.
    /// </summary>
    public int RetryCount { get; set; }

    /// <summary>
    /// Whether the reminder is active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// UTC timestamp when the reminder was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// ID of the user who created the reminder.
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Name of the user who created the reminder (populated when needed).
    /// </summary>
    public string? CreatedByName { get; set; }
}
