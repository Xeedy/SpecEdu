using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Application.Common.Models;
using SpecEdu.Domain.Entities;
using SpecEdu.Domain.Enums;
using SpecEdu.Infrastructure.Data;

namespace SpecEdu.Infrastructure.Services;

public class ExternalIntegrationService(
    ApplicationDbContext context,
    ILogger<ExternalIntegrationService> logger) : IExternalIntegrationService
{
    public async Task<IList<IntegrationEndpointDto>> GetEndpointsAsync(bool? activeOnly = null)
    {
        var query = context.IntegrationEndpoints.AsQueryable();

        if (activeOnly.HasValue)
            query = query.Where(e => e.IsActive == activeOnly.Value);

        var endpoints = await query
            .OrderBy(e => e.SystemType)
            .ThenBy(e => e.Name)
            .ToListAsync();

        return endpoints.Select(MapToDto).ToList();
    }

    public async Task<IntegrationEndpointDto?> GetEndpointByIdAsync(Guid id)
    {
        var endpoint = await context.IntegrationEndpoints.FindAsync(id);
        return endpoint is null ? null : MapToDto(endpoint);
    }

    public async Task<IntegrationEndpointDto> CreateEndpointAsync(CreateIntegrationEndpointDto dto)
    {
        var endpoint = new IntegrationEndpoint
        {
            Name = dto.Name,
            SystemType = dto.SystemType,
            BaseUrl = dto.BaseUrl,
            ApiKeyPlaceholder = dto.ApiKey,
            IsActive = dto.IsActive
        };

        context.IntegrationEndpoints.Add(endpoint);
        await context.SaveChangesAsync();

        logger.LogInformation("Integration endpoint created: {Name} ({SystemType})", endpoint.Name, endpoint.SystemType);

        return MapToDto(endpoint);
    }

    public async Task<IntegrationEndpointDto?> UpdateEndpointAsync(UpdateIntegrationEndpointDto dto)
    {
        var endpoint = await context.IntegrationEndpoints.FindAsync(dto.Id);
        if (endpoint is null) return null;

        endpoint.Name = dto.Name;
        endpoint.SystemType = dto.SystemType;
        endpoint.BaseUrl = dto.BaseUrl;
        endpoint.IsActive = dto.IsActive;

        if (!string.IsNullOrWhiteSpace(dto.ApiKey))
            endpoint.ApiKeyPlaceholder = dto.ApiKey;

        await context.SaveChangesAsync();

        logger.LogInformation("Integration endpoint updated: {Name} ({Id})", endpoint.Name, endpoint.Id);

        return MapToDto(endpoint);
    }

    public async Task<bool> DeactivateEndpointAsync(Guid id)
    {
        var endpoint = await context.IntegrationEndpoints.FindAsync(id);
        if (endpoint is null) return false;

        endpoint.IsActive = false;
        await context.SaveChangesAsync();

        logger.LogInformation("Integration endpoint deactivated: {Name} ({Id})", endpoint.Name, endpoint.Id);

        return true;
    }

    public async Task<ConnectionTestResultDto> TestConnectionAsync(Guid endpointId)
    {
        var endpoint = await context.IntegrationEndpoints.FindAsync(endpointId);
        if (endpoint is null)
        {
            return new ConnectionTestResultDto
            {
                Success = false,
                Message = "Endpoint not found.",
                TestedAt = DateTime.UtcNow
            };
        }

        // STUB: Real HTTP calls will be added once the external API specification is available.
        logger.LogWarning(
            "Connection test requested for endpoint {Name} ({SystemType}), but external API integration is not yet configured.",
            endpoint.Name, endpoint.SystemType);

        var now = DateTime.UtcNow;

        // Write audit record
        var record = new DataExchangeRecord
        {
            EndpointId = endpointId,
            Direction = DataExchangeDirection.Export,
            EntityType = "ConnectionTest",
            Status = DataExchangeStatus.Failed,
            RequestSummary = $"Connection test to {endpoint.SystemType} endpoint '{endpoint.Name}'",
            ResponseSummary = "Integration not yet configured. External API specification is pending.",
            CreatedAt = now,
            CompletedAt = now
        };

        context.DataExchangeRecords.Add(record);

        endpoint.LastTestedAt = now;
        endpoint.LastTestResult = "Not configured — API spec pending";

        await context.SaveChangesAsync();

        return new ConnectionTestResultDto
        {
            Success = false,
            Message = "Integration not yet configured. External API specification is pending.",
            TestedAt = now
        };
    }

    public async Task<(IList<DataExchangeRecordDto> Records, int TotalCount)> GetExchangeRecordsAsync(
        Guid? endpointId = null,
        DataExchangeStatus? status = null,
        int page = 1,
        int pageSize = 20)
    {
        var query = context.DataExchangeRecords
            .Include(r => r.Endpoint)
            .AsQueryable();

        if (endpointId.HasValue)
            query = query.Where(r => r.EndpointId == endpointId.Value);

        if (status.HasValue)
            query = query.Where(r => r.Status == status.Value);

        var totalCount = await query.CountAsync();

        var records = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(r => new DataExchangeRecordDto
            {
                Id = r.Id,
                EndpointId = r.EndpointId,
                EndpointName = r.Endpoint != null ? r.Endpoint.Name : null,
                Direction = r.Direction,
                EntityType = r.EntityType,
                EntityId = r.EntityId,
                Status = r.Status,
                RequestSummary = r.RequestSummary,
                ResponseSummary = r.ResponseSummary,
                CreatedAt = r.CreatedAt,
                CompletedAt = r.CompletedAt,
                InitiatedBy = r.InitiatedBy
            })
            .ToListAsync();

        return (records, totalCount);
    }

    private static IntegrationEndpointDto MapToDto(IntegrationEndpoint endpoint)
    {
        return new IntegrationEndpointDto
        {
            Id = endpoint.Id,
            Name = endpoint.Name,
            SystemType = endpoint.SystemType,
            BaseUrl = endpoint.BaseUrl,
            ApiKeyMasked = MaskApiKey(endpoint.ApiKeyPlaceholder),
            IsActive = endpoint.IsActive,
            LastTestedAt = endpoint.LastTestedAt,
            LastTestResult = endpoint.LastTestResult,
            CreatedAt = endpoint.CreatedAt
        };
    }

    private static string? MaskApiKey(string? apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey)) return null;
        if (apiKey.Length <= 4) return new string('*', apiKey.Length);
        return new string('*', apiKey.Length - 4) + apiKey[^4..];
    }
}
