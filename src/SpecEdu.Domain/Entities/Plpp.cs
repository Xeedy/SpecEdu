using SpecEdu.Domain.Common;
using SpecEdu.Domain.Enums;

namespace SpecEdu.Domain.Entities;

/// <summary>
/// Plán pedagogické podpory (PLPP) - Pedagogical Support Plan.
/// A document created by schools for students requiring level 2 support measures.
/// Can be shared between teachers, special educators, and parents.
/// Czech: Plán pedagogické podpory pro žáka se SVP
/// </summary>
public class Plpp : AuditableEntity
{
    /// <summary>
    /// The student this plan belongs to.
    /// Czech: ID žáka
    /// </summary>
    public Guid StudentId { get; set; }

    /// <summary>
    /// School year for this plan (e.g., "2024/2025").
    /// Czech: Školní rok
    /// </summary>
    public string SchoolYear { get; set; } = string.Empty;

    /// <summary>
    /// Current status of the plan.
    /// Czech: Stav plánu
    /// </summary>
    public PlppStatus Status { get; set; } = PlppStatus.Draft;

    /// <summary>
    /// Level of support measures.
    /// Czech: Stupeň podpůrných opatření
    /// </summary>
    public SupportMeasureLevel SupportLevel { get; set; } = SupportMeasureLevel.Level2;

    /// <summary>
    /// Start date of the plan validity.
    /// Czech: Platnost od
    /// </summary>
    public DateTime ValidFrom { get; set; }

    /// <summary>
    /// End date of the plan validity.
    /// Czech: Platnost do
    /// </summary>
    public DateTime ValidTo { get; set; }

    /// <summary>
    /// Description of student's strengths.
    /// Czech: Silné stránky žáka
    /// </summary>
    public string? Strengths { get; set; }

    /// <summary>
    /// Description of areas needing support.
    /// Czech: Oblasti vyžadující podporu
    /// </summary>
    public string? AreasNeedingSupport { get; set; }

    /// <summary>
    /// Recommended teaching methods and approaches.
    /// Czech: Doporučené metody a přístupy
    /// </summary>
    public string? RecommendedMethods { get; set; }

    /// <summary>
    /// Organizational adjustments (seating, timing, etc.).
    /// Czech: Organizační úpravy
    /// </summary>
    public string? OrganizationalAdjustments { get; set; }

    /// <summary>
    /// Content adjustments to curriculum.
    /// Czech: Obsahové úpravy
    /// </summary>
    public string? ContentAdjustments { get; set; }

    /// <summary>
    /// Assessment and evaluation methods.
    /// Czech: Způsob hodnocení
    /// </summary>
    public string? AssessmentMethods { get; set; }

    /// <summary>
    /// Collaboration plan with parents.
    /// Czech: Spolupráce s rodiči
    /// </summary>
    public string? ParentCollaboration { get; set; }

    /// <summary>
    /// Notes visible only to school staff.
    /// Czech: Interní poznámky
    /// </summary>
    public string? InternalNotes { get; set; }

    /// <summary>
    /// Whether the plan is visible to parents.
    /// Czech: Viditelné pro rodiče
    /// </summary>
    public bool IsVisibleToParents { get; set; } = true;

    /// <summary>
    /// Date when the plan was activated.
    /// Czech: Datum aktivace
    /// </summary>
    public DateTime? ActivatedAt { get; set; }

    /// <summary>
    /// User ID who activated the plan.
    /// Czech: Aktivoval
    /// </summary>
    public string? ActivatedBy { get; set; }

    /// <summary>
    /// Soft delete flag.
    /// Czech: Aktivní záznam
    /// </summary>
    public bool IsActive { get; set; } = true;

    // Navigation properties

    /// <summary>
    /// The student this plan belongs to.
    /// </summary>
    public Student? Student { get; set; }

    /// <summary>
    /// Goals defined in this plan.
    /// Czech: Cíle plánu
    /// </summary>
    public ICollection<PlppGoal> Goals { get; set; } = new List<PlppGoal>();

    /// <summary>
    /// Monthly evaluations of the plan.
    /// Czech: Měsíční hodnocení
    /// </summary>
    public ICollection<PlppEvaluation> Evaluations { get; set; } = new List<PlppEvaluation>();

    /// <summary>
    /// Version history of this plan.
    /// Czech: Historie verzí plánu
    /// </summary>
    public ICollection<PlppVersion> Versions { get; set; } = new List<PlppVersion>();
}
