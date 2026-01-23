using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Application.Common.Models;
using SpecEdu.Domain.Entities;
using SpecEdu.Domain.Enums;
using SpecEdu.Infrastructure.Data;
using SpecEdu.Infrastructure.Identity;

namespace SpecEdu.Infrastructure.Services;

/// <summary>
/// Implementation of IConsultationService.
/// Handles consultation event CRUD and participant management.
/// Czech: Implementace služby pro správu konzultací
/// </summary>
public class ConsultationService : IConsultationService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<ConsultationService> _logger;

    public ConsultationService(
        ApplicationDbContext dbContext,
        ILogger<ConsultationService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    #region Event CRUD

    public async Task<ConsultationEventDto> CreateEventAsync(CreateConsultationEventDto dto, string organizerId)
    {
        var evt = new ConsultationEvent
        {
            SchoolId = dto.SchoolId,
            StudentId = dto.StudentId,
            Title = dto.Title,
            Description = dto.Description,
            Type = dto.Type,
            Status = ConsultationEventStatus.Scheduled,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            Location = dto.Location,
            OnlineMeetingLink = dto.OnlineMeetingLink,
            IsVisibleToParents = dto.IsVisibleToParents,
            AllowResponses = dto.AllowResponses,
            PlppId = dto.PlppId,
            OrganizerId = organizerId,
            ReminderMinutesBefore = dto.ReminderMinutesBefore,
            IsActive = true
        };

        _dbContext.ConsultationEvents.Add(evt);
        await _dbContext.SaveChangesAsync();

        // Add organizer as participant
        var organizerParticipant = new ConsultationParticipant
        {
            ConsultationEventId = evt.Id,
            UserId = organizerId,
            IsOrganizer = true,
            IsRequired = true,
            ResponseStatus = ParticipantResponseStatus.Accepted,
            RespondedAt = DateTime.UtcNow
        };
        _dbContext.ConsultationParticipants.Add(organizerParticipant);

        // Add user participants
        foreach (var userId in dto.ParticipantUserIds.Where(id => id != organizerId))
        {
            var participant = new ConsultationParticipant
            {
                ConsultationEventId = evt.Id,
                UserId = userId,
                IsOrganizer = false,
                IsRequired = true,
                ResponseStatus = ParticipantResponseStatus.Pending
            };
            _dbContext.ConsultationParticipants.Add(participant);
        }

        // Add external participants
        foreach (var external in dto.ExternalParticipants)
        {
            var participant = new ConsultationParticipant
            {
                ConsultationEventId = evt.Id,
                ExternalName = external.Name,
                ExternalEmail = external.Email,
                RoleDescription = external.RoleDescription,
                IsOrganizer = false,
                IsRequired = external.IsRequired,
                ResponseStatus = ParticipantResponseStatus.Pending
            };
            _dbContext.ConsultationParticipants.Add(participant);
        }

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation(
            "Created consultation event {EventId} for school {SchoolId}, title: {Title}",
            evt.Id, dto.SchoolId, dto.Title);

        return await GetEventByIdAsync(evt.Id)
            ?? throw new InvalidOperationException("Failed to retrieve created event");
    }

    public async Task<ConsultationEventDto?> GetEventByIdAsync(Guid id)
    {
        var evt = await _dbContext.ConsultationEvents
            .Include(e => e.School)
            .Include(e => e.Student)
            .Include(e => e.Participants)
            .FirstOrDefaultAsync(e => e.Id == id && e.IsActive);

        if (evt == null)
        {
            return null;
        }

        return await MapToDto(evt);
    }

    public async Task<ConsultationEventDto?> UpdateEventAsync(UpdateConsultationEventDto dto)
    {
        var evt = await _dbContext.ConsultationEvents.FindAsync(dto.Id);

        if (evt == null || !evt.IsActive)
        {
            return null;
        }

        evt.Title = dto.Title;
        evt.Description = dto.Description;
        evt.Type = dto.Type;
        evt.StartTime = dto.StartTime;
        evt.EndTime = dto.EndTime;
        evt.Location = dto.Location;
        evt.OnlineMeetingLink = dto.OnlineMeetingLink;
        evt.IsVisibleToParents = dto.IsVisibleToParents;
        evt.AllowResponses = dto.AllowResponses;
        evt.Notes = dto.Notes;
        evt.ReminderMinutesBefore = dto.ReminderMinutesBefore;

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Updated consultation event {EventId}", dto.Id);

        return await GetEventByIdAsync(dto.Id);
    }

    public async Task<bool> DeleteEventAsync(Guid id)
    {
        var evt = await _dbContext.ConsultationEvents.FindAsync(id);

        if (evt == null)
        {
            return false;
        }

        evt.IsActive = false;

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Soft deleted consultation event {EventId}", id);

        return true;
    }

    public async Task<bool> ChangeEventStatusAsync(Guid id, ConsultationEventStatus newStatus, string? notes = null)
    {
        var evt = await _dbContext.ConsultationEvents.FindAsync(id);

        if (evt == null || !evt.IsActive)
        {
            return false;
        }

        evt.Status = newStatus;
        if (!string.IsNullOrEmpty(notes))
        {
            evt.Notes = notes;
        }

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Changed consultation event {EventId} status to {Status}", id, newStatus);

        return true;
    }

    public async Task<bool> CancelEventAsync(Guid id, string? reason = null)
    {
        return await ChangeEventStatusAsync(id, ConsultationEventStatus.Cancelled, reason);
    }

    public async Task<bool> CompleteEventAsync(Guid id, string? notes = null)
    {
        return await ChangeEventStatusAsync(id, ConsultationEventStatus.Completed, notes);
    }

    #endregion

    #region Query Methods

    public async Task<IList<ConsultationEventListItemDto>> GetEventsForSchoolAsync(
        Guid schoolId,
        DateTime from,
        DateTime to,
        ConsultationEventStatus? statusFilter = null,
        ConsultationType? typeFilter = null)
    {
        var query = _dbContext.ConsultationEvents
            .Include(e => e.Student)
            .Include(e => e.Participants)
            .Where(e => e.SchoolId == schoolId && e.IsActive)
            .Where(e => e.StartTime >= from && e.StartTime <= to);

        if (statusFilter.HasValue)
        {
            query = query.Where(e => e.Status == statusFilter.Value);
        }

        if (typeFilter.HasValue)
        {
            query = query.Where(e => e.Type == typeFilter.Value);
        }

        var events = await query
            .OrderBy(e => e.StartTime)
            .ToListAsync();

        var result = new List<ConsultationEventListItemDto>();
        foreach (var evt in events)
        {
            result.Add(await MapToListItemDto(evt));
        }

        return result;
    }

    public async Task<IList<ConsultationEventListItemDto>> GetEventsForStudentAsync(
        Guid studentId,
        DateTime? from = null,
        DateTime? to = null,
        bool visibleToParentsOnly = false)
    {
        var query = _dbContext.ConsultationEvents
            .Include(e => e.Student)
            .Include(e => e.Participants)
            .Where(e => e.StudentId == studentId && e.IsActive);

        if (from.HasValue)
        {
            query = query.Where(e => e.StartTime >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(e => e.StartTime <= to.Value);
        }

        if (visibleToParentsOnly)
        {
            query = query.Where(e => e.IsVisibleToParents);
        }

        var events = await query
            .OrderByDescending(e => e.StartTime)
            .ToListAsync();

        var result = new List<ConsultationEventListItemDto>();
        foreach (var evt in events)
        {
            result.Add(await MapToListItemDto(evt));
        }

        return result;
    }

    public async Task<IList<CalendarEventDto>> GetCalendarEventsForParentAsync(
        string parentUserId,
        DateTime from,
        DateTime to)
    {
        // Get children IDs for the parent
        var childrenIds = await _dbContext.StudentGuardians
            .Where(g => g.ParentUserId == parentUserId && g.IsActive)
            .Select(g => g.StudentId)
            .Distinct()
            .ToListAsync();

        if (!childrenIds.Any())
        {
            return new List<CalendarEventDto>();
        }

        // Get events that are visible to parents for any of the children
        var events = await _dbContext.ConsultationEvents
            .Include(e => e.Student)
            .Where(e => e.IsActive)
            .Where(e => e.IsVisibleToParents)
            .Where(e => e.StudentId.HasValue && childrenIds.Contains(e.StudentId.Value))
            .Where(e => e.StartTime >= from && e.EndTime <= to)
            .Where(e => e.Status != ConsultationEventStatus.Cancelled)
            .OrderBy(e => e.StartTime)
            .ToListAsync();

        return events.Select(e => new CalendarEventDto
        {
            Id = e.Id,
            Title = e.Title,
            Start = e.StartTime,
            End = e.EndTime,
            Type = e.Type,
            Status = e.Status,
            StudentId = e.StudentId,
            StudentName = e.Student?.FullName,
            Location = e.Location
        }).ToList();
    }

    public async Task<IList<ConsultationEventListItemDto>> GetUpcomingEventsForParentAsync(
        string parentUserId,
        int count = 10)
    {
        var now = DateTime.UtcNow;

        // Get children IDs for the parent
        var childrenIds = await _dbContext.StudentGuardians
            .Where(g => g.ParentUserId == parentUserId && g.IsActive)
            .Select(g => g.StudentId)
            .Distinct()
            .ToListAsync();

        if (!childrenIds.Any())
        {
            return new List<ConsultationEventListItemDto>();
        }

        var events = await _dbContext.ConsultationEvents
            .Include(e => e.Student)
            .Include(e => e.Participants)
            .Where(e => e.IsActive)
            .Where(e => e.IsVisibleToParents)
            .Where(e => e.StudentId.HasValue && childrenIds.Contains(e.StudentId.Value))
            .Where(e => e.StartTime >= now)
            .Where(e => e.Status != ConsultationEventStatus.Cancelled)
            .OrderBy(e => e.StartTime)
            .Take(count)
            .ToListAsync();

        var result = new List<ConsultationEventListItemDto>();
        foreach (var evt in events)
        {
            result.Add(await MapToListItemDto(evt));
        }

        return result;
    }

    public async Task<IList<ConsultationEventListItemDto>> GetEventsForUserAsync(
        string userId,
        DateTime from,
        DateTime to,
        bool includeDeclined = false)
    {
        var query = _dbContext.ConsultationEvents
            .Include(e => e.Student)
            .Include(e => e.Participants)
            .Where(e => e.IsActive)
            .Where(e => e.StartTime >= from && e.StartTime <= to)
            .Where(e => e.Participants.Any(p => p.UserId == userId));

        if (!includeDeclined)
        {
            query = query.Where(e => e.Participants
                .Where(p => p.UserId == userId)
                .All(p => p.ResponseStatus != ParticipantResponseStatus.Declined));
        }

        var events = await query
            .OrderBy(e => e.StartTime)
            .ToListAsync();

        var result = new List<ConsultationEventListItemDto>();
        foreach (var evt in events)
        {
            result.Add(await MapToListItemDto(evt));
        }

        return result;
    }

    public async Task<IList<CalendarEventDto>> GetCalendarEventsAsync(
        Guid schoolId,
        DateTime from,
        DateTime to,
        string? userId = null,
        Guid? studentId = null)
    {
        var query = _dbContext.ConsultationEvents
            .Include(e => e.Student)
            .Include(e => e.Participants)
            .Where(e => e.SchoolId == schoolId && e.IsActive)
            .Where(e => e.StartTime >= from && e.EndTime <= to);

        if (!string.IsNullOrEmpty(userId))
        {
            query = query.Where(e => e.Participants.Any(p => p.UserId == userId));
        }

        if (studentId.HasValue)
        {
            query = query.Where(e => e.StudentId == studentId.Value || e.StudentId == null);
        }

        var events = await query
            .OrderBy(e => e.StartTime)
            .ToListAsync();

        return events.Select(e => new CalendarEventDto
        {
            Id = e.Id,
            Title = e.Title,
            Start = e.StartTime,
            End = e.EndTime,
            Type = e.Type,
            Status = e.Status,
            StudentId = e.StudentId,
            StudentName = e.Student?.FullName,
            Location = e.Location
        }).ToList();
    }

    public async Task<IList<ConsultationEventListItemDto>> GetUpcomingEventsAsync(
        string userId,
        int count = 5)
    {
        var now = DateTime.UtcNow;

        var events = await _dbContext.ConsultationEvents
            .Include(e => e.Student)
            .Include(e => e.Participants)
            .Where(e => e.IsActive)
            .Where(e => e.StartTime >= now)
            .Where(e => e.Status != ConsultationEventStatus.Cancelled)
            .Where(e => e.Participants.Any(p => p.UserId == userId && p.ResponseStatus != ParticipantResponseStatus.Declined))
            .OrderBy(e => e.StartTime)
            .Take(count)
            .ToListAsync();

        var result = new List<ConsultationEventListItemDto>();
        foreach (var evt in events)
        {
            result.Add(await MapToListItemDto(evt));
        }

        return result;
    }

    public async Task<IList<ConsultationEventDto>> GetEventsPendingReminderAsync()
    {
        var now = DateTime.UtcNow;

        var events = await _dbContext.ConsultationEvents
            .Include(e => e.School)
            .Include(e => e.Student)
            .Include(e => e.Participants)
            .Where(e => e.IsActive)
            .Where(e => e.ReminderMinutesBefore.HasValue)
            .Where(e => !e.ReminderSent)
            .Where(e => e.Status == ConsultationEventStatus.Scheduled || e.Status == ConsultationEventStatus.Confirmed)
            .Where(e => e.StartTime <= now.AddMinutes(e.ReminderMinutesBefore!.Value))
            .Where(e => e.StartTime > now)
            .ToListAsync();

        var result = new List<ConsultationEventDto>();
        foreach (var evt in events)
        {
            result.Add(await MapToDto(evt));
        }

        return result;
    }

    #endregion

    #region Participant Management

    public async Task<ConsultationParticipantDto> AddParticipantAsync(AddParticipantDto dto)
    {
        var participant = new ConsultationParticipant
        {
            ConsultationEventId = dto.ConsultationEventId,
            UserId = dto.UserId,
            ExternalName = dto.ExternalName,
            ExternalEmail = dto.ExternalEmail,
            RoleDescription = dto.RoleDescription,
            IsRequired = dto.IsRequired,
            IsOrganizer = false,
            ResponseStatus = ParticipantResponseStatus.Pending
        };

        _dbContext.ConsultationParticipants.Add(participant);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Added participant {ParticipantId} to event {EventId}",
            participant.Id, dto.ConsultationEventId);

        return await MapParticipantToDto(participant);
    }

    public async Task<bool> RemoveParticipantAsync(Guid participantId)
    {
        var participant = await _dbContext.ConsultationParticipants.FindAsync(participantId);

        if (participant == null)
        {
            return false;
        }

        // Don't allow removing the organizer
        if (participant.IsOrganizer)
        {
            return false;
        }

        _dbContext.ConsultationParticipants.Remove(participant);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Removed participant {ParticipantId}", participantId);

        return true;
    }

    public async Task<bool> RespondToInvitationAsync(ParticipantResponseDto dto)
    {
        var participant = await _dbContext.ConsultationParticipants.FindAsync(dto.ParticipantId);

        if (participant == null)
        {
            return false;
        }

        participant.ResponseStatus = dto.ResponseStatus;
        participant.ResponseNote = dto.ResponseNote;
        participant.RespondedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Participant {ParticipantId} responded with {Status}",
            dto.ParticipantId, dto.ResponseStatus);

        return true;
    }

    public async Task<bool> MarkAttendanceAsync(MarkAttendanceDto dto)
    {
        var participant = await _dbContext.ConsultationParticipants.FindAsync(dto.ParticipantId);

        if (participant == null)
        {
            return false;
        }

        participant.Attended = dto.Attended;

        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> MarkAttendanceAsync(Guid eventId, IList<MarkAttendanceDto> attendances)
    {
        var participantIds = attendances.Select(a => a.ParticipantId).ToList();
        var participants = await _dbContext.ConsultationParticipants
            .Where(p => participantIds.Contains(p.Id) && p.ConsultationEventId == eventId)
            .ToListAsync();

        foreach (var attendance in attendances)
        {
            var participant = participants.FirstOrDefault(p => p.Id == attendance.ParticipantId);
            if (participant != null)
            {
                participant.Attended = attendance.Attended;
            }
        }

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Marked attendance for {Count} participants in event {EventId}",
            attendances.Count, eventId);

        return true;
    }

    public async Task<IList<ConsultationParticipantDto>> GetParticipantsAsync(Guid eventId)
    {
        var participants = await _dbContext.ConsultationParticipants
            .Where(p => p.ConsultationEventId == eventId)
            .OrderByDescending(p => p.IsOrganizer)
            .ThenBy(p => p.ResponseStatus)
            .ToListAsync();

        var result = new List<ConsultationParticipantDto>();
        foreach (var participant in participants)
        {
            result.Add(await MapParticipantToDto(participant));
        }

        return result;
    }

    public async Task<int> InviteStudentGuardiansAsync(Guid eventId, Guid studentId)
    {
        var guardians = await _dbContext.StudentGuardians
            .Where(g => g.StudentId == studentId && g.IsActive)
            .ToListAsync();

        var count = 0;
        foreach (var guardian in guardians)
        {
            // Check if already invited
            var existing = await _dbContext.ConsultationParticipants
                .AnyAsync(p => p.ConsultationEventId == eventId && p.UserId == guardian.ParentUserId);

            if (!existing)
            {
                var participant = new ConsultationParticipant
                {
                    ConsultationEventId = eventId,
                    UserId = guardian.ParentUserId,
                    RoleDescription = GetRelationshipTypeName(guardian.RelationshipType),
                    IsRequired = true,
                    ResponseStatus = ParticipantResponseStatus.Pending
                };
                _dbContext.ConsultationParticipants.Add(participant);
                count++;
            }
        }

        if (count > 0)
        {
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Invited {Count} guardians to event {EventId}", count, eventId);
        }

        return count;
    }

    public async Task<int> InviteStudentStaffAsync(Guid eventId, Guid studentId)
    {
        var staffLinks = await _dbContext.StudentStaffLinks
            .Where(s => s.StudentId == studentId && s.IsActive)
            .ToListAsync();

        var count = 0;
        foreach (var link in staffLinks)
        {
            // Check if already invited
            var existing = await _dbContext.ConsultationParticipants
                .AnyAsync(p => p.ConsultationEventId == eventId && p.UserId == link.UserId);

            if (!existing)
            {
                var participant = new ConsultationParticipant
                {
                    ConsultationEventId = eventId,
                    UserId = link.UserId,
                    RoleDescription = GetStaffLinkTypeName(link.LinkType),
                    IsRequired = true,
                    ResponseStatus = ParticipantResponseStatus.Pending
                };
                _dbContext.ConsultationParticipants.Add(participant);
                count++;
            }
        }

        if (count > 0)
        {
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Invited {Count} staff members to event {EventId}", count, eventId);
        }

        return count;
    }

    #endregion

    #region Notifications

    public async Task<int> SendInvitationNotificationsAsync(Guid eventId)
    {
        var participants = await _dbContext.ConsultationParticipants
            .Where(p => p.ConsultationEventId == eventId && !p.NotificationSent)
            .ToListAsync();

        // TODO: Integrate with email service to actually send notifications
        var count = 0;
        foreach (var participant in participants)
        {
            participant.NotificationSent = true;
            participant.NotificationSentAt = DateTime.UtcNow;
            count++;
        }

        if (count > 0)
        {
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Marked {Count} invitation notifications as sent for event {EventId}",
                count, eventId);
        }

        return count;
    }

    public async Task<int> SendReminderNotificationsAsync(Guid eventId)
    {
        var evt = await _dbContext.ConsultationEvents
            .Include(e => e.Participants)
            .FirstOrDefaultAsync(e => e.Id == eventId);

        if (evt == null)
        {
            return 0;
        }

        var participantsToRemind = evt.Participants
            .Where(p => p.ResponseStatus == ParticipantResponseStatus.Accepted ||
                       p.ResponseStatus == ParticipantResponseStatus.Pending ||
                       p.ResponseStatus == ParticipantResponseStatus.Tentative)
            .ToList();

        // TODO: Integrate with email service to actually send reminders
        _logger.LogInformation("Would send reminder to {Count} participants for event {EventId}",
            participantsToRemind.Count, eventId);

        return participantsToRemind.Count;
    }

    public async Task<bool> MarkReminderSentAsync(Guid eventId)
    {
        var evt = await _dbContext.ConsultationEvents.FindAsync(eventId);

        if (evt == null)
        {
            return false;
        }

        evt.ReminderSent = true;

        await _dbContext.SaveChangesAsync();

        return true;
    }

    #endregion

    #region Private Mapping Methods

    private async Task<ConsultationEventDto> MapToDto(ConsultationEvent evt)
    {
        var organizerName = await GetUserDisplayNameAsync(evt.OrganizerId);

        var participantDtos = new List<ConsultationParticipantDto>();
        foreach (var participant in evt.Participants)
        {
            participantDtos.Add(await MapParticipantToDto(participant));
        }

        return new ConsultationEventDto
        {
            Id = evt.Id,
            SchoolId = evt.SchoolId,
            SchoolName = evt.School?.Name,
            StudentId = evt.StudentId,
            StudentName = evt.Student?.FullName,
            Title = evt.Title,
            Description = evt.Description,
            Type = evt.Type,
            Status = evt.Status,
            StartTime = evt.StartTime,
            EndTime = evt.EndTime,
            Location = evt.Location,
            OnlineMeetingLink = evt.OnlineMeetingLink,
            IsVisibleToParents = evt.IsVisibleToParents,
            AllowResponses = evt.AllowResponses,
            Notes = evt.Notes,
            PlppId = evt.PlppId,
            OrganizerId = evt.OrganizerId,
            OrganizerName = organizerName,
            ReminderMinutesBefore = evt.ReminderMinutesBefore,
            ReminderSent = evt.ReminderSent,
            CreatedAt = evt.CreatedAt,
            ModifiedAt = evt.ModifiedAt,
            IsActive = evt.IsActive,
            Participants = participantDtos
        };
    }

    private async Task<ConsultationEventListItemDto> MapToListItemDto(ConsultationEvent evt)
    {
        var organizerName = await GetUserDisplayNameAsync(evt.OrganizerId);

        return new ConsultationEventListItemDto
        {
            Id = evt.Id,
            SchoolId = evt.SchoolId,
            StudentId = evt.StudentId,
            StudentName = evt.Student?.FullName,
            Title = evt.Title,
            Type = evt.Type,
            Status = evt.Status,
            StartTime = evt.StartTime,
            EndTime = evt.EndTime,
            Location = evt.Location,
            IsVisibleToParents = evt.IsVisibleToParents,
            OrganizerName = organizerName,
            ParticipantCount = evt.Participants.Count,
            AcceptedCount = evt.Participants.Count(p => p.ResponseStatus == ParticipantResponseStatus.Accepted),
            DeclinedCount = evt.Participants.Count(p => p.ResponseStatus == ParticipantResponseStatus.Declined),
            PendingCount = evt.Participants.Count(p => p.ResponseStatus == ParticipantResponseStatus.Pending)
        };
    }

    private async Task<ConsultationParticipantDto> MapParticipantToDto(ConsultationParticipant participant)
    {
        string? userName = null;
        string? userEmail = null;

        if (!string.IsNullOrEmpty(participant.UserId))
        {
            var user = await _dbContext.Users
                .OfType<ApplicationUser>()
                .FirstOrDefaultAsync(u => u.Id == participant.UserId);

            userName = user?.FullName ?? user?.Email;
            userEmail = user?.Email;
        }

        return new ConsultationParticipantDto
        {
            Id = participant.Id,
            ConsultationEventId = participant.ConsultationEventId,
            UserId = participant.UserId,
            UserName = userName,
            UserEmail = userEmail,
            ExternalName = participant.ExternalName,
            ExternalEmail = participant.ExternalEmail,
            RoleDescription = participant.RoleDescription,
            ResponseStatus = participant.ResponseStatus,
            RespondedAt = participant.RespondedAt,
            ResponseNote = participant.ResponseNote,
            IsOrganizer = participant.IsOrganizer,
            IsRequired = participant.IsRequired,
            NotificationSent = participant.NotificationSent,
            NotificationSentAt = participant.NotificationSentAt,
            Attended = participant.Attended,
            CreatedAt = participant.CreatedAt
        };
    }

    private async Task<string?> GetUserDisplayNameAsync(string? userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return null;
        }

        var user = await _dbContext.Users
            .OfType<ApplicationUser>()
            .FirstOrDefaultAsync(u => u.Id == userId);

        return user?.FullName ?? user?.Email;
    }

    private static string GetRelationshipTypeName(RelationshipType type)
    {
        return type switch
        {
            RelationshipType.Mother => "Matka",
            RelationshipType.Father => "Otec",
            RelationshipType.LegalGuardian => "Zakonny zastupce",
            RelationshipType.Other => "Jiny",
            _ => type.ToString()
        };
    }

    private static string GetStaffLinkTypeName(StaffLinkType type)
    {
        return type switch
        {
            StaffLinkType.Teacher => "Ucitel",
            StaffLinkType.Assistant => "Asistent pedagoga",
            StaffLinkType.SPP => "SPP",
            StaffLinkType.PPP => "PPP",
            StaffLinkType.SPC => "SPC",
            _ => type.ToString()
        };
    }

    #endregion
}
