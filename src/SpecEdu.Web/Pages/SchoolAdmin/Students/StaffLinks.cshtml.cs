using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Application.Common.Models;
using SpecEdu.Domain.Constants;
using SpecEdu.Domain.Enums;
using SpecEdu.Infrastructure.Authorization;
using SpecEdu.Infrastructure.Identity;

namespace SpecEdu.Web.Pages.SchoolAdmin.Students;

[Authorize(Policy = AuthorizationPolicies.RequireSchoolAdmin)]
public class StaffLinksModel : PageModel
{
    private readonly IStudentService _studentService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuditService _auditService;
    private readonly UserManager<ApplicationUser> _userManager;

    public StaffLinksModel(
        IStudentService studentService,
        ICurrentUserService currentUserService,
        IAuditService auditService,
        UserManager<ApplicationUser> userManager)
    {
        _studentService = studentService;
        _currentUserService = currentUserService;
        _auditService = auditService;
        _userManager = userManager;
    }

    public StudentDto Student { get; set; } = null!;
    public IList<StudentStaffLinkDto> StaffLinks { get; set; } = new List<StudentStaffLinkDto>();
    public IList<SelectListItem> AvailableStaff { get; set; } = new List<SelectListItem>();
    public IList<SelectListItem> LinkTypes { get; set; } = new List<SelectListItem>();
    public IList<SelectListItem> AccessLevels { get; set; } = new List<SelectListItem>();

    [BindProperty]
    public AddStaffLinkInput AddInput { get; set; } = new();

    [TempData]
    public string? SuccessMessage { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public class AddStaffLinkInput
    {
        [Required(ErrorMessage = "Vyberte zaměstnance")]
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vyberte typ vazby")]
        public StaffLinkType LinkType { get; set; }

        [Required(ErrorMessage = "Vyberte úroveň přístupu")]
        public AccessLevel AccessLevel { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var student = await _studentService.GetByIdAsync(id);
        if (student == null)
        {
            return NotFound();
        }

        if (_currentUserService.SchoolId != student.SchoolId)
        {
            return RedirectToPage("/Account/AccessDenied");
        }

        Student = student;
        StaffLinks = await _studentService.GetStaffLinksAsync(id);
        await PopulateSelectLists();

        return Page();
    }

    public async Task<IActionResult> OnPostAddAsync(Guid id)
    {
        var student = await _studentService.GetByIdAsync(id);
        if (student == null)
        {
            return NotFound();
        }

        if (_currentUserService.SchoolId != student.SchoolId)
        {
            return RedirectToPage("/Account/AccessDenied");
        }

        if (!ModelState.IsValid)
        {
            Student = student;
            StaffLinks = await _studentService.GetStaffLinksAsync(id);
            await PopulateSelectLists();
            return Page();
        }

        try
        {
            await _studentService.AddStaffLinkAsync(id, AddInput.UserId, AddInput.LinkType, AddInput.AccessLevel);
            await _auditService.LogStudentActionAsync("AddStaffLink", id, $"Added staff link {AddInput.UserId} as {AddInput.LinkType}");
            SuccessMessage = "Vazba zaměstnance byla úspěšně přidána.";
        }
        catch (Exception)
        {
            ErrorMessage = "Nepodařilo se přidat vazbu. Možná již existuje.";
        }

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostRemoveAsync(Guid id, Guid linkId)
    {
        var student = await _studentService.GetByIdAsync(id);
        if (student == null)
        {
            return NotFound();
        }

        if (_currentUserService.SchoolId != student.SchoolId)
        {
            return RedirectToPage("/Account/AccessDenied");
        }

        await _studentService.RemoveStaffLinkAsync(linkId);
        await _auditService.LogStudentActionAsync("RemoveStaffLink", id, $"Removed staff link {linkId}");
        SuccessMessage = "Vazba zaměstnance byla úspěšně odebrána.";

        return RedirectToPage(new { id });
    }

    private async Task PopulateSelectLists()
    {
        var schoolId = _currentUserService.SchoolId;

        var staffRoles = new[] { Roles.Teacher, Roles.SPP, Roles.PPP, Roles.SPC, Roles.Assistant };
        var allStaff = new List<ApplicationUser>();

        foreach (var role in staffRoles)
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync(role);
            allStaff.AddRange(usersInRole.Where(u => u.SchoolId == schoolId));
        }

        var existingLinkUserIds = StaffLinks.Select(l => l.UserId).ToHashSet();

        AvailableStaff = allStaff
            .DistinctBy(u => u.Id)
            .Where(u => !existingLinkUserIds.Contains(u.Id))
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .Select(u => new SelectListItem
            {
                Value = u.Id,
                Text = $"{u.FullName} ({u.Email})"
            })
            .ToList();

        LinkTypes = Enum.GetValues<StaffLinkType>()
            .Select(lt => new SelectListItem
            {
                Value = ((int)lt).ToString(),
                Text = lt switch
                {
                    StaffLinkType.Teacher => "Učitel",
                    StaffLinkType.Assistant => "Asistent",
                    StaffLinkType.SPP => "ŠPP",
                    StaffLinkType.PPP => "PPP",
                    StaffLinkType.SPC => "SPC",
                    _ => lt.ToString()
                }
            })
            .ToList();

        AccessLevels = Enum.GetValues<AccessLevel>()
            .Select(al => new SelectListItem
            {
                Value = ((int)al).ToString(),
                Text = al switch
                {
                    AccessLevel.Read => "Pouze čtení",
                    AccessLevel.Edit => "Čtení a zápis",
                    _ => al.ToString()
                }
            })
            .ToList();
    }
}
