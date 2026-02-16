using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Application.Common.Models;
using SpecEdu.Domain.Enums;
using SpecEdu.Infrastructure.Authorization;

namespace SpecEdu.Web.App.Pages.SchoolAdmin.Students;

[Authorize(Policy = AuthorizationPolicies.RequireSchoolAdmin)]
public class RemindersModel : PageModel
{
    private readonly IStudentService _studentService;
    private readonly IReminderService _reminderService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuditService _auditService;

    public RemindersModel(
        IStudentService studentService,
        IReminderService reminderService,
        ICurrentUserService currentUserService,
        IAuditService auditService)
    {
        _studentService = studentService;
        _reminderService = reminderService;
        _currentUserService = currentUserService;
        _auditService = auditService;
    }

    public StudentDto Student { get; set; } = null!;
    public IList<ReminderDto> Reminders { get; set; } = new List<ReminderDto>();

    [BindProperty]
    public CreateReminderInput CreateInput { get; set; } = new();

    [TempData]
    public string? SuccessMessage { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public class CreateReminderInput
    {
        [Required(ErrorMessage = "Zadejte název připomínky")]
        [StringLength(200, ErrorMessage = "Název může mít maximálně 200 znaků")]
        public string Title { get; set; } = "Kontrolní vyšetření";

        [Required(ErrorMessage = "Zadejte datum")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; } = DateTime.Today.AddMonths(6);

        [StringLength(2000, ErrorMessage = "Popis může mít maximálně 2000 znaků")]
        public string? Description { get; set; }
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
        Reminders = await _reminderService.GetForStudentAsync(id, includeInactive: true);

        return Page();
    }

    public async Task<IActionResult> OnPostCreateAsync(Guid id)
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
            Reminders = await _reminderService.GetForStudentAsync(id, includeInactive: true);
            return Page();
        }

        try
        {
            await _reminderService.CreateAsync(
                id,
                CreateInput.Title,
                CreateInput.DueDate,
                CreateInput.Description);

            await _auditService.LogStudentActionAsync(
                "CreateReminder",
                id,
                $"Created reminder: {CreateInput.Title}, DueDate: {CreateInput.DueDate:d}");

            SuccessMessage = "Připomínka byla úspěšně vytvořena.";
        }
        catch (Exception)
        {
            ErrorMessage = "Nepodařilo se vytvořit připomínku.";
        }

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostCancelAsync(Guid id, Guid reminderId)
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

        var result = await _reminderService.CancelAsync(reminderId);
        if (result)
        {
            await _auditService.LogStudentActionAsync(
                "CancelReminder",
                id,
                $"Cancelled reminder: {reminderId}");
            SuccessMessage = "Připomínka byla zrušena.";
        }
        else
        {
            ErrorMessage = "Připomínku nelze zrušit.";
        }

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id, Guid reminderId)
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

        var result = await _reminderService.DeleteAsync(reminderId);
        if (result)
        {
            await _auditService.LogStudentActionAsync(
                "DeleteReminder",
                id,
                $"Deleted reminder: {reminderId}");
            SuccessMessage = "Připomínka byla odstraněna.";
        }
        else
        {
            ErrorMessage = "Připomínku se nepodařilo odstranit.";
        }

        return RedirectToPage(new { id });
    }

    public static string GetStatusBadgeClass(ReminderStatus status) => status switch
    {
        ReminderStatus.Pending => "bg-warning text-dark",
        ReminderStatus.Sent => "bg-success",
        ReminderStatus.Failed => "bg-danger",
        ReminderStatus.Cancelled => "bg-secondary",
        _ => "bg-secondary"
    };

    public static string GetStatusName(ReminderStatus status) => status switch
    {
        ReminderStatus.Pending => "Čeká",
        ReminderStatus.Sent => "Odesláno",
        ReminderStatus.Failed => "Chyba",
        ReminderStatus.Cancelled => "Zrušeno",
        _ => status.ToString()
    };
}
