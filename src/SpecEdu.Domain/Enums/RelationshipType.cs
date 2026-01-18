namespace SpecEdu.Domain.Enums;

/// <summary>
/// Defines the type of relationship between a guardian and a student.
/// Czech: Typ vztahu mezi zákonným zástupcem a žákem
/// </summary>
public enum RelationshipType
{
    /// <summary>
    /// Mother of the student.
    /// Czech: Matka
    /// </summary>
    Mother = 1,

    /// <summary>
    /// Father of the student.
    /// Czech: Otec
    /// </summary>
    Father = 2,

    /// <summary>
    /// Legal guardian (not biological parent).
    /// Czech: Zákonný zástupce
    /// </summary>
    LegalGuardian = 3,

    /// <summary>
    /// Other relationship type.
    /// Czech: Jiný
    /// </summary>
    Other = 4
}
