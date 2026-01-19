using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Domain.Entities;
using SpecEdu.Domain.Enums;
using SpecEdu.Infrastructure.Identity;
using SpecEdu.Infrastructure.Services;
using SpecEdu.Infrastructure.Tests.TestFixtures;
using Xunit;

namespace SpecEdu.Infrastructure.Tests.Services;

/// <summary>
/// Tests for DiaryService - diary entry and attachment management.
/// </summary>
public class DiaryServiceTests : IDisposable
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;

    public DiaryServiceTests()
    {
        _userManagerMock = MockUserManagerFactory.Create();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
    }

    public void Dispose()
    {
        // Cleanup if needed
    }

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_ValidEntry_CreatesAndReturnsEntry()
    {
        // Arrange
        using var dbContext = TestDbContextFactory.Create();
        var school = new School { Id = Guid.NewGuid(), Name = "Test School" };
        var student = new Student
        {
            Id = Guid.NewGuid(),
            FirstName = "Jan",
            LastName = "Novák",
            SchoolId = school.Id,
            School = school
        };
        dbContext.Schools.Add(school);
        dbContext.Students.Add(student);
        await dbContext.SaveChangesAsync();

        var service = new DiaryService(dbContext, _userManagerMock.Object, _currentUserServiceMock.Object);

        // Act
        var result = await service.CreateAsync(
            student.Id,
            DiaryEntryType.Note,
            "Test Entry",
            "Test Content",
            DiaryVisibility.SchoolOnly,
            null);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("Test Entry");
        result.Content.Should().Be("Test Content");
        result.Type.Should().Be(DiaryEntryType.Note);
        result.Visibility.Should().Be(DiaryVisibility.SchoolOnly);
        result.StudentId.Should().Be(student.Id);
        result.IsActive.Should().BeTrue();

        // Verify in database
        var dbEntry = await dbContext.DiaryEntries.FindAsync(result.Id);
        dbEntry.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateAsync_WithOccurredAt_SetsOccurredAtDate()
    {
        // Arrange
        using var dbContext = TestDbContextFactory.Create();
        var school = new School { Id = Guid.NewGuid(), Name = "Test School" };
        var student = new Student
        {
            Id = Guid.NewGuid(),
            FirstName = "Jan",
            LastName = "Novák",
            SchoolId = school.Id,
            School = school
        };
        dbContext.Schools.Add(school);
        dbContext.Students.Add(student);
        await dbContext.SaveChangesAsync();

        var occurredAt = DateTime.UtcNow.AddDays(-1);
        var service = new DiaryService(dbContext, _userManagerMock.Object, _currentUserServiceMock.Object);

        // Act
        var result = await service.CreateAsync(
            student.Id,
            DiaryEntryType.Meeting,
            "Meeting with Parents",
            "Discussed progress",
            DiaryVisibility.ParentVisible,
            occurredAt);

        // Assert
        result.OccurredAt.Should().BeCloseTo(occurredAt, TimeSpan.FromSeconds(1));
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_ExistingEntry_ReturnsEntry()
    {
        // Arrange
        using var dbContext = TestDbContextFactory.Create();
        var school = new School { Id = Guid.NewGuid(), Name = "Test School" };
        var student = new Student
        {
            Id = Guid.NewGuid(),
            FirstName = "Jan",
            LastName = "Novák",
            SchoolId = school.Id,
            School = school
        };
        var entry = new DiaryEntry
        {
            Id = Guid.NewGuid(),
            StudentId = student.Id,
            Student = student,
            Type = DiaryEntryType.Note,
            Title = "Test Entry",
            Content = "Test Content",
            Visibility = DiaryVisibility.SchoolOnly,
            IsActive = true
        };
        dbContext.Schools.Add(school);
        dbContext.Students.Add(student);
        dbContext.DiaryEntries.Add(entry);
        await dbContext.SaveChangesAsync();

        var service = new DiaryService(dbContext, _userManagerMock.Object, _currentUserServiceMock.Object);

        // Act
        var result = await service.GetByIdAsync(entry.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(entry.Id);
        result.Title.Should().Be("Test Entry");
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingEntry_ReturnsNull()
    {
        // Arrange
        using var dbContext = TestDbContextFactory.Create();
        var service = new DiaryService(dbContext, _userManagerMock.Object, _currentUserServiceMock.Object);

        // Act
        var result = await service.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_InactiveEntry_ReturnsNull()
    {
        // Arrange
        using var dbContext = TestDbContextFactory.Create();
        var school = new School { Id = Guid.NewGuid(), Name = "Test School" };
        var student = new Student
        {
            Id = Guid.NewGuid(),
            FirstName = "Jan",
            LastName = "Novák",
            SchoolId = school.Id,
            School = school
        };
        var entry = new DiaryEntry
        {
            Id = Guid.NewGuid(),
            StudentId = student.Id,
            Student = student,
            Type = DiaryEntryType.Note,
            Title = "Deleted Entry",
            Content = "This was deleted",
            IsActive = false // Soft deleted
        };
        dbContext.Schools.Add(school);
        dbContext.Students.Add(student);
        dbContext.DiaryEntries.Add(entry);
        await dbContext.SaveChangesAsync();

        var service = new DiaryService(dbContext, _userManagerMock.Object, _currentUserServiceMock.Object);

        // Act
        var result = await service.GetByIdAsync(entry.Id);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetEntriesAsync Tests

    [Fact]
    public async Task GetEntriesAsync_ReturnsEntriesForStudent()
    {
        // Arrange
        using var dbContext = TestDbContextFactory.Create();
        var school = new School { Id = Guid.NewGuid(), Name = "Test School" };
        var student = new Student
        {
            Id = Guid.NewGuid(),
            FirstName = "Jan",
            LastName = "Novák",
            SchoolId = school.Id,
            School = school
        };
        dbContext.Schools.Add(school);
        dbContext.Students.Add(student);

        // Add multiple entries
        for (int i = 0; i < 5; i++)
        {
            dbContext.DiaryEntries.Add(new DiaryEntry
            {
                Id = Guid.NewGuid(),
                StudentId = student.Id,
                Type = DiaryEntryType.Note,
                Title = $"Entry {i}",
                Content = $"Content {i}",
                Visibility = DiaryVisibility.SchoolOnly,
                IsActive = true
            });
        }
        await dbContext.SaveChangesAsync();

        var service = new DiaryService(dbContext, _userManagerMock.Object, _currentUserServiceMock.Object);

        // Act
        var (entries, totalCount) = await service.GetEntriesAsync(student.Id, page: 1, pageSize: 10);

        // Assert
        entries.Should().HaveCount(5);
        totalCount.Should().Be(5);
    }

    [Fact]
    public async Task GetEntriesAsync_WithPagination_ReturnsPaginatedResults()
    {
        // Arrange
        using var dbContext = TestDbContextFactory.Create();
        var school = new School { Id = Guid.NewGuid(), Name = "Test School" };
        var student = new Student
        {
            Id = Guid.NewGuid(),
            FirstName = "Jan",
            LastName = "Novák",
            SchoolId = school.Id,
            School = school
        };
        dbContext.Schools.Add(school);
        dbContext.Students.Add(student);

        for (int i = 0; i < 25; i++)
        {
            dbContext.DiaryEntries.Add(new DiaryEntry
            {
                Id = Guid.NewGuid(),
                StudentId = student.Id,
                Type = DiaryEntryType.Note,
                Title = $"Entry {i}",
                Content = $"Content {i}",
                Visibility = DiaryVisibility.SchoolOnly,
                IsActive = true
            });
        }
        await dbContext.SaveChangesAsync();

        var service = new DiaryService(dbContext, _userManagerMock.Object, _currentUserServiceMock.Object);

        // Act
        var (entries, totalCount) = await service.GetEntriesAsync(student.Id, page: 2, pageSize: 10);

        // Assert
        entries.Should().HaveCount(10);
        totalCount.Should().Be(25);
    }

    [Fact]
    public async Task GetEntriesAsync_FilterByType_ReturnsFilteredResults()
    {
        // Arrange
        using var dbContext = TestDbContextFactory.Create();
        var school = new School { Id = Guid.NewGuid(), Name = "Test School" };
        var student = new Student
        {
            Id = Guid.NewGuid(),
            FirstName = "Jan",
            LastName = "Novák",
            SchoolId = school.Id,
            School = school
        };
        dbContext.Schools.Add(school);
        dbContext.Students.Add(student);

        dbContext.DiaryEntries.AddRange(
            new DiaryEntry { Id = Guid.NewGuid(), StudentId = student.Id, Type = DiaryEntryType.Note, Title = "Note 1", Content = "Content", IsActive = true },
            new DiaryEntry { Id = Guid.NewGuid(), StudentId = student.Id, Type = DiaryEntryType.Meeting, Title = "Meeting 1", Content = "Content", IsActive = true },
            new DiaryEntry { Id = Guid.NewGuid(), StudentId = student.Id, Type = DiaryEntryType.Note, Title = "Note 2", Content = "Content", IsActive = true },
            new DiaryEntry { Id = Guid.NewGuid(), StudentId = student.Id, Type = DiaryEntryType.PhoneCall, Title = "Call 1", Content = "Content", IsActive = true }
        );
        await dbContext.SaveChangesAsync();

        var service = new DiaryService(dbContext, _userManagerMock.Object, _currentUserServiceMock.Object);

        // Act
        var (entries, totalCount) = await service.GetEntriesAsync(
            student.Id, page: 1, pageSize: 10, type: DiaryEntryType.Note);

        // Assert
        entries.Should().HaveCount(2);
        entries.All(e => e.Type == DiaryEntryType.Note).Should().BeTrue();
        totalCount.Should().Be(2);
    }

    [Fact]
    public async Task GetEntriesAsync_FilterByVisibility_ReturnsFilteredResults()
    {
        // Arrange
        using var dbContext = TestDbContextFactory.Create();
        var school = new School { Id = Guid.NewGuid(), Name = "Test School" };
        var student = new Student
        {
            Id = Guid.NewGuid(),
            FirstName = "Jan",
            LastName = "Novák",
            SchoolId = school.Id,
            School = school
        };
        dbContext.Schools.Add(school);
        dbContext.Students.Add(student);

        dbContext.DiaryEntries.AddRange(
            new DiaryEntry { Id = Guid.NewGuid(), StudentId = student.Id, Type = DiaryEntryType.Note, Title = "School Only 1", Content = "Content", Visibility = DiaryVisibility.SchoolOnly, IsActive = true },
            new DiaryEntry { Id = Guid.NewGuid(), StudentId = student.Id, Type = DiaryEntryType.Note, Title = "Parent Visible 1", Content = "Content", Visibility = DiaryVisibility.ParentVisible, IsActive = true },
            new DiaryEntry { Id = Guid.NewGuid(), StudentId = student.Id, Type = DiaryEntryType.Note, Title = "Parent Visible 2", Content = "Content", Visibility = DiaryVisibility.ParentVisible, IsActive = true }
        );
        await dbContext.SaveChangesAsync();

        var service = new DiaryService(dbContext, _userManagerMock.Object, _currentUserServiceMock.Object);

        // Act
        var (entries, totalCount) = await service.GetEntriesAsync(
            student.Id, page: 1, pageSize: 10, visibilityFilter: DiaryVisibility.ParentVisible);

        // Assert
        entries.Should().HaveCount(2);
        entries.All(e => e.Visibility == DiaryVisibility.ParentVisible).Should().BeTrue();
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_ExistingEntry_UpdatesAndReturnsEntry()
    {
        // Arrange
        using var dbContext = TestDbContextFactory.Create();
        var school = new School { Id = Guid.NewGuid(), Name = "Test School" };
        var student = new Student
        {
            Id = Guid.NewGuid(),
            FirstName = "Jan",
            LastName = "Novák",
            SchoolId = school.Id,
            School = school
        };
        var entry = new DiaryEntry
        {
            Id = Guid.NewGuid(),
            StudentId = student.Id,
            Student = student,
            Type = DiaryEntryType.Note,
            Title = "Original Title",
            Content = "Original Content",
            Visibility = DiaryVisibility.SchoolOnly,
            IsActive = true
        };
        dbContext.Schools.Add(school);
        dbContext.Students.Add(student);
        dbContext.DiaryEntries.Add(entry);
        await dbContext.SaveChangesAsync();

        var service = new DiaryService(dbContext, _userManagerMock.Object, _currentUserServiceMock.Object);

        // Act
        var result = await service.UpdateAsync(
            entry.Id,
            DiaryEntryType.Meeting,
            "Updated Title",
            "Updated Content",
            DiaryVisibility.ParentVisible,
            DateTime.UtcNow);

        // Assert
        result.Should().NotBeNull();
        result!.Title.Should().Be("Updated Title");
        result.Content.Should().Be("Updated Content");
        result.Type.Should().Be(DiaryEntryType.Meeting);
        result.Visibility.Should().Be(DiaryVisibility.ParentVisible);
    }

    [Fact]
    public async Task UpdateAsync_NonExistingEntry_ReturnsNull()
    {
        // Arrange
        using var dbContext = TestDbContextFactory.Create();
        var service = new DiaryService(dbContext, _userManagerMock.Object, _currentUserServiceMock.Object);

        // Act
        var result = await service.UpdateAsync(
            Guid.NewGuid(),
            DiaryEntryType.Note,
            "Title",
            "Content",
            DiaryVisibility.SchoolOnly,
            null);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_ExistingEntry_SoftDeletesEntry()
    {
        // Arrange
        using var dbContext = TestDbContextFactory.Create();
        var entry = new DiaryEntry
        {
            Id = Guid.NewGuid(),
            StudentId = Guid.NewGuid(),
            Type = DiaryEntryType.Note,
            Title = "To Delete",
            Content = "Content",
            IsActive = true
        };
        dbContext.DiaryEntries.Add(entry);
        await dbContext.SaveChangesAsync();

        var service = new DiaryService(dbContext, _userManagerMock.Object, _currentUserServiceMock.Object);

        // Act
        var result = await service.DeleteAsync(entry.Id);

        // Assert
        result.Should().BeTrue();

        var deletedEntry = await dbContext.DiaryEntries.FindAsync(entry.Id);
        deletedEntry.Should().NotBeNull();
        deletedEntry!.IsActive.Should().BeFalse(); // Soft deleted
    }

    [Fact]
    public async Task DeleteAsync_NonExistingEntry_ReturnsFalse()
    {
        // Arrange
        using var dbContext = TestDbContextFactory.Create();
        var service = new DiaryService(dbContext, _userManagerMock.Object, _currentUserServiceMock.Object);

        // Act
        var result = await service.DeleteAsync(Guid.NewGuid());

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region CanEditAsync Tests

    [Fact]
    public async Task CanEditAsync_AuthorOfEntry_ReturnsTrue()
    {
        // Arrange
        var authorId = Guid.NewGuid().ToString();

        // Create a mock ICurrentUserService that returns authorId so UpdateAuditFields sets CreatedBy correctly
        var authorUserServiceMock = new Mock<ICurrentUserService>();
        authorUserServiceMock.Setup(x => x.UserId).Returns(authorId);

        using var dbContext = TestDbContextFactory.Create(currentUserService: authorUserServiceMock.Object);
        var entry = new DiaryEntry
        {
            Id = Guid.NewGuid(),
            StudentId = Guid.NewGuid(),
            Type = DiaryEntryType.Note,
            Title = "Entry",
            Content = "Content",
            IsActive = true
        };
        dbContext.DiaryEntries.Add(entry);
        await dbContext.SaveChangesAsync();

        var service = new DiaryService(dbContext, _userManagerMock.Object, _currentUserServiceMock.Object);

        // Act
        var result = await service.CanEditAsync(entry.Id, authorId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task CanEditAsync_NotAuthorOfEntry_ReturnsFalse()
    {
        // Arrange
        var authorId = Guid.NewGuid().ToString();
        var differentUserId = Guid.NewGuid().ToString();

        // Create a mock ICurrentUserService that returns authorId so UpdateAuditFields sets CreatedBy correctly
        var authorUserServiceMock = new Mock<ICurrentUserService>();
        authorUserServiceMock.Setup(x => x.UserId).Returns(authorId);

        using var dbContext = TestDbContextFactory.Create(currentUserService: authorUserServiceMock.Object);
        var entry = new DiaryEntry
        {
            Id = Guid.NewGuid(),
            StudentId = Guid.NewGuid(),
            Type = DiaryEntryType.Note,
            Title = "Entry",
            Content = "Content",
            IsActive = true
        };
        dbContext.DiaryEntries.Add(entry);
        await dbContext.SaveChangesAsync();

        var service = new DiaryService(dbContext, _userManagerMock.Object, _currentUserServiceMock.Object);

        // Act
        var result = await service.CanEditAsync(entry.Id, differentUserId);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region Attachment Tests

    [Fact]
    public async Task AddAttachmentAsync_ValidData_CreatesAttachment()
    {
        // Arrange
        using var dbContext = TestDbContextFactory.Create();
        var entry = new DiaryEntry
        {
            Id = Guid.NewGuid(),
            StudentId = Guid.NewGuid(),
            Type = DiaryEntryType.Note,
            Title = "Entry with attachment",
            Content = "Content",
            IsActive = true
        };
        dbContext.DiaryEntries.Add(entry);
        await dbContext.SaveChangesAsync();

        var service = new DiaryService(dbContext, _userManagerMock.Object, _currentUserServiceMock.Object);
        var fileData = new byte[] { 0x25, 0x50, 0x44, 0x46 }; // PDF header

        // Act
        var result = await service.AddAttachmentAsync(
            entry.Id,
            "document.pdf",
            "application/pdf",
            fileData);

        // Assert
        result.Should().NotBeNull();
        result.FileName.Should().Be("document.pdf");
        result.ContentType.Should().Be("application/pdf");
        result.FileSize.Should().Be(4);
        result.DiaryEntryId.Should().Be(entry.Id);
    }

    [Fact]
    public async Task GetAttachmentDataAsync_ExistingAttachment_ReturnsData()
    {
        // Arrange
        using var dbContext = TestDbContextFactory.Create();
        var fileData = new byte[] { 0x25, 0x50, 0x44, 0x46 };
        var attachment = new DiaryAttachment
        {
            Id = Guid.NewGuid(),
            DiaryEntryId = Guid.NewGuid(),
            FileName = "test.pdf",
            ContentType = "application/pdf",
            FileData = fileData,
            FileSize = fileData.Length
        };
        dbContext.DiaryAttachments.Add(attachment);
        await dbContext.SaveChangesAsync();

        var service = new DiaryService(dbContext, _userManagerMock.Object, _currentUserServiceMock.Object);

        // Act
        var result = await service.GetAttachmentDataAsync(attachment.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Value.FileData.Should().BeEquivalentTo(fileData);
        result.Value.FileName.Should().Be("test.pdf");
        result.Value.ContentType.Should().Be("application/pdf");
    }

    [Fact]
    public async Task RemoveAttachmentAsync_ExistingAttachment_RemovesFromDatabase()
    {
        // Arrange
        using var dbContext = TestDbContextFactory.Create();
        var attachment = new DiaryAttachment
        {
            Id = Guid.NewGuid(),
            DiaryEntryId = Guid.NewGuid(),
            FileName = "test.pdf",
            ContentType = "application/pdf",
            FileData = new byte[] { 0x25, 0x50, 0x44, 0x46 },
            FileSize = 4
        };
        dbContext.DiaryAttachments.Add(attachment);
        await dbContext.SaveChangesAsync();

        var service = new DiaryService(dbContext, _userManagerMock.Object, _currentUserServiceMock.Object);

        // Act
        var result = await service.RemoveAttachmentAsync(attachment.Id);

        // Assert
        result.Should().BeTrue();

        var deleted = await dbContext.DiaryAttachments.FindAsync(attachment.Id);
        deleted.Should().BeNull();
    }

    #endregion

    #region Statistics Tests

    [Fact]
    public async Task GetEntryCountsByTypeAsync_ReturnsCorrectCounts()
    {
        // Arrange
        using var dbContext = TestDbContextFactory.Create();
        var studentId = Guid.NewGuid();

        dbContext.DiaryEntries.AddRange(
            new DiaryEntry { Id = Guid.NewGuid(), StudentId = studentId, Type = DiaryEntryType.Note, Title = "Note 1", Content = "C", IsActive = true },
            new DiaryEntry { Id = Guid.NewGuid(), StudentId = studentId, Type = DiaryEntryType.Note, Title = "Note 2", Content = "C", IsActive = true },
            new DiaryEntry { Id = Guid.NewGuid(), StudentId = studentId, Type = DiaryEntryType.Meeting, Title = "Meeting 1", Content = "C", IsActive = true },
            new DiaryEntry { Id = Guid.NewGuid(), StudentId = studentId, Type = DiaryEntryType.PhoneCall, Title = "Call 1", Content = "C", IsActive = true },
            new DiaryEntry { Id = Guid.NewGuid(), StudentId = studentId, Type = DiaryEntryType.PhoneCall, Title = "Call 2", Content = "C", IsActive = true },
            new DiaryEntry { Id = Guid.NewGuid(), StudentId = studentId, Type = DiaryEntryType.PhoneCall, Title = "Call 3", Content = "C", IsActive = true }
        );
        await dbContext.SaveChangesAsync();

        var service = new DiaryService(dbContext, _userManagerMock.Object, _currentUserServiceMock.Object);

        // Act
        var result = await service.GetEntryCountsByTypeAsync(studentId);

        // Assert
        result.Should().HaveCount(3);
        result[DiaryEntryType.Note].Should().Be(2);
        result[DiaryEntryType.Meeting].Should().Be(1);
        result[DiaryEntryType.PhoneCall].Should().Be(3);
    }

    #endregion
}
