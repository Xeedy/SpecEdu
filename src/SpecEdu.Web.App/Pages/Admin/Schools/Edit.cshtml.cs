using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Infrastructure.Authorization;

namespace SpecEdu.Web.App.Pages.Admin.Schools;

[Authorize(Policy = AuthorizationPolicies.RequireAdmin)]
public class EditModel(ISchoolService schoolService, ILogger<EditModel> logger) : PageModel
{
    [BindProperty]
    public EditSchoolInput Input { get; set; } = new();

    public int UserCount { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var school = await schoolService.GetByIdAsync(id);
        if (school == null)
        {
            return NotFound();
        }

        Input = new EditSchoolInput
        {
            Id = school.Id,
            Name = school.Name,
            InstitutionType = school.InstitutionType,
            Ico = school.Ico,
            Address = school.Address,
            City = school.City,
            PostalCode = school.PostalCode,
            ContactEmail = school.ContactEmail,
            ContactPhone = school.ContactPhone,
            IsActive = school.IsActive
        };

        UserCount = await schoolService.GetUserCountAsync(id);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            UserCount = await schoolService.GetUserCountAsync(Input.Id);
            return Page();
        }

        try
        {
            var updated = await schoolService.UpdateAsync(
                Input.Id,
                Input.Name,
                Input.InstitutionType,
                Input.Ico,
                Input.Address,
                Input.City,
                Input.PostalCode,
                Input.ContactEmail,
                Input.ContactPhone,
                Input.IsActive,
                null);

            if (updated == null)
            {
                ErrorMessage = "Škola nebyla nalezena.";
                return Page();
            }

            logger.LogInformation("School {SchoolName} (ID: {SchoolId}) updated", updated.Name, updated.Id);

            TempData["SuccessMessage"] = $"Škola \"{updated.Name}\" byla úspěšně aktualizována.";
            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating school {SchoolId}", Input.Id);
            ErrorMessage = "Nepodařilo se aktualizovat školu. Zkuste to prosím znovu.";
            UserCount = await schoolService.GetUserCountAsync(Input.Id);
            return Page();
        }
    }

    public class EditSchoolInput
    {
        [Required]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Název instituce je povinný.")]
        [StringLength(200, ErrorMessage = "Název může mít maximálně 200 znaků.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Typ instituce je povinný.")]
        [StringLength(50)]
        public string InstitutionType { get; set; } = "Škola";

        [RegularExpression(@"^$|^\d{8}$", ErrorMessage = "IČO musí mít 8 číslic.")]
        public string? Ico { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(10)]
        public string? PostalCode { get; set; }

        [EmailAddress(ErrorMessage = "Neplatný formát e-mailu.")]
        [StringLength(200)]
        public string? ContactEmail { get; set; }

        [StringLength(50)]
        public string? ContactPhone { get; set; }

        public bool IsActive { get; set; }
    }
}
