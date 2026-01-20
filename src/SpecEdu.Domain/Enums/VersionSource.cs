namespace SpecEdu.Domain.Enums;

/// <summary>
/// Source/reason for creating a PLPP version.
/// Czech: Zdroj/důvod vytvoření verze PLPP
/// </summary>
public enum VersionSource
{
    /// <summary>
    /// Manual save by user.
    /// Czech: Manuální uložení
    /// </summary>
    ManualSave = 1,

    /// <summary>
    /// Plan was activated.
    /// Czech: Aktivace plánu
    /// </summary>
    Activation = 2,

    /// <summary>
    /// Plan was archived.
    /// Czech: Archivace plánu
    /// </summary>
    Archive = 3,

    /// <summary>
    /// Goal was changed.
    /// Czech: Změna cíle
    /// </summary>
    GoalChange = 4,

    /// <summary>
    /// Evaluation was changed.
    /// Czech: Změna hodnocení
    /// </summary>
    EvaluationChange = 5,

    /// <summary>
    /// Version restored from history.
    /// Czech: Obnovení z historie
    /// </summary>
    Restore = 6
}
