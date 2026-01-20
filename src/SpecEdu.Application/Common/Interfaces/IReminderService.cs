using SpecEdu.Application.Common.Models;

namespace SpecEdu.Application.Common.Interfaces;

public interface IReminderService
{
    #region CRUD Operations

    Task<ReminderDto> CreateAsync(Guid studentId, string title, DateTime dueDate, string? description = null);

    Task<ReminderDto?> GetByIdAsync(Guid id);

    Task<IList<ReminderDto>> GetForStudentAsync(Guid studentId, bool includeInactive = false);

    Task<ReminderDto?> UpdateAsync(Guid id, string title, DateTime dueDate, string? description);

    Task<bool> DeleteAsync(Guid id);

    #endregion

    #region Status Operations

    Task<IList<ReminderDto>> GetPendingRemindersAsync(int maxRetryCount = 3);

    Task<bool> MarkAsSentAsync(Guid id);

    Task<bool> MarkAsFailedAsync(Guid id, string error);

    Task<bool> CancelAsync(Guid id);

    #endregion
}
