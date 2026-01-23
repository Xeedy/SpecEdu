using SpecEdu.Domain.Enums;

namespace SpecEdu.Application.Common.Models;

/// <summary>
/// Full DTO for a consultation event.
/// Czech: Kompletní DTO konzultace
/// </summary>
public class ConsultationEventDto
{
    public Guid Id { get; set; }
    public Guid SchoolId { get; set; }
    public string? SchoolName { get; set; }
    public Guid? StudentId { get; set; }
    public string? StudentName { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ConsultationType Type { get; set; }
    public ConsultationEventStatus Status { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? Location { get; set; }
    public string? OnlineMeetingLink { get; set; }
    public bool IsVisibleToParents { get; set; }
    public bool AllowResponses { get; set; }
    public string? Notes { get; set; }
    public Guid? PlppId { get; set; }
    public string? OrganizerId { get; set; }
    public string? OrganizerName { get; set; }
    public int? ReminderMinutesBefore { get; set; }
    public bool ReminderSent { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public bool IsActive { get; set; }

    /// <summary>
    /// Participants of this event.
    /// </summary>
    public IList<ConsultationParticipantDto> Participants { get; set; } = new List<ConsultationParticipantDto>();

    /// <summary>
    /// Count of accepted participants.
    /// </summary>
    public int AcceptedCount => Participants.Count(p => p.ResponseStatus == ParticipantResponseStatus.Accepted);

    /// <summary>
    /// Count of declined participants.
    /// </summary>
    public int DeclinedCount => Participants.Count(p => p.ResponseStatus == ParticipantResponseStatus.Declined);

    /// <summary>
    /// Count of pending responses.
    /// </summary>
    public int PendingCount => Participants.Count(p => p.ResponseStatus == ParticipantResponseStatus.Pending);
}

/// <summary>
/// Lightweight DTO for consultation event list views.
/// Czech: Lehký DTO pro seznam konzultací
/// </summary>
public class ConsultationEventListItemDto
{
    public Guid Id { get; set; }
    public Guid SchoolId { get; set; }
    public Guid? StudentId { get; set; }
    public string? StudentName { get; set; }
    public string Title { get; set; } = string.Empty;
    public ConsultationType Type { get; set; }
    public ConsultationEventStatus Status { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? Location { get; set; }
    public bool IsVisibleToParents { get; set; }
    public string? OrganizerName { get; set; }
    public int ParticipantCount { get; set; }
    public int AcceptedCount { get; set; }
    public int DeclinedCount { get; set; }
    public int PendingCount { get; set; }
}

/// <summary>
/// DTO for calendar view (minimal data for performance).
/// Czech: DTO pro kalendářové zobrazení
/// </summary>
public class CalendarEventDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public ConsultationType Type { get; set; }
    public ConsultationEventStatus Status { get; set; }
    public Guid? StudentId { get; set; }
    public string? StudentName { get; set; }
    public string? Location { get; set; }
    public bool AllDay => StartTime == Start.Date && EndTime == End.Date.AddDays(1).AddSeconds(-1);

    // For FullCalendar JS library compatibility
    public DateTime StartTime => Start;
    public DateTime EndTime => End;
    public string Color => GetColorByType();
    public string BorderColor => GetBorderColorByStatus();

    private string GetColorByType()
    {
        return Type switch
        {
            ConsultationType.ParentTeacherMeeting => "#0d6efd", // primary
            ConsultationType.IndividualConsultation => "#198754", // success
            ConsultationType.PlppReview => "#dc3545", // danger
            ConsultationType.IvpReview => "#dc3545", // danger
            ConsultationType.CounselorMeeting => "#6f42c1", // purple
            ConsultationType.ExternalSpecialistMeeting => "#fd7e14", // orange
            ConsultationType.SchoolEvent => "#20c997", // teal
            _ => "#6c757d" // secondary
        };
    }

    private string GetBorderColorByStatus()
    {
        return Status switch
        {
            ConsultationEventStatus.Scheduled => "#ffc107", // warning
            ConsultationEventStatus.Confirmed => "#198754", // success
            ConsultationEventStatus.Completed => "#6c757d", // secondary
            ConsultationEventStatus.Cancelled => "#dc3545", // danger
            ConsultationEventStatus.Rescheduled => "#0dcaf0", // info
            _ => "#6c757d"
        };
    }
}

/// <summary>
/// DTO for creating a new consultation event.
/// Czech: DTO pro vytvoření konzultace
/// </summary>
public class CreateConsultationEventDto
{
    public Guid SchoolId { get; set; }
    public Guid? StudentId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ConsultationType Type { get; set; } = ConsultationType.IndividualConsultation;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? Location { get; set; }
    public string? OnlineMeetingLink { get; set; }
    public bool IsVisibleToParents { get; set; } = true;
    public bool AllowResponses { get; set; } = true;
    public Guid? PlppId { get; set; }
    public int? ReminderMinutesBefore { get; set; } = 1440;

    /// <summary>
    /// User IDs to invite as participants.
    /// </summary>
    public IList<string> ParticipantUserIds { get; set; } = new List<string>();

    /// <summary>
    /// External participants (name + email pairs).
    /// </summary>
    public IList<ExternalParticipantDto> ExternalParticipants { get; set; } = new List<ExternalParticipantDto>();
}

/// <summary>
/// DTO for updating a consultation event.
/// Czech: DTO pro úpravu konzultace
/// </summary>
public class UpdateConsultationEventDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ConsultationType Type { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? Location { get; set; }
    public string? OnlineMeetingLink { get; set; }
    public bool IsVisibleToParents { get; set; }
    public bool AllowResponses { get; set; }
    public string? Notes { get; set; }
    public int? ReminderMinutesBefore { get; set; }
}

/// <summary>
/// DTO for external (non-registered) participant.
/// Czech: DTO pro externího účastníka
/// </summary>
public class ExternalParticipantDto
{
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? RoleDescription { get; set; }
    public bool IsRequired { get; set; } = true;
}
