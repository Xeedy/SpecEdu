using SpecEdu.Domain.Common;

namespace SpecEdu.Domain.Entities;

public class DiaryAttachment : AuditableEntity
{
    public Guid DiaryEntryId { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public byte[] FileData { get; set; } = Array.Empty<byte>();

    public long FileSize { get; set; }

    public DiaryEntry? DiaryEntry { get; set; }
}
