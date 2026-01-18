using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Application.Common.Models;
using SpecEdu.Domain.Entities;
using SpecEdu.Domain.Enums;
using SpecEdu.Infrastructure.Data;
using SpecEdu.Infrastructure.Identity;

namespace SpecEdu.Infrastructure.Services;

public class StudentService : IStudentService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public StudentService(
        ApplicationDbContext dbContext,
        UserManager<ApplicationUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    #region Student CRUD

    public async Task<StudentDto?> GetByIdAsync(Guid id)
    {
        var student = await _dbContext.Students
            .Include(s => s.School)
            .FirstOrDefaultAsync(s => s.Id == id);

        return student == null ? null : MapToDto(student);
    }

    public async Task<IList<StudentDto>> GetBySchoolAsync(Guid schoolId, bool includeInactive = false)
    {
        var query = _dbContext.Students
            .Include(s => s.School)
            .Where(s => s.SchoolId == schoolId);

        if (!includeInactive)
        {
            query = query.Where(s => s.IsActive);
        }

        var students = await query
            .OrderBy(s => s.LastName)
            .ThenBy(s => s.FirstName)
            .ToListAsync();

        return students.Select(MapToDto).ToList();
    }

    public async Task<(IList<StudentDto> Students, int TotalCount)> GetPagedAsync(
        Guid schoolId,
        int page,
        int pageSize,
        string? searchTerm = null,
        bool includeInactive = false)
    {
        var query = _dbContext.Students
            .Include(s => s.School)
            .Where(s => s.SchoolId == schoolId);

        if (!includeInactive)
        {
            query = query.Where(s => s.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(s =>
                s.FirstName.ToLower().Contains(term) ||
                s.LastName.ToLower().Contains(term) ||
                (s.Class != null && s.Class.ToLower().Contains(term)));
        }

        var totalCount = await query.CountAsync();

        var students = await query
            .OrderBy(s => s.LastName)
            .ThenBy(s => s.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (students.Select(MapToDto).ToList(), totalCount);
    }

    public async Task<StudentDto> CreateAsync(
        Guid schoolId,
        string firstName,
        string lastName,
        DateTime? birthDate = null,
        string? className = null)
    {
        var student = new Student
        {
            SchoolId = schoolId,
            FirstName = firstName,
            LastName = lastName,
            BirthDate = birthDate,
            Class = className,
            IsActive = true
        };

        _dbContext.Students.Add(student);
        await _dbContext.SaveChangesAsync();

        var created = await _dbContext.Students
            .Include(s => s.School)
            .FirstAsync(s => s.Id == student.Id);

        return MapToDto(created);
    }

    public async Task<StudentDto?> UpdateAsync(
        Guid id,
        string firstName,
        string lastName,
        DateTime? birthDate,
        string? className,
        bool isActive)
    {
        var student = await _dbContext.Students
            .Include(s => s.School)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (student == null) return null;

        student.FirstName = firstName;
        student.LastName = lastName;
        student.BirthDate = birthDate;
        student.Class = className;
        student.IsActive = isActive;

        await _dbContext.SaveChangesAsync();

        return MapToDto(student);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var student = await _dbContext.Students.FindAsync(id);
        if (student == null) return false;

        student.IsActive = false;
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _dbContext.Students.AnyAsync(s => s.Id == id);
    }

    #endregion

    #region Guardian Management

    public async Task<IList<StudentGuardianDto>> GetGuardiansAsync(Guid studentId)
    {
        var guardians = await _dbContext.StudentGuardians
            .Include(sg => sg.Student)
            .Where(sg => sg.StudentId == studentId)
            .ToListAsync();

        var dtos = new List<StudentGuardianDto>();
        foreach (var guardian in guardians)
        {
            var dto = MapGuardianToDto(guardian);

            var user = await _userManager.FindByIdAsync(guardian.ParentUserId);
            if (user != null)
            {
                dto.ParentName = user.FullName;
                dto.ParentEmail = user.Email;
            }

            dtos.Add(dto);
        }

        return dtos;
    }

    public async Task<StudentGuardianDto> AddGuardianAsync(
        Guid studentId,
        string parentUserId,
        RelationshipType relationshipType)
    {
        var guardian = new StudentGuardian
        {
            StudentId = studentId,
            ParentUserId = parentUserId,
            RelationshipType = relationshipType,
            IsActive = true
        };

        _dbContext.StudentGuardians.Add(guardian);
        await _dbContext.SaveChangesAsync();

        var created = await _dbContext.StudentGuardians
            .Include(sg => sg.Student)
            .FirstAsync(sg => sg.Id == guardian.Id);

        var dto = MapGuardianToDto(created);

        var user = await _userManager.FindByIdAsync(parentUserId);
        if (user != null)
        {
            dto.ParentName = user.FullName;
            dto.ParentEmail = user.Email;
        }

        return dto;
    }

    public async Task<StudentGuardianDto?> UpdateGuardianAsync(
        Guid id,
        RelationshipType relationshipType,
        bool isActive)
    {
        var guardian = await _dbContext.StudentGuardians
            .Include(sg => sg.Student)
            .FirstOrDefaultAsync(sg => sg.Id == id);

        if (guardian == null) return null;

        guardian.RelationshipType = relationshipType;
        guardian.IsActive = isActive;

        await _dbContext.SaveChangesAsync();

        var dto = MapGuardianToDto(guardian);

        var user = await _userManager.FindByIdAsync(guardian.ParentUserId);
        if (user != null)
        {
            dto.ParentName = user.FullName;
            dto.ParentEmail = user.Email;
        }

        return dto;
    }

    public async Task<bool> RemoveGuardianAsync(Guid id)
    {
        var guardian = await _dbContext.StudentGuardians.FindAsync(id);
        if (guardian == null) return false;

        _dbContext.StudentGuardians.Remove(guardian);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    #endregion

    #region Staff Link Management

    public async Task<IList<StudentStaffLinkDto>> GetStaffLinksAsync(Guid studentId)
    {
        var links = await _dbContext.StudentStaffLinks
            .Include(ssl => ssl.Student)
            .Where(ssl => ssl.StudentId == studentId)
            .ToListAsync();

        var dtos = new List<StudentStaffLinkDto>();
        foreach (var link in links)
        {
            var dto = MapStaffLinkToDto(link);

            var user = await _userManager.FindByIdAsync(link.UserId);
            if (user != null)
            {
                dto.UserName = user.FullName;
                dto.UserEmail = user.Email;
            }

            dtos.Add(dto);
        }

        return dtos;
    }

    public async Task<StudentStaffLinkDto> AddStaffLinkAsync(
        Guid studentId,
        string userId,
        StaffLinkType linkType,
        AccessLevel accessLevel)
    {
        var link = new StudentStaffLink
        {
            StudentId = studentId,
            UserId = userId,
            LinkType = linkType,
            AccessLevel = accessLevel,
            IsActive = true
        };

        _dbContext.StudentStaffLinks.Add(link);
        await _dbContext.SaveChangesAsync();

        var created = await _dbContext.StudentStaffLinks
            .Include(ssl => ssl.Student)
            .FirstAsync(ssl => ssl.Id == link.Id);

        var dto = MapStaffLinkToDto(created);

        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            dto.UserName = user.FullName;
            dto.UserEmail = user.Email;
        }

        return dto;
    }

    public async Task<StudentStaffLinkDto?> UpdateStaffLinkAsync(
        Guid id,
        StaffLinkType linkType,
        AccessLevel accessLevel,
        bool isActive)
    {
        var link = await _dbContext.StudentStaffLinks
            .Include(ssl => ssl.Student)
            .FirstOrDefaultAsync(ssl => ssl.Id == id);

        if (link == null) return null;

        link.LinkType = linkType;
        link.AccessLevel = accessLevel;
        link.IsActive = isActive;

        await _dbContext.SaveChangesAsync();

        var dto = MapStaffLinkToDto(link);

        var user = await _userManager.FindByIdAsync(link.UserId);
        if (user != null)
        {
            dto.UserName = user.FullName;
            dto.UserEmail = user.Email;
        }

        return dto;
    }

    public async Task<bool> RemoveStaffLinkAsync(Guid id)
    {
        var link = await _dbContext.StudentStaffLinks.FindAsync(id);
        if (link == null) return false;

        _dbContext.StudentStaffLinks.Remove(link);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    #endregion

    #region Mapping

    private static StudentDto MapToDto(Student student)
    {
        return new StudentDto
        {
            Id = student.Id,
            SchoolId = student.SchoolId,
            SchoolName = student.School?.Name,
            FirstName = student.FirstName,
            LastName = student.LastName,
            BirthDate = student.BirthDate,
            Class = student.Class,
            PhotoId = student.PhotoId,
            IsActive = student.IsActive,
            CreatedAt = student.CreatedAt
        };
    }

    private static StudentGuardianDto MapGuardianToDto(StudentGuardian guardian)
    {
        return new StudentGuardianDto
        {
            Id = guardian.Id,
            StudentId = guardian.StudentId,
            StudentName = guardian.Student?.FullName,
            ParentUserId = guardian.ParentUserId,
            RelationshipType = guardian.RelationshipType,
            IsActive = guardian.IsActive,
            CreatedAt = guardian.CreatedAt
        };
    }

    private static StudentStaffLinkDto MapStaffLinkToDto(StudentStaffLink link)
    {
        return new StudentStaffLinkDto
        {
            Id = link.Id,
            StudentId = link.StudentId,
            StudentName = link.Student?.FullName,
            UserId = link.UserId,
            LinkType = link.LinkType,
            AccessLevel = link.AccessLevel,
            IsActive = link.IsActive,
            CreatedAt = link.CreatedAt
        };
    }

    #endregion
}
