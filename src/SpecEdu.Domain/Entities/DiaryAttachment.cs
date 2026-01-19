using SpecEdu.Domain.Common;

namespace SpecEdu.Domain.Entities;

/// <summary>
/// Represents a file attachment for a diary entry.
/// Czech: Příloha záznamu v deníku
/// </summary>
public class DiaryAttachment : AuditableEntity
{
    /// <summary>
    /// Foreign key to the diary entry this attachment belongs to.
    /// Czech: Záznam, ke kterému příloha patří
    /// </summary>
    public Guid DiaryEntryId { get; set; }

    /// <summary>
    /// Original file name.
    /// Czech: Název souboru
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// MIME content type of the file.
    /// Czech: Typ obsahu
    /// </summary>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// File content stored as binary data.
    /// Czech: Obsah souboru
    /// </summary>
    public byte[] FileData { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// File size in bytes.
    /// Czech: Velikost souboru v bajtech
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// Navigation property to the diary entry.
    /// </summary>
    public DiaryEntry? DiaryEntry { get; set; }
}
