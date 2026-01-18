using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SpecEdu.Application.Common.Interfaces;

namespace SpecEdu.Infrastructure.Authorization;

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
            return;
        }

        var studentId = GetStudentIdFromContext();
        if (!studentId.HasValue)
        {
            context.Succeed(requirement);
            return;
        }

        var accessResult = await _studentAccessService.CanAccessStudentAsync(userId, studentId.Value);

        if (!accessResult.CanAccess)
        {
            return;
        }

        if (requirement.RequireEditAccess && !accessResult.CanEdit)
        {
            return;
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

        var routeData = httpContext.GetRouteData();
        if (routeData?.Values.TryGetValue("studentId", out var routeValue) == true)
        {
            if (Guid.TryParse(routeValue?.ToString(), out var studentId))
            {
                return studentId;
            }
        }

        if (httpContext.Request.Query.TryGetValue("studentId", out var queryValue))
        {
            if (Guid.TryParse(queryValue.FirstOrDefault(), out var studentId))
            {
                return studentId;
            }
        }

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
