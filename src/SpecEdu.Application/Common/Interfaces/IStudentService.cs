using SpecEdu.Application.Common.Models;
using SpecEdu.Domain.Enums;

namespace SpecEdu.Application.Common.Interfaces;

/// <summary>
/// Service for student management operations.
/// Implements CRUD operations for students and their relationships.
/// </summary>
public interface IStudentService
{
    #region Student CRUD

    /// <summary>
    /// Gets a student by ID.
    /// </summary>
    /// <param name="id">The student's ID.</param>
    /// <returns>Student DTO or null if not found.</returns>
    Task<StudentDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// Gets all students for a school.
    /// </summary>
    /// <param name="schoolId">The school's ID.</param>
    /// <param name="includeInactive">Whether to include inactive students.</param>
    /// <returns>List of students.</returns>
    Task<IList<StudentDto>> GetBySchoolAsync(Guid schoolId, bool includeInactive = false);

    /// <summary>
    /// Gets students with pagination.
    /// </summary>
    /// <param name="schoolId">The school's ID.</param>
    /// <param name="page">Page number (1-based).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="searchTerm">Optional search term.</param>
    /// <param name="includeInactive">Whether to include inactive students.</param>
    /// <returns>Paginated list of students and total count.</returns>
    Task<(IList<StudentDto> Students, int TotalCount)> GetPagedAsync(
        Guid schoolId,
        int page,
        int pageSize,
        string? searchTerm = null,
        bool includeInactive = false);

    /// <summary>
    /// Creates a new student.
    /// </summary>
    /// <param name="schoolId">School ID.</param>
    /// <param name="firstName">First name.</param>
    /// <param name="lastName">Last name.</param>
    /// <param name="birthDate">Date of birth.</param>
    /// <param name="className">Class name.</param>
    /// <returns>Created student DTO.</returns>
    Task<StudentDto> CreateAsync(
        Guid schoolId,
        string firstName,
        string lastName,
        DateTime? birthDate = null,
        string? className = null);

    /// <summary>
    /// Updates an existing student.
    /// </summary>
    /// <param name="id">Student ID.</param>
    /// <param name="firstName">First name.</param>
    /// <param name="lastName">Last name.</param>
    /// <param name="birthDate">Date of birth.</param>
    /// <param name="className">Class name.</param>
    /// <param name="isActive">Active status.</param>
    /// <returns>Updated student DTO or null if not found.</returns>
    Task<StudentDto?> UpdateAsync(
        Guid id,
        string firstName,
        string lastName,
        DateTime? birthDate,
        string? className,
        bool isActive);

    /// <summary>
    /// Soft-deletes a student (sets IsActive to false).
    /// </summary>
    /// <param name="id">Student ID.</param>
    /// <returns>True if successful.</returns>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// Checks if a student exists.
    /// </summary>
    /// <param name="id">Student ID.</param>
    /// <returns>True if student exists.</returns>
    Task<bool> ExistsAsync(Guid id);

    #endregion

    #region Guardian Management

    /// <summary>
    /// Gets all guardians for a student.
    /// </summary>
    /// <param name="studentId">Student ID.</param>
    /// <returns>List of guardian relationships.</returns>
    Task<IList<StudentGuardianDto>> GetGuardiansAsync(Guid studentId);

    /// <summary>
    /// Adds a guardian to a student.
    /// </summary>
    /// <param name="studentId">Student ID.</param>
    /// <param name="parentUserId">Parent user ID.</param>
    /// <param name="relationshipType">Type of relationship.</param>
    /// <returns>Created guardian relationship DTO.</returns>
    Task<StudentGuardianDto> AddGuardianAsync(
        Guid studentId,
        string parentUserId,
        RelationshipType relationshipType);

    /// <summary>
    /// Updates a guardian relationship.
    /// </summary>
    /// <param name="id">Guardian relationship ID.</param>
    /// <param name="relationshipType">New relationship type.</param>
    /// <param name="isActive">Active status.</param>
    /// <returns>Updated guardian DTO or null if not found.</returns>
    Task<StudentGuardianDto?> UpdateGuardianAsync(
        Guid id,
        RelationshipType relationshipType,
        bool isActive);

    /// <summary>
    /// Removes a guardian from a student.
    /// </summary>
    /// <param name="id">Guardian relationship ID.</param>
    /// <returns>True if successful.</returns>
    Task<bool> RemoveGuardianAsync(Guid id);

    #endregion

    #region Staff Link Management

    /// <summary>
    /// Gets all staff links for a student.
    /// </summary>
    /// <param name="studentId">Student ID.</param>
    /// <returns>List of staff links.</returns>
    Task<IList<StudentStaffLinkDto>> GetStaffLinksAsync(Guid studentId);

    /// <summary>
    /// Adds a staff link to a student.
    /// </summary>
    /// <param name="studentId">Student ID.</param>
    /// <param name="userId">Staff user ID.</param>
    /// <param name="linkType">Type of link.</param>
    /// <param name="accessLevel">Access level.</param>
    /// <returns>Created staff link DTO.</returns>
    Task<StudentStaffLinkDto> AddStaffLinkAsync(
        Guid studentId,
        string userId,
        StaffLinkType linkType,
        AccessLevel accessLevel);

    /// <summary>
    /// Updates a staff link.
    /// </summary>
    /// <param name="id">Staff link ID.</param>
    /// <param name="linkType">New link type.</param>
    /// <param name="accessLevel">New access level.</param>
    /// <param name="isActive">Active status.</param>
    /// <returns>Updated staff link DTO or null if not found.</returns>
    Task<StudentStaffLinkDto?> UpdateStaffLinkAsync(
        Guid id,
        StaffLinkType linkType,
        AccessLevel accessLevel,
        bool isActive);

    /// <summary>
    /// Removes a staff link from a student.
    /// </summary>
    /// <param name="id">Staff link ID.</param>
    /// <returns>True if successful.</returns>
    Task<bool> RemoveStaffLinkAsync(Guid id);

    #endregion
}
