using SpecEdu.Domain.Common;

namespace SpecEdu.Domain.Entities;

/// <summary>
/// A single chat message. Inherits from BaseEntity (not AuditableEntity)
/// to minimize storage overhead on the highest-volume table.
/// Czech: Zpráva v chatu
/// </summary>
public class ChatMessage : BaseEntity
{
    public Guid ConversationId { get; set; }
    public string SenderId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Optional parent message ID for threaded replies.
    /// </summary>
    public Guid? ParentMessageId { get; set; }

    // Navigation
    public Conversation? Conversation { get; set; }
    public ICollection<ChatAttachment> Attachments { get; set; } = new List<ChatAttachment>();
}
