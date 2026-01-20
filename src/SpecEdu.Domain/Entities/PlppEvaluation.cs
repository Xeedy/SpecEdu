using SpecEdu.Domain.Common;

namespace SpecEdu.Domain.Entities;

/// <summary>
/// Monthly evaluation of a PLPP.
/// Records progress, what the student is managing, and what needs work.
/// Czech: Měsíční hodnocení PLPP
/// </summary>
public class PlppEvaluation : AuditableEntity
{
    /// <summary>
    /// The PLPP this evaluation belongs to.
    /// Czech: ID plánu
    /// </summary>
    public Guid PlppId { get; set; }

    /// <summary>
    /// Month and year of evaluation (stored as first day of month).
    /// Czech: Měsíc hodnocení
    /// </summary>
    public DateTime EvaluationMonth { get; set; }

    /// <summary>
    /// What the student is managing well.
    /// Czech: Co žák zvládá
    /// </summary>
    public string? WhatStudentManages { get; set; }

    /// <summary>
    /// What the student is struggling with.
    /// Czech: Co žák nezvládá / potřebuje podporu
    /// </summary>
    public string? WhatNeedsImprovement { get; set; }

    /// <summary>
    /// Recommended adjustments based on evaluation.
    /// Czech: Doporučené úpravy
    /// </summary>
    public string? RecommendedAdjustments { get; set; }

    /// <summary>
    /// Notes from parent consultation.
    /// Czech: Poznámky z konzultace s rodiči
    /// </summary>
    public string? ParentConsultationNotes { get; set; }

    /// <summary>
    /// Overall progress assessment (1-5 scale, 5 = excellent).
    /// Czech: Celkové hodnocení pokroku
    /// </summary>
    public int? ProgressRating { get; set; }

    /// <summary>
    /// Additional notes.
    /// Czech: Další poznámky
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Whether parents have reviewed this evaluation.
    /// Czech: Rodiče seznámeni
    /// </summary>
    public bool ParentsNotified { get; set; } = false;

    /// <summary>
    /// Date when parents were notified.
    /// Czech: Datum seznámení rodičů
    /// </summary>
    public DateTime? ParentsNotifiedAt { get; set; }

    /// <summary>
    /// Soft delete flag.
    /// </summary>
    public bool IsActive { get; set; } = true;

    // Navigation properties

    /// <summary>
    /// The PLPP this evaluation belongs to.
    /// </summary>
    public Plpp? Plpp { get; set; }
}
