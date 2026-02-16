using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Application.Common.Models;
using SpecEdu.Domain.Enums;
using SpecEdu.Infrastructure.Authorization;

namespace SpecEdu.Web.App.Pages.SchoolAdmin.Students;

[Authorize(Policy = AuthorizationPolicies.RequireSchoolAdmin)]
public class DiaryModel : PageModel
{
    private readonly IDiaryService _diaryService;
    private readonly IStudentService _studentService;
    private readonly IStudentAccessService _studentAccessService;
    private readonly ICurrentUserService _currentUserService;

    public DiaryModel(
        IDiaryService diaryService,
        IStudentService studentService,
        IStudentAccessService studentAccessService,
        ICurrentUserService currentUserService)
    {
        _diaryService = diaryService;
        _studentService = studentService;
        _studentAccessService = studentAccessService;
        _currentUserService = currentUserService;
    }

    public StudentDto Student { get; set; } = null!;
    public IList<DiaryEntryDto> Entries { get; set; } = new List<DiaryEntryDto>();
    public Dictionary<DiaryEntryType, int> EntryCounts { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty(SupportsGet = true)]
    public DiaryEntryType? FilterType { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? FilterFromDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? FilterToDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; } = 1;

    public int TotalCount { get; set; }
    public int PageSize { get; set; } = 10;
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    public List<SelectListItem> EntryTypeOptions { get; set; } = new();

    [TempData]
    public string? SuccessMessage { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var student = await _studentService.GetByIdAsync(Id);
        if (student == null)
        {
            return NotFound();
        }

        var userId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToPage("/Account/AccessDenied");
        }

        var accessResult = await _studentAccessService.CanAccessStudentAsync(userId, Id);
        if (!accessResult.CanAccess)
        {
            return RedirectToPage("/Account/AccessDenied");
        }

        Student = student;

        var (entries, totalCount) = await _diaryService.GetEntriesAsync(
            Id,
            CurrentPage,
            PageSize,
            FilterType,
            authorId: null,
            FilterFromDate,
            FilterToDate,
            visibilityFilter: null);

        Entries = entries;
        TotalCount = totalCount;

        EntryCounts = await _diaryService.GetEntryCountsByTypeAsync(Id);

        PopulateEntryTypeOptions();

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid entryId)
    {
        var userId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToPage("/Account/AccessDenied");
        }

        var canEdit = await _diaryService.CanEditAsync(entryId, userId);
        if (!canEdit)
        {
            ErrorMessage = "Nemáte oprávnění smazat tento záznam.";
            return RedirectToPage(new { Id });
        }

        var success = await _diaryService.DeleteAsync(entryId);
        if (success)
        {
            SuccessMessage = "Záznam byl úspěšně smazán.";
        }
        else
        {
            ErrorMessage = "Nepodařilo se smazat záznam.";
        }

        return RedirectToPage(new { Id });
    }

    private void PopulateEntryTypeOptions()
    {
        EntryTypeOptions = new List<SelectListItem>
        {
            new SelectListItem("Všechny typy", ""),
            new SelectListItem("Poznámka", ((int)DiaryEntryType.Note).ToString()),
            new SelectListItem("Telefonát", ((int)DiaryEntryType.PhoneCall).ToString()),
            new SelectListItem("Schůzka", ((int)DiaryEntryType.Meeting).ToString()),
            new SelectListItem("Spolupráce s rodiči", ((int)DiaryEntryType.ParentCollaboration).ToString()),
            new SelectListItem("Spolupráce s PPP/SPC", ((int)DiaryEntryType.PppSpcCollaboration).ToString())
        };
    }

    public string GetEntryTypeName(DiaryEntryType type)
    {
        return type switch
        {
            DiaryEntryType.Note => "Poznámka",
            DiaryEntryType.PhoneCall => "Telefonát",
            DiaryEntryType.Meeting => "Schůzka",
            DiaryEntryType.ParentCollaboration => "Spolupráce s rodiči",
            DiaryEntryType.PppSpcCollaboration => "Spolupráce s PPP/SPC",
            _ => type.ToString()
        };
    }

    public string GetEntryTypeIcon(DiaryEntryType type)
    {
        return type switch
        {
            DiaryEntryType.Note => "bi-journal-text",
            DiaryEntryType.PhoneCall => "bi-telephone",
            DiaryEntryType.Meeting => "bi-calendar-event",
            DiaryEntryType.ParentCollaboration => "bi-people",
            DiaryEntryType.PppSpcCollaboration => "bi-building",
            _ => "bi-circle"
        };
    }

    public string GetEntryTypeColor(DiaryEntryType type)
    {
        return type switch
        {
            DiaryEntryType.Note => "primary",
            DiaryEntryType.PhoneCall => "info",
            DiaryEntryType.Meeting => "success",
            DiaryEntryType.ParentCollaboration => "warning",
            DiaryEntryType.PppSpcCollaboration => "secondary",
            _ => "dark"
        };
    }

    public string GetVisibilityBadge(DiaryVisibility visibility)
    {
        return visibility switch
        {
            DiaryVisibility.SchoolOnly => "<span class=\"badge bg-secondary\"><i class=\"bi bi-lock me-1\"></i>Pouze škola</span>",
            DiaryVisibility.ParentVisible => "<span class=\"badge bg-success\"><i class=\"bi bi-eye me-1\"></i>Viditelné pro rodiče</span>",
            _ => ""
        };
    }
}
