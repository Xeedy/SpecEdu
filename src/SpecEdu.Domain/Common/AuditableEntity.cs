namespace SpecEdu.Domain.Common;

public abstract class AuditableEntity : BaseEntity, IAuditableEntity
{
    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public string? ModifiedBy { get; set; }
}
