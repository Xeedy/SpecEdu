using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Application.Common.Models;
using SpecEdu.Domain.Entities;
using SpecEdu.Infrastructure.Data;
using SpecEdu.Infrastructure.Identity;

namespace SpecEdu.Infrastructure.Services;

public class ChatService(
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager,
    IConfiguration configuration) : IChatService
{
    private string StorageRoot => configuration["ChatFiles:StoragePath"]
        ?? Path.Combine(AppContext.BaseDirectory, "App_Data", "ChatFiles");

    public async Task<IReadOnlyList<ChatConversationDto>> GetConversationsAsync(string userId)
    {
        var conversationIds = await context.ConversationParticipants
            .Where(cp => cp.UserId == userId && cp.IsActive)
            .Select(cp => cp.ConversationId)
            .ToListAsync();

        if (conversationIds.Count == 0) return [];

        var conversations = await context.Conversations
            .Where(c => conversationIds.Contains(c.Id) && c.IsActive)
            .OrderByDescending(c => c.LastMessageAt ?? c.CreatedAt)
            .Select(c => new
            {
                c.Id,
                c.Title,
                c.IsGroup,
                c.LastMessagePreview,
                c.LastMessageAt,
                Participants = c.Participants
                    .Where(p => p.IsActive)
                    .Select(p => new { p.UserId, p.LastReadAt })
                    .ToList()
            })
            .ToListAsync();

        var allUserIds = conversations.SelectMany(c => c.Participants.Select(p => p.UserId)).Distinct().ToList();
        var users = await userManager.Users
            .Where(u => allUserIds.Contains(u.Id))
            .Select(u => new { u.Id, u.FirstName, u.LastName })
            .ToDictionaryAsync(u => u.Id);

        var userRoles = await context.UserRoles
            .Where(ur => allUserIds.Contains(ur.UserId))
            .Join(context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { ur.UserId, r.Name })
            .ToListAsync();
        var roleMap = userRoles.GroupBy(x => x.UserId)
            .ToDictionary(g => g.Key, g => g.First().Name ?? "");

        var result = new List<ChatConversationDto>();
        foreach (var conv in conversations)
        {
            var myParticipant = conv.Participants.FirstOrDefault(p => p.UserId == userId);
            var lastRead = myParticipant?.LastReadAt;

            var unreadCount = 0;
            if (lastRead.HasValue)
            {
                unreadCount = await context.ChatMessages
                    .CountAsync(m => m.ConversationId == conv.Id
                        && m.SentAt > lastRead.Value
                        && m.SenderId != userId
                        && !m.IsDeleted);
            }
            else if (conv.LastMessageAt.HasValue)
            {
                unreadCount = await context.ChatMessages
                    .CountAsync(m => m.ConversationId == conv.Id
                        && m.SenderId != userId
                        && !m.IsDeleted);
            }

            result.Add(new ChatConversationDto
            {
                Id = conv.Id,
                Title = conv.Title,
                IsGroup = conv.IsGroup,
                LastMessagePreview = conv.LastMessagePreview,
                LastMessageAt = conv.LastMessageAt,
                UnreadCount = unreadCount,
                Participants = conv.Participants.Select(p =>
                {
                    users.TryGetValue(p.UserId, out var user);
                    roleMap.TryGetValue(p.UserId, out var role);
                    return new ChatParticipantDto
                    {
                        UserId = p.UserId,
                        FullName = user != null ? $"{user.FirstName} {user.LastName}".Trim() : "Unknown",
                        Role = role ?? ""
                    };
                }).ToList()
            });
        }

        return result;
    }

    public async Task<IReadOnlyList<ChatMessageDto>> GetMessagesAsync(
        Guid conversationId, string userId, int page = 1, int pageSize = 50)
    {
        var isParticipant = await context.ConversationParticipants
            .AnyAsync(cp => cp.ConversationId == conversationId && cp.UserId == userId && cp.IsActive);
        if (!isParticipant) return [];

        var messages = await context.ChatMessages
            .Where(m => m.ConversationId == conversationId && !m.IsDeleted)
            .OrderByDescending(m => m.SentAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(m => new
            {
                m.Id,
                m.SenderId,
                m.Content,
                m.SentAt,
                m.IsDeleted,
                Attachments = m.Attachments.Select(a => new ChatAttachmentDto
                {
                    Id = a.Id,
                    FileName = a.FileName,
                    ContentType = a.ContentType,
                    FileSizeBytes = a.FileSizeBytes
                }).ToList()
            })
            .ToListAsync();

        var senderIds = messages.Select(m => m.SenderId).Distinct().ToList();
        var senders = await userManager.Users
            .Where(u => senderIds.Contains(u.Id))
            .Select(u => new { u.Id, u.FirstName, u.LastName })
            .ToDictionaryAsync(u => u.Id);

        return messages.Select(m =>
        {
            senders.TryGetValue(m.SenderId, out var sender);
            return new ChatMessageDto
            {
                Id = m.Id,
                SenderId = m.SenderId,
                SenderName = sender != null ? $"{sender.FirstName} {sender.LastName}".Trim() : "Unknown",
                Content = m.Content,
                SentAt = m.SentAt,
                IsDeleted = m.IsDeleted,
                IsMine = m.SenderId == userId,
                Attachments = m.Attachments
            };
        }).Reverse().ToList();
    }

    public async Task<ChatMessageDto> SendMessageAsync(Guid conversationId, string senderId, string content)
    {
        var message = new ChatMessage
        {
            ConversationId = conversationId,
            SenderId = senderId,
            Content = content,
            SentAt = DateTime.UtcNow
        };

        context.ChatMessages.Add(message);

        var conversation = await context.Conversations.FindAsync(conversationId);
        if (conversation != null)
        {
            conversation.LastMessageAt = message.SentAt;
            conversation.LastMessagePreview = content.Length > 200 ? content[..197] + "..." : content;
        }

        await context.SaveChangesAsync();

        var sender = await userManager.FindByIdAsync(senderId);
        return new ChatMessageDto
        {
            Id = message.Id,
            SenderId = senderId,
            SenderName = sender != null ? $"{sender.FirstName} {sender.LastName}".Trim() : "Unknown",
            Content = content,
            SentAt = message.SentAt,
            IsMine = true,
            Attachments = []
        };
    }

    public async Task<Guid> CreateConversationAsync(
        string creatorId, IReadOnlyList<string> participantIds, string? title = null)
    {
        var allParticipantIds = participantIds.Append(creatorId).Distinct().ToList();
        var isGroup = allParticipantIds.Count > 2;

        if (!isGroup && allParticipantIds.Count == 2)
        {
            var existingConvId = await FindExisting1to1Async(allParticipantIds[0], allParticipantIds[1]);
            if (existingConvId.HasValue) return existingConvId.Value;
        }

        var conversation = new Conversation
        {
            Title = isGroup ? title : null,
            IsGroup = isGroup,
            IsActive = true
        };

        context.Conversations.Add(conversation);

        foreach (var uid in allParticipantIds)
        {
            context.ConversationParticipants.Add(new ConversationParticipant
            {
                ConversationId = conversation.Id,
                UserId = uid,
                JoinedAt = DateTime.UtcNow,
                IsActive = true
            });
        }

        await context.SaveChangesAsync();
        return conversation.Id;
    }

    public async Task MarkAsReadAsync(Guid conversationId, string userId)
    {
        var participant = await context.ConversationParticipants
            .FirstOrDefaultAsync(cp => cp.ConversationId == conversationId && cp.UserId == userId && cp.IsActive);

        if (participant != null)
        {
            participant.LastReadAt = DateTime.UtcNow;
            await context.SaveChangesAsync();
        }
    }

    public async Task<int> GetTotalUnreadCountAsync(string userId)
    {
        var participations = await context.ConversationParticipants
            .Where(cp => cp.UserId == userId && cp.IsActive)
            .Select(cp => new { cp.ConversationId, cp.LastReadAt })
            .ToListAsync();

        if (participations.Count == 0) return 0;

        var total = 0;
        foreach (var p in participations)
        {
            var query = context.ChatMessages
                .Where(m => m.ConversationId == p.ConversationId && m.SenderId != userId && !m.IsDeleted);

            if (p.LastReadAt.HasValue)
                query = query.Where(m => m.SentAt > p.LastReadAt.Value);

            total += await query.CountAsync();
        }

        return total;
    }

    public async Task<IReadOnlyList<ChatContactDto>> GetContactsAsync(string userId)
    {
        var contacts = await userManager.Users
            .Where(u => u.Id != userId && u.IsActive)
            .OrderBy(u => u.LastName).ThenBy(u => u.FirstName)
            .Select(u => new { u.Id, u.FirstName, u.LastName, SchoolName = u.School != null ? u.School.Name : null })
            .ToListAsync();

        var allIds = contacts.Select(c => c.Id).ToList();
        var userRoles = await context.UserRoles
            .Where(ur => allIds.Contains(ur.UserId))
            .Join(context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { ur.UserId, r.Name })
            .ToListAsync();
        var roleMap = userRoles.GroupBy(x => x.UserId)
            .ToDictionary(g => g.Key, g => g.First().Name ?? "");

        return contacts.Select(c =>
        {
            roleMap.TryGetValue(c.Id, out var role);
            return new ChatContactDto
            {
                UserId = c.Id,
                FullName = $"{c.FirstName} {c.LastName}".Trim(),
                Role = role ?? "",
                SchoolName = c.SchoolName
            };
        }).ToList();
    }

    public async Task<Guid> SaveAttachmentAsync(
        Guid messageId, string fileName, string contentType, long sizeBytes, Stream fileStream)
    {
        var attachment = new ChatAttachment
        {
            MessageId = messageId,
            FileName = fileName,
            ContentType = contentType,
            FileSizeBytes = sizeBytes,
            UploadedAt = DateTime.UtcNow
        };

        var dir = Path.Combine(StorageRoot, messageId.ToString());
        Directory.CreateDirectory(dir);
        var safeFileName = $"{attachment.Id}_{Path.GetFileName(fileName)}";
        var filePath = Path.Combine(dir, safeFileName);
        attachment.StoragePath = Path.Combine(messageId.ToString(), safeFileName);

        await using (var fs = new FileStream(filePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(fs);
        }

        context.ChatAttachments.Add(attachment);
        await context.SaveChangesAsync();

        return attachment.Id;
    }

    public async Task<(ChatAttachmentDto? Metadata, Stream? FileStream)> GetAttachmentAsync(
        Guid attachmentId, string userId)
    {
        var attachment = await context.ChatAttachments
            .Include(a => a.Message)
            .FirstOrDefaultAsync(a => a.Id == attachmentId);

        if (attachment?.Message == null) return (null, null);

        var isParticipant = await context.ConversationParticipants
            .AnyAsync(cp => cp.ConversationId == attachment.Message.ConversationId
                && cp.UserId == userId && cp.IsActive);
        if (!isParticipant) return (null, null);

        var fullPath = Path.Combine(StorageRoot, attachment.StoragePath);
        if (!File.Exists(fullPath)) return (null, null);

        var dto = new ChatAttachmentDto
        {
            Id = attachment.Id,
            FileName = attachment.FileName,
            ContentType = attachment.ContentType,
            FileSizeBytes = attachment.FileSizeBytes
        };

        return (dto, new FileStream(fullPath, FileMode.Open, FileAccess.Read));
    }

    public async Task<ChatMessageDto> SendMessageWithAttachmentsAsync(
        Guid conversationId, string senderId, string content,
        IList<(string FileName, string ContentType, long Size, Stream Data)> files)
    {
        var message = new ChatMessage
        {
            ConversationId = conversationId,
            SenderId = senderId,
            Content = content,
            SentAt = DateTime.UtcNow
        };
        context.ChatMessages.Add(message);

        var conversation = await context.Conversations.FindAsync(conversationId);
        if (conversation != null)
        {
            var preview = files.Count > 0 && string.IsNullOrWhiteSpace(content)
                ? $"[{files.Count} file(s)]"
                : content;
            conversation.LastMessageAt = message.SentAt;
            conversation.LastMessagePreview = preview.Length > 200 ? preview[..197] + "..." : preview;
        }

        await context.SaveChangesAsync();

        var attachmentDtos = new List<ChatAttachmentDto>();
        foreach (var file in files)
        {
            var attId = await SaveAttachmentAsync(message.Id, file.FileName, file.ContentType, file.Size, file.Data);
            attachmentDtos.Add(new ChatAttachmentDto
            {
                Id = attId,
                FileName = file.FileName,
                ContentType = file.ContentType,
                FileSizeBytes = file.Size
            });
        }

        var sender = await userManager.FindByIdAsync(senderId);
        return new ChatMessageDto
        {
            Id = message.Id,
            SenderId = senderId,
            SenderName = sender != null ? $"{sender.FirstName} {sender.LastName}".Trim() : "Unknown",
            Content = content,
            SentAt = message.SentAt,
            IsMine = true,
            Attachments = attachmentDtos
        };
    }

    private async Task<Guid?> FindExisting1to1Async(string userId1, string userId2)
    {
        var user1Convs = context.ConversationParticipants
            .Where(cp => cp.UserId == userId1 && cp.IsActive)
            .Select(cp => cp.ConversationId);

        var user2Convs = context.ConversationParticipants
            .Where(cp => cp.UserId == userId2 && cp.IsActive)
            .Select(cp => cp.ConversationId);

        var sharedConvIds = await user1Convs
            .Intersect(user2Convs)
            .ToListAsync();

        foreach (var convId in sharedConvIds)
        {
            var conv = await context.Conversations
                .Where(c => c.Id == convId && !c.IsGroup && c.IsActive)
                .FirstOrDefaultAsync();

            if (conv != null)
            {
                var count = await context.ConversationParticipants
                    .CountAsync(cp => cp.ConversationId == convId && cp.IsActive);
                if (count == 2) return convId;
            }
        }

        return null;
    }
}
