namespace SpecEdu.Domain.Enums;

/// <summary>
/// Level of support measures (Podpůrná opatření) according to Czech legislation.
/// Czech: Stupeň podpůrných opatření
/// </summary>
public enum SupportMeasureLevel
{
    /// <summary>
    /// Level 1 - Minor adjustments, no formal documentation required.
    /// Czech: 1. stupeň - drobné úpravy
    /// </summary>
    Level1 = 1,

    /// <summary>
    /// Level 2 - PLPP required, school-based support.
    /// Czech: 2. stupeň - vyžaduje PLPP
    /// </summary>
    Level2 = 2,

    /// <summary>
    /// Level 3 - IVP required, PPP/SPC recommendation.
    /// Czech: 3. stupeň - vyžaduje IVP
    /// </summary>
    Level3 = 3,

    /// <summary>
    /// Level 4 - Significant support, assistant may be needed.
    /// Czech: 4. stupeň - významná podpora
    /// </summary>
    Level4 = 4,

    /// <summary>
    /// Level 5 - Highest level of support, special school placement possible.
    /// Czech: 5. stupeň - nejvyšší podpora
    /// </summary>
    Level5 = 5
}
