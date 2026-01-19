using FluentAssertions;
using SpecEdu.Domain.Entities;
using Xunit;

namespace SpecEdu.Domain.Tests.Entities;

/// <summary>
/// Tests for the DiaryAttachment entity.
/// </summary>
public class DiaryAttachmentTests
{
    [Fact]
    public void DiaryAttachment_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var attachment = new DiaryAttachment();

        // Assert
        attachment.FileName.Should().BeEmpty();
        attachment.ContentType.Should().BeEmpty();
        attachment.FileData.Should().BeEmpty();
        attachment.FileSize.Should().Be(0);
        attachment.DiaryEntry.Should().BeNull();
    }

    [Fact]
    public void DiaryAttachment_SetAllProperties_PropertiesAreSet()
    {
        // Arrange
        var entryId = Guid.NewGuid();
        var fileData = new byte[] { 0x25, 0x50, 0x44, 0x46 }; // PDF magic bytes

        // Act
        var attachment = new DiaryAttachment
        {
            DiaryEntryId = entryId,
            FileName = "document.pdf",
            ContentType = "application/pdf",
            FileData = fileData,
            FileSize = fileData.Length
        };

        // Assert
        attachment.DiaryEntryId.Should().Be(entryId);
        attachment.FileName.Should().Be("document.pdf");
        attachment.ContentType.Should().Be("application/pdf");
        attachment.FileData.Should().BeEquivalentTo(fileData);
        attachment.FileSize.Should().Be(4);
    }

    [Theory]
    [InlineData("document.pdf", "application/pdf")]
    [InlineData("image.jpg", "image/jpeg")]
    [InlineData("image.png", "image/png")]
    [InlineData("spreadsheet.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    [InlineData("document.docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
    public void DiaryAttachment_CommonFileTypes_CanBeStored(string fileName, string contentType)
    {
        // Arrange & Act
        var attachment = new DiaryAttachment
        {
            FileName = fileName,
            ContentType = contentType
        };

        // Assert
        attachment.FileName.Should().Be(fileName);
        attachment.ContentType.Should().Be(contentType);
    }

    [Fact]
    public void DiaryAttachment_WithDiaryEntry_NavigationPropertyIsSet()
    {
        // Arrange
        var entry = new DiaryEntry
        {
            Title = "Test Entry"
        };

        // Act
        var attachment = new DiaryAttachment
        {
            DiaryEntry = entry,
            FileName = "test.pdf"
        };

        // Assert
        attachment.DiaryEntry.Should().Be(entry);
    }

    [Fact]
    public void DiaryAttachment_LargeFile_FileSizeIsCorrect()
    {
        // Arrange
        var largeFileData = new byte[5 * 1024 * 1024]; // 5 MB
        new Random().NextBytes(largeFileData);

        // Act
        var attachment = new DiaryAttachment
        {
            FileName = "large-file.pdf",
            ContentType = "application/pdf",
            FileData = largeFileData,
            FileSize = largeFileData.Length
        };

        // Assert
        attachment.FileSize.Should().Be(5 * 1024 * 1024);
        attachment.FileData.Should().HaveCount(5 * 1024 * 1024);
    }

    [Fact]
    public void DiaryAttachment_EmptyFile_HasZeroSize()
    {
        // Arrange & Act
        var attachment = new DiaryAttachment
        {
            FileName = "empty.txt",
            ContentType = "text/plain",
            FileData = Array.Empty<byte>(),
            FileSize = 0
        };

        // Assert
        attachment.FileSize.Should().Be(0);
        attachment.FileData.Should().BeEmpty();
    }
}
