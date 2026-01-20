using SpecEdu.Domain.Common;
using SpecEdu.Domain.Enums;

namespace SpecEdu.Domain.Entities;

public class Reminder : AuditableEntity
{
    public Guid StudentId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime DueDate { get; set; }

    public DateTime NotifyAt { get; set; }

    public NotificationChannel Channel { get; set; } = NotificationChannel.Email;

    public ReminderStatus Status { get; set; } = ReminderStatus.Pending;

    public DateTime? SentAt { get; set; }

    public string? LastError { get; set; }

    public int RetryCount { get; set; } = 0;

    public bool IsActive { get; set; } = true;

    public Student? Student { get; set; }
}
