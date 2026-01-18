namespace SpecEdu.Domain.Enums;

/// <summary>
/// Defines the type of staff link between a user and a student.
/// Czech: Typ vazby zaměstnance na žáka
/// </summary>
public enum StaffLinkType
{
    /// <summary>
    /// Teacher/Pedagogue.
    /// Czech: Učitel / Pedagog
    /// </summary>
    Teacher = 1,

    /// <summary>
    /// Teaching assistant.
    /// Czech: Asistent pedagoga
    /// </summary>
    Assistant = 2,

    /// <summary>
    /// School counseling center staff.
    /// Czech: Školní poradenské pracoviště
    /// </summary>
    SPP = 3,

    /// <summary>
    /// Pedagogical-psychological counseling center staff.
    /// Czech: Pedagogicko-psychologická poradna
    /// </summary>
    PPP = 4,

    /// <summary>
    /// Special pedagogical center staff.
    /// Czech: Speciálně pedagogické centrum
    /// </summary>
    SPC = 5
}
