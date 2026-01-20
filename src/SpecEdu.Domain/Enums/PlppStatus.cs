namespace SpecEdu.Domain.Enums;

/// <summary>
/// Status of a Pedagogical Support Plan (PLPP).
/// Czech: Stav plánu pedagogické podpory
/// </summary>
public enum PlppStatus
{
    /// <summary>
    /// Plan is being drafted, not yet active.
    /// Czech: Rozpracováno
    /// </summary>
    Draft = 1,

    /// <summary>
    /// Plan is active and being implemented.
    /// Czech: Aktivní
    /// </summary>
    Active = 2,

    /// <summary>
    /// Plan is under review/evaluation.
    /// Czech: V revizi
    /// </summary>
    UnderReview = 3,

    /// <summary>
    /// Plan has been completed and archived.
    /// Czech: Archivováno
    /// </summary>
    Archived = 4
}
