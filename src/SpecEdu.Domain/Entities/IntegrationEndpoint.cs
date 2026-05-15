using SpecEdu.Domain.Common;
using SpecEdu.Domain.Enums;

namespace SpecEdu.Domain.Entities;

public class IntegrationEndpoint : AuditableEntity
{
    public string Name { get; set; } = string.Empty;

    public ExternalSystemType SystemType { get; set; }

    public string? BaseUrl { get; set; }

    public string? ApiKeyPlaceholder { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime? LastTestedAt { get; set; }

    public string? LastTestResult { get; set; }

    public ICollection<DataExchangeRecord> ExchangeRecords { get; set; } = new List<DataExchangeRecord>();
}
