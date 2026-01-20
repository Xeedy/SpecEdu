namespace SpecEdu.Application.Common.Models;

/// <summary>
/// Configuration settings for email sending (SMTP).
/// Bound from appsettings.json "Mail" section.
/// </summary>
public class EmailSettings
{
    /// <summary>
    /// Configuration section name in appsettings.json.
    /// </summary>
    public const string SectionName = "Mail";

    /// <summary>
    /// SMTP server hostname.
    /// </summary>
    public string SmtpHost { get; set; } = string.Empty;

    /// <summary>
    /// SMTP server port (typically 587 for TLS).
    /// </summary>
    public int SmtpPort { get; set; } = 587;

    /// <summary>
    /// SMTP authentication username.
    /// </summary>
    public string SmtpUser { get; set; } = string.Empty;

    /// <summary>
    /// SMTP authentication password.
    /// </summary>
    public string SmtpPass { get; set; } = string.Empty;

    /// <summary>
    /// Email address to send from.
    /// </summary>
    public string FromEmail { get; set; } = string.Empty;

    /// <summary>
    /// Display name for the sender.
    /// </summary>
    public string FromName { get; set; } = "SpecEdu";

    /// <summary>
    /// Default recipient email for testing purposes.
    /// If set, all emails are sent here instead of actual recipients.
    /// </summary>
    public string? DefaultTo { get; set; }
}
