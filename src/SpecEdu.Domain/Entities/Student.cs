using SpecEdu.Domain.Common;

namespace SpecEdu.Domain.Entities;

public class Student : AuditableEntity
{
    public Guid SchoolId { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public DateTime? BirthDate { get; set; }

    public string? Class { get; set; }

    public Guid? PhotoId { get; set; }

    public bool IsActive { get; set; } = true;

    public School? School { get; set; }

    public ICollection<StudentGuardian> Guardians { get; set; } = new List<StudentGuardian>();

    public ICollection<StudentStaffLink> StaffLinks { get; set; } = new List<StudentStaffLink>();

    public string FullName => $"{FirstName} {LastName}".Trim();
}
