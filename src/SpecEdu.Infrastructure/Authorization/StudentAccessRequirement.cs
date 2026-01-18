using Microsoft.AspNetCore.Authorization;

namespace SpecEdu.Infrastructure.Authorization;

public class StudentAccessRequirement : IAuthorizationRequirement
{
    public bool RequireEditAccess { get; }

    public StudentAccessRequirement(bool requireEditAccess = false)
    {
        RequireEditAccess = requireEditAccess;
    }
}
