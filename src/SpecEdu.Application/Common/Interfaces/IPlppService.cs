using SpecEdu.Application.Common.Models;
using SpecEdu.Domain.Enums;

namespace SpecEdu.Application.Common.Interfaces;

public interface IPlppService
{
    #region PLPP CRUD Operations

    Task<PlppDto> CreateAsync(CreatePlppDto dto);

    Task<PlppDto?> GetByIdAsync(Guid id);

    Task<PlppListItemDto?> GetListItemByIdAsync(Guid id);

    Task<IList<PlppListItemDto>> GetByStudentIdAsync(Guid studentId);

    Task<IList<PlppListItemDto>> GetBySchoolIdAsync(Guid schoolId);

    Task<IList<PlppListItemDto>> GetByStatusAsync(Guid schoolId, PlppStatus status);

    Task<IList<PlppListItemDto>> GetActiveAsync(Guid schoolId);

    Task<PlppDto?> UpdateAsync(UpdatePlppDto dto);

    Task<bool> ActivateAsync(Guid id, string activatedBy);

    Task<bool> ArchiveAsync(Guid id);

    Task<bool> DeleteAsync(Guid id);

    #endregion

    #region Goal Operations

    Task<PlppGoalDto> AddGoalAsync(CreatePlppGoalDto dto);

    Task<PlppGoalDto?> UpdateGoalAsync(UpdatePlppGoalDto dto);

    Task<bool> UpdateGoalStatusAsync(Guid goalId, GoalStatus status, string? progressNotes = null);

    Task<bool> CompleteGoalAsync(Guid goalId, string? progressNotes = null);

    Task<bool> ReorderGoalsAsync(Guid plppId, IList<Guid> goalIdsInOrder);

    Task<bool> DeleteGoalAsync(Guid goalId);

    Task<IList<PlppGoalDto>> GetGoalsAsync(Guid plppId);

    #endregion

    #region Evaluation Operations

    Task<PlppEvaluationDto> AddEvaluationAsync(CreatePlppEvaluationDto dto);

    Task<PlppEvaluationDto?> UpdateEvaluationAsync(UpdatePlppEvaluationDto dto);

    Task<bool> MarkParentsNotifiedAsync(Guid evaluationId);

    Task<bool> DeleteEvaluationAsync(Guid evaluationId);

    Task<IList<PlppEvaluationDto>> GetEvaluationsAsync(Guid plppId);

    Task<IList<PlppEvaluationDto>> GetEvaluationsInRangeAsync(Guid plppId, DateTime from, DateTime to);

    #endregion

    #region Utility Operations

    Task<bool> HasActivePlppAsync(Guid studentId);

    string GetCurrentSchoolYear();

    Task<PlppDto> DuplicateForNewYearAsync(Guid plppId, string newSchoolYear);

    #endregion
}
