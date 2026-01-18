using Microsoft.AspNetCore.Authorization;

namespace SpecEdu.Infrastructure.Authorization;

/// <summary>
/// Authorization requirement for accessing student data.
/// Checks relationship-based access through IStudentAccessService.
/// </summary>
public class StudentAccessRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// Whether edit access is required (true) or read access is sufficient (false).
    /// </summary>
    public bool RequireEditAccess { get; }

    /// <summary>
    /// Creates a new student access requirement.
    /// </summary>
    /// <param name="requireEditAccess">True if edit access is required, false for read-only.</param>
    public StudentAccessRequirement(bool requireEditAccess = false)
    {
        RequireEditAccess = requireEditAccess;
    }
}
