using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Application.Common.Models;
using SpecEdu.Domain.Constants;
using SpecEdu.Domain.Entities;
using SpecEdu.Infrastructure.Data;
using SpecEdu.Infrastructure.Identity;

namespace SpecEdu.Infrastructure.Services;

/// <summary>
/// CRITICAL SERVICE: Implementation of relationship-based access control.
/// This is the core service for determining student access.
/// Role = what you CAN do, Relationship = on WHOM you can do it.
/// </summary>
public class StudentAccessService : IStudentAccessService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public StudentAccessService(
        ApplicationDbContext dbContext,
        UserManager<ApplicationUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    /// <summary>
    /// CORE ACCESS CHECK METHOD.
    ///
    /// Access Control Logic:
    /// 1. Admin → FullAccess (global)
    /// 2. User.SchoolId != Student.SchoolId → NoAccess
    /// 3. SchoolAdmin → FullAccess (within school)
    /// 4. StudentGuardian exists → ReadOnly
    /// 5. StudentStaffLink exists → AccessLevel from link
    /// 6. Otherwise → NoAccess
    /// </summary>
    public async Task<StudentAccessResult> CanAccessStudentAsync(string userId, Guid studentId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return StudentAccessResult.NoAccess("Uživatel nenalezen.");
        }

        var student = await _dbContext.Students.FindAsync(studentId);
        if (student == null)
        {
            return StudentAccessResult.NoAccess("Žák nenalezen.");
        }

        // 1. Check if user is Admin (global access)
        var isAdmin = await _userManager.IsInRoleAsync(user, Roles.Admin);
        if (isAdmin)
        {
            return StudentAccessResult.FullAccess(
                AccessReason.Admin,
                "Přístup udělen jako globální administrátor.");
        }

        // 2. Check school match (except for Admin who passed above)
        if (user.SchoolId != student.SchoolId)
        {
            return StudentAccessResult.NoAccess("Žák patří do jiné školy.");
        }

        // 3. Check if user is SchoolAdmin (within school)
        var isSchoolAdmin = await _userManager.IsInRoleAsync(user, Roles.SchoolAdmin);
        if (isSchoolAdmin)
        {
            return StudentAccessResult.FullAccess(
                AccessReason.SchoolAdmin,
                "Přístup udělen jako správce školy.");
        }

        // 4. Check guardian relationship (Parent role)
        var guardianLink = await _dbContext.StudentGuardians
            .FirstOrDefaultAsync(sg =>
                sg.StudentId == studentId &&
                sg.ParentUserId == userId &&
                sg.IsActive);

        if (guardianLink != null)
        {
            return StudentAccessResult.ReadOnly(
                AccessReason.Guardian,
                $"Přístup udělen jako {guardianLink.RelationshipType switch
                {
                    Domain.Enums.RelationshipType.Mother => "matka",
                    Domain.Enums.RelationshipType.Father => "otec",
                    Domain.Enums.RelationshipType.LegalGuardian => "zákonný zástupce",
                    _ => "příbuzný"
                }}.");
        }

        // 5. Check staff link
        var staffLink = await _dbContext.StudentStaffLinks
            .FirstOrDefaultAsync(ssl =>
                ssl.StudentId == studentId &&
                ssl.UserId == userId &&
                ssl.IsActive);

        if (staffLink != null)
        {
            return StudentAccessResult.FromAccessLevel(
                staffLink.AccessLevel,
                AccessReason.StaffLink,
                $"Přístup udělen jako {staffLink.LinkType switch
                {
                    Domain.Enums.StaffLinkType.Teacher => "učitel",
                    Domain.Enums.StaffLinkType.Assistant => "asistent",
                    Domain.Enums.StaffLinkType.SPP => "pracovník ŠPP",
                    Domain.Enums.StaffLinkType.PPP => "pracovník PPP",
                    Domain.Enums.StaffLinkType.SPC => "pracovník SPC",
                    _ => "zaměstnanec"
                }}.");
        }

        // 6. No access
        return StudentAccessResult.NoAccess("Nemáte oprávnění k přístupu k tomuto žákovi.");
    }

    public async Task<IList<StudentDto>> GetAccessibleStudentsAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new List<StudentDto>();
        }

        // Admin can see all students
        var isAdmin = await _userManager.IsInRoleAsync(user, Roles.Admin);
        if (isAdmin)
        {
            return await GetAllStudentsAsync();
        }

        // SchoolAdmin can see all students in their school
        var isSchoolAdmin = await _userManager.IsInRoleAsync(user, Roles.SchoolAdmin);
        if (isSchoolAdmin && user.SchoolId.HasValue)
        {
            return await GetStudentsForSchoolAsync(user.SchoolId.Value);
        }

        // Get students from guardian relationships
        var guardianStudents = await GetStudentsForParentAsync(userId);

        // Get students from staff links
        var staffStudents = await GetStudentsForStaffAsync(userId);

        // Combine and deduplicate
        var allStudents = guardianStudents
            .Concat(staffStudents)
            .GroupBy(s => s.Id)
            .Select(g => g.First())
            .OrderBy(s => s.LastName)
            .ThenBy(s => s.FirstName)
            .ToList();

        return allStudents;
    }

    public async Task<IList<StudentDto>> GetStudentsForParentAsync(string parentUserId)
    {
        var studentIds = await _dbContext.StudentGuardians
            .Where(sg => sg.ParentUserId == parentUserId && sg.IsActive)
            .Select(sg => sg.StudentId)
            .ToListAsync();

        if (!studentIds.Any())
        {
            return new List<StudentDto>();
        }

        var students = await _dbContext.Students
            .Include(s => s.School)
            .Where(s => studentIds.Contains(s.Id) && s.IsActive)
            .OrderBy(s => s.LastName)
            .ThenBy(s => s.FirstName)
            .ToListAsync();

        return students.Select(MapToDto).ToList();
    }

    public async Task<IList<StudentDto>> GetStudentsForStaffAsync(string userId)
    {
        var studentIds = await _dbContext.StudentStaffLinks
            .Where(ssl => ssl.UserId == userId && ssl.IsActive)
            .Select(ssl => ssl.StudentId)
            .ToListAsync();

        if (!studentIds.Any())
        {
            return new List<StudentDto>();
        }

        var students = await _dbContext.Students
            .Include(s => s.School)
            .Where(s => studentIds.Contains(s.Id) && s.IsActive)
            .OrderBy(s => s.LastName)
            .ThenBy(s => s.FirstName)
            .ToListAsync();

        return students.Select(MapToDto).ToList();
    }

    public async Task<IList<StudentDto>> GetStudentsForSchoolAsync(Guid schoolId, bool includeInactive = false)
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

    private async Task<IList<StudentDto>> GetAllStudentsAsync()
    {
        var students = await _dbContext.Students
            .Include(s => s.School)
            .Where(s => s.IsActive)
            .OrderBy(s => s.School!.Name)
            .ThenBy(s => s.LastName)
            .ThenBy(s => s.FirstName)
            .ToListAsync();

        return students.Select(MapToDto).ToList();
    }

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
}
