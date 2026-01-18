using SpecEdu.Application.Common.Models;

namespace SpecEdu.Application.Common.Interfaces;

public interface IStudentAccessService
{
    Task<StudentAccessResult> CanAccessStudentAsync(string userId, Guid studentId);

    Task<IList<StudentDto>> GetAccessibleStudentsAsync(string userId);

    Task<IList<StudentDto>> GetStudentsForParentAsync(string parentUserId);

    Task<IList<StudentDto>> GetStudentsForStaffAsync(string userId);

    Task<IList<StudentDto>> GetStudentsForSchoolAsync(Guid schoolId, bool includeInactive = false);
}
