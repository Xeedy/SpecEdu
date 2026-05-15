using SpecEdu.Domain.Common;

namespace SpecEdu.Domain.Entities;

public class Conversation : AuditableEntity
{
    public string? Title { get; set; }

    public bool IsGroup { get; set; }

    public DateTime? LastMessageAt { get; set; }

    public string? LastMessagePreview { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<ConversationParticipant> Participants { get; set; } = new List<ConversationParticipant>();
    public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
}
