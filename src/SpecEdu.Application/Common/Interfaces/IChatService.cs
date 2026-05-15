using SpecEdu.Application.Common.Models;

namespace SpecEdu.Application.Common.Interfaces;

public interface IChatService
{
    /// <summary>
    /// Get all conversations for a user, ordered by most recent message.
    /// </summary>
    Task<IReadOnlyList<ChatConversationDto>> GetConversationsAsync(string userId);

    /// <summary>
    /// Get messages for a conversation (paginated, newest first).
    /// </summary>
    Task<IReadOnlyList<ChatMessageDto>> GetMessagesAsync(Guid conversationId, string userId, int page = 1, int pageSize = 50);

    /// <summary>
    /// Send a message in a conversation.
    /// </summary>
    Task<ChatMessageDto> SendMessageAsync(Guid conversationId, string senderId, string content);

    /// <summary>
    /// Create a new 1:1 or group conversation. Returns existing 1:1 if one exists.
    /// </summary>
    Task<Guid> CreateConversationAsync(string creatorId, IReadOnlyList<string> participantIds, string? title = null);

    /// <summary>
    /// Mark a conversation as read for a user.
    /// </summary>
    Task MarkAsReadAsync(Guid conversationId, string userId);

    /// <summary>
    /// Get total unread message count across all conversations.
    /// </summary>
    Task<int> GetTotalUnreadCountAsync(string userId);

    /// <summary>
    /// Get available contacts (all active users except self).
    /// </summary>
    Task<IReadOnlyList<ChatContactDto>> GetContactsAsync(string userId);

    /// <summary>
    /// Save an uploaded attachment and return its ID.
    /// </summary>
    Task<Guid> SaveAttachmentAsync(Guid messageId, string fileName, string contentType, long sizeBytes, Stream fileStream);

    /// <summary>
    /// Get attachment metadata and file stream for download.
    /// </summary>
    Task<(ChatAttachmentDto? Metadata, Stream? FileStream)> GetAttachmentAsync(Guid attachmentId, string userId);

    /// <summary>
    /// Send a message with attachments in one step.
    /// </summary>
    Task<ChatMessageDto> SendMessageWithAttachmentsAsync(Guid conversationId, string senderId, string content, IList<(string FileName, string ContentType, long Size, Stream Data)> files);
}
