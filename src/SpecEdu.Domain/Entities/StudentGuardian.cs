using SpecEdu.Domain.Common;
using SpecEdu.Domain.Enums;

namespace SpecEdu.Domain.Entities;

public class StudentGuardian : AuditableEntity
{
    public Guid StudentId { get; set; }

    public string ParentUserId { get; set; } = string.Empty;

    public RelationshipType RelationshipType { get; set; }

    public bool IsActive { get; set; } = true;

    public Student? Student { get; set; }
}
