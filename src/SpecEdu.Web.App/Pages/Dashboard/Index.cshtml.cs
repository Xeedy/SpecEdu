using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Application.Common.Models;
using SpecEdu.Domain.Enums;

namespace SpecEdu.Web.App.Pages.Dashboard;

[Authorize(Policy = "CanViewStudent")]
public class IndexModel(
    IStudentAccessService studentAccessService,
    ICurrentUserService currentUserService,
    IPlppService plppService,
    IReminderService reminderService,
    IConsultationService consultationService,
    IAuditService auditService) : PageModel
{
    public IList<StudentDto> Students { get; set; } = new List<StudentDto>();
    public int StudentCount => Students.Count;
    public int ActivePlppCount { get; set; }
    public int PendingReminderCount { get; set; }
    public int UpcomingEventsCount { get; set; }
    public IList<AuditLogDto> RecentActivity { get; set; } = new List<AuditLogDto>();

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = currentUserService.UserId;
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToPage("/Account/Login");
        }

        Students = await studentAccessService.GetAccessibleStudentsAsync(userId);

        // Get real stats based on role
        var schoolId = currentUserService.SchoolId;
        if (schoolId.HasValue)
        {
            var activePlpps = await plppService.GetActiveAsync(schoolId.Value);
            ActivePlppCount = activePlpps.Count;

            var now = DateTime.UtcNow;
            var events = await consultationService.GetEventsForSchoolAsync(schoolId.Value, now, now.AddMonths(1));
            UpcomingEventsCount = events.Count;
        }

        // Pending reminders across all accessible students
        var pendingReminders = 0;
        foreach (var student in Students)
        {
            var reminders = await reminderService.GetForStudentAsync(student.Id);
            pendingReminders += reminders.Count(r => r.Status == ReminderStatus.Pending);
        }
        PendingReminderCount = pendingReminders;

        // Recent audit activity
        if (schoolId.HasValue)
        {
            var (logs, _) = await auditService.GetSchoolLogsAsync(schoolId.Value, 1, 5);
            RecentActivity = logs;
        }
        else
        {
            var (logs, _) = await auditService.GetLogsAsync(1, 5);
            RecentActivity = logs;
        }

        return Page();
    }
}
