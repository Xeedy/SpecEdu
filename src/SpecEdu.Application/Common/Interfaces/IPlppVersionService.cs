using SpecEdu.Application.Common.Models;
using SpecEdu.Domain.Enums;

namespace SpecEdu.Application.Common.Interfaces;

/// <summary>
/// Service interface for managing PLPP version history.
/// Czech: Rozhraní služby pro správu historie verzí PLPP
/// </summary>
public interface IPlppVersionService
{
    /// <summary>
    /// Creates a new version snapshot of the specified PLPP.
    /// Czech: Vytvoří nový snímek verze zadaného PLPP
    /// </summary>
    /// <param name="plppId">The PLPP to snapshot.</param>
    /// <param name="source">The reason for creating this version.</param>
    /// <param name="changeSummary">Optional description of what changed.</param>
    /// <returns>The created version.</returns>
    Task<PlppVersionDto> CreateVersionAsync(Guid plppId, VersionSource source, string? changeSummary = null);

    /// <summary>
    /// Gets a specific version by ID.
    /// Czech: Získá specifickou verzi podle ID
    /// </summary>
    Task<PlppVersionDto?> GetVersionByIdAsync(Guid versionId);

    /// <summary>
    /// Gets all versions for a PLPP, ordered by version number descending.
    /// Czech: Získá všechny verze PLPP seřazené sestupně podle čísla verze
    /// </summary>
    Task<IList<PlppVersionListItemDto>> GetVersionsAsync(Guid plppId);

    /// <summary>
    /// Gets the latest version number for a PLPP.
    /// Czech: Získá nejnovější číslo verze PLPP
    /// </summary>
    Task<int> GetLatestVersionNumberAsync(Guid plppId);

    /// <summary>
    /// Gets the snapshot data for a specific version.
    /// Czech: Získá data snímku pro specifickou verzi
    /// </summary>
    Task<PlppVersionSnapshot?> GetVersionSnapshotAsync(Guid versionId);

    /// <summary>
    /// Restores a PLPP to a previous version state.
    /// Creates a new version with source = Restore before applying changes.
    /// Czech: Obnoví PLPP do stavu předchozí verze
    /// </summary>
    /// <param name="versionId">The version to restore to.</param>
    /// <returns>The newly created version after restore.</returns>
    Task<PlppVersionDto> RestoreVersionAsync(Guid versionId);

    /// <summary>
    /// Compares two versions and returns a summary of differences.
    /// Czech: Porovná dvě verze a vrátí souhrn rozdílů
    /// </summary>
    Task<string> CompareVersionsAsync(Guid versionId1, Guid versionId2);
}
