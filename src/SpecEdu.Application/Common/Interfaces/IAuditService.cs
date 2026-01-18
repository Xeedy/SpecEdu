using SpecEdu.Application.Common.Models;

namespace SpecEdu.Application.Common.Interfaces;

public interface IAuditService
{
    Task<AuditLogDto> LogAsync(
        string action,
        string entityType,
        string? entityId = null,
        Guid? studentId = null,
        Guid? schoolId = null,
        string? details = null);

    Task<AuditLogDto> LogStudentActionAsync(
        string action,
        Guid studentId,
        string? details = null);

    Task<(IList<AuditLogDto> Logs, int TotalCount)> GetLogsAsync(
        int page,
        int pageSize,
        string? userId = null,
        Guid? studentId = null,
        Guid? schoolId = null,
        string? action = null,
        DateTime? fromDate = null,
        DateTime? toDate = null);

    Task<(IList<AuditLogDto> Logs, int TotalCount)> GetStudentLogsAsync(
        Guid studentId,
        int page,
        int pageSize);

    Task<(IList<AuditLogDto> Logs, int TotalCount)> GetSchoolLogsAsync(
        Guid schoolId,
        int page,
        int pageSize,
        DateTime? fromDate = null,
        DateTime? toDate = null);
}
