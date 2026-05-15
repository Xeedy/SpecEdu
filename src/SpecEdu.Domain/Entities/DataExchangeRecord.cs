using SpecEdu.Domain.Common;
using SpecEdu.Domain.Enums;

namespace SpecEdu.Domain.Entities;

/// <summary>
/// Append-only audit trail of data exchange operations with external systems.
/// Inherits from BaseEntity (not AuditableEntity) — this is an immutable log.
/// Czech: Neměnný protokol operací výměny dat s externími systémy
/// </summary>
public class DataExchangeRecord : BaseEntity
{
    /// <summary>
    /// ID of the integration endpoint used for this exchange.
    /// Czech: ID integračního endpointu
    /// </summary>
    public Guid EndpointId { get; set; }

    /// <summary>
    /// Direction of data flow (Export or Import).
    /// Czech: Směr toku dat
    /// </summary>
    public DataExchangeDirection Direction { get; set; }

    /// <summary>
    /// Type of entity being exchanged (e.g. "Plpp", "Student", "ConnectionTest").
    /// Czech: Typ entity ve výměně
    /// </summary>
    public string? EntityType { get; set; }

    /// <summary>
    /// ID of the specific entity being exchanged, if applicable.
    /// Czech: ID konkrétní entity
    /// </summary>
    public Guid? EntityId { get; set; }

    /// <summary>
    /// Current status of this exchange operation.
    /// Czech: Stav operace výměny
    /// </summary>
    public DataExchangeStatus Status { get; set; }

    /// <summary>
    /// Summary of the request sent (no PII).
    /// Czech: Shrnutí odeslaného požadavku (bez osobních údajů)
    /// </summary>
    public string? RequestSummary { get; set; }

    /// <summary>
    /// Summary of the response received (no PII).
    /// Czech: Shrnutí přijaté odpovědi (bez osobních údajů)
    /// </summary>
    public string? ResponseSummary { get; set; }

    /// <summary>
    /// UTC timestamp when this record was created.
    /// Czech: Čas vytvoření záznamu
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// UTC timestamp when the exchange completed (success or failure).
    /// Czech: Čas dokončení výměny
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// User ID of whoever initiated this exchange.
    /// Czech: ID uživatele, který výměnu zahájil
    /// </summary>
    public string? InitiatedBy { get; set; }

    // Navigation properties

    /// <summary>
    /// The integration endpoint used for this exchange.
    /// Czech: Integrační endpoint použitý pro tuto výměnu
    /// </summary>
    public IntegrationEndpoint? Endpoint { get; set; }
}
