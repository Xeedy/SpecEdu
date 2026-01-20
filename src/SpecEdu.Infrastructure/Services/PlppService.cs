using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Application.Common.Models;
using SpecEdu.Domain.Entities;
using SpecEdu.Domain.Enums;
using SpecEdu.Infrastructure.Data;

namespace SpecEdu.Infrastructure.Services;

public class PlppService : IPlppService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<PlppService> _logger;
    private readonly Lazy<IPlppVersionService> _versionService;

    public PlppService(
        ApplicationDbContext dbContext,
        ILogger<PlppService> logger,
        Lazy<IPlppVersionService> versionService)
    {
        _dbContext = dbContext;
        _logger = logger;
        _versionService = versionService;
    }

    #region PLPP CRUD Operations

    public async Task<PlppDto> CreateAsync(CreatePlppDto dto)
    {
        var plpp = new Plpp
        {
            StudentId = dto.StudentId,
            SchoolYear = dto.SchoolYear,
            Status = PlppStatus.Draft,
            SupportLevel = dto.SupportLevel,
            ValidFrom = dto.ValidFrom,
            ValidTo = dto.ValidTo,
            Strengths = dto.Strengths,
            AreasNeedingSupport = dto.AreasNeedingSupport,
            RecommendedMethods = dto.RecommendedMethods,
            OrganizationalAdjustments = dto.OrganizationalAdjustments,
            ContentAdjustments = dto.ContentAdjustments,
            AssessmentMethods = dto.AssessmentMethods,
            ParentCollaboration = dto.ParentCollaboration,
            InternalNotes = dto.InternalNotes,
            IsVisibleToParents = dto.IsVisibleToParents,
            IsActive = true
        };

        _dbContext.Plpps.Add(plpp);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation(
            "Created PLPP {PlppId} for student {StudentId}, school year {SchoolYear}",
            plpp.Id, dto.StudentId, dto.SchoolYear);

        return await GetByIdAsync(plpp.Id) ?? throw new InvalidOperationException("Failed to retrieve created PLPP");
    }

    public async Task<PlppDto?> GetByIdAsync(Guid id)
    {
        var plpp = await _dbContext.Plpps
            .Include(p => p.Student)
            .Include(p => p.Goals.Where(g => g.IsActive).OrderBy(g => g.Order))
            .Include(p => p.Evaluations.Where(e => e.IsActive).OrderByDescending(e => e.EvaluationMonth))
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

        return plpp == null ? null : MapToDto(plpp);
    }

    public async Task<PlppListItemDto?> GetListItemByIdAsync(Guid id)
    {
        var plpp = await _dbContext.Plpps
            .Include(p => p.Student)
            .Include(p => p.Goals.Where(g => g.IsActive))
            .Include(p => p.Evaluations.Where(e => e.IsActive))
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

        return plpp == null ? null : MapToListItemDto(plpp);
    }

    public async Task<IList<PlppListItemDto>> GetByStudentIdAsync(Guid studentId)
    {
        var plpps = await _dbContext.Plpps
            .Include(p => p.Student)
            .Include(p => p.Goals.Where(g => g.IsActive))
            .Include(p => p.Evaluations.Where(e => e.IsActive))
            .Where(p => p.StudentId == studentId && p.IsActive)
            .OrderByDescending(p => p.SchoolYear)
            .ThenByDescending(p => p.CreatedAt)
            .ToListAsync();

        return plpps.Select(MapToListItemDto).ToList();
    }

    public async Task<IList<PlppListItemDto>> GetBySchoolIdAsync(Guid schoolId)
    {
        var plpps = await _dbContext.Plpps
            .Include(p => p.Student)
            .Include(p => p.Goals.Where(g => g.IsActive))
            .Include(p => p.Evaluations.Where(e => e.IsActive))
            .Where(p => p.Student!.SchoolId == schoolId && p.IsActive)
            .OrderByDescending(p => p.SchoolYear)
            .ThenBy(p => p.Student!.LastName)
            .ToListAsync();

        return plpps.Select(MapToListItemDto).ToList();
    }

    public async Task<IList<PlppListItemDto>> GetByStatusAsync(Guid schoolId, PlppStatus status)
    {
        var plpps = await _dbContext.Plpps
            .Include(p => p.Student)
            .Include(p => p.Goals.Where(g => g.IsActive))
            .Include(p => p.Evaluations.Where(e => e.IsActive))
            .Where(p => p.Student!.SchoolId == schoolId && p.Status == status && p.IsActive)
            .OrderByDescending(p => p.SchoolYear)
            .ThenBy(p => p.Student!.LastName)
            .ToListAsync();

        return plpps.Select(MapToListItemDto).ToList();
    }

    public async Task<IList<PlppListItemDto>> GetActiveAsync(Guid schoolId)
    {
        var now = DateTime.UtcNow;

        var plpps = await _dbContext.Plpps
            .Include(p => p.Student)
            .Include(p => p.Goals.Where(g => g.IsActive))
            .Include(p => p.Evaluations.Where(e => e.IsActive))
            .Where(p =>
                p.Student!.SchoolId == schoolId &&
                p.Status == PlppStatus.Active &&
                p.ValidFrom <= now &&
                p.ValidTo >= now &&
                p.IsActive)
            .OrderBy(p => p.Student!.LastName)
            .ToListAsync();

        return plpps.Select(MapToListItemDto).ToList();
    }

    public async Task<PlppDto?> UpdateAsync(UpdatePlppDto dto)
    {
        var plpp = await _dbContext.Plpps.FindAsync(dto.Id);

        if (plpp == null || !plpp.IsActive)
        {
            return null;
        }

        await _versionService.Value.CreateVersionAsync(dto.Id, VersionSource.ManualSave, "Ulozeni zmen");

        plpp.SchoolYear = dto.SchoolYear;
        plpp.SupportLevel = dto.SupportLevel;
        plpp.ValidFrom = dto.ValidFrom;
        plpp.ValidTo = dto.ValidTo;
        plpp.Strengths = dto.Strengths;
        plpp.AreasNeedingSupport = dto.AreasNeedingSupport;
        plpp.RecommendedMethods = dto.RecommendedMethods;
        plpp.OrganizationalAdjustments = dto.OrganizationalAdjustments;
        plpp.ContentAdjustments = dto.ContentAdjustments;
        plpp.AssessmentMethods = dto.AssessmentMethods;
        plpp.ParentCollaboration = dto.ParentCollaboration;
        plpp.InternalNotes = dto.InternalNotes;
        plpp.IsVisibleToParents = dto.IsVisibleToParents;

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Updated PLPP {PlppId}", dto.Id);

        return await GetByIdAsync(dto.Id);
    }

    public async Task<bool> ActivateAsync(Guid id, string activatedBy)
    {
        var plpp = await _dbContext.Plpps.FindAsync(id);

        if (plpp == null || !plpp.IsActive)
        {
            return false;
        }

        if (plpp.Status != PlppStatus.Draft)
        {
            _logger.LogWarning("Attempted to activate non-draft PLPP {PlppId}", id);
            return false;
        }

        await _versionService.Value.CreateVersionAsync(id, VersionSource.Activation, "Aktivace planu");

        plpp.Status = PlppStatus.Active;
        plpp.ActivatedAt = DateTime.UtcNow;
        plpp.ActivatedBy = activatedBy;

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Activated PLPP {PlppId} by {UserId}", id, activatedBy);

        return true;
    }

    public async Task<bool> ArchiveAsync(Guid id)
    {
        var plpp = await _dbContext.Plpps.FindAsync(id);

        if (plpp == null || !plpp.IsActive)
        {
            return false;
        }

        await _versionService.Value.CreateVersionAsync(id, VersionSource.Archive, "Archivace planu");

        plpp.Status = PlppStatus.Archived;

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Archived PLPP {PlppId}", id);

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var plpp = await _dbContext.Plpps.FindAsync(id);

        if (plpp == null)
        {
            return false;
        }

        plpp.IsActive = false;

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Soft deleted PLPP {PlppId}", id);

        return true;
    }

    #endregion

    #region Goal Operations

    public async Task<PlppGoalDto> AddGoalAsync(CreatePlppGoalDto dto)
    {
        var goal = new PlppGoal
        {
            PlppId = dto.PlppId,
            Order = dto.Order,
            Subject = dto.Subject,
            Description = dto.Description,
            SuccessCriteria = dto.SuccessCriteria,
            Methods = dto.Methods,
            ResponsiblePerson = dto.ResponsiblePerson,
            TargetDate = dto.TargetDate,
            Status = GoalStatus.NotStarted,
            IsActive = true
        };

        _dbContext.PlppGoals.Add(goal);
        await _dbContext.SaveChangesAsync();

        await _versionService.Value.CreateVersionAsync(dto.PlppId, VersionSource.GoalChange, $"Pridan cil: {dto.Description}");

        _logger.LogInformation("Added goal {GoalId} to PLPP {PlppId}", goal.Id, dto.PlppId);

        return MapGoalToDto(goal);
    }

    public async Task<PlppGoalDto?> UpdateGoalAsync(UpdatePlppGoalDto dto)
    {
        var goal = await _dbContext.PlppGoals.FindAsync(dto.Id);

        if (goal == null || !goal.IsActive)
        {
            return null;
        }

        goal.Order = dto.Order;
        goal.Subject = dto.Subject;
        goal.Description = dto.Description;
        goal.SuccessCriteria = dto.SuccessCriteria;
        goal.Methods = dto.Methods;
        goal.ResponsiblePerson = dto.ResponsiblePerson;
        goal.TargetDate = dto.TargetDate;
        goal.Status = dto.Status;
        goal.ProgressNotes = dto.ProgressNotes;

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Updated goal {GoalId}", dto.Id);

        return MapGoalToDto(goal);
    }

    public async Task<bool> UpdateGoalStatusAsync(Guid goalId, GoalStatus status, string? progressNotes = null)
    {
        var goal = await _dbContext.PlppGoals.FindAsync(goalId);

        if (goal == null || !goal.IsActive)
        {
            return false;
        }

        goal.Status = status;

        if (!string.IsNullOrEmpty(progressNotes))
        {
            goal.ProgressNotes = progressNotes;
        }

        if (status == GoalStatus.Achieved)
        {
            goal.CompletedAt = DateTime.UtcNow;
        }

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Updated goal {GoalId} status to {Status}", goalId, status);

        return true;
    }

    public async Task<bool> CompleteGoalAsync(Guid goalId, string? progressNotes = null)
    {
        return await UpdateGoalStatusAsync(goalId, GoalStatus.Achieved, progressNotes);
    }

    public async Task<bool> ReorderGoalsAsync(Guid plppId, IList<Guid> goalIdsInOrder)
    {
        var goals = await _dbContext.PlppGoals
            .Where(g => g.PlppId == plppId && g.IsActive)
            .ToListAsync();

        for (int i = 0; i < goalIdsInOrder.Count; i++)
        {
            var goal = goals.FirstOrDefault(g => g.Id == goalIdsInOrder[i]);
            if (goal != null)
            {
                goal.Order = i + 1;
            }
        }

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Reordered goals for PLPP {PlppId}", plppId);

        return true;
    }

    public async Task<bool> DeleteGoalAsync(Guid goalId)
    {
        var goal = await _dbContext.PlppGoals.FindAsync(goalId);

        if (goal == null)
        {
            return false;
        }

        goal.IsActive = false;

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Soft deleted goal {GoalId}", goalId);

        return true;
    }

    public async Task<IList<PlppGoalDto>> GetGoalsAsync(Guid plppId)
    {
        var goals = await _dbContext.PlppGoals
            .Where(g => g.PlppId == plppId && g.IsActive)
            .OrderBy(g => g.Order)
            .ToListAsync();

        return goals.Select(MapGoalToDto).ToList();
    }

    #endregion

    #region Evaluation Operations

    public async Task<PlppEvaluationDto> AddEvaluationAsync(CreatePlppEvaluationDto dto)
    {
        var evaluationMonth = new DateTime(dto.EvaluationMonth.Year, dto.EvaluationMonth.Month, 1);

        var evaluation = new PlppEvaluation
        {
            PlppId = dto.PlppId,
            EvaluationMonth = evaluationMonth,
            WhatStudentManages = dto.WhatStudentManages,
            WhatNeedsImprovement = dto.WhatNeedsImprovement,
            RecommendedAdjustments = dto.RecommendedAdjustments,
            ParentConsultationNotes = dto.ParentConsultationNotes,
            ProgressRating = dto.ProgressRating,
            Notes = dto.Notes,
            ParentsNotified = false,
            IsActive = true
        };

        _dbContext.PlppEvaluations.Add(evaluation);
        await _dbContext.SaveChangesAsync();

        await _versionService.Value.CreateVersionAsync(dto.PlppId, VersionSource.EvaluationChange,
            $"Pridano hodnoceni za {evaluationMonth:MM/yyyy}");

        _logger.LogInformation(
            "Added evaluation {EvaluationId} to PLPP {PlppId} for month {Month}",
            evaluation.Id, dto.PlppId, evaluationMonth.ToString("yyyy-MM"));

        return MapEvaluationToDto(evaluation);
    }

    public async Task<PlppEvaluationDto?> UpdateEvaluationAsync(UpdatePlppEvaluationDto dto)
    {
        var evaluation = await _dbContext.PlppEvaluations.FindAsync(dto.Id);

        if (evaluation == null || !evaluation.IsActive)
        {
            return null;
        }

        evaluation.WhatStudentManages = dto.WhatStudentManages;
        evaluation.WhatNeedsImprovement = dto.WhatNeedsImprovement;
        evaluation.RecommendedAdjustments = dto.RecommendedAdjustments;
        evaluation.ParentConsultationNotes = dto.ParentConsultationNotes;
        evaluation.ProgressRating = dto.ProgressRating;
        evaluation.Notes = dto.Notes;

        if (dto.ParentsNotified && !evaluation.ParentsNotified)
        {
            evaluation.ParentsNotified = true;
            evaluation.ParentsNotifiedAt = DateTime.UtcNow;
        }

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Updated evaluation {EvaluationId}", dto.Id);

        return MapEvaluationToDto(evaluation);
    }

    public async Task<bool> MarkParentsNotifiedAsync(Guid evaluationId)
    {
        var evaluation = await _dbContext.PlppEvaluations.FindAsync(evaluationId);

        if (evaluation == null || !evaluation.IsActive)
        {
            return false;
        }

        evaluation.ParentsNotified = true;
        evaluation.ParentsNotifiedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Marked parents notified for evaluation {EvaluationId}", evaluationId);

        return true;
    }

    public async Task<bool> DeleteEvaluationAsync(Guid evaluationId)
    {
        var evaluation = await _dbContext.PlppEvaluations.FindAsync(evaluationId);

        if (evaluation == null)
        {
            return false;
        }

        evaluation.IsActive = false;

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Soft deleted evaluation {EvaluationId}", evaluationId);

        return true;
    }

    public async Task<IList<PlppEvaluationDto>> GetEvaluationsAsync(Guid plppId)
    {
        var evaluations = await _dbContext.PlppEvaluations
            .Where(e => e.PlppId == plppId && e.IsActive)
            .OrderByDescending(e => e.EvaluationMonth)
            .ToListAsync();

        return evaluations.Select(MapEvaluationToDto).ToList();
    }

    public async Task<IList<PlppEvaluationDto>> GetEvaluationsInRangeAsync(Guid plppId, DateTime from, DateTime to)
    {
        var evaluations = await _dbContext.PlppEvaluations
            .Where(e =>
                e.PlppId == plppId &&
                e.IsActive &&
                e.EvaluationMonth >= from &&
                e.EvaluationMonth <= to)
            .OrderByDescending(e => e.EvaluationMonth)
            .ToListAsync();

        return evaluations.Select(MapEvaluationToDto).ToList();
    }

    #endregion

    #region Utility Operations

    public async Task<bool> HasActivePlppAsync(Guid studentId)
    {
        var now = DateTime.UtcNow;

        return await _dbContext.Plpps.AnyAsync(p =>
            p.StudentId == studentId &&
            p.Status == PlppStatus.Active &&
            p.ValidFrom <= now &&
            p.ValidTo >= now &&
            p.IsActive);
    }

    public string GetCurrentSchoolYear()
    {
        var now = DateTime.UtcNow;

        var year = now.Month >= 9 ? now.Year : now.Year - 1;
        return $"{year}/{year + 1}";
    }

    public async Task<PlppDto> DuplicateForNewYearAsync(Guid plppId, string newSchoolYear)
    {
        var source = await _dbContext.Plpps
            .Include(p => p.Goals.Where(g => g.IsActive))
            .FirstOrDefaultAsync(p => p.Id == plppId && p.IsActive);

        if (source == null)
        {
            throw new InvalidOperationException($"PLPP {plppId} not found");
        }

        var validFrom = source.ValidFrom.AddYears(1);
        var validTo = source.ValidTo.AddYears(1);

        var newPlpp = new Plpp
        {
            StudentId = source.StudentId,
            SchoolYear = newSchoolYear,
            Status = PlppStatus.Draft,
            SupportLevel = source.SupportLevel,
            ValidFrom = validFrom,
            ValidTo = validTo,
            Strengths = source.Strengths,
            AreasNeedingSupport = source.AreasNeedingSupport,
            RecommendedMethods = source.RecommendedMethods,
            OrganizationalAdjustments = source.OrganizationalAdjustments,
            ContentAdjustments = source.ContentAdjustments,
            AssessmentMethods = source.AssessmentMethods,
            ParentCollaboration = source.ParentCollaboration,
            InternalNotes = null,
            IsVisibleToParents = source.IsVisibleToParents,
            IsActive = true
        };

        _dbContext.Plpps.Add(newPlpp);
        await _dbContext.SaveChangesAsync();

        foreach (var sourceGoal in source.Goals)
        {
            var newGoal = new PlppGoal
            {
                PlppId = newPlpp.Id,
                Order = sourceGoal.Order,
                Subject = sourceGoal.Subject,
                Description = sourceGoal.Description,
                SuccessCriteria = sourceGoal.SuccessCriteria,
                Methods = sourceGoal.Methods,
                ResponsiblePerson = sourceGoal.ResponsiblePerson,
                TargetDate = sourceGoal.TargetDate?.AddYears(1),
                Status = GoalStatus.NotStarted,
                IsActive = true
            };

            _dbContext.PlppGoals.Add(newGoal);
        }

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation(
            "Duplicated PLPP {SourceId} to new PLPP {NewId} for school year {SchoolYear}",
            plppId, newPlpp.Id, newSchoolYear);

        return await GetByIdAsync(newPlpp.Id) ?? throw new InvalidOperationException("Failed to retrieve duplicated PLPP");
    }

    #endregion

    #region Private Mapping Methods

    private static PlppDto MapToDto(Plpp plpp)
    {
        return new PlppDto
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
            IsActive = plpp.IsActive,
            Goals = plpp.Goals.Select(MapGoalToDto).ToList(),
            Evaluations = plpp.Evaluations.Select(MapEvaluationToDto).ToList()
        };
    }

    private static PlppListItemDto MapToListItemDto(Plpp plpp)
    {
        return new PlppListItemDto
        {
            Id = plpp.Id,
            StudentId = plpp.StudentId,
            StudentName = plpp.Student?.FullName,
            SchoolYear = plpp.SchoolYear,
            Status = plpp.Status,
            SupportLevel = plpp.SupportLevel,
            ValidFrom = plpp.ValidFrom,
            ValidTo = plpp.ValidTo,
            GoalCount = plpp.Goals.Count,
            EvaluationCount = plpp.Evaluations.Count,
            CreatedAt = plpp.CreatedAt,
            IsActive = plpp.IsActive
        };
    }

    private static PlppGoalDto MapGoalToDto(PlppGoal goal)
    {
        return new PlppGoalDto
        {
            Id = goal.Id,
            PlppId = goal.PlppId,
            Order = goal.Order,
            Subject = goal.Subject,
            Description = goal.Description,
            SuccessCriteria = goal.SuccessCriteria,
            Methods = goal.Methods,
            ResponsiblePerson = goal.ResponsiblePerson,
            TargetDate = goal.TargetDate,
            Status = goal.Status,
            ProgressNotes = goal.ProgressNotes,
            CompletedAt = goal.CompletedAt,
            CreatedAt = goal.CreatedAt,
            ModifiedAt = goal.ModifiedAt,
            IsActive = goal.IsActive
        };
    }

    private static PlppEvaluationDto MapEvaluationToDto(PlppEvaluation evaluation)
    {
        return new PlppEvaluationDto
        {
            Id = evaluation.Id,
            PlppId = evaluation.PlppId,
            EvaluationMonth = evaluation.EvaluationMonth,
            WhatStudentManages = evaluation.WhatStudentManages,
            WhatNeedsImprovement = evaluation.WhatNeedsImprovement,
            RecommendedAdjustments = evaluation.RecommendedAdjustments,
            ParentConsultationNotes = evaluation.ParentConsultationNotes,
            ProgressRating = evaluation.ProgressRating,
            Notes = evaluation.Notes,
            ParentsNotified = evaluation.ParentsNotified,
            ParentsNotifiedAt = evaluation.ParentsNotifiedAt,
            CreatedAt = evaluation.CreatedAt,
            ModifiedAt = evaluation.ModifiedAt,
            IsActive = evaluation.IsActive
        };
    }

    #endregion
}
