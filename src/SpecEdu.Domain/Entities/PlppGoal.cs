using SpecEdu.Domain.Common;
using SpecEdu.Domain.Enums;

namespace SpecEdu.Domain.Entities;

/// <summary>
/// A specific goal within a PLPP.
/// Goals are SMART objectives that the student should achieve.
/// Czech: Konkrétní cíl v rámci PLPP
/// </summary>
public class PlppGoal : AuditableEntity
{
    /// <summary>
    /// The PLPP this goal belongs to.
    /// Czech: ID plánu
    /// </summary>
    public Guid PlppId { get; set; }

    /// <summary>
    /// Order/priority of the goal (1 = highest).
    /// Czech: Pořadí cíle
    /// </summary>
    public int Order { get; set; } = 1;

    /// <summary>
    /// Subject or area this goal relates to (e.g., "Matematika", "Čtení").
    /// Czech: Předmět/oblast
    /// </summary>
    public string? Subject { get; set; }

    /// <summary>
    /// Description of the goal.
    /// Czech: Popis cíle
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// How success will be measured (criteria).
    /// Czech: Kritéria úspěchu
    /// </summary>
    public string? SuccessCriteria { get; set; }

    /// <summary>
    /// Methods and strategies to achieve the goal.
    /// Czech: Metody a strategie
    /// </summary>
    public string? Methods { get; set; }

    /// <summary>
    /// Who is responsible for supporting this goal.
    /// Czech: Odpovědná osoba
    /// </summary>
    public string? ResponsiblePerson { get; set; }

    /// <summary>
    /// Target date for achieving the goal.
    /// Czech: Cílové datum
    /// </summary>
    public DateTime? TargetDate { get; set; }

    /// <summary>
    /// Current status of the goal.
    /// Czech: Stav cíle
    /// </summary>
    public GoalStatus Status { get; set; } = GoalStatus.NotStarted;

    /// <summary>
    /// Progress notes and updates.
    /// Czech: Poznámky k pokroku
    /// </summary>
    public string? ProgressNotes { get; set; }

    /// <summary>
    /// Date when the goal was completed.
    /// Czech: Datum dokončení
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Soft delete flag.
    /// </summary>
    public bool IsActive { get; set; } = true;

    // Navigation properties

    /// <summary>
    /// The PLPP this goal belongs to.
    /// </summary>
    public Plpp? Plpp { get; set; }
}
