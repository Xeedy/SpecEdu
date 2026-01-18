using SpecEdu.Application.Common.Models;
using SpecEdu.Domain.Enums;

namespace SpecEdu.Application.Common.Interfaces;

public interface IStudentService
{
    #region Student CRUD

    Task<StudentDto?> GetByIdAsync(Guid id);

    Task<IList<StudentDto>> GetBySchoolAsync(Guid schoolId, bool includeInactive = false);

    Task<(IList<StudentDto> Students, int TotalCount)> GetPagedAsync(
        Guid schoolId,
        int page,
        int pageSize,
        string? searchTerm = null,
        bool includeInactive = false);

    Task<StudentDto> CreateAsync(
        Guid schoolId,
        string firstName,
        string lastName,
        DateTime? birthDate = null,
        string? className = null);

    Task<StudentDto?> UpdateAsync(
        Guid id,
        string firstName,
        string lastName,
        DateTime? birthDate,
        string? className,
        bool isActive);

    Task<bool> DeleteAsync(Guid id);

    Task<bool> ExistsAsync(Guid id);

    #endregion

    #region Guardian Management

    Task<IList<StudentGuardianDto>> GetGuardiansAsync(Guid studentId);

    Task<StudentGuardianDto> AddGuardianAsync(
        Guid studentId,
        string parentUserId,
        RelationshipType relationshipType);

    Task<StudentGuardianDto?> UpdateGuardianAsync(
        Guid id,
        RelationshipType relationshipType,
        bool isActive);

    Task<bool> RemoveGuardianAsync(Guid id);

    #endregion

    #region Staff Link Management

    Task<IList<StudentStaffLinkDto>> GetStaffLinksAsync(Guid studentId);

    Task<StudentStaffLinkDto> AddStaffLinkAsync(
        Guid studentId,
        string userId,
        StaffLinkType linkType,
        AccessLevel accessLevel);

    Task<StudentStaffLinkDto?> UpdateStaffLinkAsync(
        Guid id,
        StaffLinkType linkType,
        AccessLevel accessLevel,
        bool isActive);

    Task<bool> RemoveStaffLinkAsync(Guid id);

    #endregion
}
