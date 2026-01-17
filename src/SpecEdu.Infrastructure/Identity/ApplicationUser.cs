using Microsoft.AspNetCore.Identity;

namespace SpecEdu.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public bool IsActive { get; set; } = true;

    public string FullName => $"{FirstName} {LastName}".Trim();
}
