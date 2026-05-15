namespace SpecEdu.Domain.Enums;

/// <summary>
/// Direction of data exchange with an external system.
/// Czech: Směr výměny dat s externím systémem
/// </summary>
public enum DataExchangeDirection
{
    /// <summary>
    /// Data exported from SpecEdu to external system.
    /// Czech: Export dat ze SpecEdu do externího systému
    /// </summary>
    Export = 1,

    /// <summary>
    /// Data imported from external system into SpecEdu.
    /// Czech: Import dat z externího systému do SpecEdu
    /// </summary>
    Import = 2
}
