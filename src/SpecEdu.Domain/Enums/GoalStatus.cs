namespace SpecEdu.Domain.Enums;

/// <summary>
/// Status of a goal within a pedagogical plan.
/// Czech: Stav cíle
/// </summary>
public enum GoalStatus
{
    /// <summary>
    /// Goal is not started yet.
    /// Czech: Nezahájeno
    /// </summary>
    NotStarted = 1,

    /// <summary>
    /// Goal is in progress.
    /// Czech: Probíhá
    /// </summary>
    InProgress = 2,

    /// <summary>
    /// Goal has been achieved.
    /// Czech: Splněno
    /// </summary>
    Achieved = 3,

    /// <summary>
    /// Goal was partially achieved.
    /// Czech: Částečně splněno
    /// </summary>
    PartiallyAchieved = 4,

    /// <summary>
    /// Goal was not achieved.
    /// Czech: Nesplněno
    /// </summary>
    NotAchieved = 5
}
