namespace SpecEdu.Application.Common;

public record ChangelogEntry(string Version, string Date, string[] Changes);

public static class ChangelogEntries
{
    public static readonly IReadOnlyList<ChangelogEntry> All =
    [
        new("1.0.0", "2026-02-17",
        [
            "Student management with guardian and staff linking",
            "Communication diary with attachments and visibility control",
            "Pedagogical support plans (PLPP) with goals, evaluations, and versioning",
            "Consultation calendar with participant management and invitations",
            "Reminder system with email notifications",
            "In-app notification system",
            "GDPR compliance: cookie consent, data export, consent management",
            "Security hardening: rate limiting, CSP headers, antiforgery",
            "Admin panel with user management, school management, and audit log",
            "Multi-language support: Czech, English, German",
            "Role-based access: Admin, SchoolAdmin, Teacher, SPP, Parent"
        ])
    ];
}
