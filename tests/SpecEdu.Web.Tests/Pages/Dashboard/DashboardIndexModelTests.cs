using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Application.Common.Models;
using SpecEdu.Web.Pages.Dashboard;
using SpecEdu.Web.Tests.TestFixtures;
using Xunit;

namespace SpecEdu.Web.Tests.Pages.Dashboard;

public class DashboardIndexModelTests : PageModelTestBase
{
    private readonly Mock<IStudentAccessService> _studentAccessServiceMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;

    public DashboardIndexModelTests()
    {
        _studentAccessServiceMock = MockServiceFactory.CreateStudentAccessService();
        _currentUserServiceMock = MockServiceFactory.CreateCurrentUserService();
    }

    private IndexModel CreateIndexModel()
    {
        var model = new IndexModel(
            _studentAccessServiceMock.Object,
            _currentUserServiceMock.Object);

        SetupPageModel(model);
        return model;
    }

    #region OnGetAsync Tests

    [Fact]
    public async Task OnGetAsync_NoUserId_RedirectsToLogin()
    {
        _currentUserServiceMock.Setup(x => x.UserId).Returns((string?)null);
        var model = CreateIndexModel();

        var result = await model.OnGetAsync();

        result.Should().BeOfType<RedirectToPageResult>();
        var redirectResult = result as RedirectToPageResult;
        redirectResult!.PageName.Should().Be("/Account/Login");
    }

    [Fact]
    public async Task OnGetAsync_EmptyUserId_RedirectsToLogin()
    {
        _currentUserServiceMock.Setup(x => x.UserId).Returns(string.Empty);
        var model = CreateIndexModel();

        var result = await model.OnGetAsync();

        result.Should().BeOfType<RedirectToPageResult>();
    }

    [Fact]
    public async Task OnGetAsync_ValidUser_ReturnsPageWithStudents()
    {
        var userId = "test-user-id";
        var students = new List<StudentDto>
        {
            new StudentDto { Id = Guid.NewGuid(), FirstName = "Jan", LastName = "Novák" },
            new StudentDto { Id = Guid.NewGuid(), FirstName = "Marie", LastName = "Kovářová" }
        };

        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
        _studentAccessServiceMock
            .Setup(x => x.GetAccessibleStudentsAsync(userId))
            .ReturnsAsync(students);

        var model = CreateIndexModel();

        var result = await model.OnGetAsync();

        result.Should().BeOfType<PageResult>();
        model.Students.Should().HaveCount(2);
        model.StudentCount.Should().Be(2);
    }

    [Fact]
    public async Task OnGetAsync_UserWithNoAccessibleStudents_ReturnsEmptyList()
    {
        var userId = "test-user-id";
        var emptyList = new List<StudentDto>();

        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
        _studentAccessServiceMock
            .Setup(x => x.GetAccessibleStudentsAsync(userId))
            .ReturnsAsync(emptyList);

        var model = CreateIndexModel();

        var result = await model.OnGetAsync();

        result.Should().BeOfType<PageResult>();
        model.Students.Should().BeEmpty();
        model.StudentCount.Should().Be(0);
    }

    [Fact]
    public async Task OnGetAsync_CallsStudentAccessServiceWithCorrectUserId()
    {
        var userId = "specific-user-id";
        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
        _studentAccessServiceMock
            .Setup(x => x.GetAccessibleStudentsAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<StudentDto>());

        var model = CreateIndexModel();

        await model.OnGetAsync();

        _studentAccessServiceMock.Verify(
            x => x.GetAccessibleStudentsAsync(userId),
            Times.Once);
    }

    #endregion

    #region Properties Tests

    [Fact]
    public void Students_DefaultValue_IsEmptyList()
    {
        var model = CreateIndexModel();

        model.Students.Should().NotBeNull();
        model.Students.Should().BeEmpty();
    }

    [Fact]
    public void StudentCount_ReflectsStudentsList()
    {
        var model = CreateIndexModel();
        model.Students = new List<StudentDto>
        {
            new StudentDto(),
            new StudentDto(),
            new StudentDto()
        };

        model.StudentCount.Should().Be(3);
    }

    #endregion
}
