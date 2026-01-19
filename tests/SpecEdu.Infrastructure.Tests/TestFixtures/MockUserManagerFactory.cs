using Microsoft.AspNetCore.Identity;
using Moq;
using SpecEdu.Infrastructure.Identity;

namespace SpecEdu.Infrastructure.Tests.TestFixtures;

/// <summary>
/// Factory for creating mock UserManager instances for testing.
/// </summary>
public static class MockUserManagerFactory
{
    /// <summary>
    /// Creates a mock UserManager for ApplicationUser.
    /// </summary>
    /// <returns>A mock UserManager instance.</returns>
    public static Mock<UserManager<ApplicationUser>> Create()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        var userManager = new Mock<UserManager<ApplicationUser>>(
            store.Object,
            null!, null!, null!, null!, null!, null!, null!, null!);

        return userManager;
    }

    /// <summary>
    /// Sets up the mock to return a specific user when FindByIdAsync is called.
    /// </summary>
    public static void SetupFindByIdAsync(
        this Mock<UserManager<ApplicationUser>> mock,
        string userId,
        ApplicationUser? user)
    {
        mock.Setup(m => m.FindByIdAsync(userId))
            .ReturnsAsync(user);
    }

    /// <summary>
    /// Sets up the mock to return whether a user is in a specific role.
    /// </summary>
    public static void SetupIsInRoleAsync(
        this Mock<UserManager<ApplicationUser>> mock,
        ApplicationUser user,
        string role,
        bool result)
    {
        mock.Setup(m => m.IsInRoleAsync(user, role))
            .ReturnsAsync(result);
    }

    /// <summary>
    /// Sets up the mock to return the roles for a specific user.
    /// </summary>
    public static void SetupGetRolesAsync(
        this Mock<UserManager<ApplicationUser>> mock,
        ApplicationUser user,
        IList<string> roles)
    {
        mock.Setup(m => m.GetRolesAsync(user))
            .ReturnsAsync(roles);
    }
}
