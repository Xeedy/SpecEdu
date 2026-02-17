using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Application.Common.Models;
using SpecEdu.Domain.Constants;
using SpecEdu.Infrastructure.Authorization;

namespace SpecEdu.Web.App.Pages.Admin.Users;

[Authorize(Policy = AuthorizationPolicies.RequireAdmin)]
public class CreateModel(
    IIdentityService identityService,
    ISchoolService schoolService,
    ILogger<CreateModel> logger) : PageModel
{
    [BindProperty]
    public CreateUserInput Input { get; set; } = new();

    [BindProperty]
    public List<string> SelectedRoles { get; set; } = new();

    public IList<SchoolDto> Schools { get; set; } = new List<SchoolDto>();

    public List<RoleOption> AvailableRoles { get; set; } = new();

    [TempData]
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        await LoadFormDataAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadFormDataAsync();
            return Page();
        }

        if (!SelectedRoles.Any())
        {
            ModelState.AddModelError(string.Empty, "Vyberte alespoň jednu roli.");
            await LoadFormDataAsync();
            return Page();
        }

        try
        {
            Guid? schoolId = Input.SchoolId == Guid.Empty ? null : Input.SchoolId;

            var (succeeded, user, errors) = await identityService.CreateUserAsync(
                Input.Email,
                Input.Password,
                Input.FirstName,
                Input.LastName,
                schoolId,
                SelectedRoles);

            if (!succeeded)
            {
                foreach (var error in errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                await LoadFormDataAsync();
                return Page();
            }

            logger.LogInformation("User {Email} (ID: {UserId}) created", Input.Email, user?.Id);

            TempData["SuccessMessage"] = $"Uživatel \"{Input.Email}\" byl úspěšně vytvořen.";
            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating user {Email}", Input.Email);
            ErrorMessage = "Nepodařilo se vytvořit uživatele. Zkuste to prosím znovu.";
            await LoadFormDataAsync();
            return Page();
        }
    }

    private async Task LoadFormDataAsync()
    {
        Schools = await schoolService.GetAllAsync(includeInactive: false);
        AvailableRoles = new List<RoleOption>
        {
            new("Admin", "Administrátor"),
            new("SchoolAdmin", "Správce školy"),
            new("Teacher", "Pedagog"),
            new("Parent", "Rodič"),
            new("SPP", "ŠPP"),
            new("Assistant", "Asistent")
        };
    }

    public class CreateUserInput
    {
        [Required(ErrorMessage = "E-mail je povinný.")]
        [EmailAddress(ErrorMessage = "Neplatný formát e-mailu.")]
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Heslo je povinné.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Heslo musí mít alespoň 6 znaků.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Jméno je povinné.")]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Příjmení je povinné.")]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        public Guid? SchoolId { get; set; }
    }

    public record RoleOption(string Value, string Label);
}
