using SpecEdu.Domain.Common;

namespace SpecEdu.Domain.Entities;

/// <summary>
/// A file attachment on a chat message.
/// Czech: Příloha zprávy v chatu
/// </summary>
public class ChatAttachment : BaseEntity
{
    public Guid MessageId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }

    /// <summary>
    /// Relative storage path on disk (or blob URI in production).
    /// </summary>
    public string StoragePath { get; set; } = string.Empty;

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ChatMessage? Message { get; set; }
}
