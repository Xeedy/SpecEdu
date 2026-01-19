using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Moq;
using SpecEdu.Infrastructure.Identity;
using SpecEdu.Web.Pages.Account;
using SpecEdu.Web.Tests.TestFixtures;
using Xunit;

namespace SpecEdu.Web.Tests.Pages.Account;

/// <summary>
/// Tests for the Login page model.
/// </summary>
public class LoginModelTests : PageModelTestBase
{
    private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<Microsoft.Extensions.Logging.ILogger<LoginModel>> _loggerMock;

    public LoginModelTests()
    {
        _userManagerMock = MockServiceFactory.CreateUserManager();
        _signInManagerMock = MockServiceFactory.CreateSignInManager(_userManagerMock);
        _loggerMock = MockServiceFactory.CreateLogger<LoginModel>();
    }

    private LoginModel CreateLoginModel()
    {
        var model = new LoginModel(
            _signInManagerMock.Object,
            _userManagerMock.Object,
            _loggerMock.Object);

        SetupPageModel(model);
        return model;
    }

    #region OnGetAsync Tests

    [Fact]
    public async Task OnGetAsync_SetsReturnUrl()
    {
        // Arrange
        var model = CreateLoginModel();
        var expectedReturnUrl = "/dashboard";

        // Act
        await model.OnGetAsync(expectedReturnUrl);

        // Assert
        model.ReturnUrl.Should().Be(expectedReturnUrl);
    }

    [Fact]
    public async Task OnGetAsync_WithNullReturnUrl_SetsDefaultUrl()
    {
        // Arrange
        var model = CreateLoginModel();

        // Act
        await model.OnGetAsync(null);

        // Assert
        model.ReturnUrl.Should().NotBeNull();
    }

    [Fact]
    public async Task OnGetAsync_WithErrorMessage_AddsModelError()
    {
        // Arrange
        var model = CreateLoginModel();
        model.ErrorMessage = "Test error message";

        // Act
        await model.OnGetAsync(null);

        // Assert
        model.ModelState.IsValid.Should().BeFalse();
        model.ModelState.Should().ContainKey(string.Empty);
    }

    #endregion

    #region OnPostAsync Tests

    [Fact]
    public async Task OnPostAsync_InvalidModelState_ReturnsPage()
    {
        // Arrange
        var model = CreateLoginModel();
        model.ModelState.AddModelError("Test", "Test error");

        // Act
        var result = await model.OnPostAsync(null);

        // Assert
        result.Should().BeOfType<PageResult>();
    }

    [Fact]
    public async Task OnPostAsync_UserNotFound_ReturnsPageWithError()
    {
        // Arrange
        var model = CreateLoginModel();
        model.Input = new LoginModel.LoginInput
        {
            Email = "nonexistent@test.com",
            Password = "Password123!"
        };

        _userManagerMock
            .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((ApplicationUser?)null);

        // Act
        var result = await model.OnPostAsync(null);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.ErrorMessage.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task OnPostAsync_InactiveUser_ReturnsPageWithError()
    {
        // Arrange
        var model = CreateLoginModel();
        model.Input = new LoginModel.LoginInput
        {
            Email = "inactive@test.com",
            Password = "Password123!"
        };

        var inactiveUser = new ApplicationUser
        {
            Email = "inactive@test.com",
            IsActive = false
        };

        _userManagerMock
            .Setup(x => x.FindByEmailAsync("inactive@test.com"))
            .ReturnsAsync(inactiveUser);

        // Act
        var result = await model.OnPostAsync(null);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.ErrorMessage.Should().Contain("deaktivován");
    }

    [Fact]
    public async Task OnPostAsync_SuccessfulLogin_RedirectsToReturnUrl()
    {
        // Arrange
        var model = CreateLoginModel();
        model.Input = new LoginModel.LoginInput
        {
            Email = "test@test.com",
            Password = "Password123!",
            RememberMe = false
        };

        var activeUser = new ApplicationUser
        {
            Email = "test@test.com",
            IsActive = true
        };

        _userManagerMock
            .Setup(x => x.FindByEmailAsync("test@test.com"))
            .ReturnsAsync(activeUser);

        _signInManagerMock
            .Setup(x => x.PasswordSignInAsync(
                activeUser,
                "Password123!",
                false,
                true))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

        // Act
        var result = await model.OnPostAsync("/dashboard");

        // Assert
        result.Should().BeOfType<LocalRedirectResult>();
        var redirectResult = result as LocalRedirectResult;
        redirectResult!.Url.Should().Be("/dashboard");
    }

    [Fact]
    public async Task OnPostAsync_LockedOut_ReturnsPageWithLockoutError()
    {
        // Arrange
        var model = CreateLoginModel();
        model.Input = new LoginModel.LoginInput
        {
            Email = "locked@test.com",
            Password = "Password123!"
        };

        var lockedUser = new ApplicationUser
        {
            Email = "locked@test.com",
            IsActive = true
        };

        _userManagerMock
            .Setup(x => x.FindByEmailAsync("locked@test.com"))
            .ReturnsAsync(lockedUser);

        _signInManagerMock
            .Setup(x => x.PasswordSignInAsync(
                lockedUser,
                It.IsAny<string>(),
                It.IsAny<bool>(),
                true))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.LockedOut);

        // Act
        var result = await model.OnPostAsync(null);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.ErrorMessage.Should().Contain("zablokován");
    }

    [Fact]
    public async Task OnPostAsync_InvalidPassword_ReturnsPageWithError()
    {
        // Arrange
        var model = CreateLoginModel();
        model.Input = new LoginModel.LoginInput
        {
            Email = "test@test.com",
            Password = "WrongPassword"
        };

        var user = new ApplicationUser
        {
            Email = "test@test.com",
            IsActive = true
        };

        _userManagerMock
            .Setup(x => x.FindByEmailAsync("test@test.com"))
            .ReturnsAsync(user);

        _signInManagerMock
            .Setup(x => x.PasswordSignInAsync(
                user,
                "WrongPassword",
                It.IsAny<bool>(),
                true))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

        // Act
        var result = await model.OnPostAsync(null);

        // Assert
        result.Should().BeOfType<PageResult>();
        model.ErrorMessage.Should().NotBeNullOrEmpty();
    }

    #endregion

    #region LoginInput Validation Tests

    [Fact]
    public void LoginInput_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var input = new LoginModel.LoginInput();

        // Assert
        input.Email.Should().BeEmpty();
        input.Password.Should().BeEmpty();
        input.RememberMe.Should().BeFalse();
    }

    [Fact]
    public void LoginInput_CanSetAllProperties()
    {
        // Arrange & Act
        var input = new LoginModel.LoginInput
        {
            Email = "test@example.com",
            Password = "SecurePassword123!",
            RememberMe = true
        };

        // Assert
        input.Email.Should().Be("test@example.com");
        input.Password.Should().Be("SecurePassword123!");
        input.RememberMe.Should().BeTrue();
    }

    #endregion
}
