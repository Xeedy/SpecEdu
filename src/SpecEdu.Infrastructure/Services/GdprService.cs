using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Domain.Entities;
using SpecEdu.Infrastructure.Data;
using SpecEdu.Infrastructure.Identity;

namespace SpecEdu.Infrastructure.Services;

public class GdprService(
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager) : IGdprService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task<string> ExportUserDataAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null) return "{}";

        var data = new
        {
            Profile = new
            {
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                user.PhoneNumber,
                user.EmailConfirmed,
                user.CreatedAt
            },
            Roles = await userManager.GetRolesAsync(user),
            GuardianLinks = await context.StudentGuardians
                .Where(sg => sg.ParentUserId == userId && sg.IsActive)
                .Select(sg => new { sg.StudentId, sg.RelationshipType, sg.CreatedAt })
                .ToListAsync(),
            StaffLinks = await context.StudentStaffLinks
                .Where(sl => sl.UserId == userId && sl.IsActive)
                .Select(sl => new { sl.StudentId, sl.LinkType, sl.AccessLevel, sl.CreatedAt })
                .ToListAsync(),
            AuditLogs = await context.AuditLogs
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Timestamp)
                .Take(100)
                .Select(a => new { a.Action, a.EntityType, a.Timestamp })
                .ToListAsync(),
            Consents = await context.UserConsents
                .Where(c => c.UserId == userId)
                .Select(c => new { c.ConsentType, c.IsGranted, c.GrantedAt })
                .ToListAsync()
        };

        return JsonSerializer.Serialize(data, JsonOptions);
    }

    public async Task<string> ExportStudentDataAsync(Guid studentId)
    {
        var student = await context.Students
            .Include(s => s.School)
            .FirstOrDefaultAsync(s => s.Id == studentId);

        if (student == null) return "{}";

        var data = new
        {
            Student = new
            {
                student.FirstName,
                student.LastName,
                student.BirthDate,
                student.Class,
                student.IsActive,
                student.CreatedAt,
                School = student.School?.Name
            },
            DiaryEntries = await context.DiaryEntries
                .Where(d => d.StudentId == studentId && d.IsActive)
                .Select(d => new { d.Title, d.Type, d.OccurredAt, d.CreatedAt })
                .ToListAsync(),
            Plpps = await context.Plpps
                .Where(p => p.StudentId == studentId && p.IsActive)
                .Select(p => new { p.SchoolYear, p.Status, p.SupportLevel, p.ValidFrom, p.ValidTo })
                .ToListAsync(),
            Reminders = await context.Reminders
                .Where(r => r.StudentId == studentId && r.IsActive)
                .Select(r => new { r.Title, r.DueDate, r.Status })
                .ToListAsync()
        };

        return JsonSerializer.Serialize(data, JsonOptions);
    }

    public async Task<bool> AnonymizeUserDataAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null) return false;

        var anonymized = $"anonymized-{Guid.NewGuid():N[..8]}";
        user.FirstName = "Anonymized";
        user.LastName = "User";
        user.Email = $"{anonymized}@anonymized.local";
        user.NormalizedEmail = user.Email.ToUpperInvariant();
        user.UserName = user.Email;
        user.NormalizedUserName = user.NormalizedEmail;
        user.PhoneNumber = null;

        await userManager.UpdateAsync(user);
        return true;
    }

    public async Task<bool> AnonymizeStudentDataAsync(Guid studentId)
    {
        var student = await context.Students.FindAsync(studentId);
        if (student == null) return false;

        student.FirstName = "Anonymized";
        student.LastName = "Student";
        student.BirthDate = null;
        student.IsActive = false;

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<IDictionary<string, bool>> GetConsentStatusAsync(string userId)
    {
        var consents = await context.UserConsents
            .Where(c => c.UserId == userId)
            .ToListAsync();

        var result = new Dictionary<string, bool>
        {
            ["essential"] = true,
            ["analytics"] = false,
            ["marketing"] = false
        };

        foreach (var consent in consents)
        {
            result[consent.ConsentType] = consent.IsGranted;
        }

        return result;
    }

    public async Task<bool> UpdateConsentAsync(string userId, string consentType, bool granted)
    {
        var existing = await context.UserConsents
            .FirstOrDefaultAsync(c => c.UserId == userId && c.ConsentType == consentType);

        if (existing != null)
        {
            existing.IsGranted = granted;
            existing.GrantedAt = DateTime.UtcNow;
        }
        else
        {
            context.UserConsents.Add(new UserConsent
            {
                UserId = userId,
                ConsentType = consentType,
                IsGranted = granted,
                GrantedAt = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync();
        return true;
    }

    public IDictionary<string, string> GetDataRetentionPolicy()
    {
        return new Dictionary<string, string>
        {
            ["Personal data"] = "Retained while account is active, deleted within 30 days of account closure",
            ["Student records"] = "Retained for 10 years per Czech education law requirements",
            ["Audit logs"] = "Retained for 5 years for compliance purposes",
            ["Communication diary"] = "Retained while student is active, archived after graduation",
            ["PLPP documents"] = "Retained for 10 years per education law"
        };
    }
}
