namespace SpecEdu.Domain.Enums;

/// <summary>
/// Status of a data exchange operation with an external system.
/// Czech: Stav operace výměny dat s externím systémem
/// </summary>
public enum DataExchangeStatus
{
    /// <summary>
    /// Exchange is queued and waiting to start.
    /// Czech: Čeká na zpracování
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Exchange is currently in progress.
    /// Czech: Probíhá
    /// </summary>
    InProgress = 2,

    /// <summary>
    /// Exchange completed successfully.
    /// Czech: Dokončeno
    /// </summary>
    Completed = 3,

    /// <summary>
    /// Exchange failed.
    /// Czech: Selhalo
    /// </summary>
    Failed = 4,

    /// <summary>
    /// Exchange partially completed with some errors.
    /// Czech: Částečně dokončeno
    /// </summary>
    PartiallyCompleted = 5
}
