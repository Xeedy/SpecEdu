using SpecEdu.Domain.Enums;

namespace SpecEdu.Application.Common.Models;

/// <summary>
/// Data transfer object for DiaryEntry entity.
/// Used to display diary entries in timeline and detail views.
/// </summary>
public class DiaryEntryDto
{
    /// <summary>
    /// Unique identifier of the diary entry.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// ID of the student this entry belongs to.
    /// </summary>
    public Guid StudentId { get; set; }

    /// <summary>
    /// Name of the student (populated when needed).
    /// </summary>
    public string? StudentName { get; set; }

    /// <summary>
    /// Type of the diary entry.
    /// </summary>
    public DiaryEntryType Type { get; set; }

    /// <summary>
    /// Title/subject of the entry.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Content/description of the entry.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Visibility level of the entry.
    /// </summary>
    public DiaryVisibility Visibility { get; set; }

    /// <summary>
    /// Date/time when the event occurred.
    /// </summary>
    public DateTime? OccurredAt { get; set; }

    /// <summary>
    /// Indicates whether the entry is active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// UTC timestamp when the entry was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// ID of the user who created the entry.
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Name of the user who created the entry (populated when needed).
    /// </summary>
    public string? CreatedByName { get; set; }

    /// <summary>
    /// UTC timestamp when the entry was last modified.
    /// </summary>
    public DateTime? ModifiedAt { get; set; }

    /// <summary>
    /// Number of attachments (populated when needed).
    /// </summary>
    public int AttachmentCount { get; set; }

    /// <summary>
    /// Attachments (populated when requested).
    /// </summary>
    public IList<DiaryAttachmentDto>? Attachments { get; set; }
}
