namespace SpecEdu.Application.Common.Interfaces;

/// <summary>
/// Service for sending email notifications.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends an email to the specified recipient.
    /// </summary>
    /// <param name="to">Recipient email address.</param>
    /// <param name="subject">Email subject line.</param>
    /// <param name="htmlBody">HTML content of the email.</param>
    /// <param name="textBody">Optional plain text alternative.</param>
    /// <returns>True if email was sent successfully, false otherwise.</returns>
    Task<bool> SendEmailAsync(string to, string subject, string htmlBody, string? textBody = null);

    /// <summary>
    /// Sends an email to multiple recipients.
    /// </summary>
    /// <param name="recipients">List of recipient email addresses.</param>
    /// <param name="subject">Email subject line.</param>
    /// <param name="htmlBody">HTML content of the email.</param>
    /// <param name="textBody">Optional plain text alternative.</param>
    /// <returns>True if emails were sent successfully to all recipients, false if any failed.</returns>
    Task<bool> SendEmailAsync(IEnumerable<string> recipients, string subject, string htmlBody, string? textBody = null);
}
