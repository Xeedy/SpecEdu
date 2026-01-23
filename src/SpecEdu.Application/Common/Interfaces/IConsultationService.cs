using SpecEdu.Application.Common.Models;
using SpecEdu.Domain.Enums;

namespace SpecEdu.Application.Common.Interfaces;

/// <summary>
/// Service interface for managing consultation events.
/// Czech: Rozhraní služby pro správu konzultací
/// </summary>
public interface IConsultationService
{
    #region Event CRUD

    /// <summary>
    /// Creates a new consultation event.
    /// Czech: Vytvoří novou konzultaci
    /// </summary>
    Task<ConsultationEventDto> CreateEventAsync(CreateConsultationEventDto dto, string organizerId);

    /// <summary>
    /// Gets a consultation event by ID.
    /// Czech: Získá konzultaci podle ID
    /// </summary>
    Task<ConsultationEventDto?> GetEventByIdAsync(Guid id);

    /// <summary>
    /// Updates a consultation event.
    /// Czech: Aktualizuje konzultaci
    /// </summary>
    Task<ConsultationEventDto?> UpdateEventAsync(UpdateConsultationEventDto dto);

    /// <summary>
    /// Soft deletes (deactivates) a consultation event.
    /// Czech: Smaže konzultaci
    /// </summary>
    Task<bool> DeleteEventAsync(Guid id);

    /// <summary>
    /// Changes the status of a consultation event.
    /// Czech: Změní stav konzultace
    /// </summary>
    Task<bool> ChangeEventStatusAsync(Guid id, ConsultationEventStatus newStatus, string? notes = null);

    /// <summary>
    /// Cancels a consultation event.
    /// Czech: Zruší konzultaci
    /// </summary>
    Task<bool> CancelEventAsync(Guid id, string? reason = null);

    /// <summary>
    /// Marks a consultation event as completed.
    /// Czech: Označí konzultaci jako proběhlou
    /// </summary>
    Task<bool> CompleteEventAsync(Guid id, string? notes = null);

    #endregion

    #region Query Methods

    /// <summary>
    /// Gets all events for a school in a date range.
    /// Czech: Získá všechny konzultace školy v období
    /// </summary>
    Task<IList<ConsultationEventListItemDto>> GetEventsForSchoolAsync(
        Guid schoolId,
        DateTime from,
        DateTime to,
        ConsultationEventStatus? statusFilter = null,
        ConsultationType? typeFilter = null);

    /// <summary>
    /// Gets all events for a specific student.
    /// Czech: Získá všechny konzultace žáka
    /// </summary>
    Task<IList<ConsultationEventListItemDto>> GetEventsForStudentAsync(
        Guid studentId,
        DateTime? from = null,
        DateTime? to = null,
        bool visibleToParentsOnly = false);

    /// <summary>
    /// Gets calendar events for a parent (visible events for their children).
    /// Czech: Získá kalendářové události pro rodiče (viditelné pro jejich děti)
    /// </summary>
    Task<IList<CalendarEventDto>> GetCalendarEventsForParentAsync(
        string parentUserId,
        DateTime from,
        DateTime to);

    /// <summary>
    /// Gets upcoming visible events for a parent's children.
    /// Czech: Získá nadcházející viditelné události pro děti rodiče
    /// </summary>
    Task<IList<ConsultationEventListItemDto>> GetUpcomingEventsForParentAsync(
        string parentUserId,
        int count = 10);

    /// <summary>
    /// Gets all events where a user is a participant.
    /// Czech: Získá všechny konzultace, kde je uživatel účastníkem
    /// </summary>
    Task<IList<ConsultationEventListItemDto>> GetEventsForUserAsync(
        string userId,
        DateTime from,
        DateTime to,
        bool includeDeclined = false);

    /// <summary>
    /// Gets calendar events for rendering in a calendar view.
    /// Czech: Získá události pro kalendářové zobrazení
    /// </summary>
    Task<IList<CalendarEventDto>> GetCalendarEventsAsync(
        Guid schoolId,
        DateTime from,
        DateTime to,
        string? userId = null,
        Guid? studentId = null);

    /// <summary>
    /// Gets upcoming events for a user (for dashboard).
    /// Czech: Získá nadcházející konzultace uživatele
    /// </summary>
    Task<IList<ConsultationEventListItemDto>> GetUpcomingEventsAsync(
        string userId,
        int count = 5);

    /// <summary>
    /// Gets events that need reminder notifications sent.
    /// Czech: Získá konzultace čekající na připomínku
    /// </summary>
    Task<IList<ConsultationEventDto>> GetEventsPendingReminderAsync();

    #endregion

    #region Participant Management

    /// <summary>
    /// Adds a participant to an event.
    /// Czech: Přidá účastníka ke konzultaci
    /// </summary>
    Task<ConsultationParticipantDto> AddParticipantAsync(AddParticipantDto dto);

    /// <summary>
    /// Removes a participant from an event.
    /// Czech: Odebere účastníka z konzultace
    /// </summary>
    Task<bool> RemoveParticipantAsync(Guid participantId);

    /// <summary>
    /// Records a participant's response to an event invitation.
    /// Czech: Zaznamená odpověď účastníka
    /// </summary>
    Task<bool> RespondToInvitationAsync(ParticipantResponseDto dto);

    /// <summary>
    /// Marks a participant's attendance after the event.
    /// Czech: Zaznamená účast po události
    /// </summary>
    Task<bool> MarkAttendanceAsync(MarkAttendanceDto dto);

    /// <summary>
    /// Marks attendance for multiple participants.
    /// Czech: Zaznamená účast více účastníků
    /// </summary>
    Task<bool> MarkAttendanceAsync(Guid eventId, IList<MarkAttendanceDto> attendances);

    /// <summary>
    /// Gets participants for an event.
    /// Czech: Získá účastníky konzultace
    /// </summary>
    Task<IList<ConsultationParticipantDto>> GetParticipantsAsync(Guid eventId);

    /// <summary>
    /// Invites guardians of a student to an event.
    /// Czech: Pozve zákonné zástupce žáka
    /// </summary>
    Task<int> InviteStudentGuardiansAsync(Guid eventId, Guid studentId);

    /// <summary>
    /// Invites all staff linked to a student to an event.
    /// Czech: Pozve všechny zaměstnance přiřazené k žákovi
    /// </summary>
    Task<int> InviteStudentStaffAsync(Guid eventId, Guid studentId);

    #endregion

    #region Notifications

    /// <summary>
    /// Sends invitation notifications to all pending participants.
    /// Czech: Odešle pozvánky všem čekajícím účastníkům
    /// </summary>
    Task<int> SendInvitationNotificationsAsync(Guid eventId);

    /// <summary>
    /// Sends reminder notifications for an event.
    /// Czech: Odešle připomínky pro konzultaci
    /// </summary>
    Task<int> SendReminderNotificationsAsync(Guid eventId);

    /// <summary>
    /// Marks event reminder as sent.
    /// Czech: Označí připomínku jako odeslanou
    /// </summary>
    Task<bool> MarkReminderSentAsync(Guid eventId);

    #endregion
}
