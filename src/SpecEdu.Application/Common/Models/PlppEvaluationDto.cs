namespace SpecEdu.Application.Common.Models;

/// <summary>
/// Data transfer object for PLPP monthly evaluations.
/// Czech: DTO pro měsíční hodnocení PLPP
/// </summary>
public class PlppEvaluationDto
{
    public Guid Id { get; set; }
    public Guid PlppId { get; set; }
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
    public bool IsActive { get; set; }
}

/// <summary>
/// DTO for creating a new PLPP evaluation.
/// </summary>
public class CreatePlppEvaluationDto
{
    public Guid PlppId { get; set; }
    public DateTime EvaluationMonth { get; set; }
    public string? WhatStudentManages { get; set; }
    public string? WhatNeedsImprovement { get; set; }
    public string? RecommendedAdjustments { get; set; }
    public string? ParentConsultationNotes { get; set; }
    public int? ProgressRating { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// DTO for updating an existing PLPP evaluation.
/// </summary>
public class UpdatePlppEvaluationDto
{
    public Guid Id { get; set; }
    public string? WhatStudentManages { get; set; }
    public string? WhatNeedsImprovement { get; set; }
    public string? RecommendedAdjustments { get; set; }
    public string? ParentConsultationNotes { get; set; }
    public int? ProgressRating { get; set; }
    public string? Notes { get; set; }
    public bool ParentsNotified { get; set; }
}
