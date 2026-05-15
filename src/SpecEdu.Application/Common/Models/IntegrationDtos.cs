using SpecEdu.Domain.Enums;

namespace SpecEdu.Application.Common.Models;

public class IntegrationEndpointDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ExternalSystemType SystemType { get; set; }
    public string? BaseUrl { get; set; }
    public string? ApiKeyMasked { get; set; }
    public bool IsActive { get; set; }
    public DateTime? LastTestedAt { get; set; }
    public string? LastTestResult { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateIntegrationEndpointDto
{
    public string Name { get; set; } = string.Empty;
    public ExternalSystemType SystemType { get; set; }
    public string? BaseUrl { get; set; }
    public string? ApiKey { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdateIntegrationEndpointDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ExternalSystemType SystemType { get; set; }
    public string? BaseUrl { get; set; }
    public string? ApiKey { get; set; }
    public bool IsActive { get; set; }
}

public class ConnectionTestResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime TestedAt { get; set; }
}

public class DataExchangeRecordDto
{
    public Guid Id { get; set; }
    public Guid EndpointId { get; set; }
    public string? EndpointName { get; set; }
    public DataExchangeDirection Direction { get; set; }
    public string? EntityType { get; set; }
    public Guid? EntityId { get; set; }
    public DataExchangeStatus Status { get; set; }
    public string? RequestSummary { get; set; }
    public string? ResponseSummary { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? InitiatedBy { get; set; }
}
