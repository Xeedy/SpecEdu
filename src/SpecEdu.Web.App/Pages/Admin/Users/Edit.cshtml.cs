using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Application.Common.Models;
using SpecEdu.Infrastructure.Authorization;

namespace SpecEdu.Web.App.Pages.Admin.Users;

[Authorize(Policy = AuthorizationPolicies.RequireAdmin)]
public class EditModel(
    IIdentityService identityService,
    ILogger<EditModel> logger) : PageModel
{
    [BindProperty]
    public EditUserInput Input { get; set; } = new();

    [BindProperty]
    public List<string> SelectedRoles { get; set; } = new();

    [BindProperty]
    public string? NewPassword { get; set; }

    public ApplicationUserDto? UserDto { get; set; }

    public List<RoleOption> AvailableRoles { get; set; } = new();

    [TempData]
    public string? ErrorMessage { get; set; }

    [TempData]
    public string? SuccessMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        var user = await identityService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        UserDto = user;
        Input = new EditUserInput
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            IsActive = user.IsActive
        };
        SelectedRoles = user.Roles.ToList();

        LoadAvailableRoles();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await ReloadUserAsync(Input.Id);
            LoadAvailableRoles();
            return Page();
        }

        try
        {
            // Update basic info
            var updated = await identityService.UpdateUserAsync(
                Input.Id, Input.FirstName, Input.LastName, Input.IsActive);

            if (!updated)
            {
                ErrorMessage = "Nepodařilo se aktualizovat uživatele.";
                await ReloadUserAsync(Input.Id);
                LoadAvailableRoles();
                return Page();
            }

            // Update roles
            var currentRoles = await identityService.GetRolesAsync(Input.Id);
            var rolesToRemove = currentRoles.Except(SelectedRoles).ToList();
            var rolesToAdd = SelectedRoles.Except(currentRoles).ToList();

            foreach (var role in rolesToRemove)
            {
                await identityService.RemoveFromRoleAsync(Input.Id, role);
            }
            foreach (var role in rolesToAdd)
            {
                await identityService.AddToRoleAsync(Input.Id, role);
            }

            // Reset password if provided
            if (!string.IsNullOrWhiteSpace(NewPassword))
            {
                var (succeeded, errors) = await identityService.ResetPasswordAsync(Input.Id, NewPassword);
                if (!succeeded)
                {
                    foreach (var error in errors)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                    await ReloadUserAsync(Input.Id);
                    LoadAvailableRoles();
                    return Page();
                }
            }

            logger.LogInformation("User {UserId} updated", Input.Id);

            TempData["SuccessMessage"] = "Uživatel byl úspěšně aktualizován.";
            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating user {UserId}", Input.Id);
            ErrorMessage = "Nepodařilo se aktualizovat uživatele. Zkuste to prosím znovu.";
            await ReloadUserAsync(Input.Id);
            LoadAvailableRoles();
            return Page();
        }
    }

    private async Task ReloadUserAsync(string id)
    {
        UserDto = await identityService.GetUserByIdAsync(id);
    }

    private void LoadAvailableRoles()
    {
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

    public class EditUserInput
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "Jméno je povinné.")]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Příjmení je povinné.")]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }

    public record RoleOption(string Value, string Label);
}
