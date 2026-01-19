namespace SpecEdu.Domain.Enums;

/// <summary>
/// Defines the type of diary entry for a student.
/// Czech: Typ záznamu v deníku žáka
/// </summary>
public enum DiaryEntryType
{
    /// <summary>
    /// General note.
    /// Czech: Poznámka
    /// </summary>
    Note = 1,

    /// <summary>
    /// Phone call record.
    /// Czech: Telefonát
    /// </summary>
    PhoneCall = 2,

    /// <summary>
    /// Meeting record.
    /// Czech: Schůzka
    /// </summary>
    Meeting = 3,

    /// <summary>
    /// Parent collaboration record.
    /// Czech: Spolupráce s rodiči
    /// </summary>
    ParentCollaboration = 4,

    /// <summary>
    /// PPP/SPC collaboration record.
    /// Czech: Spolupráce s PPP/SPC
    /// </summary>
    PppSpcCollaboration = 5
}
