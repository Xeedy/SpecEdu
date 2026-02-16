using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MimeKit;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;

namespace SpecEdu.Web.Public.Pages.Account
{
    public class RegisterModel(ILogger<RegisterModel> Log, IConfiguration config) : PageModel
    {
        [BindProperty]
        public RegisterRequest Input { get; set; } = new();

        [TempData] public string? SuccessMessage { get; set; }
        [TempData] public string? ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();
            if (!Input.Consent)
            {
                ModelState.AddModelError("Input.Consent", "Mus\u00edte souhlasit se zpracov\u00e1n\u00edm \u00fadaj\u016f.");
                return Page();
            }

            try
            {
                await SendEmailAsync(Input);

                SuccessMessage = "\u017d\u00e1dost byla odesl\u00e1na. Ozveme se v\u00e1m co nejd\u0159\u00edve.";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                Log.LogError(ex, "Sending institution request email failed");
                ErrorMessage = "Nepoda\u0159ilo se odeslat \u017e\u00e1dost. Zkuste to pros\u00edm pozd\u011bji.";
                return Page();
            }
        }

        private async Task SendEmailAsync(RegisterRequest req)
        {
            var mailSection = config.GetSection("Mail");

            var toEmail = mailSection["To"] ?? "michal.tesik1@gmail.com";
            var host = mailSection["SmtpHost"] ?? throw new InvalidOperationException("Mail:SmtpHost missing");
            var port = int.TryParse(mailSection["SmtpPort"], out var p) ? p : 587;

            var user = mailSection["SmtpUser"] ?? throw new InvalidOperationException("Mail:SmtpUser missing");
            var pass = mailSection["SmtpPass"] ?? throw new InvalidOperationException("Mail:SmtpPass missing");

            var fromEmail = mailSection["FromEmail"] ?? user;
            var fromName = mailSection["FromName"] ?? "SpecEdu";

            var msg = new MimeMessage();
            msg.From.Add(new MailboxAddress(fromName, fromEmail));
            msg.To.Add(MailboxAddress.Parse(toEmail));

            msg.ReplyTo.Add(new MailboxAddress(req.ContactName, req.Email));

            msg.Subject = $"[SpecEdu] \u017d\u00e1dost o z\u0159\u00edzen\u00ed instituce: {req.InstitutionName} ({req.InstitutionType})";

            var html = BuildHtml(req);

            msg.Body = new BodyBuilder
            {
                HtmlBody = html,
                TextBody = BuildText(req)
            }.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(host, port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(user, pass);
            await smtp.SendAsync(msg);
            await smtp.DisconnectAsync(true);
        }

        private string BuildHtml(RegisterRequest req)
        {
            string E(string? s) => HtmlEncoder.Default.Encode(s ?? "");

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "-";
            var now = DateTimeOffset.Now;

            return $@"
<div style=""font-family:Arial,sans-serif;line-height:1.5"">
  <h2 style=""margin:0 0 12px 0"">\u017d\u00e1dost o z\u0159\u00edzen\u00ed instituce</h2>

  <table style=""border-collapse:collapse;width:100%;max-width:820px"">
    <tr><td style=""padding:8px 10px;border:1px solid #ddd;width:240px""><b>Instituce</b></td>
        <td style=""padding:8px 10px;border:1px solid #ddd"">{E(req.InstitutionName)}</td></tr>
    <tr><td style=""padding:8px 10px;border:1px solid #ddd""><b>Typ instituce</b></td>
        <td style=""padding:8px 10px;border:1px solid #ddd"">{E(req.InstitutionType)}</td></tr>
    <tr><td style=""padding:8px 10px;border:1px solid #ddd""><b>I\u010cO</b></td>
        <td style=""padding:8px 10px;border:1px solid #ddd"">{E(req.Ico)}</td></tr>
    <tr><td style=""padding:8px 10px;border:1px solid #ddd""><b>Adresa</b></td>
        <td style=""padding:8px 10px;border:1px solid #ddd"">{E(req.Address)}</td></tr>

    <tr><td style=""padding:8px 10px;border:1px solid #ddd""><b>Kontaktn\u00ed osoba</b></td>
        <td style=""padding:8px 10px;border:1px solid #ddd"">{E(req.ContactName)}</td></tr>
    <tr><td style=""padding:8px 10px;border:1px solid #ddd""><b>Funkce</b></td>
        <td style=""padding:8px 10px;border:1px solid #ddd"">{E(req.ContactRole)}</td></tr>
    <tr><td style=""padding:8px 10px;border:1px solid #ddd""><b>E-mail</b></td>
        <td style=""padding:8px 10px;border:1px solid #ddd"">{E(req.Email)}</td></tr>
    <tr><td style=""padding:8px 10px;border:1px solid #ddd""><b>Telefon</b></td>
        <td style=""padding:8px 10px;border:1px solid #ddd"">{E(req.Phone)}</td></tr>

    <tr><td style=""padding:8px 10px;border:1px solid #ddd""><b>Po\u010det \u017e\u00e1k\u016f (odhad)</b></td>
        <td style=""padding:8px 10px;border:1px solid #ddd"">{E(req.StudentsCount?.ToString())}</td></tr>
    <tr><td style=""padding:8px 10px;border:1px solid #ddd""><b>Po\u010det u\u017eivatel\u016f (odhad)</b></td>
        <td style=""padding:8px 10px;border:1px solid #ddd"">{E(req.UsersCount?.ToString())}</td></tr>
    <tr><td style=""padding:8px 10px;border:1px solid #ddd""><b>Preferovan\u00fd start</b></td>
        <td style=""padding:8px 10px;border:1px solid #ddd"">{E(req.PreferredStart)}</td></tr>

    <tr><td style=""padding:8px 10px;border:1px solid #ddd""><b>Odesl\u00e1no</b></td>
        <td style=""padding:8px 10px;border:1px solid #ddd"">{now:yyyy-MM-dd HH:mm}</td></tr>
    <tr><td style=""padding:8px 10px;border:1px solid #ddd""><b>IP</b></td>
        <td style=""padding:8px 10px;border:1px solid #ddd"">{E(ip)}</td></tr>
  </table>

  <h3 style=""margin:18px 0 8px 0"">Zpr\u00e1va / po\u017eadavky</h3>
  <div style=""border:1px solid #ddd;padding:12px;border-radius:8px;background:#fafafa"">
    {(E(req.Message).Replace("\n", "<br/>"))}
  </div>
</div>";
        }

        private string BuildText(RegisterRequest req)
        {
            return $@"
\u017d\u00e1dost o z\u0159\u00edzen\u00ed instituce

Instituce: {req.InstitutionName}
Typ: {req.InstitutionType}
I\u010cO: {req.Ico}
Adresa: {req.Address}

Kontaktn\u00ed osoba: {req.ContactName}
Funkce: {req.ContactRole}
E-mail: {req.Email}
Telefon: {req.Phone}

Odhad \u017e\u00e1k\u016f: {req.StudentsCount}
Odhad u\u017eivatel\u016f: {req.UsersCount}
Preferovan\u00fd start: {req.PreferredStart}

Zpr\u00e1va:
{req.Message}
";
        }

        public class RegisterRequest
        {
            [Required, StringLength(200)]
            public string InstitutionName { get; set; } = "";

            [Required, StringLength(80)]
            public string InstitutionType { get; set; } = "";

            [RegularExpression(@"^$|^\d{8}$", ErrorMessage = "I\u010cO mus\u00ed m\u00edt 8 \u010d\u00edslic.")]
            public string? Ico { get; set; }

            [StringLength(200)]
            public string? Address { get; set; }

            [Required, StringLength(120)]
            public string ContactName { get; set; } = "";

            [StringLength(120)]
            public string? ContactRole { get; set; }

            [Required, EmailAddress, StringLength(200)]
            public string Email { get; set; } = "";

            [StringLength(50)]
            public string? Phone { get; set; }

            public int? StudentsCount { get; set; }
            public int? UsersCount { get; set; }

            [StringLength(40)]
            public string? PreferredStart { get; set; } = "Co nejd\u0159\u00edve";

            [StringLength(2000)]
            public string? Message { get; set; }

            [Required(ErrorMessage = "Mus\u00edte souhlasit se zpracov\u00e1n\u00edm \u00fadaj\u016f.")]
            public bool Consent { get; set; }
        }
    }
}
