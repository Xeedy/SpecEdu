using SpecEdu.Domain.Common;

namespace SpecEdu.Domain.Entities;

public class ConversationParticipant : BaseEntity
{
    public Guid ConversationId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastReadAt { get; set; }

    public bool IsActive { get; set; } = true;

    public Conversation? Conversation { get; set; }
}
