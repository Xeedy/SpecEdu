using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Application.Common.Models;
using SpecEdu.Domain.Entities;
using SpecEdu.Domain.Enums;
using SpecEdu.Infrastructure.Data;
using SpecEdu.Infrastructure.Identity;

namespace SpecEdu.Infrastructure.Services;

public class PlppVersionService : IPlppVersionService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<PlppVersionService> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public PlppVersionService(
        ApplicationDbContext dbContext,
        ILogger<PlppVersionService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<PlppVersionDto> CreateVersionAsync(Guid plppId, VersionSource source, string? changeSummary = null)
    {
        var plpp = await _dbContext.Plpps
            .Include(p => p.Student)
            .Include(p => p.Goals.Where(g => g.IsActive).OrderBy(g => g.Order))
            .Include(p => p.Evaluations.Where(e => e.IsActive).OrderByDescending(e => e.EvaluationMonth))
            .FirstOrDefaultAsync(p => p.Id == plppId && p.IsActive);

        if (plpp == null)
        {
            throw new InvalidOperationException($"PLPP {plppId} not found");
        }

        var latestVersionNumber = await GetLatestVersionNumberAsync(plppId);
        var snapshot = CreateSnapshot(plpp);
        var snapshotJson = JsonSerializer.Serialize(snapshot, JsonOptions);

        var version = new PlppVersion
        {
            PlppId = plppId,
            VersionNumber = latestVersionNumber + 1,
            Snapshot = snapshotJson,
            ChangeSummary = changeSummary,
            Source = source
        };

        _dbContext.PlppVersions.Add(version);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation(
            "Created version {VersionNumber} for PLPP {PlppId} with source {Source}",
            version.VersionNumber, plppId, source);

        return await GetVersionByIdAsync(version.Id)
            ?? throw new InvalidOperationException("Failed to retrieve created version");
    }

    public async Task<PlppVersionDto?> GetVersionByIdAsync(Guid versionId)
    {
        var version = await _dbContext.PlppVersions
            .FirstOrDefaultAsync(v => v.Id == versionId);

        if (version == null)
        {
            return null;
        }

        var createdByName = await GetUserDisplayNameAsync(version.CreatedBy);
        var snapshot = DeserializeSnapshot(version.Snapshot);

        return new PlppVersionDto
        {
            Id = version.Id,
            PlppId = version.PlppId,
            VersionNumber = version.VersionNumber,
            ChangeSummary = version.ChangeSummary,
            Source = version.Source,
            CreatedAt = version.CreatedAt,
            CreatedBy = version.CreatedBy,
            CreatedByName = createdByName,
            Snapshot = snapshot
        };
    }

    public async Task<IList<PlppVersionListItemDto>> GetVersionsAsync(Guid plppId)
    {
        var versions = await _dbContext.PlppVersions
            .Where(v => v.PlppId == plppId)
            .OrderByDescending(v => v.VersionNumber)
            .ToListAsync();

        var result = new List<PlppVersionListItemDto>();

        foreach (var version in versions)
        {
            var createdByName = await GetUserDisplayNameAsync(version.CreatedBy);
            var snapshot = DeserializeSnapshot(version.Snapshot);

            result.Add(new PlppVersionListItemDto
            {
                Id = version.Id,
                PlppId = version.PlppId,
                VersionNumber = version.VersionNumber,
                ChangeSummary = version.ChangeSummary,
                Source = version.Source,
                CreatedAt = version.CreatedAt,
                CreatedBy = version.CreatedBy,
                CreatedByName = createdByName,
                PlppStatus = snapshot?.Status
            });
        }

        return result;
    }

    public async Task<int> GetLatestVersionNumberAsync(Guid plppId)
    {
        var maxVersion = await _dbContext.PlppVersions
            .Where(v => v.PlppId == plppId)
            .MaxAsync(v => (int?)v.VersionNumber);

        return maxVersion ?? 0;
    }

    public async Task<PlppVersionSnapshot?> GetVersionSnapshotAsync(Guid versionId)
    {
        var version = await _dbContext.PlppVersions
            .FirstOrDefaultAsync(v => v.Id == versionId);

        if (version == null)
        {
            return null;
        }

        return DeserializeSnapshot(version.Snapshot);
    }

    public async Task<PlppVersionDto> RestoreVersionAsync(Guid versionId)
    {
        var version = await _dbContext.PlppVersions
            .FirstOrDefaultAsync(v => v.Id == versionId);

        if (version == null)
        {
            throw new InvalidOperationException($"Version {versionId} not found");
        }

        var snapshot = DeserializeSnapshot(version.Snapshot);
        if (snapshot == null)
        {
            throw new InvalidOperationException($"Could not deserialize snapshot for version {versionId}");
        }

        var plpp = await _dbContext.Plpps
            .Include(p => p.Goals)
            .Include(p => p.Evaluations)
            .FirstOrDefaultAsync(p => p.Id == version.PlppId && p.IsActive);

        if (plpp == null)
        {
            throw new InvalidOperationException($"PLPP {version.PlppId} not found");
        }

        await CreateVersionAsync(version.PlppId, VersionSource.Restore,
            $"Stav před obnovením z verze {version.VersionNumber}");

        plpp.SchoolYear = snapshot.SchoolYear;
        plpp.Status = snapshot.Status;
        plpp.SupportLevel = snapshot.SupportLevel;
        plpp.ValidFrom = snapshot.ValidFrom;
        plpp.ValidTo = snapshot.ValidTo;
        plpp.Strengths = snapshot.Strengths;
        plpp.AreasNeedingSupport = snapshot.AreasNeedingSupport;
        plpp.RecommendedMethods = snapshot.RecommendedMethods;
        plpp.OrganizationalAdjustments = snapshot.OrganizationalAdjustments;
        plpp.ContentAdjustments = snapshot.ContentAdjustments;
        plpp.AssessmentMethods = snapshot.AssessmentMethods;
        plpp.ParentCollaboration = snapshot.ParentCollaboration;
        plpp.InternalNotes = snapshot.InternalNotes;
        plpp.IsVisibleToParents = snapshot.IsVisibleToParents;

        foreach (var goal in plpp.Goals.Where(g => g.IsActive))
        {
            goal.IsActive = false;
        }
        foreach (var eval in plpp.Evaluations.Where(e => e.IsActive))
        {
            eval.IsActive = false;
        }

        foreach (var goalSnapshot in snapshot.Goals)
        {
            var goal = new PlppGoal
            {
                PlppId = plpp.Id,
                Order = goalSnapshot.Order,
                Subject = goalSnapshot.Subject,
                Description = goalSnapshot.Description,
                SuccessCriteria = goalSnapshot.SuccessCriteria,
                Methods = goalSnapshot.Methods,
                ResponsiblePerson = goalSnapshot.ResponsiblePerson,
                TargetDate = goalSnapshot.TargetDate,
                Status = goalSnapshot.Status,
                ProgressNotes = goalSnapshot.ProgressNotes,
                CompletedAt = goalSnapshot.CompletedAt,
                IsActive = true
            };
            _dbContext.PlppGoals.Add(goal);
        }

        foreach (var evalSnapshot in snapshot.Evaluations)
        {
            var eval = new PlppEvaluation
            {
                PlppId = plpp.Id,
                EvaluationMonth = evalSnapshot.EvaluationMonth,
                WhatStudentManages = evalSnapshot.WhatStudentManages,
                WhatNeedsImprovement = evalSnapshot.WhatNeedsImprovement,
                RecommendedAdjustments = evalSnapshot.RecommendedAdjustments,
                ParentConsultationNotes = evalSnapshot.ParentConsultationNotes,
                ProgressRating = evalSnapshot.ProgressRating,
                Notes = evalSnapshot.Notes,
                ParentsNotified = evalSnapshot.ParentsNotified,
                ParentsNotifiedAt = evalSnapshot.ParentsNotifiedAt,
                IsActive = true
            };
            _dbContext.PlppEvaluations.Add(eval);
        }

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation(
            "Restored PLPP {PlppId} to version {VersionNumber}",
            version.PlppId, version.VersionNumber);

        return await CreateVersionAsync(version.PlppId, VersionSource.Restore,
            $"Obnoveno z verze {version.VersionNumber}");
    }

    public async Task<string> CompareVersionsAsync(Guid versionId1, Guid versionId2)
    {
        var snapshot1 = await GetVersionSnapshotAsync(versionId1);
        var snapshot2 = await GetVersionSnapshotAsync(versionId2);

        if (snapshot1 == null || snapshot2 == null)
        {
            return "Nelze porovnat - jedna z verzí nebyla nalezena.";
        }

        var differences = new List<string>();

        if (snapshot1.Status != snapshot2.Status)
            differences.Add($"Stav: {snapshot1.Status} → {snapshot2.Status}");

        if (snapshot1.SupportLevel != snapshot2.SupportLevel)
            differences.Add($"Stupeň podpory: {snapshot1.SupportLevel} → {snapshot2.SupportLevel}");

        if (snapshot1.SchoolYear != snapshot2.SchoolYear)
            differences.Add($"Školní rok: {snapshot1.SchoolYear} → {snapshot2.SchoolYear}");

        if (snapshot1.ValidFrom != snapshot2.ValidFrom)
            differences.Add($"Platnost od: {snapshot1.ValidFrom:d} → {snapshot2.ValidFrom:d}");

        if (snapshot1.ValidTo != snapshot2.ValidTo)
            differences.Add($"Platnost do: {snapshot1.ValidTo:d} → {snapshot2.ValidTo:d}");

        if (snapshot1.Strengths != snapshot2.Strengths)
            differences.Add("Změna v sekci: Silné stránky");

        if (snapshot1.AreasNeedingSupport != snapshot2.AreasNeedingSupport)
            differences.Add("Změna v sekci: Oblasti vyžadující podporu");

        if (snapshot1.RecommendedMethods != snapshot2.RecommendedMethods)
            differences.Add("Změna v sekci: Doporučené metody");

        if (snapshot1.OrganizationalAdjustments != snapshot2.OrganizationalAdjustments)
            differences.Add("Změna v sekci: Organizační úpravy");

        if (snapshot1.ContentAdjustments != snapshot2.ContentAdjustments)
            differences.Add("Změna v sekci: Obsahové úpravy");

        if (snapshot1.AssessmentMethods != snapshot2.AssessmentMethods)
            differences.Add("Změna v sekci: Způsob hodnocení");

        if (snapshot1.ParentCollaboration != snapshot2.ParentCollaboration)
            differences.Add("Změna v sekci: Spolupráce s rodiči");

        if (snapshot1.InternalNotes != snapshot2.InternalNotes)
            differences.Add("Změna v sekci: Interní poznámky");

        if (snapshot1.IsVisibleToParents != snapshot2.IsVisibleToParents)
            differences.Add($"Viditelnost pro rodiče: {snapshot1.IsVisibleToParents} → {snapshot2.IsVisibleToParents}");

        if (snapshot1.Goals.Count != snapshot2.Goals.Count)
            differences.Add($"Počet cílů: {snapshot1.Goals.Count} → {snapshot2.Goals.Count}");

        if (snapshot1.Evaluations.Count != snapshot2.Evaluations.Count)
            differences.Add($"Počet hodnocení: {snapshot1.Evaluations.Count} → {snapshot2.Evaluations.Count}");

        return differences.Count > 0
            ? string.Join("\n", differences)
            : "Žádné změny.";
    }

    private static PlppVersionSnapshot CreateSnapshot(Plpp plpp)
    {
        return new PlppVersionSnapshot
        {
            Id = plpp.Id,
            StudentId = plpp.StudentId,
            StudentName = plpp.Student?.FullName,
            SchoolYear = plpp.SchoolYear,
            Status = plpp.Status,
            SupportLevel = plpp.SupportLevel,
            ValidFrom = plpp.ValidFrom,
            ValidTo = plpp.ValidTo,
            Strengths = plpp.Strengths,
            AreasNeedingSupport = plpp.AreasNeedingSupport,
            RecommendedMethods = plpp.RecommendedMethods,
            OrganizationalAdjustments = plpp.OrganizationalAdjustments,
            ContentAdjustments = plpp.ContentAdjustments,
            AssessmentMethods = plpp.AssessmentMethods,
            ParentCollaboration = plpp.ParentCollaboration,
            InternalNotes = plpp.InternalNotes,
            IsVisibleToParents = plpp.IsVisibleToParents,
            ActivatedAt = plpp.ActivatedAt,
            ActivatedBy = plpp.ActivatedBy,
            CreatedAt = plpp.CreatedAt,
            ModifiedAt = plpp.ModifiedAt,
            Goals = plpp.Goals.Select(g => new PlppGoalSnapshot
            {
                Id = g.Id,
                Order = g.Order,
                Subject = g.Subject,
                Description = g.Description,
                SuccessCriteria = g.SuccessCriteria,
                Methods = g.Methods,
                ResponsiblePerson = g.ResponsiblePerson,
                TargetDate = g.TargetDate,
                Status = g.Status,
                ProgressNotes = g.ProgressNotes,
                CompletedAt = g.CompletedAt,
                CreatedAt = g.CreatedAt,
                ModifiedAt = g.ModifiedAt
            }).ToList(),
            Evaluations = plpp.Evaluations.Select(e => new PlppEvaluationSnapshot
            {
                Id = e.Id,
                EvaluationMonth = e.EvaluationMonth,
                WhatStudentManages = e.WhatStudentManages,
                WhatNeedsImprovement = e.WhatNeedsImprovement,
                RecommendedAdjustments = e.RecommendedAdjustments,
                ParentConsultationNotes = e.ParentConsultationNotes,
                ProgressRating = e.ProgressRating,
                Notes = e.Notes,
                ParentsNotified = e.ParentsNotified,
                ParentsNotifiedAt = e.ParentsNotifiedAt,
                CreatedAt = e.CreatedAt,
                ModifiedAt = e.ModifiedAt
            }).ToList()
        };
    }

    private static PlppVersionSnapshot? DeserializeSnapshot(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<PlppVersionSnapshot>(json, JsonOptions);
        }
        catch
        {
            return null;
        }
    }

    private async Task<string?> GetUserDisplayNameAsync(string? userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return null;
        }

        var user = await _dbContext.Users
            .OfType<ApplicationUser>()
            .FirstOrDefaultAsync(u => u.Id == userId);

        return user?.FullName ?? user?.Email;
    }
}
