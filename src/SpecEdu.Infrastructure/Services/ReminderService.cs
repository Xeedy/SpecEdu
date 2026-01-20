using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Application.Common.Models;
using SpecEdu.Domain.Entities;
using SpecEdu.Domain.Enums;
using SpecEdu.Infrastructure.Data;
using SpecEdu.Infrastructure.Identity;

namespace SpecEdu.Infrastructure.Services;

public class ReminderService : IReminderService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<ReminderService> _logger;

    private const int DefaultNotifyMonthsBefore = 2;

    public ReminderService(
        ApplicationDbContext dbContext,
        UserManager<ApplicationUser> userManager,
        ILogger<ReminderService> logger)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _logger = logger;
    }

    #region CRUD Operations

    public async Task<ReminderDto> CreateAsync(Guid studentId, string title, DateTime dueDate, string? description = null)
    {
        var notifyAt = dueDate.AddMonths(-DefaultNotifyMonthsBefore);

        if (notifyAt < DateTime.UtcNow)
        {
            notifyAt = DateTime.UtcNow;
        }

        var reminder = new Reminder
        {
            StudentId = studentId,
            Title = title,
            Description = description,
            DueDate = dueDate,
            NotifyAt = notifyAt,
            Channel = NotificationChannel.Email,
            Status = ReminderStatus.Pending,
            IsActive = true
        };

        _dbContext.Reminders.Add(reminder);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation(
            "Created reminder {ReminderId} for student {StudentId}: DueDate={DueDate}, NotifyAt={NotifyAt}",
            reminder.Id, studentId, dueDate, notifyAt);

        return await GetByIdAsync(reminder.Id) ?? throw new InvalidOperationException("Failed to retrieve created reminder");
    }

    public async Task<ReminderDto?> GetByIdAsync(Guid id)
    {
        var reminder = await _dbContext.Reminders
            .Include(r => r.Student)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (reminder == null) return null;

        return await MapToDtoAsync(reminder);
    }

    public async Task<IList<ReminderDto>> GetForStudentAsync(Guid studentId, bool includeInactive = false)
    {
        var query = _dbContext.Reminders
            .Include(r => r.Student)
            .Where(r => r.StudentId == studentId);

        if (!includeInactive)
        {
            query = query.Where(r => r.IsActive);
        }

        var reminders = await query
            .OrderByDescending(r => r.DueDate)
            .ToListAsync();

        var dtos = new List<ReminderDto>();
        foreach (var reminder in reminders)
        {
            dtos.Add(await MapToDtoAsync(reminder));
        }

        return dtos;
    }

    public async Task<ReminderDto?> UpdateAsync(Guid id, string title, DateTime dueDate, string? description)
    {
        var reminder = await _dbContext.Reminders.FindAsync(id);

        if (reminder == null || !reminder.IsActive)
        {
            return null;
        }

        if (reminder.Status != ReminderStatus.Pending)
        {
            _logger.LogWarning("Attempted to update non-pending reminder {ReminderId}", id);
            return null;
        }

        reminder.Title = title;
        reminder.Description = description;
        reminder.DueDate = dueDate;

        var notifyAt = dueDate.AddMonths(-DefaultNotifyMonthsBefore);
        reminder.NotifyAt = notifyAt < DateTime.UtcNow ? DateTime.UtcNow : notifyAt;

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Updated reminder {ReminderId}: DueDate={DueDate}", id, dueDate);

        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var reminder = await _dbContext.Reminders.FindAsync(id);

        if (reminder == null)
        {
            return false;
        }

        _dbContext.Reminders.Remove(reminder);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Deleted reminder {ReminderId}", id);

        return true;
    }

    #endregion

    #region Status Operations

    public async Task<IList<ReminderDto>> GetPendingRemindersAsync(int maxRetryCount = 3)
    {
        var now = DateTime.UtcNow;

        var reminders = await _dbContext.Reminders
            .Include(r => r.Student)
            .Where(r =>
                r.IsActive &&
                r.Status == ReminderStatus.Pending &&
                r.NotifyAt <= now &&
                r.RetryCount < maxRetryCount)
            .OrderBy(r => r.NotifyAt)
            .ToListAsync();

        var dtos = new List<ReminderDto>();
        foreach (var reminder in reminders)
        {
            dtos.Add(await MapToDtoAsync(reminder));
        }

        return dtos;
    }

    public async Task<bool> MarkAsSentAsync(Guid id)
    {
        var reminder = await _dbContext.Reminders.FindAsync(id);

        if (reminder == null)
        {
            return false;
        }

        reminder.Status = ReminderStatus.Sent;
        reminder.SentAt = DateTime.UtcNow;
        reminder.LastError = null;

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Marked reminder {ReminderId} as sent", id);

        return true;
    }

    public async Task<bool> MarkAsFailedAsync(Guid id, string error)
    {
        var reminder = await _dbContext.Reminders.FindAsync(id);

        if (reminder == null)
        {
            return false;
        }

        reminder.Status = ReminderStatus.Failed;
        reminder.LastError = error.Length > 1000 ? error.Substring(0, 1000) : error;
        reminder.RetryCount++;

        if (reminder.RetryCount < 3)
        {
            reminder.Status = ReminderStatus.Pending;
        }

        await _dbContext.SaveChangesAsync();

        _logger.LogWarning(
            "Marked reminder {ReminderId} as failed (attempt {RetryCount}): {Error}",
            id, reminder.RetryCount, error);

        return true;
    }

    public async Task<bool> CancelAsync(Guid id)
    {
        var reminder = await _dbContext.Reminders.FindAsync(id);

        if (reminder == null || !reminder.IsActive)
        {
            return false;
        }

        if (reminder.Status != ReminderStatus.Pending)
        {
            _logger.LogWarning("Attempted to cancel non-pending reminder {ReminderId}", id);
            return false;
        }

        reminder.Status = ReminderStatus.Cancelled;
        reminder.IsActive = false;

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Cancelled reminder {ReminderId}", id);

        return true;
    }

    #endregion

    #region Private Methods

    private async Task<ReminderDto> MapToDtoAsync(Reminder reminder)
    {
        var dto = new ReminderDto
        {
            Id = reminder.Id,
            StudentId = reminder.StudentId,
            StudentName = reminder.Student?.FullName,
            Title = reminder.Title,
            Description = reminder.Description,
            DueDate = reminder.DueDate,
            NotifyAt = reminder.NotifyAt,
            Channel = reminder.Channel,
            Status = reminder.Status,
            SentAt = reminder.SentAt,
            LastError = reminder.LastError,
            RetryCount = reminder.RetryCount,
            IsActive = reminder.IsActive,
            CreatedAt = reminder.CreatedAt,
            CreatedBy = reminder.CreatedBy
        };

        if (!string.IsNullOrEmpty(reminder.CreatedBy))
        {
            var creator = await _userManager.FindByIdAsync(reminder.CreatedBy);
            dto.CreatedByName = creator?.FullName;
        }

        return dto;
    }

    #endregion
}
