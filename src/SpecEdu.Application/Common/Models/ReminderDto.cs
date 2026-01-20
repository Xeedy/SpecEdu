using SpecEdu.Domain.Enums;

namespace SpecEdu.Application.Common.Models;

public class ReminderDto
{
    public Guid Id { get; set; }

    public Guid StudentId { get; set; }

    public string? StudentName { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime DueDate { get; set; }

    public DateTime NotifyAt { get; set; }

    public NotificationChannel Channel { get; set; }

    public ReminderStatus Status { get; set; }

    public DateTime? SentAt { get; set; }

    public string? LastError { get; set; }

    public int RetryCount { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? CreatedByName { get; set; }
}
