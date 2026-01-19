using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Infrastructure.Identity;

namespace SpecEdu.Web.Tests.TestFixtures;

public static class MockServiceFactory
{
    public static Mock<UserManager<ApplicationUser>> CreateUserManager()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        var userManager = new Mock<UserManager<ApplicationUser>>(
            store.Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<IPasswordHasher<ApplicationUser>>().Object,
            Array.Empty<IUserValidator<ApplicationUser>>(),
            Array.Empty<IPasswordValidator<ApplicationUser>>(),
            new Mock<ILookupNormalizer>().Object,
            new Mock<IdentityErrorDescriber>().Object,
            new Mock<IServiceProvider>().Object,
            new Mock<ILogger<UserManager<ApplicationUser>>>().Object);

        return userManager;
    }

    public static Mock<SignInManager<ApplicationUser>> CreateSignInManager(
        Mock<UserManager<ApplicationUser>>? userManager = null)
    {
        userManager ??= CreateUserManager();

        var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
        var claimsFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
        var options = new Mock<IOptions<IdentityOptions>>();
        var logger = new Mock<ILogger<SignInManager<ApplicationUser>>>();
        var schemes = new Mock<Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider>();
        var confirmation = new Mock<IUserConfirmation<ApplicationUser>>();

        var signInManager = new Mock<SignInManager<ApplicationUser>>(
            userManager.Object,
            contextAccessor.Object,
            claimsFactory.Object,
            options.Object,
            logger.Object,
            schemes.Object,
            confirmation.Object);

        return signInManager;
    }

    public static Mock<ICurrentUserService> CreateCurrentUserService(
        string? userId = null,
        Guid? schoolId = null)
    {
        var mock = new Mock<ICurrentUserService>();
        mock.Setup(x => x.UserId).Returns(userId);
        mock.Setup(x => x.SchoolId).Returns(schoolId);
        return mock;
    }

    public static Mock<IStudentAccessService> CreateStudentAccessService()
    {
        return new Mock<IStudentAccessService>();
    }

    public static Mock<IStudentService> CreateStudentService()
    {
        return new Mock<IStudentService>();
    }

    public static Mock<ILogger<T>> CreateLogger<T>()
    {
        return new Mock<ILogger<T>>();
    }
}
