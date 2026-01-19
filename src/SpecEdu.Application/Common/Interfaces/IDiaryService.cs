using SpecEdu.Application.Common.Models;
using SpecEdu.Domain.Enums;

namespace SpecEdu.Application.Common.Interfaces;

public interface IDiaryService
{
    #region Diary Entry CRUD

    Task<DiaryEntryDto?> GetByIdAsync(Guid id, bool includeAttachments = false);

    Task<(IList<DiaryEntryDto> Entries, int TotalCount)> GetEntriesAsync(
        Guid studentId,
        int page,
        int pageSize,
        DiaryEntryType? type = null,
        string? authorId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        DiaryVisibility? visibilityFilter = null);

    Task<DiaryEntryDto> CreateAsync(
        Guid studentId,
        DiaryEntryType type,
        string title,
        string content,
        DiaryVisibility visibility,
        DateTime? occurredAt = null);

    Task<DiaryEntryDto?> UpdateAsync(
        Guid id,
        DiaryEntryType type,
        string title,
        string content,
        DiaryVisibility visibility,
        DateTime? occurredAt);

    Task<bool> DeleteAsync(Guid id);

    Task<bool> CanEditAsync(Guid entryId, string userId);

    #endregion

    #region Attachments

    Task<IList<DiaryAttachmentDto>> GetAttachmentsAsync(Guid entryId);

    Task<(byte[] FileData, string ContentType, string FileName)?> GetAttachmentDataAsync(Guid attachmentId);

    Task<DiaryAttachmentDto> AddAttachmentAsync(
        Guid entryId,
        string fileName,
        string contentType,
        byte[] fileData);

    Task<bool> RemoveAttachmentAsync(Guid attachmentId);

    #endregion

    #region Statistics

    Task<Dictionary<DiaryEntryType, int>> GetEntryCountsByTypeAsync(Guid studentId);

    #endregion
}
