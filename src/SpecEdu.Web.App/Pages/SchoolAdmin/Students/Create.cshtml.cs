using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Infrastructure.Authorization;

namespace SpecEdu.Web.App.Pages.SchoolAdmin.Students;

[Authorize(Policy = AuthorizationPolicies.RequireSchoolAdmin)]
public class CreateModel : PageModel
{
    private readonly IStudentService _studentService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuditService _auditService;

    public CreateModel(
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

    public class InputModel
    {
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
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var schoolId = _currentUserService.SchoolId;
        if (!schoolId.HasValue)
        {
            return RedirectToPage("/Account/AccessDenied");
        }

        var student = await _studentService.CreateAsync(
            schoolId.Value,
            Input.FirstName,
            Input.LastName,
            Input.BirthDate,
            Input.Class);

        await _auditService.LogStudentActionAsync("Create", student.Id, $"Created student {student.FullName}");

        TempData["SuccessMessage"] = $"Žák {student.FullName} byl úspěšně vytvořen.";

        return RedirectToPage("Index");
    }
}
