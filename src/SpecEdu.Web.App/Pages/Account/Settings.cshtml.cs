using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpecEdu.Application.Common.Interfaces;

namespace SpecEdu.Web.App.Pages.Account;

[Authorize]
public class SettingsModel(
    IIdentityService identityService,
    ICurrentUserService currentUserService) : PageModel
{
    [BindProperty]
    public PasswordInputModel PasswordInput { get; set; } = new();

    public class PasswordInputModel
    {
        [Required(ErrorMessage = "Current password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please confirm your new password")]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostChangePasswordAsync()
    {
        var userId = currentUserService.UserId;
        if (userId == null)
        {
            return RedirectToPage("/Account/Login");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var (succeeded, errors) = await identityService.ChangePasswordAsync(
            userId,
            PasswordInput.CurrentPassword,
            PasswordInput.NewPassword);

        if (succeeded)
        {
            TempData["SuccessMessage"] = "SettingsPage.PasswordChangeSuccess";
            return RedirectToPage();
        }

        foreach (var error in errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }

        return Page();
    }
}
