using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Application.Common.Models;
using SpecEdu.Domain.Enums;
using SpecEdu.Infrastructure.Authorization;
using System.ComponentModel.DataAnnotations;

namespace SpecEdu.Web.Pages.SchoolAdmin.Students;

[Authorize(Policy = AuthorizationPolicies.RequireSchoolAdmin)]
public class DiaryEditModel : PageModel
{
    private readonly IDiaryService _diaryService;
    private readonly IStudentService _studentService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuditService _auditService;

    private const int MaxFileSize = 5 * 1024 * 1024;
    private static readonly string[] AllowedExtensions = { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".jpg", ".jpeg", ".png", ".gif" };

    public DiaryEditModel(
        IDiaryService diaryService,
        IStudentService studentService,
        ICurrentUserService currentUserService,
        IAuditService auditService)
    {
        _diaryService = diaryService;
        _studentService = studentService;
        _currentUserService = currentUserService;
        _auditService = auditService;
    }

    public DiaryEntryDto Entry { get; set; } = null!;
    public StudentDto Student { get; set; } = null!;
    public IList<DiaryAttachmentDto> Attachments { get; set; } = new List<DiaryAttachmentDto>();

    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    [BindProperty]
    public List<IFormFile>? NewAttachments { get; set; }

    public List<SelectListItem> EntryTypeOptions { get; set; } = new();
    public List<SelectListItem> VisibilityOptions { get; set; } = new();

    public class InputModel
    {
        [Required(ErrorMessage = "Vyberte typ záznamu")]
        public DiaryEntryType Type { get; set; }

        [Required(ErrorMessage = "Zadejte předmět záznamu")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Zadejte obsah záznamu")]
        [StringLength(4000)]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vyberte viditelnost")]
        public DiaryVisibility Visibility { get; set; }

        public DateTime? OccurredAt { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var entry = await _diaryService.GetByIdAsync(Id, includeAttachments: true);
        if (entry == null)
        {
            return NotFound();
        }

        var userId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToPage("/Account/AccessDenied");
        }

        var canEdit = await _diaryService.CanEditAsync(Id, userId);
        if (!canEdit)
        {
            TempData["ErrorMessage"] = "Nemáte oprávnění upravit tento záznam.";
            return RedirectToPage("Diary", new { id = entry.StudentId });
        }

        Entry = entry;
        Attachments = entry.Attachments ?? new List<DiaryAttachmentDto>();

        var student = await _studentService.GetByIdAsync(entry.StudentId);
        if (student == null)
        {
            return NotFound();
        }
        Student = student;

        Input = new InputModel
        {
            Type = entry.Type,
            Title = entry.Title,
            Content = entry.Content,
            Visibility = entry.Visibility,
            OccurredAt = entry.OccurredAt
        };

        PopulateSelectLists();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var entry = await _diaryService.GetByIdAsync(Id, includeAttachments: true);
        if (entry == null)
        {
            return NotFound();
        }

        var userId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToPage("/Account/AccessDenied");
        }

        var canEdit = await _diaryService.CanEditAsync(Id, userId);
        if (!canEdit)
        {
            TempData["ErrorMessage"] = "Nemáte oprávnění upravit tento záznam.";
            return RedirectToPage("Diary", new { id = entry.StudentId });
        }

        Entry = entry;
        Attachments = entry.Attachments ?? new List<DiaryAttachmentDto>();

        var student = await _studentService.GetByIdAsync(entry.StudentId);
        if (student == null)
        {
            return NotFound();
        }
        Student = student;

        if (!ModelState.IsValid)
        {
            PopulateSelectLists();
            return Page();
        }

        if (NewAttachments != null && NewAttachments.Any())
        {
            foreach (var file in NewAttachments)
            {
                if (file.Length > MaxFileSize)
                {
                    ModelState.AddModelError("NewAttachments", $"Soubor {file.FileName} je příliš velký.");
                    PopulateSelectLists();
                    return Page();
                }

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!AllowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("NewAttachments", $"Soubor {file.FileName} má nepovolený typ.");
                    PopulateSelectLists();
                    return Page();
                }
            }
        }

        await _diaryService.UpdateAsync(
            Id,
            Input.Type,
            Input.Title,
            Input.Content,
            Input.Visibility,
            Input.OccurredAt);

        if (NewAttachments != null && NewAttachments.Any())
        {
            foreach (var file in NewAttachments)
            {
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                var fileData = memoryStream.ToArray();

                await _diaryService.AddAttachmentAsync(
                    Id,
                    file.FileName,
                    file.ContentType,
                    fileData);
            }
        }

        await _auditService.LogStudentActionAsync(
            "DiaryEntry.Update",
            entry.StudentId,
            $"Upraven záznam: {Input.Title}");

        TempData["SuccessMessage"] = "Záznam byl úspěšně upraven.";
        return RedirectToPage("Diary", new { id = entry.StudentId });
    }

    public async Task<IActionResult> OnPostDeleteAttachmentAsync(Guid attachmentId)
    {
        var entry = await _diaryService.GetByIdAsync(Id);
        if (entry == null)
        {
            return NotFound();
        }

        var userId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToPage("/Account/AccessDenied");
        }

        var canEdit = await _diaryService.CanEditAsync(Id, userId);
        if (!canEdit)
        {
            TempData["ErrorMessage"] = "Nemáte oprávnění upravit tento záznam.";
            return RedirectToPage("Diary", new { id = entry.StudentId });
        }

        await _diaryService.RemoveAttachmentAsync(attachmentId);

        TempData["SuccessMessage"] = "Příloha byla odstraněna.";
        return RedirectToPage(new { Id });
    }

    private void PopulateSelectLists()
    {
        EntryTypeOptions = new List<SelectListItem>
        {
            new SelectListItem("Poznámka", ((int)DiaryEntryType.Note).ToString()),
            new SelectListItem("Telefonát", ((int)DiaryEntryType.PhoneCall).ToString()),
            new SelectListItem("Schůzka", ((int)DiaryEntryType.Meeting).ToString()),
            new SelectListItem("Spolupráce s rodiči", ((int)DiaryEntryType.ParentCollaboration).ToString()),
            new SelectListItem("Spolupráce s PPP/SPC", ((int)DiaryEntryType.PppSpcCollaboration).ToString())
        };

        VisibilityOptions = new List<SelectListItem>
        {
            new SelectListItem("Pouze pro školu", ((int)DiaryVisibility.SchoolOnly).ToString()),
            new SelectListItem("Viditelné pro rodiče", ((int)DiaryVisibility.ParentVisible).ToString())
        };
    }
}
