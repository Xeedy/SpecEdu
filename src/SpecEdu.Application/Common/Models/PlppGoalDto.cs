using SpecEdu.Domain.Enums;

namespace SpecEdu.Application.Common.Models;

/// <summary>
/// Data transfer object for PLPP goals.
/// Czech: DTO pro cíle PLPP
/// </summary>
public class PlppGoalDto
{
    public Guid Id { get; set; }
    public Guid PlppId { get; set; }
    public int Order { get; set; }
    public string? Subject { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? SuccessCriteria { get; set; }
    public string? Methods { get; set; }
    public string? ResponsiblePerson { get; set; }
    public DateTime? TargetDate { get; set; }
    public GoalStatus Status { get; set; }
    public string? ProgressNotes { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// DTO for creating a new PLPP goal.
/// </summary>
public class CreatePlppGoalDto
{
    public Guid PlppId { get; set; }
    public int Order { get; set; } = 1;
    public string? Subject { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? SuccessCriteria { get; set; }
    public string? Methods { get; set; }
    public string? ResponsiblePerson { get; set; }
    public DateTime? TargetDate { get; set; }
}

/// <summary>
/// DTO for updating an existing PLPP goal.
/// </summary>
public class UpdatePlppGoalDto
{
    public Guid Id { get; set; }
    public int Order { get; set; }
    public string? Subject { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? SuccessCriteria { get; set; }
    public string? Methods { get; set; }
    public string? ResponsiblePerson { get; set; }
    public DateTime? TargetDate { get; set; }
    public GoalStatus Status { get; set; }
    public string? ProgressNotes { get; set; }
}
