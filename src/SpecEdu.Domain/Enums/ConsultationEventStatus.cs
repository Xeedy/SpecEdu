namespace SpecEdu.Domain.Enums;

/// <summary>
/// Status of a consultation event.
/// Czech: Stav konzultace
/// </summary>
public enum ConsultationEventStatus
{
    /// <summary>
    /// Event is scheduled.
    /// Czech: Naplánováno
    /// </summary>
    Scheduled = 1,

    /// <summary>
    /// Event is confirmed by all participants.
    /// Czech: Potvrzeno
    /// </summary>
    Confirmed = 2,

    /// <summary>
    /// Event was completed.
    /// Czech: Proběhlo
    /// </summary>
    Completed = 3,

    /// <summary>
    /// Event was cancelled.
    /// Czech: Zrušeno
    /// </summary>
    Cancelled = 4,

    /// <summary>
    /// Event was rescheduled.
    /// Czech: Přesunuto
    /// </summary>
    Rescheduled = 5
}
