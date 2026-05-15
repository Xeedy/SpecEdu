using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Application.Common.Models;

namespace SpecEdu.Web.App.Pages.Chat;

[Authorize]
public class IndexModel(
    IChatService chatService,
    ICurrentUserService currentUserService) : PageModel
{
    public IReadOnlyList<ChatConversationDto> Conversations { get; set; } = [];
    public Guid? ActiveConversationId { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid? conversationId = null)
    {
        var userId = currentUserService.UserId;
        if (userId == null) return RedirectToPage("/Account/Login");

        Conversations = await chatService.GetConversationsAsync(userId);
        ActiveConversationId = conversationId;
        return Page();
    }

    // AJAX: Get messages for a conversation
    public async Task<IActionResult> OnGetMessagesAsync(Guid conversationId, int page = 1)
    {
        var userId = currentUserService.UserId;
        if (userId == null) return Unauthorized();

        var messages = await chatService.GetMessagesAsync(conversationId, userId, page);
        await chatService.MarkAsReadAsync(conversationId, userId);
        return new JsonResult(messages);
    }

    // AJAX: Get conversations list (for refresh)
    public async Task<IActionResult> OnGetConversationsAsync()
    {
        var userId = currentUserService.UserId;
        if (userId == null) return Unauthorized();

        var conversations = await chatService.GetConversationsAsync(userId);
        return new JsonResult(conversations);
    }

    // AJAX: Send a message
    public async Task<IActionResult> OnPostSendAsync([FromBody] SendMessageRequest request)
    {
        var userId = currentUserService.UserId;
        if (userId == null) return Unauthorized();
        if (string.IsNullOrWhiteSpace(request.Content) && request.HasFiles != true) return BadRequest();

        var msg = await chatService.SendMessageAsync(request.ConversationId, userId, request.Content ?? "");
        return new JsonResult(msg);
    }

    // AJAX: Upload files with message
    public async Task<IActionResult> OnPostSendWithFilesAsync()
    {
        var userId = currentUserService.UserId;
        if (userId == null) return Unauthorized();

        var convIdStr = Request.Form["conversationId"].ToString();
        if (!Guid.TryParse(convIdStr, out var conversationId)) return BadRequest();

        var content = Request.Form["content"].ToString();
        var files = Request.Form.Files;

        if (string.IsNullOrWhiteSpace(content) && files.Count == 0) return BadRequest();

        var fileList = new List<(string FileName, string ContentType, long Size, Stream Data)>();
        foreach (var file in files)
        {
            if (file.Length > 10 * 1024 * 1024) continue; // Skip files > 10MB
            fileList.Add((file.FileName, file.ContentType, file.Length, file.OpenReadStream()));
        }

        var msg = await chatService.SendMessageWithAttachmentsAsync(conversationId, userId, content ?? "", fileList);
        return new JsonResult(msg);
    }

    // AJAX: Create new conversation
    public async Task<IActionResult> OnPostNewConversationAsync([FromBody] NewConversationRequest request)
    {
        var userId = currentUserService.UserId;
        if (userId == null) return Unauthorized();
        if (request.ParticipantIds == null || request.ParticipantIds.Count == 0) return BadRequest();

        var convId = await chatService.CreateConversationAsync(userId, request.ParticipantIds, request.Title);
        return new JsonResult(new { conversationId = convId });
    }

    // AJAX: Get contacts for new conversation
    public async Task<IActionResult> OnGetContactsAsync(string? search = null)
    {
        var userId = currentUserService.UserId;
        if (userId == null) return Unauthorized();

        var contacts = await chatService.GetContactsAsync(userId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            contacts = contacts
                .Where(c => c.FullName.Contains(search, StringComparison.OrdinalIgnoreCase)
                    || c.Role.Contains(search, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return new JsonResult(contacts);
    }

    // File download
    public async Task<IActionResult> OnGetDownloadAsync(Guid attachmentId)
    {
        var userId = currentUserService.UserId;
        if (userId == null) return Unauthorized();

        var (metadata, stream) = await chatService.GetAttachmentAsync(attachmentId, userId);
        if (metadata == null || stream == null) return NotFound();

        return File(stream, metadata.ContentType, metadata.FileName);
    }

    // AJAX: Get unread count for badge
    public async Task<IActionResult> OnGetUnreadCountAsync()
    {
        var userId = currentUserService.UserId;
        if (userId == null) return Unauthorized();

        var count = await chatService.GetTotalUnreadCountAsync(userId);
        return new JsonResult(new { count });
    }

    public class SendMessageRequest
    {
        public Guid ConversationId { get; set; }
        public string? Content { get; set; }
        public bool? HasFiles { get; set; }
    }

    public class NewConversationRequest
    {
        public List<string> ParticipantIds { get; set; } = [];
        public string? Title { get; set; }
    }
}
