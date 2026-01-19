using SpecEdu.Domain.Common;
using SpecEdu.Domain.Enums;

namespace SpecEdu.Domain.Entities;

/// <summary>
/// Represents a diary/communication log entry for a student.
/// Czech: Záznam v komunikačním deníku žáka
/// </summary>
public class DiaryEntry : AuditableEntity
{
    /// <summary>
    /// Foreign key to the student this entry belongs to.
    /// Czech: Žák, kterému záznam patří
    /// </summary>
    public Guid StudentId { get; set; }

    /// <summary>
    /// Type of the diary entry.
    /// Czech: Typ záznamu
    /// </summary>
    public DiaryEntryType Type { get; set; }

    /// <summary>
    /// Title/subject of the entry.
    /// Czech: Předmět záznamu
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Content/description of the entry.
    /// Czech: Obsah záznamu
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Visibility level of the entry.
    /// Czech: Viditelnost záznamu
    /// </summary>
    public DiaryVisibility Visibility { get; set; } = DiaryVisibility.SchoolOnly;

    /// <summary>
    /// Optional date/time when the event occurred (e.g., meeting date).
    /// Czech: Datum události
    /// </summary>
    public DateTime? OccurredAt { get; set; }

    /// <summary>
    /// Indicates whether the entry is active (soft delete).
    /// Czech: Aktivní stav záznamu
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Navigation property to the student.
    /// </summary>
    public Student? Student { get; set; }

    /// <summary>
    /// Navigation property to attachments.
    /// </summary>
    public ICollection<DiaryAttachment> Attachments { get; set; } = new List<DiaryAttachment>();
}
