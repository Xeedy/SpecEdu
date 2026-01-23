using SpecEdu.Application.Common.Models;
using SpecEdu.Domain.Enums;

namespace SpecEdu.Application.Common.Interfaces;

public interface IConsultationService
{
    #region Event CRUD

    Task<ConsultationEventDto> CreateEventAsync(CreateConsultationEventDto dto, string organizerId);

    Task<ConsultationEventDto?> GetEventByIdAsync(Guid id);

    Task<ConsultationEventDto?> UpdateEventAsync(UpdateConsultationEventDto dto);

    Task<bool> DeleteEventAsync(Guid id);

    Task<bool> ChangeEventStatusAsync(Guid id, ConsultationEventStatus newStatus, string? notes = null);

    Task<bool> CancelEventAsync(Guid id, string? reason = null);

    Task<bool> CompleteEventAsync(Guid id, string? notes = null);

    #endregion

    #region Query Methods

    Task<IList<ConsultationEventListItemDto>> GetEventsForSchoolAsync(
        Guid schoolId,
        DateTime from,
        DateTime to,
        ConsultationEventStatus? statusFilter = null,
        ConsultationType? typeFilter = null);

    Task<IList<ConsultationEventListItemDto>> GetEventsForStudentAsync(
        Guid studentId,
        DateTime? from = null,
        DateTime? to = null,
        bool visibleToParentsOnly = false);

    Task<IList<CalendarEventDto>> GetCalendarEventsForParentAsync(
        string parentUserId,
        DateTime from,
        DateTime to);

    Task<IList<ConsultationEventListItemDto>> GetUpcomingEventsForParentAsync(
        string parentUserId,
        int count = 10);

    Task<IList<ConsultationEventListItemDto>> GetEventsForUserAsync(
        string userId,
        DateTime from,
        DateTime to,
        bool includeDeclined = false);

    Task<IList<CalendarEventDto>> GetCalendarEventsAsync(
        Guid schoolId,
        DateTime from,
        DateTime to,
        string? userId = null,
        Guid? studentId = null);

    Task<IList<ConsultationEventListItemDto>> GetUpcomingEventsAsync(
        string userId,
        int count = 5);

    Task<IList<ConsultationEventDto>> GetEventsPendingReminderAsync();

    #endregion

    #region Participant Management

    Task<ConsultationParticipantDto> AddParticipantAsync(AddParticipantDto dto);

    Task<bool> RemoveParticipantAsync(Guid participantId);

    Task<bool> RespondToInvitationAsync(ParticipantResponseDto dto);

    Task<bool> MarkAttendanceAsync(MarkAttendanceDto dto);

    Task<bool> MarkAttendanceAsync(Guid eventId, IList<MarkAttendanceDto> attendances);

    Task<IList<ConsultationParticipantDto>> GetParticipantsAsync(Guid eventId);

    Task<int> InviteStudentGuardiansAsync(Guid eventId, Guid studentId);

    Task<int> InviteStudentStaffAsync(Guid eventId, Guid studentId);

    #endregion

    #region Notifications

    Task<int> SendInvitationNotificationsAsync(Guid eventId);

    Task<int> SendReminderNotificationsAsync(Guid eventId);

    Task<bool> MarkReminderSentAsync(Guid eventId);

    #endregion
}
