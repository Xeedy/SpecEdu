using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Application.Common.Models;
using SpecEdu.Domain.Enums;

namespace SpecEdu.Web.Pages.MyChildren;

[Authorize(Roles = "Parent")]
public class ChildDiaryModel : PageModel
{
    private readonly IStudentService _studentService;
    private readonly IStudentAccessService _studentAccessService;
    private readonly IDiaryService _diaryService;
    private readonly ICurrentUserService _currentUserService;

    public ChildDiaryModel(
        IStudentService studentService,
        IStudentAccessService studentAccessService,
        IDiaryService diaryService,
        ICurrentUserService currentUserService)
    {
        _studentService = studentService;
        _studentAccessService = studentAccessService;
        _diaryService = diaryService;
        _currentUserService = currentUserService;
    }

    public StudentDto Child { get; set; } = null!;
    public IList<DiaryEntryDto> Entries { get; set; } = new List<DiaryEntryDto>();

    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty(SupportsGet = true)]
    public DiaryEntryType? FilterType { get; set; }

    [BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; } = 1;

    public int TotalCount { get; set; }
    public int PageSize { get; set; } = 10;
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    public List<SelectListItem> EntryTypeOptions { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToPage("/Account/Login");
        }

        // Check if user has access to this student
        var accessResult = await _studentAccessService.CanAccessStudentAsync(userId, Id);
        if (!accessResult.CanAccess)
        {
            return RedirectToPage("/Account/AccessDenied");
        }

        var student = await _studentService.GetByIdAsync(Id);
        if (student == null)
        {
            return NotFound();
        }

        Child = student;

        // Get diary entries - only ParentVisible for parents
        var (entries, totalCount) = await _diaryService.GetEntriesAsync(
            Id,
            CurrentPage,
            PageSize,
            FilterType,
            authorId: null,
            fromDate: null,
            toDate: null,
            visibilityFilter: DiaryVisibility.ParentVisible);

        Entries = entries;
        TotalCount = totalCount;

        PopulateEntryTypeOptions();

        return Page();
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
}
