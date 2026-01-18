namespace SpecEdu.Application.Common.Models;

public class StudentDto
{
    public Guid Id { get; set; }

    public Guid SchoolId { get; set; }

    public string? SchoolName { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string FullName => $"{FirstName} {LastName}".Trim();

    public DateTime? BirthDate { get; set; }

    public string? Class { get; set; }

    public Guid? PhotoId { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? GuardianCount { get; set; }

    public int? StaffLinkCount { get; set; }
}
