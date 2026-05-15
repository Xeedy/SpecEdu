using SpecEdu.Domain.Common;
using SpecEdu.Domain.Enums;

namespace SpecEdu.Domain.Entities;

public class DataExchangeRecord : BaseEntity
{
    public Guid EndpointId { get; set; }

    public DataExchangeDirection Direction { get; set; }

    public string? EntityType { get; set; }

    public Guid? EntityId { get; set; }

    public DataExchangeStatus Status { get; set; }

    public string? RequestSummary { get; set; }

    public string? ResponseSummary { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? CompletedAt { get; set; }

    public string? InitiatedBy { get; set; }

    public IntegrationEndpoint? Endpoint { get; set; }
}
