using SpecEdu.Application.Common.Models;

namespace SpecEdu.Application.Common.Interfaces;

/// <summary>
/// Service for school (tenant) management operations.
/// Implements CRUD operations for schools.
/// </summary>
public interface ISchoolService
{
    /// <summary>
    /// Gets a school by its unique identifier.
    /// </summary>
    /// <param name="id">The school's ID.</param>
    /// <returns>School DTO or null if not found.</returns>
    Task<SchoolDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// Gets all schools.
    /// </summary>
    /// <param name="includeInactive">Whether to include inactive schools.</param>
    /// <returns>List of all schools.</returns>
    Task<IList<SchoolDto>> GetAllAsync(bool includeInactive = false);

    /// <summary>
    /// Gets schools with pagination.
    /// </summary>
    /// <param name="page">Page number (1-based).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="searchTerm">Optional search term for filtering.</param>
    /// <param name="includeInactive">Whether to include inactive schools.</param>
    /// <returns>Paginated list of schools and total count.</returns>
    Task<(IList<SchoolDto> Schools, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? searchTerm = null,
        bool includeInactive = false);

    /// <summary>
    /// Creates a new school.
    /// </summary>
    /// <param name="name">School name.</param>
    /// <param name="institutionType">Type of institution.</param>
    /// <param name="ico">Optional IČO.</param>
    /// <param name="address">Optional address.</param>
    /// <param name="city">Optional city.</param>
    /// <param name="postalCode">Optional postal code.</param>
    /// <param name="contactEmail">Optional contact email.</param>
    /// <param name="contactPhone">Optional contact phone.</param>
    /// <returns>Created school DTO.</returns>
    Task<SchoolDto> CreateAsync(
        string name,
        string institutionType,
        string? ico = null,
        string? address = null,
        string? city = null,
        string? postalCode = null,
        string? contactEmail = null,
        string? contactPhone = null);

    /// <summary>
    /// Updates an existing school.
    /// </summary>
    /// <param name="id">School ID.</param>
    /// <param name="name">New name.</param>
    /// <param name="institutionType">New institution type.</param>
    /// <param name="ico">New IČO.</param>
    /// <param name="address">New address.</param>
    /// <param name="city">New city.</param>
    /// <param name="postalCode">New postal code.</param>
    /// <param name="contactEmail">New contact email.</param>
    /// <param name="contactPhone">New contact phone.</param>
    /// <param name="isActive">Whether school is active.</param>
    /// <param name="licenseExpiresAt">License expiration date.</param>
    /// <returns>Updated school DTO or null if not found.</returns>
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

    /// <summary>
    /// Deletes a school.
    /// Note: This is a soft delete that deactivates the school.
    /// </summary>
    /// <param name="id">School ID.</param>
    /// <returns>True if successful.</returns>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// Checks if a school exists.
    /// </summary>
    /// <param name="id">School ID.</param>
    /// <returns>True if school exists.</returns>
    Task<bool> ExistsAsync(Guid id);

    /// <summary>
    /// Gets the user count for a school.
    /// </summary>
    /// <param name="id">School ID.</param>
    /// <returns>Number of users in the school.</returns>
    Task<int> GetUserCountAsync(Guid id);
}
