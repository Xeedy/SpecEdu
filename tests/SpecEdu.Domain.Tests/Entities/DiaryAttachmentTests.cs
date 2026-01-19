using FluentAssertions;
using SpecEdu.Domain.Entities;
using Xunit;

namespace SpecEdu.Domain.Tests.Entities;

public class DiaryAttachmentTests
{
    [Fact]
    public void DiaryAttachment_DefaultValues_AreCorrect()
    {
        var attachment = new DiaryAttachment();

        attachment.FileName.Should().BeEmpty();
        attachment.ContentType.Should().BeEmpty();
        attachment.FileData.Should().BeEmpty();
        attachment.FileSize.Should().Be(0);
        attachment.DiaryEntry.Should().BeNull();
    }

    [Fact]
    public void DiaryAttachment_SetAllProperties_PropertiesAreSet()
    {
        var entryId = Guid.NewGuid();
        var fileData = new byte[] { 0x25, 0x50, 0x44, 0x46 };

        var attachment = new DiaryAttachment
        {
            DiaryEntryId = entryId,
            FileName = "document.pdf",
            ContentType = "application/pdf",
            FileData = fileData,
            FileSize = fileData.Length
        };

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
        var attachment = new DiaryAttachment
        {
            FileName = fileName,
            ContentType = contentType
        };

        attachment.FileName.Should().Be(fileName);
        attachment.ContentType.Should().Be(contentType);
    }

    [Fact]
    public void DiaryAttachment_WithDiaryEntry_NavigationPropertyIsSet()
    {
        var entry = new DiaryEntry
        {
            Title = "Test Entry"
        };

        var attachment = new DiaryAttachment
        {
            DiaryEntry = entry,
            FileName = "test.pdf"
        };

        attachment.DiaryEntry.Should().Be(entry);
    }

    [Fact]
    public void DiaryAttachment_LargeFile_FileSizeIsCorrect()
    {
        var largeFileData = new byte[5 * 1024 * 1024];
        new Random().NextBytes(largeFileData);

        var attachment = new DiaryAttachment
        {
            FileName = "large-file.pdf",
            ContentType = "application/pdf",
            FileData = largeFileData,
            FileSize = largeFileData.Length
        };

        attachment.FileSize.Should().Be(5 * 1024 * 1024);
        attachment.FileData.Should().HaveCount(5 * 1024 * 1024);
    }

    [Fact]
    public void DiaryAttachment_EmptyFile_HasZeroSize()
    {
        var attachment = new DiaryAttachment
        {
            FileName = "empty.txt",
            ContentType = "text/plain",
            FileData = Array.Empty<byte>(),
            FileSize = 0
        };

        attachment.FileSize.Should().Be(0);
        attachment.FileData.Should().BeEmpty();
    }
}
