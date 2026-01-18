namespace SpecEdu.Domain.Enums;

/// <summary>
/// Defines the level of access a user has to student data.
/// Czech: Úroveň přístupu k datům žáka
/// </summary>
public enum AccessLevel
{
    /// <summary>
    /// Read-only access to student data.
    /// Czech: Pouze čtení
    /// </summary>
    Read = 1,

    /// <summary>
    /// Full edit access to student data.
    /// Czech: Čtení a zápis
    /// </summary>
    Edit = 2
}
