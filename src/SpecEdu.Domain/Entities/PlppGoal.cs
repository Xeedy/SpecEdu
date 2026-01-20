using SpecEdu.Domain.Common;
using SpecEdu.Domain.Enums;

namespace SpecEdu.Domain.Entities;

public class PlppGoal : AuditableEntity
{
    public Guid PlppId { get; set; }

    public int Order { get; set; } = 1;

    public string? Subject { get; set; }

    public string Description { get; set; } = string.Empty;

    public string? SuccessCriteria { get; set; }

    public string? Methods { get; set; }

    public string? ResponsiblePerson { get; set; }

    public DateTime? TargetDate { get; set; }

    public GoalStatus Status { get; set; } = GoalStatus.NotStarted;

    public string? ProgressNotes { get; set; }

    public DateTime? CompletedAt { get; set; }

    public bool IsActive { get; set; } = true;

    public Plpp? Plpp { get; set; }
}
