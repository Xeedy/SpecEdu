using SpecEdu.Domain.Enums;

namespace SpecEdu.Application.Common.Models;

public class DiaryEntryDto
{
    public Guid Id { get; set; }

    public Guid StudentId { get; set; }

    public string? StudentName { get; set; }

    public DiaryEntryType Type { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public DiaryVisibility Visibility { get; set; }

    public DateTime? OccurredAt { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? CreatedByName { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int AttachmentCount { get; set; }

    public IList<DiaryAttachmentDto>? Attachments { get; set; }
}
