using Microsoft.AspNetCore.Identity;
using Moq;
using SpecEdu.Infrastructure.Identity;

namespace SpecEdu.Infrastructure.Tests.TestFixtures;

public static class MockUserManagerFactory
{
    public static Mock<UserManager<ApplicationUser>> Create()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        var userManager = new Mock<UserManager<ApplicationUser>>(
            store.Object,
            null!, null!, null!, null!, null!, null!, null!, null!);

        return userManager;
    }

    public static void SetupFindByIdAsync(
        this Mock<UserManager<ApplicationUser>> mock,
        string userId,
        ApplicationUser? user)
    {
        mock.Setup(m => m.FindByIdAsync(userId))
            .ReturnsAsync(user);
    }

    public static void SetupIsInRoleAsync(
        this Mock<UserManager<ApplicationUser>> mock,
        ApplicationUser user,
        string role,
        bool result)
    {
        mock.Setup(m => m.IsInRoleAsync(user, role))
            .ReturnsAsync(result);
    }

    public static void SetupGetRolesAsync(
        this Mock<UserManager<ApplicationUser>> mock,
        ApplicationUser user,
        IList<string> roles)
    {
        mock.Setup(m => m.GetRolesAsync(user))
            .ReturnsAsync(roles);
    }
}
