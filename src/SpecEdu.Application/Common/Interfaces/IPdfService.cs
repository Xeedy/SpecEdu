using SpecEdu.Application.Common.Models;

namespace SpecEdu.Application.Common.Interfaces;

/// <summary>
/// Service interface for PDF document generation.
/// Czech: Rozhraní služby pro generování PDF dokumentů
/// </summary>
public interface IPdfService
{
    /// <summary>
    /// Generates a PDF document for a PLPP.
    /// Czech: Generuje PDF dokument pro PLPP
    /// </summary>
    /// <param name="plpp">The PLPP data to render.</param>
    /// <param name="schoolName">Name of the school for header.</param>
    /// <param name="schoolAddress">School address for header.</param>
    /// <param name="schoolIco">School IČO (identification number).</param>
    /// <param name="includeInternalNotes">Whether to include internal notes (staff only).</param>
    /// <returns>PDF document as byte array.</returns>
    Task<byte[]> GeneratePlppPdfAsync(
        PlppDto plpp,
        string schoolName,
        string? schoolAddress,
        string? schoolIco,
        bool includeInternalNotes = false);

    /// <summary>
    /// Generates a PDF document from a PLPP version snapshot.
    /// Czech: Generuje PDF dokument ze snímku verze PLPP
    /// </summary>
    /// <param name="snapshot">The version snapshot data.</param>
    /// <param name="versionNumber">The version number for display.</param>
    /// <param name="schoolName">Name of the school for header.</param>
    /// <param name="schoolAddress">School address for header.</param>
    /// <param name="schoolIco">School IČO (identification number).</param>
    /// <param name="includeInternalNotes">Whether to include internal notes (staff only).</param>
    /// <returns>PDF document as byte array.</returns>
    Task<byte[]> GeneratePlppVersionPdfAsync(
        PlppVersionSnapshot snapshot,
        int versionNumber,
        string schoolName,
        string? schoolAddress,
        string? schoolIco,
        bool includeInternalNotes = false);

    /// <summary>
    /// Gets the suggested filename for a PLPP PDF.
    /// Format: PLPP_{LastName}_{SchoolYear}_{Date}.pdf
    /// Czech: Vrátí doporučený název souboru pro PLPP PDF
    /// </summary>
    string GetPlppPdfFilename(string studentLastName, string schoolYear);
}
