using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Application.Common.Models;

namespace SpecEdu.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        IOptions<EmailSettings> settings,
        ILogger<EmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string htmlBody, string? textBody = null)
    {
        return await SendEmailAsync(new[] { to }, subject, htmlBody, textBody);
    }

    public async Task<bool> SendEmailAsync(IEnumerable<string> recipients, string subject, string htmlBody, string? textBody = null)
    {
        var recipientList = recipients.ToList();
        if (!recipientList.Any())
        {
            _logger.LogWarning("SendEmailAsync called with no recipients");
            return false;
        }

        if (!string.IsNullOrEmpty(_settings.DefaultTo))
        {
            _logger.LogInformation("Using DefaultTo email override: {DefaultTo}", _settings.DefaultTo);
            recipientList = new List<string> { _settings.DefaultTo };
        }

        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));

            foreach (var recipient in recipientList)
            {
                if (!string.IsNullOrWhiteSpace(recipient))
                {
                    message.To.Add(MailboxAddress.Parse(recipient));
                }
            }

            if (!message.To.Any())
            {
                _logger.LogWarning("No valid recipients after parsing");
                return false;
            }

            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = htmlBody
            };

            if (!string.IsNullOrEmpty(textBody))
            {
                bodyBuilder.TextBody = textBody;
            }

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();

            await client.ConnectAsync(
                _settings.SmtpHost,
                _settings.SmtpPort,
                SecureSocketOptions.StartTls);

            if (!string.IsNullOrEmpty(_settings.SmtpUser))
            {
                await client.AuthenticateAsync(_settings.SmtpUser, _settings.SmtpPass);
            }

            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation(
                "Email sent successfully to {RecipientCount} recipient(s): Subject='{Subject}'",
                message.To.Count,
                subject);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send email to {Recipients}: {Error}",
                string.Join(", ", recipientList),
                ex.Message);

            return false;
        }
    }
}
