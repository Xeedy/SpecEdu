using SpecEdu.Application.Common.Models;

namespace SpecEdu.Application.Common.Interfaces;

/// <summary>
/// Service for managing examination reminder notifications.
/// </summary>
public interface IReminderService
{
    #region CRUD Operations

    /// <summary>
    /// Creates a new reminder for a student.
    /// Automatically calculates NotifyAt as DueDate minus 2 months.
    /// </summary>
    /// <param name="studentId">The student's ID.</param>
    /// <param name="title">Reminder title (e.g., "Kontrolní vyšetření").</param>
    /// <param name="dueDate">The due date of the event.</param>
    /// <param name="description">Optional detailed description.</param>
    /// <returns>The created reminder DTO.</returns>
    Task<ReminderDto> CreateAsync(Guid studentId, string title, DateTime dueDate, string? description = null);

    /// <summary>
    /// Gets a reminder by ID.
    /// </summary>
    /// <param name="id">The reminder's ID.</param>
    /// <returns>Reminder DTO or null if not found.</returns>
    Task<ReminderDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// Gets all reminders for a student.
    /// </summary>
    /// <param name="studentId">The student's ID.</param>
    /// <param name="includeInactive">Whether to include cancelled/inactive reminders.</param>
    /// <returns>List of reminder DTOs.</returns>
    Task<IList<ReminderDto>> GetForStudentAsync(Guid studentId, bool includeInactive = false);

    /// <summary>
    /// Updates an existing reminder.
    /// </summary>
    /// <param name="id">The reminder's ID.</param>
    /// <param name="title">New title.</param>
    /// <param name="dueDate">New due date (NotifyAt will be recalculated).</param>
    /// <param name="description">New description.</param>
    /// <returns>Updated reminder DTO or null if not found.</returns>
    Task<ReminderDto?> UpdateAsync(Guid id, string title, DateTime dueDate, string? description);

    /// <summary>
    /// Permanently deletes a reminder.
    /// </summary>
    /// <param name="id">The reminder's ID.</param>
    /// <returns>True if deleted, false if not found.</returns>
    Task<bool> DeleteAsync(Guid id);

    #endregion

    #region Status Operations

    /// <summary>
    /// Gets pending reminders that are due to be sent.
    /// Used by the background service.
    /// </summary>
    /// <param name="maxRetryCount">Maximum retry attempts to include.</param>
    /// <returns>List of pending reminder DTOs.</returns>
    Task<IList<ReminderDto>> GetPendingRemindersAsync(int maxRetryCount = 3);

    /// <summary>
    /// Marks a reminder as successfully sent.
    /// </summary>
    /// <param name="id">The reminder's ID.</param>
    /// <returns>True if updated, false if not found.</returns>
    Task<bool> MarkAsSentAsync(Guid id);

    /// <summary>
    /// Marks a reminder as failed with an error message.
    /// Increments the retry count.
    /// </summary>
    /// <param name="id">The reminder's ID.</param>
    /// <param name="error">Error message describing the failure.</param>
    /// <returns>True if updated, false if not found.</returns>
    Task<bool> MarkAsFailedAsync(Guid id, string error);

    /// <summary>
    /// Cancels a pending reminder so it will not be sent.
    /// </summary>
    /// <param name="id">The reminder's ID.</param>
    /// <returns>True if cancelled, false if not found or already sent.</returns>
    Task<bool> CancelAsync(Guid id);

    #endregion
}
