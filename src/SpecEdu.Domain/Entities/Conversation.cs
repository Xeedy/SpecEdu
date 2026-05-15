using SpecEdu.Domain.Common;

namespace SpecEdu.Domain.Entities;

/// <summary>
/// A chat conversation (1:1 or group).
/// Czech: Konverzace (přímá zpráva nebo skupinový chat)
/// </summary>
public class Conversation : AuditableEntity
{
    /// <summary>
    /// Display title (used for group chats; null for 1:1).
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Whether this is a group conversation.
    /// </summary>
    public bool IsGroup { get; set; }

    /// <summary>
    /// Denormalized: timestamp of the most recent message (for fast sorting).
    /// </summary>
    public DateTime? LastMessageAt { get; set; }

    /// <summary>
    /// Denormalized: preview text of the most recent message (for conversation list UI).
    /// </summary>
    public string? LastMessagePreview { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<ConversationParticipant> Participants { get; set; } = new List<ConversationParticipant>();
    public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
}
