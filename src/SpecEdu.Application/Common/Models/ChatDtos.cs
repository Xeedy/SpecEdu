namespace SpecEdu.Application.Common.Models;

public class ChatConversationDto
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public bool IsGroup { get; set; }
    public string? LastMessagePreview { get; set; }
    public DateTime? LastMessageAt { get; set; }
    public int UnreadCount { get; set; }
    public List<ChatParticipantDto> Participants { get; set; } = [];
}

public class ChatParticipantDto
{
    public string UserId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class ChatMessageDto
{
    public Guid Id { get; set; }
    public string SenderId { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsMine { get; set; }
    public List<ChatAttachmentDto> Attachments { get; set; } = [];
}

public class ChatAttachmentDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
}

public class ChatContactDto
{
    public string UserId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? SchoolName { get; set; }
}
