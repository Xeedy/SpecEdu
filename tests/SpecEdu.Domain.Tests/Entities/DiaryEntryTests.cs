using FluentAssertions;
using SpecEdu.Domain.Entities;
using SpecEdu.Domain.Enums;
using Xunit;

namespace SpecEdu.Domain.Tests.Entities;

public class DiaryEntryTests
{
    [Fact]
    public void DiaryEntry_DefaultValues_AreCorrect()
    {
        var entry = new DiaryEntry();

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
        var studentId = Guid.NewGuid();
        var occurredAt = DateTime.UtcNow.AddDays(-1);

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
        var entry = new DiaryEntry
        {
            Type = entryType
        };

        entry.Type.Should().Be(entryType);
    }

    [Theory]
    [InlineData(DiaryVisibility.SchoolOnly)]
    [InlineData(DiaryVisibility.ParentVisible)]
    public void DiaryEntry_AllVisibilityLevels_CanBeSet(DiaryVisibility visibility)
    {
        var entry = new DiaryEntry
        {
            Visibility = visibility
        };

        entry.Visibility.Should().Be(visibility);
    }

    [Fact]
    public void DiaryEntry_AddAttachment_AttachmentIsAdded()
    {
        var entry = new DiaryEntry();
        var attachment = new DiaryAttachment
        {
            FileName = "test.pdf",
            ContentType = "application/pdf"
        };

        entry.Attachments.Add(attachment);

        entry.Attachments.Should().ContainSingle();
        entry.Attachments.First().Should().Be(attachment);
    }

    [Fact]
    public void DiaryEntry_MultipleAttachments_AllAttachmentsAdded()
    {
        var entry = new DiaryEntry();
        var attachments = new[]
        {
            new DiaryAttachment { FileName = "doc1.pdf", ContentType = "application/pdf" },
            new DiaryAttachment { FileName = "image1.jpg", ContentType = "image/jpeg" },
            new DiaryAttachment { FileName = "doc2.docx", ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document" }
        };

        foreach (var attachment in attachments)
        {
            entry.Attachments.Add(attachment);
        }

        entry.Attachments.Should().HaveCount(3);
        entry.Attachments.Select(a => a.FileName).Should().Contain(new[] { "doc1.pdf", "image1.jpg", "doc2.docx" });
    }

    [Fact]
    public void DiaryEntry_SoftDelete_SetsIsActiveToFalse()
    {
        var entry = new DiaryEntry
        {
            Title = "Test Entry",
            Content = "Test Content",
            IsActive = true
        };

        entry.IsActive = false;

        entry.IsActive.Should().BeFalse();
    }

    [Fact]
    public void DiaryEntry_WithStudent_NavigationPropertyIsSet()
    {
        var student = new Student
        {
            FirstName = "Jan",
            LastName = "Novák"
        };

        var entry = new DiaryEntry
        {
            Student = student,
            Title = "Test Entry"
        };

        entry.Student.Should().Be(student);
        entry.Student!.FullName.Should().Be("Jan Novák");
    }
}
