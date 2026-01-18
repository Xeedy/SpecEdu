using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Application.Common.Models;
using SpecEdu.Domain.Entities;
using SpecEdu.Infrastructure.Data;

namespace SpecEdu.Infrastructure.Services;

public class AuditService : IAuditService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditService(
        ApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<AuditLogDto> LogAsync(
        string action,
        string entityType,
        string? entityId = null,
        Guid? studentId = null,
        Guid? schoolId = null,
        string? details = null)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        var auditLog = new AuditLog
        {
            UserId = _currentUserService.UserId,
            UserName = null,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            StudentId = studentId,
            SchoolId = schoolId,
            Timestamp = DateTime.UtcNow,
            IpAddress = GetIpAddress(httpContext),
            UserAgent = GetUserAgent(httpContext),
            Details = details
        };

        _dbContext.AuditLogs.Add(auditLog);
        await _dbContext.SaveChangesAsync();

        return MapToDto(auditLog);
    }

    public async Task<AuditLogDto> LogStudentActionAsync(
        string action,
        Guid studentId,
        string? details = null)
    {
        var student = await _dbContext.Students.FindAsync(studentId);

        return await LogAsync(
            action,
            "Student",
            studentId.ToString(),
            studentId,
            student?.SchoolId,
            details);
    }

    public async Task<(IList<AuditLogDto> Logs, int TotalCount)> GetLogsAsync(
        int page,
        int pageSize,
        string? userId = null,
        Guid? studentId = null,
        Guid? schoolId = null,
        string? action = null,
        DateTime? fromDate = null,
        DateTime? toDate = null)
    {
        var query = _dbContext.AuditLogs.AsQueryable();

        if (!string.IsNullOrEmpty(userId))
        {
            query = query.Where(al => al.UserId == userId);
        }

        if (studentId.HasValue)
        {
            query = query.Where(al => al.StudentId == studentId);
        }

        if (schoolId.HasValue)
        {
            query = query.Where(al => al.SchoolId == schoolId);
        }

        if (!string.IsNullOrEmpty(action))
        {
            query = query.Where(al => al.Action == action);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(al => al.Timestamp >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(al => al.Timestamp <= toDate.Value);
        }

        var totalCount = await query.CountAsync();

        var logs = await query
            .OrderByDescending(al => al.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var enrichedLogs = await EnrichLogsAsync(logs);

        return (enrichedLogs, totalCount);
    }

    public async Task<(IList<AuditLogDto> Logs, int TotalCount)> GetStudentLogsAsync(
        Guid studentId,
        int page,
        int pageSize)
    {
        return await GetLogsAsync(
            page,
            pageSize,
            studentId: studentId);
    }

    public async Task<(IList<AuditLogDto> Logs, int TotalCount)> GetSchoolLogsAsync(
        Guid schoolId,
        int page,
        int pageSize,
        DateTime? fromDate = null,
        DateTime? toDate = null)
    {
        return await GetLogsAsync(
            page,
            pageSize,
            schoolId: schoolId,
            fromDate: fromDate,
            toDate: toDate);
    }

    private async Task<IList<AuditLogDto>> EnrichLogsAsync(List<AuditLog> logs)
    {
        var dtos = logs.Select(MapToDto).ToList();

        var studentIds = logs
            .Where(l => l.StudentId.HasValue)
            .Select(l => l.StudentId!.Value)
            .Distinct()
            .ToList();

        var schoolIds = logs
            .Where(l => l.SchoolId.HasValue)
            .Select(l => l.SchoolId!.Value)
            .Distinct()
            .ToList();

        var students = await _dbContext.Students
            .Where(s => studentIds.Contains(s.Id))
            .ToDictionaryAsync(s => s.Id, s => s.FullName);

        var schools = await _dbContext.Schools
            .Where(s => schoolIds.Contains(s.Id))
            .ToDictionaryAsync(s => s.Id, s => s.Name);

        foreach (var dto in dtos)
        {
            if (dto.StudentId.HasValue && students.TryGetValue(dto.StudentId.Value, out var studentName))
            {
                dto.StudentName = studentName;
            }

            if (dto.SchoolId.HasValue && schools.TryGetValue(dto.SchoolId.Value, out var schoolName))
            {
                dto.SchoolName = schoolName;
            }
        }

        return dtos;
    }

    private static string? GetIpAddress(HttpContext? httpContext)
    {
        if (httpContext == null) return null;

        var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',').First().Trim();
        }

        return httpContext.Connection.RemoteIpAddress?.ToString();
    }

    private static string? GetUserAgent(HttpContext? httpContext)
    {
        if (httpContext == null) return null;

        var userAgent = httpContext.Request.Headers["User-Agent"].FirstOrDefault();

        if (!string.IsNullOrEmpty(userAgent) && userAgent.Length > 500)
        {
            return userAgent[..500];
        }

        return userAgent;
    }

    private static AuditLogDto MapToDto(AuditLog log)
    {
        return new AuditLogDto
        {
            Id = log.Id,
            UserId = log.UserId,
            UserName = log.UserName,
            Action = log.Action,
            EntityType = log.EntityType,
            EntityId = log.EntityId,
            StudentId = log.StudentId,
            SchoolId = log.SchoolId,
            Timestamp = log.Timestamp,
            IpAddress = log.IpAddress,
            UserAgent = log.UserAgent,
            Details = log.Details
        };
    }
}
