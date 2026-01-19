namespace SpecEdu.Domain.Enums;

/// <summary>
/// Defines the visibility level of a diary entry.
/// Czech: Viditelnost záznamu v deníku
/// </summary>
public enum DiaryVisibility
{
    /// <summary>
    /// Visible only to school staff (not to parents).
    /// Czech: Pouze pro školu
    /// </summary>
    SchoolOnly = 1,

    /// <summary>
    /// Visible to school staff and parents with guardian relationship.
    /// Czech: Viditelné pro rodiče
    /// </summary>
    ParentVisible = 2
}
