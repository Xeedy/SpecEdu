using SpecEdu.Domain.Common;

namespace SpecEdu.Domain.Entities;

public class PlppEvaluation : AuditableEntity
{
    public Guid PlppId { get; set; }

    public DateTime EvaluationMonth { get; set; }

    public string? WhatStudentManages { get; set; }

    public string? WhatNeedsImprovement { get; set; }

    public string? RecommendedAdjustments { get; set; }

    public string? ParentConsultationNotes { get; set; }

    public int? ProgressRating { get; set; }

    public string? Notes { get; set; }

    public bool ParentsNotified { get; set; } = false;

    public DateTime? ParentsNotifiedAt { get; set; }

    public bool IsActive { get; set; } = true;

    public Plpp? Plpp { get; set; }
}
