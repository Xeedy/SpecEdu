using SpecEdu.Domain.Common;
using SpecEdu.Domain.Enums;

namespace SpecEdu.Domain.Entities;

public class StudentStaffLink : AuditableEntity
{
    public Guid StudentId { get; set; }

    public string UserId { get; set; } = string.Empty;

    public StaffLinkType LinkType { get; set; }

    public AccessLevel AccessLevel { get; set; } = AccessLevel.Read;

    public bool IsActive { get; set; } = true;

    public Student? Student { get; set; }
}
