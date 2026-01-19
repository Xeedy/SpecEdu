using SpecEdu.Domain.Common;
using SpecEdu.Domain.Enums;

namespace SpecEdu.Domain.Entities;

public class DiaryEntry : AuditableEntity
{
    public Guid StudentId { get; set; }

    public DiaryEntryType Type { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public DiaryVisibility Visibility { get; set; } = DiaryVisibility.SchoolOnly;

    public DateTime? OccurredAt { get; set; }

    public bool IsActive { get; set; } = true;

    public Student? Student { get; set; }

    public ICollection<DiaryAttachment> Attachments { get; set; } = new List<DiaryAttachment>();
}
