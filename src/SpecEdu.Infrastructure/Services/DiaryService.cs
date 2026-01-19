using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Application.Common.Models;
using SpecEdu.Domain.Entities;
using SpecEdu.Domain.Enums;
using SpecEdu.Infrastructure.Data;
using SpecEdu.Infrastructure.Identity;

namespace SpecEdu.Infrastructure.Services;

public class DiaryService : IDiaryService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUserService _currentUserService;

    public DiaryService(
        ApplicationDbContext dbContext,
        UserManager<ApplicationUser> userManager,
        ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _currentUserService = currentUserService;
    }

    #region Diary Entry CRUD

    public async Task<DiaryEntryDto?> GetByIdAsync(Guid id, bool includeAttachments = false)
    {
        var query = _dbContext.DiaryEntries
            .Include(d => d.Student)
            .AsQueryable();

        if (includeAttachments)
        {
            query = query.Include(d => d.Attachments);
        }

        var entry = await query.FirstOrDefaultAsync(d => d.Id == id && d.IsActive);

        if (entry == null) return null;

        var dto = MapToDto(entry);

        var author = await _userManager.FindByIdAsync(entry.CreatedBy ?? "");
        if (author != null)
        {
            dto.CreatedByName = author.FullName;
        }

        if (includeAttachments)
        {
            dto.Attachments = entry.Attachments.Select(MapAttachmentToDto).ToList();
        }

        return dto;
    }

    public async Task<(IList<DiaryEntryDto> Entries, int TotalCount)> GetEntriesAsync(
        Guid studentId,
        int page,
        int pageSize,
        DiaryEntryType? type = null,
        string? authorId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        DiaryVisibility? visibilityFilter = null)
    {
        var query = _dbContext.DiaryEntries
            .Include(d => d.Student)
            .Include(d => d.Attachments)
            .Where(d => d.StudentId == studentId && d.IsActive);

        if (type.HasValue)
        {
            query = query.Where(d => d.Type == type.Value);
        }

        if (!string.IsNullOrEmpty(authorId))
        {
            query = query.Where(d => d.CreatedBy == authorId);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(d => d.CreatedAt >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(d => d.CreatedAt <= toDate.Value);
        }

        if (visibilityFilter.HasValue)
        {
            query = query.Where(d => d.Visibility == visibilityFilter.Value);
        }

        var totalCount = await query.CountAsync();

        var entries = await query
            .OrderByDescending(d => d.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var authorIds = entries
            .Where(e => !string.IsNullOrEmpty(e.CreatedBy))
            .Select(e => e.CreatedBy!)
            .Distinct()
            .ToList();

        var authorNames = new Dictionary<string, string>();
        foreach (var id in authorIds)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                authorNames[id] = user.FullName ?? user.Email ?? id;
            }
        }

        var dtos = entries.Select(e =>
        {
            var dto = MapToDto(e);
            if (!string.IsNullOrEmpty(e.CreatedBy) && authorNames.TryGetValue(e.CreatedBy, out var name))
            {
                dto.CreatedByName = name;
            }
            dto.AttachmentCount = e.Attachments.Count;
            return dto;
        }).ToList();

        return (dtos, totalCount);
    }

    public async Task<DiaryEntryDto> CreateAsync(
        Guid studentId,
        DiaryEntryType type,
        string title,
        string content,
        DiaryVisibility visibility,
        DateTime? occurredAt = null)
    {
        var entry = new DiaryEntry
        {
            StudentId = studentId,
            Type = type,
            Title = title,
            Content = content,
            Visibility = visibility,
            OccurredAt = occurredAt,
            IsActive = true
        };

        _dbContext.DiaryEntries.Add(entry);
        await _dbContext.SaveChangesAsync();

        var created = await _dbContext.DiaryEntries
            .Include(d => d.Student)
            .FirstAsync(d => d.Id == entry.Id);

        var dto = MapToDto(created);

        var author = await _userManager.FindByIdAsync(created.CreatedBy ?? "");
        if (author != null)
        {
            dto.CreatedByName = author.FullName;
        }

        return dto;
    }

    public async Task<DiaryEntryDto?> UpdateAsync(
        Guid id,
        DiaryEntryType type,
        string title,
        string content,
        DiaryVisibility visibility,
        DateTime? occurredAt)
    {
        var entry = await _dbContext.DiaryEntries
            .Include(d => d.Student)
            .FirstOrDefaultAsync(d => d.Id == id && d.IsActive);

        if (entry == null) return null;

        entry.Type = type;
        entry.Title = title;
        entry.Content = content;
        entry.Visibility = visibility;
        entry.OccurredAt = occurredAt;

        await _dbContext.SaveChangesAsync();

        var dto = MapToDto(entry);

        var author = await _userManager.FindByIdAsync(entry.CreatedBy ?? "");
        if (author != null)
        {
            dto.CreatedByName = author.FullName;
        }

        return dto;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entry = await _dbContext.DiaryEntries.FindAsync(id);
        if (entry == null) return false;

        entry.IsActive = false;
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> CanEditAsync(Guid entryId, string userId)
    {
        var entry = await _dbContext.DiaryEntries.FindAsync(entryId);
        if (entry == null) return false;

        return entry.CreatedBy == userId;
    }

    #endregion

    #region Attachments

    public async Task<IList<DiaryAttachmentDto>> GetAttachmentsAsync(Guid entryId)
    {
        var attachments = await _dbContext.DiaryAttachments
            .Where(a => a.DiaryEntryId == entryId)
            .OrderBy(a => a.CreatedAt)
            .ToListAsync();

        return attachments.Select(MapAttachmentToDto).ToList();
    }

    public async Task<(byte[] FileData, string ContentType, string FileName)?> GetAttachmentDataAsync(Guid attachmentId)
    {
        var attachment = await _dbContext.DiaryAttachments.FindAsync(attachmentId);
        if (attachment == null) return null;

        return (attachment.FileData, attachment.ContentType, attachment.FileName);
    }

    public async Task<DiaryAttachmentDto> AddAttachmentAsync(
        Guid entryId,
        string fileName,
        string contentType,
        byte[] fileData)
    {
        var attachment = new DiaryAttachment
        {
            DiaryEntryId = entryId,
            FileName = fileName,
            ContentType = contentType,
            FileData = fileData,
            FileSize = fileData.Length
        };

        _dbContext.DiaryAttachments.Add(attachment);
        await _dbContext.SaveChangesAsync();

        return MapAttachmentToDto(attachment);
    }

    public async Task<bool> RemoveAttachmentAsync(Guid attachmentId)
    {
        var attachment = await _dbContext.DiaryAttachments.FindAsync(attachmentId);
        if (attachment == null) return false;

        _dbContext.DiaryAttachments.Remove(attachment);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    #endregion

    #region Statistics

    public async Task<Dictionary<DiaryEntryType, int>> GetEntryCountsByTypeAsync(Guid studentId)
    {
        var counts = await _dbContext.DiaryEntries
            .Where(d => d.StudentId == studentId && d.IsActive)
            .GroupBy(d => d.Type)
            .Select(g => new { Type = g.Key, Count = g.Count() })
            .ToListAsync();

        return counts.ToDictionary(x => x.Type, x => x.Count);
    }

    #endregion

    #region Mapping

    private static DiaryEntryDto MapToDto(DiaryEntry entry)
    {
        return new DiaryEntryDto
        {
            Id = entry.Id,
            StudentId = entry.StudentId,
            StudentName = entry.Student?.FullName,
            Type = entry.Type,
            Title = entry.Title,
            Content = entry.Content,
            Visibility = entry.Visibility,
            OccurredAt = entry.OccurredAt,
            IsActive = entry.IsActive,
            CreatedAt = entry.CreatedAt,
            CreatedBy = entry.CreatedBy,
            ModifiedAt = entry.ModifiedAt
        };
    }

    private static DiaryAttachmentDto MapAttachmentToDto(DiaryAttachment attachment)
    {
        return new DiaryAttachmentDto
        {
            Id = attachment.Id,
            DiaryEntryId = attachment.DiaryEntryId,
            FileName = attachment.FileName,
            ContentType = attachment.ContentType,
            FileSize = attachment.FileSize,
            CreatedAt = attachment.CreatedAt
        };
    }

    #endregion
}
