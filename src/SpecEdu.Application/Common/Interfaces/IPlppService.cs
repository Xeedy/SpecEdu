using SpecEdu.Application.Common.Models;
using SpecEdu.Domain.Enums;

namespace SpecEdu.Application.Common.Interfaces;

/// <summary>
/// Service interface for managing PLPP (Pedagogical Support Plans).
/// Czech: Rozhraní služby pro správu Plánů pedagogické podpory
/// </summary>
public interface IPlppService
{
    #region PLPP CRUD Operations

    /// <summary>
    /// Creates a new PLPP for a student.
    /// </summary>
    Task<PlppDto> CreateAsync(CreatePlppDto dto);

    /// <summary>
    /// Gets a PLPP by its ID with all related data.
    /// </summary>
    Task<PlppDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// Gets a PLPP by ID without related data (lightweight).
    /// </summary>
    Task<PlppListItemDto?> GetListItemByIdAsync(Guid id);

    /// <summary>
    /// Gets all PLPPs for a specific student.
    /// </summary>
    Task<IList<PlppListItemDto>> GetByStudentIdAsync(Guid studentId);

    /// <summary>
    /// Gets all PLPPs for a school.
    /// </summary>
    Task<IList<PlppListItemDto>> GetBySchoolIdAsync(Guid schoolId);

    /// <summary>
    /// Gets PLPPs by status for a school.
    /// </summary>
    Task<IList<PlppListItemDto>> GetByStatusAsync(Guid schoolId, PlppStatus status);

    /// <summary>
    /// Gets active PLPPs (status = Active and within validity period).
    /// </summary>
    Task<IList<PlppListItemDto>> GetActiveAsync(Guid schoolId);

    /// <summary>
    /// Updates an existing PLPP.
    /// </summary>
    Task<PlppDto?> UpdateAsync(UpdatePlppDto dto);

    /// <summary>
    /// Activates a PLPP (changes status from Draft to Active).
    /// </summary>
    Task<bool> ActivateAsync(Guid id, string activatedBy);

    /// <summary>
    /// Archives a PLPP.
    /// </summary>
    Task<bool> ArchiveAsync(Guid id);

    /// <summary>
    /// Soft deletes a PLPP.
    /// </summary>
    Task<bool> DeleteAsync(Guid id);

    #endregion

    #region Goal Operations

    /// <summary>
    /// Adds a goal to a PLPP.
    /// </summary>
    Task<PlppGoalDto> AddGoalAsync(CreatePlppGoalDto dto);

    /// <summary>
    /// Updates an existing goal.
    /// </summary>
    Task<PlppGoalDto?> UpdateGoalAsync(UpdatePlppGoalDto dto);

    /// <summary>
    /// Updates the status of a goal.
    /// </summary>
    Task<bool> UpdateGoalStatusAsync(Guid goalId, GoalStatus status, string? progressNotes = null);

    /// <summary>
    /// Marks a goal as completed.
    /// </summary>
    Task<bool> CompleteGoalAsync(Guid goalId, string? progressNotes = null);

    /// <summary>
    /// Reorders goals within a PLPP.
    /// </summary>
    Task<bool> ReorderGoalsAsync(Guid plppId, IList<Guid> goalIdsInOrder);

    /// <summary>
    /// Soft deletes a goal.
    /// </summary>
    Task<bool> DeleteGoalAsync(Guid goalId);

    /// <summary>
    /// Gets all goals for a PLPP.
    /// </summary>
    Task<IList<PlppGoalDto>> GetGoalsAsync(Guid plppId);

    #endregion

    #region Evaluation Operations

    /// <summary>
    /// Adds a monthly evaluation to a PLPP.
    /// </summary>
    Task<PlppEvaluationDto> AddEvaluationAsync(CreatePlppEvaluationDto dto);

    /// <summary>
    /// Updates an existing evaluation.
    /// </summary>
    Task<PlppEvaluationDto?> UpdateEvaluationAsync(UpdatePlppEvaluationDto dto);

    /// <summary>
    /// Marks parents as notified for an evaluation.
    /// </summary>
    Task<bool> MarkParentsNotifiedAsync(Guid evaluationId);

    /// <summary>
    /// Soft deletes an evaluation.
    /// </summary>
    Task<bool> DeleteEvaluationAsync(Guid evaluationId);

    /// <summary>
    /// Gets all evaluations for a PLPP.
    /// </summary>
    Task<IList<PlppEvaluationDto>> GetEvaluationsAsync(Guid plppId);

    /// <summary>
    /// Gets evaluations for a PLPP within a date range.
    /// </summary>
    Task<IList<PlppEvaluationDto>> GetEvaluationsInRangeAsync(Guid plppId, DateTime from, DateTime to);

    #endregion

    #region Utility Operations

    /// <summary>
    /// Checks if a student has an active PLPP.
    /// </summary>
    Task<bool> HasActivePlppAsync(Guid studentId);

    /// <summary>
    /// Gets the current school year string (e.g., "2024/2025").
    /// </summary>
    string GetCurrentSchoolYear();

    /// <summary>
    /// Duplicates a PLPP for a new school year.
    /// </summary>
    Task<PlppDto> DuplicateForNewYearAsync(Guid plppId, string newSchoolYear);

    #endregion
}
