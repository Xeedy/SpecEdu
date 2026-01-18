using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MimeKit;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;

namespace SpecEdu.Web.Pages.Account
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
                ModelState.AddModelError("Input.Consent", "Musíte souhlasit se zpracováním údajù.");
                return Page();
            }

            try
            {
                await SendEmailAsync(Input);

                SuccessMessage = "Žádost byla odeslána. Ozveme se vám co nejdøíve.";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                Log.LogError(ex, "Sending institution request email failed");
                ErrorMessage = "Nepodaøilo se odeslat žádost. Zkuste to prosím pozdìji.";
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

            msg.Subject = $"[SpecEdu] Žádost o zøízení instituce: {req.InstitutionName} ({req.InstitutionType})";

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
  <h2 style=""margin:0 0 12px 0"">Žádost o zøízení instituce</h2>

  <table style=""border-collapse:collapse;width:100%;max-width:820px"">
    <tr><td style=""padding:8px 10px;border:1px solid #ddd;width:240px""><b>Instituce</b></td>
        <td style=""padding:8px 10px;border:1px solid #ddd"">{E(req.InstitutionName)}</td></tr>
    <tr><td style=""padding:8px 10px;border:1px solid #ddd""><b>Typ instituce</b></td>
        <td style=""padding:8px 10px;border:1px solid #ddd"">{E(req.InstitutionType)}</td></tr>
    <tr><td style=""padding:8px 10px;border:1px solid #ddd""><b>IÈO</b></td>
        <td style=""padding:8px 10px;border:1px solid #ddd"">{E(req.Ico)}</td></tr>
    <tr><td style=""padding:8px 10px;border:1px solid #ddd""><b>Adresa</b></td>
        <td style=""padding:8px 10px;border:1px solid #ddd"">{E(req.Address)}</td></tr>

    <tr><td style=""padding:8px 10px;border:1px solid #ddd""><b>Kontaktní osoba</b></td>
        <td style=""padding:8px 10px;border:1px solid #ddd"">{E(req.ContactName)}</td></tr>
    <tr><td style=""padding:8px 10px;border:1px solid #ddd""><b>Funkce</b></td>
        <td style=""padding:8px 10px;border:1px solid #ddd"">{E(req.ContactRole)}</td></tr>
    <tr><td style=""padding:8px 10px;border:1px solid #ddd""><b>E-mail</b></td>
        <td style=""padding:8px 10px;border:1px solid #ddd"">{E(req.Email)}</td></tr>
    <tr><td style=""padding:8px 10px;border:1px solid #ddd""><b>Telefon</b></td>
        <td style=""padding:8px 10px;border:1px solid #ddd"">{E(req.Phone)}</td></tr>

    <tr><td style=""padding:8px 10px;border:1px solid #ddd""><b>Poèet žákù (odhad)</b></td>
        <td style=""padding:8px 10px;border:1px solid #ddd"">{E(req.StudentsCount?.ToString())}</td></tr>
    <tr><td style=""padding:8px 10px;border:1px solid #ddd""><b>Poèet uživatelù (odhad)</b></td>
        <td style=""padding:8px 10px;border:1px solid #ddd"">{E(req.UsersCount?.ToString())}</td></tr>
    <tr><td style=""padding:8px 10px;border:1px solid #ddd""><b>Preferovaný start</b></td>
        <td style=""padding:8px 10px;border:1px solid #ddd"">{E(req.PreferredStart)}</td></tr>

    <tr><td style=""padding:8px 10px;border:1px solid #ddd""><b>Odesláno</b></td>
        <td style=""padding:8px 10px;border:1px solid #ddd"">{now:yyyy-MM-dd HH:mm}</td></tr>
    <tr><td style=""padding:8px 10px;border:1px solid #ddd""><b>IP</b></td>
        <td style=""padding:8px 10px;border:1px solid #ddd"">{E(ip)}</td></tr>
  </table>

  <h3 style=""margin:18px 0 8px 0"">Zpráva / požadavky</h3>
  <div style=""border:1px solid #ddd;padding:12px;border-radius:8px;background:#fafafa"">
    {(E(req.Message).Replace("\n", "<br/>"))}
  </div>
</div>";
        }

        private string BuildText(RegisterRequest req)
        {
            return $@"
Žádost o zøízení instituce

Instituce: {req.InstitutionName}
Typ: {req.InstitutionType}
IÈO: {req.Ico}
Adresa: {req.Address}

Kontaktní osoba: {req.ContactName}
Funkce: {req.ContactRole}
E-mail: {req.Email}
Telefon: {req.Phone}

Odhad žákù: {req.StudentsCount}
Odhad uživatelù: {req.UsersCount}
Preferovaný start: {req.PreferredStart}

Zpráva:
{req.Message}
";
        }

        public class RegisterRequest
        {
            [Required, StringLength(200)]
            public string InstitutionName { get; set; } = "";

            [Required, StringLength(80)]
            public string InstitutionType { get; set; } = "";

            [RegularExpression(@"^$|^\d{8}$", ErrorMessage = "IÈO musí mít 8 èíslic.")]
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
            public string? PreferredStart { get; set; } = "Co nejdøíve";

            [StringLength(2000)]
            public string? Message { get; set; }

            [Required(ErrorMessage = "Musíte souhlasit se zpracováním údajù.")]
            public bool Consent { get; set; }
        }
    }
}
