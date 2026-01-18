using SpecEdu.Application.Common.Models;

namespace SpecEdu.Application.Common.Interfaces;

/// <summary>
/// Service for audit logging operations.
/// Provides methods for logging and querying audit entries.
/// </summary>
public interface IAuditService
{
    /// <summary>
    /// Logs an action to the audit log.
    /// </summary>
    /// <param name="action">The action performed (Create, Update, Delete, View, etc.).</param>
    /// <param name="entityType">Type of entity affected.</param>
    /// <param name="entityId">ID of the entity affected.</param>
    /// <param name="studentId">Optional student ID if action relates to a student.</param>
    /// <param name="schoolId">Optional school ID where action occurred.</param>
    /// <param name="details">Optional additional details (JSON).</param>
    /// <returns>The created audit log entry.</returns>
    Task<AuditLogDto> LogAsync(
        string action,
        string entityType,
        string? entityId = null,
        Guid? studentId = null,
        Guid? schoolId = null,
        string? details = null);

    /// <summary>
    /// Logs a student-related action.
    /// Automatically captures student and school context.
    /// </summary>
    /// <param name="action">The action performed.</param>
    /// <param name="studentId">The student's ID.</param>
    /// <param name="details">Optional additional details.</param>
    /// <returns>The created audit log entry.</returns>
    Task<AuditLogDto> LogStudentActionAsync(
        string action,
        Guid studentId,
        string? details = null);

    /// <summary>
    /// Gets audit logs with filtering and pagination.
    /// </summary>
    /// <param name="page">Page number (1-based).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="userId">Optional filter by user ID.</param>
    /// <param name="studentId">Optional filter by student ID.</param>
    /// <param name="schoolId">Optional filter by school ID.</param>
    /// <param name="action">Optional filter by action type.</param>
    /// <param name="fromDate">Optional filter from date.</param>
    /// <param name="toDate">Optional filter to date.</param>
    /// <returns>Paginated list of audit logs and total count.</returns>
    Task<(IList<AuditLogDto> Logs, int TotalCount)> GetLogsAsync(
        int page,
        int pageSize,
        string? userId = null,
        Guid? studentId = null,
        Guid? schoolId = null,
        string? action = null,
        DateTime? fromDate = null,
        DateTime? toDate = null);

    /// <summary>
    /// Gets audit logs for a specific student.
    /// </summary>
    /// <param name="studentId">The student's ID.</param>
    /// <param name="page">Page number (1-based).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <returns>Paginated list of audit logs and total count.</returns>
    Task<(IList<AuditLogDto> Logs, int TotalCount)> GetStudentLogsAsync(
        Guid studentId,
        int page,
        int pageSize);

    /// <summary>
    /// Gets audit logs for a specific school.
    /// </summary>
    /// <param name="schoolId">The school's ID.</param>
    /// <param name="page">Page number (1-based).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="fromDate">Optional filter from date.</param>
    /// <param name="toDate">Optional filter to date.</param>
    /// <returns>Paginated list of audit logs and total count.</returns>
    Task<(IList<AuditLogDto> Logs, int TotalCount)> GetSchoolLogsAsync(
        Guid schoolId,
        int page,
        int pageSize,
        DateTime? fromDate = null,
        DateTime? toDate = null);
}
