namespace SpecEdu.Domain.Enums;

/// <summary>
/// Response status of a consultation participant.
/// Czech: Stav odpovědi účastníka
/// </summary>
public enum ParticipantResponseStatus
{
    /// <summary>
    /// No response yet.
    /// Czech: Čeká na odpověď
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Participant accepted the invitation.
    /// Czech: Přijato
    /// </summary>
    Accepted = 2,

    /// <summary>
    /// Participant declined the invitation.
    /// Czech: Odmítnuto
    /// </summary>
    Declined = 3,

    /// <summary>
    /// Participant might attend.
    /// Czech: Možná
    /// </summary>
    Tentative = 4
}
