namespace SpecEdu.Application.Common.Interfaces;

public interface ICurrentUserService
{
    string? UserId { get; }

    Guid? SchoolId { get; }

    bool IsAuthenticated { get; }

    IReadOnlyList<string> Roles { get; }

    bool IsInRole(string role);
}
