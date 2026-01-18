using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Application.Common.Models;
using SpecEdu.Domain.Constants;
using SpecEdu.Domain.Enums;
using SpecEdu.Infrastructure.Authorization;
using SpecEdu.Infrastructure.Identity;

namespace SpecEdu.Web.Pages.SchoolAdmin.Students;

[Authorize(Policy = AuthorizationPolicies.RequireSchoolAdmin)]
public class GuardiansModel : PageModel
{
    private readonly IStudentService _studentService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuditService _auditService;
    private readonly UserManager<ApplicationUser> _userManager;

    public GuardiansModel(
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
    public IList<StudentGuardianDto> Guardians { get; set; } = new List<StudentGuardianDto>();
    public IList<SelectListItem> AvailableParents { get; set; } = new List<SelectListItem>();
    public IList<SelectListItem> RelationshipTypes { get; set; } = new List<SelectListItem>();

    [BindProperty]
    public AddGuardianInput AddInput { get; set; } = new();

    [TempData]
    public string? SuccessMessage { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public class AddGuardianInput
    {
        [Required(ErrorMessage = "Vyberte rodiče")]
        public string ParentUserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vyberte typ vztahu")]
        public RelationshipType RelationshipType { get; set; }
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
        Guardians = await _studentService.GetGuardiansAsync(id);
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
            Guardians = await _studentService.GetGuardiansAsync(id);
            await PopulateSelectLists();
            return Page();
        }

        try
        {
            await _studentService.AddGuardianAsync(id, AddInput.ParentUserId, AddInput.RelationshipType);
            await _auditService.LogStudentActionAsync("AddGuardian", id, $"Added guardian {AddInput.ParentUserId}");
            SuccessMessage = "Zákonný zástupce byl úspěšně přidán.";
        }
        catch (Exception)
        {
            ErrorMessage = "Nepodařilo se přidat zákonného zástupce. Možná již existuje.";
        }

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostRemoveAsync(Guid id, Guid guardianId)
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

        await _studentService.RemoveGuardianAsync(guardianId);
        await _auditService.LogStudentActionAsync("RemoveGuardian", id, $"Removed guardian {guardianId}");
        SuccessMessage = "Zákonný zástupce byl úspěšně odebrán.";

        return RedirectToPage(new { id });
    }

    private async Task PopulateSelectLists()
    {
        // Get all parents in the system (or from the same school)
        var parentsInRole = await _userManager.GetUsersInRoleAsync(Roles.Parent);

        // Filter to same school if needed, or all for flexibility
        var schoolId = _currentUserService.SchoolId;
        var existingGuardianIds = Guardians.Select(g => g.ParentUserId).ToHashSet();

        AvailableParents = parentsInRole
            .Where(p => !existingGuardianIds.Contains(p.Id))
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .Select(p => new SelectListItem
            {
                Value = p.Id,
                Text = $"{p.FullName} ({p.Email})"
            })
            .ToList();

        RelationshipTypes = Enum.GetValues<RelationshipType>()
            .Select(rt => new SelectListItem
            {
                Value = ((int)rt).ToString(),
                Text = rt switch
                {
                    RelationshipType.Mother => "Matka",
                    RelationshipType.Father => "Otec",
                    RelationshipType.LegalGuardian => "Zákonný zástupce",
                    RelationshipType.Other => "Jiný",
                    _ => rt.ToString()
                }
            })
            .ToList();
    }
}
