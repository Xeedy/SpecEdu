using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpecEdu.Infrastructure.Identity;

namespace SpecEdu.Web.Pages.Account;

public class LoginModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        ILogger<LoginModel> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
    }

    [BindProperty]
    public LoginInput Input { get; set; } = new();

    public string? ReturnUrl { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync(string? returnUrl = null)
    {
        if (!string.IsNullOrEmpty(ErrorMessage))
        {
            ModelState.AddModelError(string.Empty, ErrorMessage);
        }

        // Clear existing external cookie
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        ReturnUrl = returnUrl ?? Url.Content("~/");
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _userManager.FindByEmailAsync(Input.Email);

        if (user == null)
        {
            _logger.LogWarning("Login attempt for non-existent user: {Email}", Input.Email);
            ErrorMessage = "Neplatný e-mail nebo heslo.";
            return Page();
        }

        if (!user.IsActive)
        {
            _logger.LogWarning("Login attempt for inactive user: {Email}", Input.Email);
            ErrorMessage = "Účet je deaktivován. Kontaktujte správce.";
            return Page();
        }

        var result = await _signInManager.PasswordSignInAsync(
            user,
            Input.Password,
            Input.RememberMe,
            lockoutOnFailure: true);

        if (result.Succeeded)
        {
            _logger.LogInformation("User {Email} logged in successfully", Input.Email);

            // Add custom claims (SchoolId)
            if (user.SchoolId.HasValue)
            {
                var claims = new List<Claim>
                {
                    new(JwtTokenService.SchoolIdClaimType, user.SchoolId.Value.ToString())
                };
                await _signInManager.SignInWithClaimsAsync(user, Input.RememberMe, claims);
            }

            return LocalRedirect(returnUrl);
        }

        if (result.IsLockedOut)
        {
            _logger.LogWarning("User {Email} is locked out", Input.Email);
            ErrorMessage = "Účet je dočasně zablokován kvůli příliš mnoha neúspěšným pokusům o přihlášení. Zkuste to prosím za 5 minut.";
            return Page();
        }

        _logger.LogWarning("Invalid login attempt for user: {Email}", Input.Email);
        ErrorMessage = "Neplatný e-mail nebo heslo.";
        return Page();
    }

    public class LoginInput
    {
        [Required(ErrorMessage = "E-mail je povinný.")]
        [EmailAddress(ErrorMessage = "Neplatný formát e-mailu.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Heslo je povinné.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Zapamatovat si mě")]
        public bool RememberMe { get; set; }
    }
}
