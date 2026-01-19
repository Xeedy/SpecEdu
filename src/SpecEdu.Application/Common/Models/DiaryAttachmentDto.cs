namespace SpecEdu.Application.Common.Models;

/// <summary>
/// Data transfer object for DiaryAttachment entity.
/// Used to display attachment metadata (not file content).
/// </summary>
public class DiaryAttachmentDto
{
    /// <summary>
    /// Unique identifier of the attachment.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// ID of the diary entry this attachment belongs to.
    /// </summary>
    public Guid DiaryEntryId { get; set; }

    /// <summary>
    /// Original file name.
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// MIME content type of the file.
    /// </summary>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// File size in bytes.
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// Human-readable file size (e.g., "1.5 MB").
    /// </summary>
    public string FileSizeFormatted => FormatFileSize(FileSize);

    /// <summary>
    /// UTC timestamp when the attachment was uploaded.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    private static string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len /= 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }
}
