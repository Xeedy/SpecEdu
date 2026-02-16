using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using SpecEdu.Infrastructure.Identity;

namespace SpecEdu.Web.Public.Pages.Account;

public class ResetPasswordModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<ResetPasswordModel> _logger;

    public ResetPasswordModel(
        UserManager<ApplicationUser> userManager,
        ILogger<ResetPasswordModel> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required(ErrorMessage = "E-mail je povinny.")]
        [EmailAddress(ErrorMessage = "Neplatny format e-mailu.")]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Heslo je povinne.")]
        [StringLength(100, ErrorMessage = "Heslo musi mit alespon {2} znaku.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Nove heslo")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Potvrzeni hesla")]
        [Compare("Password", ErrorMessage = "Hesla se neshoduji.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        public string Code { get; set; } = string.Empty;
    }

    public IActionResult OnGet(string? code = null)
    {
        if (code == null)
        {
            return BadRequest("Pro obnoveni hesla je potreba platny kod.");
        }

        Input = new InputModel
        {
            Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _userManager.FindByEmailAsync(Input.Email);
        if (user == null)
        {
            // Don't reveal that the user does not exist
            return RedirectToPage("./ResetPasswordConfirmation");
        }

        var result = await _userManager.ResetPasswordAsync(user, Input.Code, Input.Password);
        if (result.Succeeded)
        {
            _logger.LogInformation("Password reset successfully for user {Email}", Input.Email);
            return RedirectToPage("./ResetPasswordConfirmation");
        }

        foreach (var error in result.Errors)
        {
            var errorMessage = error.Code switch
            {
                "InvalidToken" => "Odkaz pro obnoveni hesla je neplatny nebo vyprsel.",
                "PasswordTooShort" => "Heslo je prilis kratke.",
                "PasswordRequiresNonAlphanumeric" => "Heslo musi obsahovat alespon jeden specialni znak.",
                "PasswordRequiresDigit" => "Heslo musi obsahovat alespon jednu cislici.",
                "PasswordRequiresLower" => "Heslo musi obsahovat alespon jedno male pismeno.",
                "PasswordRequiresUpper" => "Heslo musi obsahovat alespon jedno velke pismeno.",
                "PasswordRequiresUniqueChars" => "Heslo musi obsahovat vice ruznych znaku.",
                _ => error.Description
            };
            ModelState.AddModelError(string.Empty, errorMessage);
        }

        return Page();
    }
}
