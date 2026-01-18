using Microsoft.EntityFrameworkCore;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Application.Common.Models;
using SpecEdu.Domain.Entities;
using SpecEdu.Infrastructure.Data;

namespace SpecEdu.Infrastructure.Services;

/// <summary>
/// Implementation of ISchoolService.
/// Handles school (tenant) CRUD operations.
/// </summary>
public class SchoolService : ISchoolService
{
    private readonly ApplicationDbContext _dbContext;

    public SchoolService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SchoolDto?> GetByIdAsync(Guid id)
    {
        var school = await _dbContext.Schools.FindAsync(id);
        return school == null ? null : MapToDto(school);
    }

    public async Task<IList<SchoolDto>> GetAllAsync(bool includeInactive = false)
    {
        var query = _dbContext.Schools.AsQueryable();

        if (!includeInactive)
        {
            query = query.Where(s => s.IsActive);
        }

        var schools = await query
            .OrderBy(s => s.Name)
            .ToListAsync();

        return schools.Select(MapToDto).ToList();
    }

    public async Task<(IList<SchoolDto> Schools, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? searchTerm = null,
        bool includeInactive = false)
    {
        var query = _dbContext.Schools.AsQueryable();

        if (!includeInactive)
        {
            query = query.Where(s => s.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(s =>
                s.Name.ToLower().Contains(term) ||
                (s.City != null && s.City.ToLower().Contains(term)) ||
                (s.Ico != null && s.Ico.Contains(term)));
        }

        var totalCount = await query.CountAsync();

        var schools = await query
            .OrderBy(s => s.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (schools.Select(MapToDto).ToList(), totalCount);
    }

    public async Task<SchoolDto> CreateAsync(
        string name,
        string institutionType,
        string? ico = null,
        string? address = null,
        string? city = null,
        string? postalCode = null,
        string? contactEmail = null,
        string? contactPhone = null)
    {
        var school = new School
        {
            Name = name,
            InstitutionType = institutionType,
            Ico = ico,
            Address = address,
            City = city,
            PostalCode = postalCode,
            ContactEmail = contactEmail,
            ContactPhone = contactPhone,
            IsActive = true
        };

        _dbContext.Schools.Add(school);
        await _dbContext.SaveChangesAsync();

        return MapToDto(school);
    }

    public async Task<SchoolDto?> UpdateAsync(
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
        DateTime? licenseExpiresAt)
    {
        var school = await _dbContext.Schools.FindAsync(id);
        if (school == null) return null;

        school.Name = name;
        school.InstitutionType = institutionType;
        school.Ico = ico;
        school.Address = address;
        school.City = city;
        school.PostalCode = postalCode;
        school.ContactEmail = contactEmail;
        school.ContactPhone = contactPhone;
        school.IsActive = isActive;
        school.LicenseExpiresAt = licenseExpiresAt;

        await _dbContext.SaveChangesAsync();

        return MapToDto(school);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var school = await _dbContext.Schools.FindAsync(id);
        if (school == null) return false;

        // Soft delete - just deactivate
        school.IsActive = false;
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _dbContext.Schools.AnyAsync(s => s.Id == id);
    }

    public async Task<int> GetUserCountAsync(Guid id)
    {
        return await _dbContext.Users.CountAsync(u => u.SchoolId == id);
    }

    private static SchoolDto MapToDto(School school)
    {
        return new SchoolDto
        {
            Id = school.Id,
            Name = school.Name,
            Ico = school.Ico,
            Address = school.Address,
            City = school.City,
            PostalCode = school.PostalCode,
            ContactEmail = school.ContactEmail,
            ContactPhone = school.ContactPhone,
            InstitutionType = school.InstitutionType,
            IsActive = school.IsActive,
            LicenseExpiresAt = school.LicenseExpiresAt,
            CreatedAt = school.CreatedAt
        };
    }
}
