using SpecEdu.Application.Common.Models;
using SpecEdu.Domain.Enums;

namespace SpecEdu.Application.Common.Interfaces;

public interface IExternalIntegrationService
{
    Task<IList<IntegrationEndpointDto>> GetEndpointsAsync(bool? activeOnly = null);

    Task<IntegrationEndpointDto?> GetEndpointByIdAsync(Guid id);

    Task<IntegrationEndpointDto> CreateEndpointAsync(CreateIntegrationEndpointDto dto);

    Task<IntegrationEndpointDto?> UpdateEndpointAsync(UpdateIntegrationEndpointDto dto);

    Task<bool> DeactivateEndpointAsync(Guid id);

    Task<ConnectionTestResultDto> TestConnectionAsync(Guid endpointId);

    Task<(IList<DataExchangeRecordDto> Records, int TotalCount)> GetExchangeRecordsAsync(
        Guid? endpointId = null,
        DataExchangeStatus? status = null,
        int page = 1,
        int pageSize = 20);
}
