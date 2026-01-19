using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Application.Common.Models;
using SpecEdu.Web.Pages.MyChildren;
using SpecEdu.Web.Tests.TestFixtures;
using Xunit;

namespace SpecEdu.Web.Tests.Pages.MyChildren;

public class MyChildrenIndexModelTests : PageModelTestBase
{
    private readonly Mock<IStudentAccessService> _studentAccessServiceMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;

    public MyChildrenIndexModelTests()
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
    public async Task OnGetAsync_ParentWithChildren_ReturnsPageWithChildren()
    {
        var parentUserId = "parent-user-id";
        var children = new List<StudentDto>
        {
            new StudentDto { Id = Guid.NewGuid(), FirstName = "Jan", LastName = "Novák" },
            new StudentDto { Id = Guid.NewGuid(), FirstName = "Anna", LastName = "Nováková" }
        };

        _currentUserServiceMock.Setup(x => x.UserId).Returns(parentUserId);
        _studentAccessServiceMock
            .Setup(x => x.GetStudentsForParentAsync(parentUserId))
            .ReturnsAsync(children);

        var model = CreateIndexModel();

        var result = await model.OnGetAsync();

        result.Should().BeOfType<PageResult>();
        model.Children.Should().HaveCount(2);
        model.Children.Should().Contain(c => c.FirstName == "Jan");
        model.Children.Should().Contain(c => c.FirstName == "Anna");
    }

    [Fact]
    public async Task OnGetAsync_ParentWithNoChildren_ReturnsEmptyList()
    {
        var parentUserId = "parent-user-id";
        var emptyList = new List<StudentDto>();

        _currentUserServiceMock.Setup(x => x.UserId).Returns(parentUserId);
        _studentAccessServiceMock
            .Setup(x => x.GetStudentsForParentAsync(parentUserId))
            .ReturnsAsync(emptyList);

        var model = CreateIndexModel();

        var result = await model.OnGetAsync();

        result.Should().BeOfType<PageResult>();
        model.Children.Should().BeEmpty();
    }

    [Fact]
    public async Task OnGetAsync_CallsGetStudentsForParentAsyncWithCorrectUserId()
    {
        var parentUserId = "specific-parent-id";
        _currentUserServiceMock.Setup(x => x.UserId).Returns(parentUserId);
        _studentAccessServiceMock
            .Setup(x => x.GetStudentsForParentAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<StudentDto>());

        var model = CreateIndexModel();

        await model.OnGetAsync();

        _studentAccessServiceMock.Verify(
            x => x.GetStudentsForParentAsync(parentUserId),
            Times.Once);
    }

    #endregion

    #region Properties Tests

    [Fact]
    public void Children_DefaultValue_IsEmptyList()
    {
        var model = CreateIndexModel();

        model.Children.Should().NotBeNull();
        model.Children.Should().BeEmpty();
    }

    [Fact]
    public void Children_CanBeSet()
    {
        var model = CreateIndexModel();
        var children = new List<StudentDto>
        {
            new StudentDto { FirstName = "Test", LastName = "Child" }
        };

        model.Children = children;

        model.Children.Should().HaveCount(1);
        model.Children.First().FirstName.Should().Be("Test");
    }

    #endregion
}
