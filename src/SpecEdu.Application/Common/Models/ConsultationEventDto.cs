using SpecEdu.Domain.Enums;

namespace SpecEdu.Application.Common.Models;

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

    public IList<ConsultationParticipantDto> Participants { get; set; } = new List<ConsultationParticipantDto>();

    public int AcceptedCount => Participants.Count(p => p.ResponseStatus == ParticipantResponseStatus.Accepted);

    public int DeclinedCount => Participants.Count(p => p.ResponseStatus == ParticipantResponseStatus.Declined);

    public int PendingCount => Participants.Count(p => p.ResponseStatus == ParticipantResponseStatus.Pending);
}

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

    public DateTime StartTime => Start;
    public DateTime EndTime => End;
    public string Color => GetColorByType();
    public string BorderColor => GetBorderColorByStatus();

    private string GetColorByType()
    {
        return Type switch
        {
            ConsultationType.ParentTeacherMeeting => "#0d6efd",
            ConsultationType.IndividualConsultation => "#198754",
            ConsultationType.PlppReview => "#dc3545",
            ConsultationType.IvpReview => "#dc3545",
            ConsultationType.CounselorMeeting => "#6f42c1",
            ConsultationType.ExternalSpecialistMeeting => "#fd7e14",
            ConsultationType.SchoolEvent => "#20c997",
            _ => "#6c757d"
        };
    }

    private string GetBorderColorByStatus()
    {
        return Status switch
        {
            ConsultationEventStatus.Scheduled => "#ffc107",
            ConsultationEventStatus.Confirmed => "#198754",
            ConsultationEventStatus.Completed => "#6c757d",
            ConsultationEventStatus.Cancelled => "#dc3545",
            ConsultationEventStatus.Rescheduled => "#0dcaf0",
            _ => "#6c757d"
        };
    }
}

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

    public IList<string> ParticipantUserIds { get; set; } = new List<string>();

    public IList<ExternalParticipantDto> ExternalParticipants { get; set; } = new List<ExternalParticipantDto>();
}

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

public class ExternalParticipantDto
{
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? RoleDescription { get; set; }
    public bool IsRequired { get; set; } = true;
}
