using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Application.Common.Models;
using SpecEdu.Web.Pages.SchoolAdmin.Students;
using SpecEdu.Web.Tests.TestFixtures;
using Xunit;

namespace SpecEdu.Web.Tests.Pages.SchoolAdmin.Students;

/// <summary>
/// Tests for the SchoolAdmin Students Index page model.
/// </summary>
public class StudentsIndexModelTests : PageModelTestBase
{
    private readonly Mock<IStudentService> _studentServiceMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;

    public StudentsIndexModelTests()
    {
        _studentServiceMock = MockServiceFactory.CreateStudentService();
        _currentUserServiceMock = MockServiceFactory.CreateCurrentUserService();
    }

    private IndexModel CreateIndexModel()
    {
        var model = new IndexModel(
            _studentServiceMock.Object,
            _currentUserServiceMock.Object);

        SetupPageModel(model);
        return model;
    }

    #region OnGetAsync Tests

    [Fact]
    public async Task OnGetAsync_NoSchoolId_RedirectsToAccessDenied()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.SchoolId).Returns((Guid?)null);
        var model = CreateIndexModel();

        // Act
        var result = await model.OnGetAsync();

        // Assert
        result.Should().BeOfType<RedirectToPageResult>();
        var redirectResult = result as RedirectToPageResult;
        redirectResult!.PageName.Should().Be("/Account/AccessDenied");
    }

    [Fact]
    public async Task OnGetAsync_WithSchoolId_ReturnsPageWithStudents()
    {
        // Arrange
        var schoolId = Guid.NewGuid();
        var students = new List<StudentDto>
        {
            new StudentDto { Id = Guid.NewGuid(), FirstName = "Jan", LastName = "Novák" },
            new StudentDto { Id = Guid.NewGuid(), FirstName = "Marie", LastName = "Kovářová" }
        };

        _currentUserServiceMock.Setup(x => x.SchoolId).Returns(schoolId);
        _studentServiceMock
            .Setup(x => x.GetPagedAsync(schoolId, 1, 20, null, true))
            .ReturnsAsync((students, 2));

        var model = CreateIndexModel();

        // Act
        var result = await model.OnGetAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
        model.Students.Should().HaveCount(2);
        model.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task OnGetAsync_WithSearchTerm_PassesSearchTermToService()
    {
        // Arrange
        var schoolId = Guid.NewGuid();
        var searchTerm = "Novák";

        _currentUserServiceMock.Setup(x => x.SchoolId).Returns(schoolId);
        _studentServiceMock
            .Setup(x => x.GetPagedAsync(schoolId, 1, 20, searchTerm, true))
            .ReturnsAsync((new List<StudentDto>(), 0));

        var model = CreateIndexModel();
        model.SearchTerm = searchTerm;

        // Act
        await model.OnGetAsync();

        // Assert
        _studentServiceMock.Verify(
            x => x.GetPagedAsync(schoolId, 1, 20, searchTerm, true),
            Times.Once);
    }

    [Fact]
    public async Task OnGetAsync_WithPagination_PassesCorrectPageNumber()
    {
        // Arrange
        var schoolId = Guid.NewGuid();
        var pageNumber = 3;

        _currentUserServiceMock.Setup(x => x.SchoolId).Returns(schoolId);
        _studentServiceMock
            .Setup(x => x.GetPagedAsync(schoolId, pageNumber, 20, null, true))
            .ReturnsAsync((new List<StudentDto>(), 0));

        var model = CreateIndexModel();
        model.CurrentPage = pageNumber;

        // Act
        await model.OnGetAsync();

        // Assert
        _studentServiceMock.Verify(
            x => x.GetPagedAsync(schoolId, pageNumber, 20, null, true),
            Times.Once);
    }

    [Fact]
    public async Task OnGetAsync_EmptySchool_ReturnsEmptyList()
    {
        // Arrange
        var schoolId = Guid.NewGuid();

        _currentUserServiceMock.Setup(x => x.SchoolId).Returns(schoolId);
        _studentServiceMock
            .Setup(x => x.GetPagedAsync(schoolId, 1, 20, null, true))
            .ReturnsAsync((new List<StudentDto>(), 0));

        var model = CreateIndexModel();

        // Act
        var result = await model.OnGetAsync();

        // Assert
        result.Should().BeOfType<PageResult>();
        model.Students.Should().BeEmpty();
        model.TotalCount.Should().Be(0);
    }

    #endregion

    #region Pagination Properties Tests

    [Fact]
    public void TotalPages_CalculatesCorrectly()
    {
        // Arrange
        var model = CreateIndexModel();
        model.TotalCount = 45;
        // PageSize is 20

        // Act & Assert
        model.TotalPages.Should().Be(3); // 45/20 = 2.25, ceiling = 3
    }

    [Fact]
    public void TotalPages_WithExactMultiple_CalculatesCorrectly()
    {
        // Arrange
        var model = CreateIndexModel();
        model.TotalCount = 40;

        // Act & Assert
        model.TotalPages.Should().Be(2); // 40/20 = 2
    }

    [Fact]
    public void TotalPages_WithZeroCount_ReturnsZero()
    {
        // Arrange
        var model = CreateIndexModel();
        model.TotalCount = 0;

        // Act & Assert
        model.TotalPages.Should().Be(0);
    }

    [Fact]
    public void CurrentPage_DefaultValue_IsOne()
    {
        // Arrange & Act
        var model = CreateIndexModel();

        // Assert
        model.CurrentPage.Should().Be(1);
    }

    [Fact]
    public void PageSize_DefaultValue_IsTwenty()
    {
        // Arrange & Act
        var model = CreateIndexModel();

        // Assert
        model.PageSize.Should().Be(20);
    }

    #endregion

    #region Properties Tests

    [Fact]
    public void Students_DefaultValue_IsEmptyList()
    {
        // Arrange & Act
        var model = CreateIndexModel();

        // Assert
        model.Students.Should().NotBeNull();
        model.Students.Should().BeEmpty();
    }

    [Fact]
    public void SearchTerm_DefaultValue_IsNull()
    {
        // Arrange & Act
        var model = CreateIndexModel();

        // Assert
        model.SearchTerm.Should().BeNull();
    }

    [Fact]
    public void SearchTerm_CanBeSet()
    {
        // Arrange
        var model = CreateIndexModel();

        // Act
        model.SearchTerm = "test search";

        // Assert
        model.SearchTerm.Should().Be("test search");
    }

    #endregion
}
