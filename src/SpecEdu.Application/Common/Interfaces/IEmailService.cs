namespace SpecEdu.Application.Common.Interfaces;

public interface IEmailService
{
    Task<bool> SendEmailAsync(string to, string subject, string htmlBody, string? textBody = null);

    Task<bool> SendEmailAsync(IEnumerable<string> recipients, string subject, string htmlBody, string? textBody = null);
}
