using SpecEdu.Application.Common.Models;

namespace SpecEdu.Application.Common.Interfaces;

public interface IChatService
{
    Task<IReadOnlyList<ChatConversationDto>> GetConversationsAsync(string userId);

    Task<IReadOnlyList<ChatMessageDto>> GetMessagesAsync(Guid conversationId, string userId, int page = 1, int pageSize = 50);

    Task<ChatMessageDto> SendMessageAsync(Guid conversationId, string senderId, string content);

    Task<Guid> CreateConversationAsync(string creatorId, IReadOnlyList<string> participantIds, string? title = null);

    Task MarkAsReadAsync(Guid conversationId, string userId);

    Task<int> GetTotalUnreadCountAsync(string userId);

    Task<IReadOnlyList<ChatContactDto>> GetContactsAsync(string userId);

    Task<Guid> SaveAttachmentAsync(Guid messageId, string fileName, string contentType, long sizeBytes, Stream fileStream);

    Task<(ChatAttachmentDto? Metadata, Stream? FileStream)> GetAttachmentAsync(Guid attachmentId, string userId);

    Task<ChatMessageDto> SendMessageWithAttachmentsAsync(Guid conversationId, string senderId, string content, IList<(string FileName, string ContentType, long Size, Stream Data)> files);
}
