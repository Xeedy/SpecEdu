using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SpecEdu.Application.Common.Interfaces;

namespace SpecEdu.Infrastructure.Authorization;

/// <summary>
/// Authorization handler that evaluates student access requirements.
/// Uses IStudentAccessService to check relationship-based access.
/// </summary>
public class StudentAccessAuthorizationHandler : AuthorizationHandler<StudentAccessRequirement>
{
    private readonly IStudentAccessService _studentAccessService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public StudentAccessAuthorizationHandler(
        IStudentAccessService studentAccessService,
        IHttpContextAccessor httpContextAccessor)
    {
        _studentAccessService = studentAccessService;
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        StudentAccessRequirement requirement)
    {
        var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return; // Not authenticated
        }

        // Get student ID from route
        var studentId = GetStudentIdFromContext();
        if (!studentId.HasValue)
        {
            // No student ID in route - this might be a list page
            // For list pages, we succeed and let the service filter results
            context.Succeed(requirement);
            return;
        }

        // Check access through the service
        var accessResult = await _studentAccessService.CanAccessStudentAsync(userId, studentId.Value);

        if (!accessResult.CanAccess)
        {
            return; // No access
        }

        if (requirement.RequireEditAccess && !accessResult.CanEdit)
        {
            return; // Requires edit but only has read access
        }

        context.Succeed(requirement);
    }

    private Guid? GetStudentIdFromContext()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            return null;
        }

        // Try route data first
        var routeData = httpContext.GetRouteData();
        if (routeData?.Values.TryGetValue("studentId", out var routeValue) == true)
        {
            if (Guid.TryParse(routeValue?.ToString(), out var studentId))
            {
                return studentId;
            }
        }

        // Try query string
        if (httpContext.Request.Query.TryGetValue("studentId", out var queryValue))
        {
            if (Guid.TryParse(queryValue.FirstOrDefault(), out var studentId))
            {
                return studentId;
            }
        }

        // Try route value "id" (common pattern)
        if (routeData?.Values.TryGetValue("id", out var idValue) == true)
        {
            if (Guid.TryParse(idValue?.ToString(), out var studentId))
            {
                return studentId;
            }
        }

        return null;
    }
}
