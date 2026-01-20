using SpecEdu.Domain.Common;
using SpecEdu.Domain.Enums;

namespace SpecEdu.Domain.Entities;

public class Plpp : AuditableEntity
{
    public Guid StudentId { get; set; }

    public string SchoolYear { get; set; } = string.Empty;

    public PlppStatus Status { get; set; } = PlppStatus.Draft;

    public SupportMeasureLevel SupportLevel { get; set; } = SupportMeasureLevel.Level2;

    public DateTime ValidFrom { get; set; }

    public DateTime ValidTo { get; set; }

    public string? Strengths { get; set; }

    public string? AreasNeedingSupport { get; set; }

    public string? RecommendedMethods { get; set; }

    public string? OrganizationalAdjustments { get; set; }

    public string? ContentAdjustments { get; set; }

    public string? AssessmentMethods { get; set; }

    public string? ParentCollaboration { get; set; }

    public string? InternalNotes { get; set; }

    public bool IsVisibleToParents { get; set; } = true;

    public DateTime? ActivatedAt { get; set; }

    public string? ActivatedBy { get; set; }

    public bool IsActive { get; set; } = true;

    public Student? Student { get; set; }

    public ICollection<PlppGoal> Goals { get; set; } = new List<PlppGoal>();

    public ICollection<PlppEvaluation> Evaluations { get; set; } = new List<PlppEvaluation>();

    public ICollection<PlppVersion> Versions { get; set; } = new List<PlppVersion>();
}
