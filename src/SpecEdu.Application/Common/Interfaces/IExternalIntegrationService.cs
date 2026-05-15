using SpecEdu.Application.Common.Models;
using SpecEdu.Domain.Enums;

namespace SpecEdu.Application.Common.Interfaces;

public interface IExternalIntegrationService
{
    /// <summary>
    /// Get all configured integration endpoints, optionally filtered by active status.
    /// </summary>
    Task<IList<IntegrationEndpointDto>> GetEndpointsAsync(bool? activeOnly = null);

    /// <summary>
    /// Get a single integration endpoint by ID.
    /// </summary>
    Task<IntegrationEndpointDto?> GetEndpointByIdAsync(Guid id);

    /// <summary>
    /// Create a new integration endpoint configuration.
    /// </summary>
    Task<IntegrationEndpointDto> CreateEndpointAsync(CreateIntegrationEndpointDto dto);

    /// <summary>
    /// Update an existing integration endpoint configuration.
    /// </summary>
    Task<IntegrationEndpointDto?> UpdateEndpointAsync(UpdateIntegrationEndpointDto dto);

    /// <summary>
    /// Deactivate an integration endpoint (soft delete).
    /// </summary>
    Task<bool> DeactivateEndpointAsync(Guid id);

    /// <summary>
    /// Test connectivity to an external system endpoint.
    /// Currently a stub — returns "not configured" until external API spec is available.
    /// </summary>
    Task<ConnectionTestResultDto> TestConnectionAsync(Guid endpointId);

    /// <summary>
    /// Get paginated exchange records, optionally filtered by endpoint and status.
    /// </summary>
    Task<(IList<DataExchangeRecordDto> Records, int TotalCount)> GetExchangeRecordsAsync(
        Guid? endpointId = null,
        DataExchangeStatus? status = null,
        int page = 1,
        int pageSize = 20);
}
