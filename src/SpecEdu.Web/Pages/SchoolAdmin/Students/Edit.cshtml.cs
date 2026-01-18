using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Application.Common.Models;
using SpecEdu.Infrastructure.Authorization;

namespace SpecEdu.Web.Pages.SchoolAdmin.Students;

[Authorize(Policy = AuthorizationPolicies.RequireSchoolAdmin)]
public class EditModel : PageModel
{
    private readonly IStudentService _studentService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuditService _auditService;

    public EditModel(
        IStudentService studentService,
        ICurrentUserService currentUserService,
        IAuditService auditService)
    {
        _studentService = studentService;
        _currentUserService = currentUserService;
        _auditService = auditService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public StudentDto? Student { get; set; }

    public class InputModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Jméno je povinné")]
        [StringLength(100, ErrorMessage = "Jméno může mít maximálně 100 znaků")]
        [Display(Name = "Jméno")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Příjmení je povinné")]
        [StringLength(100, ErrorMessage = "Příjmení může mít maximálně 100 znaků")]
        [Display(Name = "Příjmení")]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "Datum narození")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        [StringLength(50, ErrorMessage = "Třída může mít maximálně 50 znaků")]
        [Display(Name = "Třída")]
        public string? Class { get; set; }

        [Display(Name = "Aktivní")]
        public bool IsActive { get; set; }
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
        Input = new InputModel
        {
            Id = student.Id,
            FirstName = student.FirstName,
            LastName = student.LastName,
            BirthDate = student.BirthDate,
            Class = student.Class,
            IsActive = student.IsActive
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var existingStudent = await _studentService.GetByIdAsync(Input.Id);
        if (existingStudent == null)
        {
            return NotFound();
        }

        if (_currentUserService.SchoolId != existingStudent.SchoolId)
        {
            return RedirectToPage("/Account/AccessDenied");
        }

        var updatedStudent = await _studentService.UpdateAsync(
            Input.Id,
            Input.FirstName,
            Input.LastName,
            Input.BirthDate,
            Input.Class,
            Input.IsActive);

        if (updatedStudent != null)
        {
            await _auditService.LogStudentActionAsync("Update", updatedStudent.Id, $"Updated student {updatedStudent.FullName}");
            TempData["SuccessMessage"] = $"Žák {updatedStudent.FullName} byl úspěšně upraven.";
        }

        return RedirectToPage("Index");
    }
}
