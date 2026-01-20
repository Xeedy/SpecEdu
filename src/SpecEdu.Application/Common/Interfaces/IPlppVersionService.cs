using SpecEdu.Application.Common.Models;
using SpecEdu.Domain.Enums;

namespace SpecEdu.Application.Common.Interfaces;

public interface IPlppVersionService
{
    Task<PlppVersionDto> CreateVersionAsync(Guid plppId, VersionSource source, string? changeSummary = null);

    Task<PlppVersionDto?> GetVersionByIdAsync(Guid versionId);

    Task<IList<PlppVersionListItemDto>> GetVersionsAsync(Guid plppId);

    Task<int> GetLatestVersionNumberAsync(Guid plppId);

    Task<PlppVersionSnapshot?> GetVersionSnapshotAsync(Guid versionId);

    Task<PlppVersionDto> RestoreVersionAsync(Guid versionId);

    Task<string> CompareVersionsAsync(Guid versionId1, Guid versionId2);
}
