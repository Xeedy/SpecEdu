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
public class DiaryCreateModel : PageModel
{
    private readonly IDiaryService _diaryService;
    private readonly IStudentService _studentService;
    private readonly IStudentAccessService _studentAccessService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuditService _auditService;

    private const int MaxFileSize = 5 * 1024 * 1024;
    private static readonly string[] AllowedExtensions = { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".jpg", ".jpeg", ".png", ".gif" };

    public DiaryCreateModel(
        IDiaryService diaryService,
        IStudentService studentService,
        IStudentAccessService studentAccessService,
        ICurrentUserService currentUserService,
        IAuditService auditService)
    {
        _diaryService = diaryService;
        _studentService = studentService;
        _studentAccessService = studentAccessService;
        _currentUserService = currentUserService;
        _auditService = auditService;
    }

    public StudentDto Student { get; set; } = null!;

    [BindProperty(SupportsGet = true)]
    public Guid StudentId { get; set; }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    [BindProperty]
    public List<IFormFile>? Attachments { get; set; }

    public List<SelectListItem> EntryTypeOptions { get; set; } = new();
    public List<SelectListItem> VisibilityOptions { get; set; } = new();

    public class InputModel
    {
        [Required(ErrorMessage = "Vyberte typ záznamu")]
        [Display(Name = "Typ záznamu")]
        public DiaryEntryType Type { get; set; } = DiaryEntryType.Note;

        [Required(ErrorMessage = "Zadejte předmět záznamu")]
        [StringLength(200, ErrorMessage = "Předmět může mít maximálně 200 znaků")]
        [Display(Name = "Předmět")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Zadejte obsah záznamu")]
        [StringLength(4000, ErrorMessage = "Obsah může mít maximálně 4000 znaků")]
        [Display(Name = "Obsah")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vyberte viditelnost")]
        [Display(Name = "Viditelnost")]
        public DiaryVisibility Visibility { get; set; } = DiaryVisibility.SchoolOnly;

        [Display(Name = "Datum události")]
        public DateTime? OccurredAt { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var student = await _studentService.GetByIdAsync(StudentId);
        if (student == null)
        {
            return NotFound();
        }

        var userId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToPage("/Account/AccessDenied");
        }

        var accessResult = await _studentAccessService.CanAccessStudentAsync(userId, StudentId);
        if (!accessResult.CanAccess)
        {
            return RedirectToPage("/Account/AccessDenied");
        }

        Student = student;
        PopulateSelectLists();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var student = await _studentService.GetByIdAsync(StudentId);
        if (student == null)
        {
            return NotFound();
        }

        var userId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToPage("/Account/AccessDenied");
        }

        var accessResult = await _studentAccessService.CanAccessStudentAsync(userId, StudentId);
        if (!accessResult.CanAccess)
        {
            return RedirectToPage("/Account/AccessDenied");
        }

        Student = student;

        if (!ModelState.IsValid)
        {
            PopulateSelectLists();
            return Page();
        }

        if (Attachments != null && Attachments.Any())
        {
            foreach (var file in Attachments)
            {
                if (file.Length > MaxFileSize)
                {
                    ModelState.AddModelError("Attachments", $"Soubor {file.FileName} je příliš velký. Maximální velikost je 5 MB.");
                    PopulateSelectLists();
                    return Page();
                }

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!AllowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("Attachments", $"Soubor {file.FileName} má nepovolený typ. Povolené typy: {string.Join(", ", AllowedExtensions)}");
                    PopulateSelectLists();
                    return Page();
                }
            }
        }

        var entry = await _diaryService.CreateAsync(
            StudentId,
            Input.Type,
            Input.Title,
            Input.Content,
            Input.Visibility,
            Input.OccurredAt);

        if (Attachments != null && Attachments.Any())
        {
            foreach (var file in Attachments)
            {
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                var fileData = memoryStream.ToArray();

                await _diaryService.AddAttachmentAsync(
                    entry.Id,
                    file.FileName,
                    file.ContentType,
                    fileData);
            }
        }

        await _auditService.LogStudentActionAsync(
            "DiaryEntry.Create",
            StudentId,
            $"Vytvořen záznam: {Input.Title}");

        TempData["SuccessMessage"] = "Záznam byl úspěšně vytvořen.";
        return RedirectToPage("Diary", new { id = StudentId });
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
