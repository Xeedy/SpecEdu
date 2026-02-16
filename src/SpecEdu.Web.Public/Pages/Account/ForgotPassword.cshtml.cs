using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Infrastructure.Identity;

namespace SpecEdu.Web.Public.Pages.Account;

public class ForgotPasswordModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly ILogger<ForgotPasswordModel> _logger;

    public ForgotPasswordModel(
        UserManager<ApplicationUser> userManager,
        IEmailService emailService,
        ILogger<ForgotPasswordModel> logger)
    {
        _userManager = userManager;
        _emailService = emailService;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    [TempData]
    public string? ErrorMessage { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "E-mail je povinny.")]
        [EmailAddress(ErrorMessage = "Neplatny format e-mailu.")]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = string.Empty;
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

        var user = await _userManager.FindByEmailAsync(Input.Email);

        // Always redirect to confirmation to prevent email enumeration
        if (user == null || !user.IsActive)
        {
            _logger.LogWarning("Password reset requested for non-existent or inactive email: {Email}", Input.Email);
            return RedirectToPage("./ForgotPasswordConfirmation");
        }

        // Generate password reset token
        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        // Build callback URL
        var callbackUrl = Url.Page(
            "/Account/ResetPassword",
            pageHandler: null,
            values: new { code },
            protocol: Request.Scheme);

        // Send email
        var emailSubject = "Obnoveni hesla - SpecEdu";
        var emailBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
        <h2 style='color: #1565c0;'>Obnoveni hesla</h2>
        <p>Dobry den,</p>
        <p>Obdrzeli jsme zadost o obnoveni hesla pro vas ucet v systemu SpecEdu.</p>
        <p>Pro nastaveni noveho hesla kliknete na nasledujici odkaz:</p>
        <p style='margin: 20px 0;'>
            <a href='{HtmlEncoder.Default.Encode(callbackUrl ?? "")}'
               style='background-color: #1565c0; color: white; padding: 12px 24px; text-decoration: none; border-radius: 4px; display: inline-block;'>
                Obnovit heslo
            </a>
        </p>
        <p><small>Pokud jste o obnoveni hesla nezadali, tento e-mail muzete ignorovat.</small></p>
        <p><small>Odkaz je platny 24 hodin.</small></p>
        <hr style='border: none; border-top: 1px solid #eee; margin: 20px 0;'>
        <p style='color: #666; font-size: 12px;'>
            Tento e-mail byl odeslan automaticky systemem SpecEdu.
        </p>
    </div>
</body>
</html>";

        var emailSent = await _emailService.SendEmailAsync(
            Input.Email,
            emailSubject,
            emailBody,
            $"Obnoveni hesla - kliknete na nasledujici odkaz: {callbackUrl}");

        if (emailSent)
        {
            _logger.LogInformation("Password reset email sent to {Email}", Input.Email);
        }
        else
        {
            _logger.LogError("Failed to send password reset email to {Email}", Input.Email);
        }

        return RedirectToPage("./ForgotPasswordConfirmation");
    }
}
