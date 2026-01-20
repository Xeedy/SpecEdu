using SpecEdu.Domain.Enums;

namespace SpecEdu.Application.Common.Models;

/// <summary>
/// Data transfer object for PLPP (Pedagogical Support Plan).
/// Czech: DTO pro Plán pedagogické podpory
/// </summary>
public class PlppDto
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public string? StudentName { get; set; }
    public string SchoolYear { get; set; } = string.Empty;
    public PlppStatus Status { get; set; }
    public SupportMeasureLevel SupportLevel { get; set; }
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
    public bool IsVisibleToParents { get; set; }
    public DateTime? ActivatedAt { get; set; }
    public string? ActivatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public bool IsActive { get; set; }

    /// <summary>
    /// Goals defined in this plan.
    /// </summary>
    public IList<PlppGoalDto> Goals { get; set; } = new List<PlppGoalDto>();

    /// <summary>
    /// Monthly evaluations of the plan.
    /// </summary>
    public IList<PlppEvaluationDto> Evaluations { get; set; } = new List<PlppEvaluationDto>();
}

/// <summary>
/// Lightweight DTO for PLPP list views.
/// </summary>
public class PlppListItemDto
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public string? StudentName { get; set; }
    public string SchoolYear { get; set; } = string.Empty;
    public PlppStatus Status { get; set; }
    public SupportMeasureLevel SupportLevel { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public int GoalCount { get; set; }
    public int EvaluationCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// DTO for creating a new PLPP.
/// </summary>
public class CreatePlppDto
{
    public Guid StudentId { get; set; }
    public string SchoolYear { get; set; } = string.Empty;
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
}

/// <summary>
/// DTO for updating an existing PLPP.
/// </summary>
public class UpdatePlppDto
{
    public Guid Id { get; set; }
    public string SchoolYear { get; set; } = string.Empty;
    public SupportMeasureLevel SupportLevel { get; set; }
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
    public bool IsVisibleToParents { get; set; }
}
