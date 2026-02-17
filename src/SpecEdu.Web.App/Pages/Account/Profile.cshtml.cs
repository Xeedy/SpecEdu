using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpecEdu.Application.Common.Interfaces;

namespace SpecEdu.Web.App.Pages.Account;

[Authorize]
public class ProfileModel(
    IIdentityService identityService,
    ICurrentUserService currentUserService,
    ISchoolService schoolService) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? SchoolName { get; set; }
    public IList<string> Roles { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public string Initials { get; set; } = string.Empty;

    public class InputModel
    {
        [Required(ErrorMessage = "First name is required")]
        [StringLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = currentUserService.UserId;
        if (userId == null)
        {
            return RedirectToPage("/Account/Login");
        }

        var user = await identityService.GetUserByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        PopulateFromUser(user);
        await LoadSchoolNameAsync(user.SchoolId);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userId = currentUserService.UserId;
        if (userId == null)
        {
            return RedirectToPage("/Account/Login");
        }

        if (!ModelState.IsValid)
        {
            var user = await identityService.GetUserByIdAsync(userId);
            if (user != null)
            {
                PopulateFromUser(user);
                await LoadSchoolNameAsync(user.SchoolId);
            }
            return Page();
        }

        var success = await identityService.UpdateUserAsync(
            userId,
            Input.FirstName,
            Input.LastName,
            true);

        if (success)
        {
            TempData["SuccessMessage"] = "ProfilePage.UpdateSuccess";
        }
        else
        {
            TempData["ErrorMessage"] = "ProfilePage.UpdateError";
        }

        return RedirectToPage();
    }

    private void PopulateFromUser(Application.Common.Models.ApplicationUserDto user)
    {
        Input = new InputModel
        {
            FirstName = user.FirstName,
            LastName = user.LastName
        };
        Email = user.Email;
        Roles = user.Roles;
        CreatedAt = user.CreatedAt;
        IsActive = user.IsActive;
        Initials = GetInitials(user.FirstName, user.LastName);
    }

    private async Task LoadSchoolNameAsync(Guid? schoolId)
    {
        if (schoolId.HasValue)
        {
            var school = await schoolService.GetByIdAsync(schoolId.Value);
            SchoolName = school?.Name;
        }
    }

    private static string GetInitials(string firstName, string lastName)
    {
        var first = string.IsNullOrWhiteSpace(firstName) ? "" : firstName[..1].ToUpperInvariant();
        var last = string.IsNullOrWhiteSpace(lastName) ? "" : lastName[..1].ToUpperInvariant();
        return $"{first}{last}";
    }
}
