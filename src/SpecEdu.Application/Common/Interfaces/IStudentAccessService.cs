using SpecEdu.Application.Common.Models;

namespace SpecEdu.Application.Common.Interfaces;

/// <summary>
/// CRITICAL SERVICE: Relationship-based access control for students.
/// This service is the core of the access control system.
/// Role = what you CAN do, Relationship = on WHOM you can do it.
/// </summary>
public interface IStudentAccessService
{
    /// <summary>
    /// Checks if a user can access a specific student.
    /// This is the CORE access check method.
    ///
    /// Access Control Logic:
    /// 1. Admin → FullAccess (global)
    /// 2. User.SchoolId != Student.SchoolId → NoAccess
    /// 3. SchoolAdmin → FullAccess (within school)
    /// 4. StudentGuardian exists → ReadOnly
    /// 5. StudentStaffLink exists → AccessLevel from link
    /// 6. Otherwise → NoAccess
    /// </summary>
    /// <param name="userId">The user's ID.</param>
    /// <param name="studentId">The student's ID.</param>
    /// <returns>Access result with CanAccess, CanEdit, and reason.</returns>
    Task<StudentAccessResult> CanAccessStudentAsync(string userId, Guid studentId);

    /// <summary>
    /// Gets all students that a user has access to.
    /// Uses the access control logic to filter students.
    /// </summary>
    /// <param name="userId">The user's ID.</param>
    /// <returns>List of accessible students.</returns>
    Task<IList<StudentDto>> GetAccessibleStudentsAsync(string userId);

    /// <summary>
    /// Gets all students linked to a parent via StudentGuardian relationship.
    /// Used specifically for the Parent role.
    /// </summary>
    /// <param name="parentUserId">The parent user's ID.</param>
    /// <returns>List of children (students).</returns>
    Task<IList<StudentDto>> GetStudentsForParentAsync(string parentUserId);

    /// <summary>
    /// Gets all students linked to a staff member via StudentStaffLink.
    /// Used specifically for staff roles (Teacher, SPP, PPP, SPC, Assistant).
    /// </summary>
    /// <param name="userId">The staff user's ID.</param>
    /// <returns>List of linked students.</returns>
    Task<IList<StudentDto>> GetStudentsForStaffAsync(string userId);

    /// <summary>
    /// Gets all students in a school (for SchoolAdmin or Admin).
    /// </summary>
    /// <param name="schoolId">The school's ID.</param>
    /// <param name="includeInactive">Whether to include inactive students.</param>
    /// <returns>List of students in the school.</returns>
    Task<IList<StudentDto>> GetStudentsForSchoolAsync(Guid schoolId, bool includeInactive = false);
}
