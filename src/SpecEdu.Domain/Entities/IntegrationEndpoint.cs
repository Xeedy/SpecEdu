using SpecEdu.Domain.Common;
using SpecEdu.Domain.Enums;

namespace SpecEdu.Domain.Entities;

/// <summary>
/// Represents a configured integration endpoint for PPP/SPC data warehouse connectivity.
/// Czech: Konfigurovaný integrační endpoint pro propojení s datovými sklady PPP/SPC
/// </summary>
public class IntegrationEndpoint : AuditableEntity
{
    /// <summary>
    /// Display name of this endpoint configuration.
    /// Czech: Název integračního bodu
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Type of external system (PPP or SPC).
    /// Czech: Typ externího systému
    /// </summary>
    public ExternalSystemType SystemType { get; set; }

    /// <summary>
    /// Base URL of the external API. Null until spec is available.
    /// Czech: Základní URL externího API
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// Placeholder for API key. TODO: Replace with secrets manager before production.
    /// Czech: Zástupný API klíč (přesunout do secrets manageru před nasazením)
    /// </summary>
    public string? ApiKeyPlaceholder { get; set; }

    /// <summary>
    /// Whether this endpoint is currently active.
    /// Czech: Zda je endpoint aktivní
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// When the connection was last tested.
    /// Czech: Poslední test připojení
    /// </summary>
    public DateTime? LastTestedAt { get; set; }

    /// <summary>
    /// Result message of the last connection test.
    /// Czech: Výsledek posledního testu připojení
    /// </summary>
    public string? LastTestResult { get; set; }

    // Navigation properties

    /// <summary>
    /// Exchange records associated with this endpoint.
    /// Czech: Záznamy výměny dat pro tento endpoint
    /// </summary>
    public ICollection<DataExchangeRecord> ExchangeRecords { get; set; } = new List<DataExchangeRecord>();
}
