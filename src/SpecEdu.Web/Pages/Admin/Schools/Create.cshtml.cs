using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Infrastructure.Authorization;

namespace SpecEdu.Web.Pages.Admin.Schools;

[Authorize(Policy = AuthorizationPolicies.RequireAdmin)]
public class CreateModel(ISchoolService schoolService, ILogger<CreateModel> logger) : PageModel
{
    [BindProperty]
    public CreateSchoolInput Input { get; set; } = new();

    [TempData]
    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var school = await schoolService.CreateAsync(
                Input.Name,
                Input.InstitutionType,
                Input.Ico,
                Input.Address,
                Input.City,
                Input.PostalCode,
                Input.ContactEmail,
                Input.ContactPhone);

            logger.LogInformation("School {SchoolName} (ID: {SchoolId}) created", school.Name, school.Id);

            TempData["SuccessMessage"] = $"Škola \"{school.Name}\" byla úspěšně vytvořena.";
            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating school {SchoolName}", Input.Name);
            ErrorMessage = "Nepodařilo se vytvořit školu. Zkuste to prosím znovu.";
            return Page();
        }
    }

    #region SchoolModel

    public class CreateSchoolInput
    {
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
    }

    #endregion
}
