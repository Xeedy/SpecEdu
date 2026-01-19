using SpecEdu.Application.Common.Models;
using SpecEdu.Domain.Enums;

namespace SpecEdu.Application.Common.Interfaces;

/// <summary>
/// Service for student diary/communication log operations.
/// Implements CRUD operations for diary entries and attachments.
/// </summary>
public interface IDiaryService
{
    #region Diary Entry CRUD

    /// <summary>
    /// Gets a diary entry by ID.
    /// </summary>
    /// <param name="id">The entry's ID.</param>
    /// <param name="includeAttachments">Whether to include attachments.</param>
    /// <returns>Diary entry DTO or null if not found.</returns>
    Task<DiaryEntryDto?> GetByIdAsync(Guid id, bool includeAttachments = false);

    /// <summary>
    /// Gets diary entries for a student with pagination and filtering.
    /// </summary>
    /// <param name="studentId">The student's ID.</param>
    /// <param name="page">Page number (1-based).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="type">Optional filter by entry type.</param>
    /// <param name="authorId">Optional filter by author user ID.</param>
    /// <param name="fromDate">Optional filter from date.</param>
    /// <param name="toDate">Optional filter to date.</param>
    /// <param name="visibilityFilter">Optional visibility filter (for parents).</param>
    /// <returns>Paginated list of entries and total count.</returns>
    Task<(IList<DiaryEntryDto> Entries, int TotalCount)> GetEntriesAsync(
        Guid studentId,
        int page,
        int pageSize,
        DiaryEntryType? type = null,
        string? authorId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        DiaryVisibility? visibilityFilter = null);

    /// <summary>
    /// Creates a new diary entry.
    /// </summary>
    /// <param name="studentId">Student ID.</param>
    /// <param name="type">Entry type.</param>
    /// <param name="title">Entry title.</param>
    /// <param name="content">Entry content.</param>
    /// <param name="visibility">Visibility level.</param>
    /// <param name="occurredAt">When the event occurred (optional).</param>
    /// <returns>Created entry DTO.</returns>
    Task<DiaryEntryDto> CreateAsync(
        Guid studentId,
        DiaryEntryType type,
        string title,
        string content,
        DiaryVisibility visibility,
        DateTime? occurredAt = null);

    /// <summary>
    /// Updates an existing diary entry.
    /// </summary>
    /// <param name="id">Entry ID.</param>
    /// <param name="type">Entry type.</param>
    /// <param name="title">Entry title.</param>
    /// <param name="content">Entry content.</param>
    /// <param name="visibility">Visibility level.</param>
    /// <param name="occurredAt">When the event occurred.</param>
    /// <returns>Updated entry DTO or null if not found.</returns>
    Task<DiaryEntryDto?> UpdateAsync(
        Guid id,
        DiaryEntryType type,
        string title,
        string content,
        DiaryVisibility visibility,
        DateTime? occurredAt);

    /// <summary>
    /// Soft-deletes a diary entry (sets IsActive to false).
    /// </summary>
    /// <param name="id">Entry ID.</param>
    /// <returns>True if successful.</returns>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// Checks if the current user can edit a diary entry.
    /// Only the author can edit their own entries.
    /// </summary>
    /// <param name="entryId">Entry ID.</param>
    /// <param name="userId">User ID.</param>
    /// <returns>True if user can edit.</returns>
    Task<bool> CanEditAsync(Guid entryId, string userId);

    #endregion

    #region Attachments

    /// <summary>
    /// Gets all attachments for a diary entry.
    /// </summary>
    /// <param name="entryId">Entry ID.</param>
    /// <returns>List of attachment DTOs (metadata only).</returns>
    Task<IList<DiaryAttachmentDto>> GetAttachmentsAsync(Guid entryId);

    /// <summary>
    /// Gets attachment file data for download.
    /// </summary>
    /// <param name="attachmentId">Attachment ID.</param>
    /// <returns>Tuple of (file data, content type, file name) or null if not found.</returns>
    Task<(byte[] FileData, string ContentType, string FileName)?> GetAttachmentDataAsync(Guid attachmentId);

    /// <summary>
    /// Adds an attachment to a diary entry.
    /// </summary>
    /// <param name="entryId">Entry ID.</param>
    /// <param name="fileName">Original file name.</param>
    /// <param name="contentType">MIME content type.</param>
    /// <param name="fileData">File content.</param>
    /// <returns>Created attachment DTO.</returns>
    Task<DiaryAttachmentDto> AddAttachmentAsync(
        Guid entryId,
        string fileName,
        string contentType,
        byte[] fileData);

    /// <summary>
    /// Removes an attachment.
    /// </summary>
    /// <param name="attachmentId">Attachment ID.</param>
    /// <returns>True if successful.</returns>
    Task<bool> RemoveAttachmentAsync(Guid attachmentId);

    #endregion

    #region Statistics

    /// <summary>
    /// Gets entry count by type for a student.
    /// </summary>
    /// <param name="studentId">Student ID.</param>
    /// <returns>Dictionary of type to count.</returns>
    Task<Dictionary<DiaryEntryType, int>> GetEntryCountsByTypeAsync(Guid studentId);

    #endregion
}
