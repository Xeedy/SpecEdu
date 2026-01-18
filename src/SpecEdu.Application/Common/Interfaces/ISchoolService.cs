using SpecEdu.Application.Common.Models;

namespace SpecEdu.Application.Common.Interfaces;

public interface ISchoolService
{
    Task<SchoolDto?> GetByIdAsync(Guid id);

    Task<IList<SchoolDto>> GetAllAsync(bool includeInactive = false);

    Task<(IList<SchoolDto> Schools, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? searchTerm = null,
        bool includeInactive = false);

    Task<SchoolDto> CreateAsync(
        string name,
        string institutionType,
        string? ico = null,
        string? address = null,
        string? city = null,
        string? postalCode = null,
        string? contactEmail = null,
        string? contactPhone = null);

    Task<SchoolDto?> UpdateAsync(
        Guid id,
        string name,
        string institutionType,
        string? ico,
        string? address,
        string? city,
        string? postalCode,
        string? contactEmail,
        string? contactPhone,
        bool isActive,
        DateTime? licenseExpiresAt);

    Task<bool> DeleteAsync(Guid id);

    Task<bool> ExistsAsync(Guid id);

    Task<int> GetUserCountAsync(Guid id);
}
