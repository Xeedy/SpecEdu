using SpecEdu.Domain.Common;

namespace SpecEdu.Domain.Entities;

public class ChatAttachment : BaseEntity
{
    public Guid MessageId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }

    public string StoragePath { get; set; } = string.Empty;

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public ChatMessage? Message { get; set; }
}
