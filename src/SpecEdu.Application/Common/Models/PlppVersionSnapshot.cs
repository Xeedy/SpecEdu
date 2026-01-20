using SpecEdu.Domain.Enums;

namespace SpecEdu.Application.Common.Models;

/// <summary>
/// Complete snapshot of a PLPP state for version history.
/// Used for JSON serialization/deserialization.
/// Czech: Kompletní snímek stavu PLPP
/// </summary>
public class PlppVersionSnapshot
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

    /// <summary>
    /// Goals at the time of snapshot.
    /// </summary>
    public IList<PlppGoalSnapshot> Goals { get; set; } = new List<PlppGoalSnapshot>();

    /// <summary>
    /// Evaluations at the time of snapshot.
    /// </summary>
    public IList<PlppEvaluationSnapshot> Evaluations { get; set; } = new List<PlppEvaluationSnapshot>();
}

/// <summary>
/// Snapshot of a PLPP goal for version history.
/// Czech: Snímek cíle PLPP
/// </summary>
public class PlppGoalSnapshot
{
    public Guid Id { get; set; }
    public int Order { get; set; }
    public string? Subject { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? SuccessCriteria { get; set; }
    public string? Methods { get; set; }
    public string? ResponsiblePerson { get; set; }
    public DateTime? TargetDate { get; set; }
    public GoalStatus Status { get; set; }
    public string? ProgressNotes { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}

/// <summary>
/// Snapshot of a PLPP evaluation for version history.
/// Czech: Snímek hodnocení PLPP
/// </summary>
public class PlppEvaluationSnapshot
{
    public Guid Id { get; set; }
    public DateTime EvaluationMonth { get; set; }
    public string? WhatStudentManages { get; set; }
    public string? WhatNeedsImprovement { get; set; }
    public string? RecommendedAdjustments { get; set; }
    public string? ParentConsultationNotes { get; set; }
    public int? ProgressRating { get; set; }
    public string? Notes { get; set; }
    public bool ParentsNotified { get; set; }
    public DateTime? ParentsNotifiedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
