using SpecEdu.Domain.Common;

namespace SpecEdu.Domain.Entities;

/// <summary>
/// A participant in a conversation.
/// Tracks per-user read state for unread counts.
/// Czech: Účastník konverzace
/// </summary>
public class ConversationParticipant : BaseEntity
{
    public Guid ConversationId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Timestamp of the last message the user has seen.
    /// Messages with SentAt > LastReadAt are unread.
    /// </summary>
    public DateTime? LastReadAt { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation
    public Conversation? Conversation { get; set; }
}
