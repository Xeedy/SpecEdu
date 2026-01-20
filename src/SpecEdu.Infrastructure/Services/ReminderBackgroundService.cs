using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Infrastructure.Data;
using SpecEdu.Infrastructure.Identity;

namespace SpecEdu.Infrastructure.Services;

/// <summary>
/// Background service that processes pending reminders and sends email notifications.
/// Runs periodically (every hour) to check for due reminders.
/// </summary>
public class ReminderBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ReminderBackgroundService> _logger;

    /// <summary>
    /// Interval between processing cycles (1 hour).
    /// </summary>
    private static readonly TimeSpan ProcessingInterval = TimeSpan.FromHours(1);

    /// <summary>
    /// Maximum retry attempts for failed notifications.
    /// </summary>
    private const int MaxRetryCount = 3;

    public ReminderBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<ReminderBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ReminderBackgroundService started");

        // Initial delay to let the application start up
        await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPendingRemindersAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Graceful shutdown
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing pending reminders");
            }

            try
            {
                await Task.Delay(ProcessingInterval, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Graceful shutdown
                break;
            }
        }

        _logger.LogInformation("ReminderBackgroundService stopped");
    }

    private async Task ProcessPendingRemindersAsync(CancellationToken stoppingToken)
    {
        _logger.LogDebug("Processing pending reminders...");

        using var scope = _serviceProvider.CreateScope();

        var reminderService = scope.ServiceProvider.GetRequiredService<IReminderService>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var pendingReminders = await reminderService.GetPendingRemindersAsync(MaxRetryCount);

        if (!pendingReminders.Any())
        {
            _logger.LogDebug("No pending reminders to process");
            return;
        }

        _logger.LogInformation("Found {Count} pending reminder(s) to process", pendingReminders.Count);

        foreach (var reminder in pendingReminders)
        {
            if (stoppingToken.IsCancellationRequested)
            {
                break;
            }

            try
            {
                await ProcessReminderAsync(
                    reminder.Id,
                    reminder.StudentId,
                    reminder.StudentName ?? "Student",
                    reminder.Title,
                    reminder.Description,
                    reminder.DueDate,
                    dbContext,
                    userManager,
                    emailService,
                    reminderService);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process reminder {ReminderId}", reminder.Id);
                await reminderService.MarkAsFailedAsync(reminder.Id, ex.Message);
            }
        }
    }

    private async Task ProcessReminderAsync(
        Guid reminderId,
        Guid studentId,
        string studentName,
        string title,
        string? description,
        DateTime dueDate,
        ApplicationDbContext dbContext,
        UserManager<ApplicationUser> userManager,
        IEmailService emailService,
        IReminderService reminderService)
    {
        // Get guardian emails for this student
        var guardianUserIds = await dbContext.StudentGuardians
            .Where(sg => sg.StudentId == studentId && sg.IsActive)
            .Select(sg => sg.ParentUserId)
            .ToListAsync();

        if (!guardianUserIds.Any())
        {
            _logger.LogWarning("No active guardians found for student {StudentId}", studentId);
            await reminderService.MarkAsFailedAsync(reminderId, "No active guardians found");
            return;
        }

        var guardianEmails = new List<string>();
        foreach (var userId in guardianUserIds)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user?.Email != null)
            {
                guardianEmails.Add(user.Email);
            }
        }

        if (!guardianEmails.Any())
        {
            _logger.LogWarning("No guardian emails found for student {StudentId}", studentId);
            await reminderService.MarkAsFailedAsync(reminderId, "No guardian emails found");
            return;
        }

        // Build email content
        var subject = $"Připomínka: {title} - {studentName}";
        var htmlBody = BuildEmailHtml(studentName, title, description, dueDate);
        var textBody = BuildEmailText(studentName, title, description, dueDate);

        // Send email
        var success = await emailService.SendEmailAsync(guardianEmails, subject, htmlBody, textBody);

        if (success)
        {
            await reminderService.MarkAsSentAsync(reminderId);
            _logger.LogInformation(
                "Sent reminder {ReminderId} to {EmailCount} guardian(s) for student {StudentName}",
                reminderId, guardianEmails.Count, studentName);
        }
        else
        {
            await reminderService.MarkAsFailedAsync(reminderId, "Email sending failed");
        }
    }

    private static string BuildEmailHtml(string studentName, string title, string? description, DateTime dueDate)
    {
        var descriptionHtml = string.IsNullOrEmpty(description)
            ? ""
            : $"<p><strong>Podrobnosti:</strong> {System.Net.WebUtility.HtmlEncode(description)}</p>";

        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
        <h2 style='color: #2c3e50;'>Připomínka kontrolního vyšetření</h2>

        <p>Vážení rodiče,</p>

        <p>dovolujeme si Vám připomenout blížící se termín pro Vaše dítě:</p>

        <div style='background-color: #f8f9fa; padding: 15px; border-radius: 5px; margin: 20px 0;'>
            <p><strong>Žák:</strong> {System.Net.WebUtility.HtmlEncode(studentName)}</p>
            <p><strong>Typ:</strong> {System.Net.WebUtility.HtmlEncode(title)}</p>
            <p><strong>Termín:</strong> {dueDate:d. M. yyyy}</p>
            {descriptionHtml}
        </div>

        <p>Prosíme o zajištění vyšetření do uvedeného data.</p>

        <p>S pozdravem,<br>SpecEdu</p>

        <hr style='border: none; border-top: 1px solid #eee; margin: 30px 0;'>
        <p style='font-size: 12px; color: #666;'>
            Tato zpráva byla vygenerována automaticky systémem SpecEdu.
        </p>
    </div>
</body>
</html>";
    }

    private static string BuildEmailText(string studentName, string title, string? description, DateTime dueDate)
    {
        var descriptionText = string.IsNullOrEmpty(description)
            ? ""
            : $"\nPodrobnosti: {description}";

        return $@"Připomínka kontrolního vyšetření

Vážení rodiče,

dovolujeme si Vám připomenout blížící se termín pro Vaše dítě:

Žák: {studentName}
Typ: {title}
Termín: {dueDate:d. M. yyyy}{descriptionText}

Prosíme o zajištění vyšetření do uvedeného data.

S pozdravem,
SpecEdu

---
Tato zpráva byla vygenerována automaticky systémem SpecEdu.";
    }
}
