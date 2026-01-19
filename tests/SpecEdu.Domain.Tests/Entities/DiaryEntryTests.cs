using FluentAssertions;
using SpecEdu.Domain.Entities;
using SpecEdu.Domain.Enums;
using Xunit;

namespace SpecEdu.Domain.Tests.Entities;

/// <summary>
/// Tests for the DiaryEntry entity.
/// </summary>
public class DiaryEntryTests
{
    [Fact]
    public void DiaryEntry_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var entry = new DiaryEntry();

        // Assert
        entry.Title.Should().BeEmpty();
        entry.Content.Should().BeEmpty();
        entry.Visibility.Should().Be(DiaryVisibility.SchoolOnly);
        entry.IsActive.Should().BeTrue();
        entry.OccurredAt.Should().BeNull();
        entry.Student.Should().BeNull();
        entry.Attachments.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void DiaryEntry_SetAllProperties_PropertiesAreSet()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var occurredAt = DateTime.UtcNow.AddDays(-1);

        // Act
        var entry = new DiaryEntry
        {
            StudentId = studentId,
            Type = DiaryEntryType.Meeting,
            Title = "Schůzka s rodiči",
            Content = "Probírali jsme prospěch žáka.",
            Visibility = DiaryVisibility.ParentVisible,
            OccurredAt = occurredAt,
            IsActive = true
        };

        // Assert
        entry.StudentId.Should().Be(studentId);
        entry.Type.Should().Be(DiaryEntryType.Meeting);
        entry.Title.Should().Be("Schůzka s rodiči");
        entry.Content.Should().Be("Probírali jsme prospěch žáka.");
        entry.Visibility.Should().Be(DiaryVisibility.ParentVisible);
        entry.OccurredAt.Should().Be(occurredAt);
        entry.IsActive.Should().BeTrue();
    }

    [Theory]
    [InlineData(DiaryEntryType.Note)]
    [InlineData(DiaryEntryType.PhoneCall)]
    [InlineData(DiaryEntryType.Meeting)]
    [InlineData(DiaryEntryType.ParentCollaboration)]
    [InlineData(DiaryEntryType.PppSpcCollaboration)]
    public void DiaryEntry_AllEntryTypes_CanBeSet(DiaryEntryType entryType)
    {
        // Arrange & Act
        var entry = new DiaryEntry
        {
            Type = entryType
        };

        // Assert
        entry.Type.Should().Be(entryType);
    }

    [Theory]
    [InlineData(DiaryVisibility.SchoolOnly)]
    [InlineData(DiaryVisibility.ParentVisible)]
    public void DiaryEntry_AllVisibilityLevels_CanBeSet(DiaryVisibility visibility)
    {
        // Arrange & Act
        var entry = new DiaryEntry
        {
            Visibility = visibility
        };

        // Assert
        entry.Visibility.Should().Be(visibility);
    }

    [Fact]
    public void DiaryEntry_AddAttachment_AttachmentIsAdded()
    {
        // Arrange
        var entry = new DiaryEntry();
        var attachment = new DiaryAttachment
        {
            FileName = "test.pdf",
            ContentType = "application/pdf"
        };

        // Act
        entry.Attachments.Add(attachment);

        // Assert
        entry.Attachments.Should().ContainSingle();
        entry.Attachments.First().Should().Be(attachment);
    }

    [Fact]
    public void DiaryEntry_MultipleAttachments_AllAttachmentsAdded()
    {
        // Arrange
        var entry = new DiaryEntry();
        var attachments = new[]
        {
            new DiaryAttachment { FileName = "doc1.pdf", ContentType = "application/pdf" },
            new DiaryAttachment { FileName = "image1.jpg", ContentType = "image/jpeg" },
            new DiaryAttachment { FileName = "doc2.docx", ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document" }
        };

        // Act
        foreach (var attachment in attachments)
        {
            entry.Attachments.Add(attachment);
        }

        // Assert
        entry.Attachments.Should().HaveCount(3);
        entry.Attachments.Select(a => a.FileName).Should().Contain(new[] { "doc1.pdf", "image1.jpg", "doc2.docx" });
    }

    [Fact]
    public void DiaryEntry_SoftDelete_SetsIsActiveToFalse()
    {
        // Arrange
        var entry = new DiaryEntry
        {
            Title = "Test Entry",
            Content = "Test Content",
            IsActive = true
        };

        // Act
        entry.IsActive = false;

        // Assert
        entry.IsActive.Should().BeFalse();
    }

    [Fact]
    public void DiaryEntry_WithStudent_NavigationPropertyIsSet()
    {
        // Arrange
        var student = new Student
        {
            FirstName = "Jan",
            LastName = "Novák"
        };

        // Act
        var entry = new DiaryEntry
        {
            Student = student,
            Title = "Test Entry"
        };

        // Assert
        entry.Student.Should().Be(student);
        entry.Student!.FullName.Should().Be("Jan Novák");
    }
}
