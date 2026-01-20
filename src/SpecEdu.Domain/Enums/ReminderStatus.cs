namespace SpecEdu.Domain.Enums;

/// <summary>
/// Defines the status of a reminder notification.
/// Czech: Stav připomínky
/// </summary>
public enum ReminderStatus
{
    /// <summary>
    /// Reminder is waiting to be sent.
    /// Czech: Čeká na odeslání
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Reminder was successfully sent.
    /// Czech: Odesláno
    /// </summary>
    Sent = 2,

    /// <summary>
    /// Reminder failed to send.
    /// Czech: Chyba při odesílání
    /// </summary>
    Failed = 3,

    /// <summary>
    /// Reminder was cancelled and will not be sent.
    /// Czech: Zrušeno
    /// </summary>
    Cancelled = 4
}
