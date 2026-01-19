using FluentAssertions;
using SpecEdu.Application.Common.Models;
using SpecEdu.Domain.Enums;
using Xunit;

namespace SpecEdu.Application.Tests.Models;

/// <summary>
/// Tests for DiaryEntryDto data transfer object.
/// </summary>
public class DiaryEntryDtoTests
{
    [Fact]
    public void DiaryEntryDto_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var dto = new DiaryEntryDto();

        // Assert
        dto.Id.Should().Be(Guid.Empty);
        dto.StudentId.Should().Be(Guid.Empty);
        dto.StudentName.Should().BeNull();
        dto.Type.Should().Be(default); // Default enum value (0, which is not a defined enum value)
        dto.Title.Should().BeEmpty();
        dto.Content.Should().BeEmpty();
        dto.Visibility.Should().Be(default); // Default enum value (0)
        dto.OccurredAt.Should().BeNull();
        dto.IsActive.Should().BeFalse();
        dto.CreatedBy.Should().BeNull();
        dto.CreatedByName.Should().BeNull();
        dto.ModifiedAt.Should().BeNull();
        dto.AttachmentCount.Should().Be(0);
        dto.Attachments.Should().BeNull();
    }

    [Fact]
    public void DiaryEntryDto_SetAllProperties_PropertiesAreSet()
    {
        // Arrange
        var id = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;
        var modifiedAt = DateTime.UtcNow.AddHours(1);
        var occurredAt = DateTime.UtcNow.AddDays(-1);
        var attachments = new List<DiaryAttachmentDto>
        {
            new DiaryAttachmentDto { FileName = "test.pdf" }
        };

        // Act
        var dto = new DiaryEntryDto
        {
            Id = id,
            StudentId = studentId,
            StudentName = "Jan Novák",
            Type = DiaryEntryType.Meeting,
            Title = "Schůzka s rodiči",
            Content = "Diskuze o prospěchu.",
            Visibility = DiaryVisibility.ParentVisible,
            OccurredAt = occurredAt,
            IsActive = true,
            CreatedAt = createdAt,
            CreatedBy = "user123",
            CreatedByName = "Učitel",
            ModifiedAt = modifiedAt,
            AttachmentCount = 1,
            Attachments = attachments
        };

        // Assert
        dto.Id.Should().Be(id);
        dto.StudentId.Should().Be(studentId);
        dto.StudentName.Should().Be("Jan Novák");
        dto.Type.Should().Be(DiaryEntryType.Meeting);
        dto.Title.Should().Be("Schůzka s rodiči");
        dto.Content.Should().Be("Diskuze o prospěchu.");
        dto.Visibility.Should().Be(DiaryVisibility.ParentVisible);
        dto.OccurredAt.Should().Be(occurredAt);
        dto.IsActive.Should().BeTrue();
        dto.CreatedAt.Should().Be(createdAt);
        dto.CreatedBy.Should().Be("user123");
        dto.CreatedByName.Should().Be("Učitel");
        dto.ModifiedAt.Should().Be(modifiedAt);
        dto.AttachmentCount.Should().Be(1);
        dto.Attachments.Should().HaveCount(1);
    }

    [Theory]
    [InlineData(DiaryEntryType.Note)]
    [InlineData(DiaryEntryType.PhoneCall)]
    [InlineData(DiaryEntryType.Meeting)]
    [InlineData(DiaryEntryType.ParentCollaboration)]
    [InlineData(DiaryEntryType.PppSpcCollaboration)]
    public void DiaryEntryDto_AllEntryTypes_CanBeSet(DiaryEntryType entryType)
    {
        // Arrange & Act
        var dto = new DiaryEntryDto
        {
            Type = entryType
        };

        // Assert
        dto.Type.Should().Be(entryType);
    }

    [Theory]
    [InlineData(DiaryVisibility.SchoolOnly)]
    [InlineData(DiaryVisibility.ParentVisible)]
    public void DiaryEntryDto_AllVisibilityLevels_CanBeSet(DiaryVisibility visibility)
    {
        // Arrange & Act
        var dto = new DiaryEntryDto
        {
            Visibility = visibility
        };

        // Assert
        dto.Visibility.Should().Be(visibility);
    }

    [Fact]
    public void DiaryEntryDto_WithMultipleAttachments_AttachmentsAreSet()
    {
        // Arrange
        var attachments = new List<DiaryAttachmentDto>
        {
            new DiaryAttachmentDto { FileName = "doc1.pdf", FileSize = 1024 },
            new DiaryAttachmentDto { FileName = "image1.jpg", FileSize = 2048 },
            new DiaryAttachmentDto { FileName = "doc2.docx", FileSize = 512 }
        };

        // Act
        var dto = new DiaryEntryDto
        {
            Attachments = attachments,
            AttachmentCount = 3
        };

        // Assert
        dto.Attachments.Should().HaveCount(3);
        dto.AttachmentCount.Should().Be(3);
        dto.Attachments.Select(a => a.FileName).Should().Contain(new[] { "doc1.pdf", "image1.jpg", "doc2.docx" });
    }
}
