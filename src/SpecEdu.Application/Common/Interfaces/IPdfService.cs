using SpecEdu.Application.Common.Models;

namespace SpecEdu.Application.Common.Interfaces;

public interface IPdfService
{
    Task<byte[]> GeneratePlppPdfAsync(
        PlppDto plpp,
        string schoolName,
        string? schoolAddress,
        string? schoolIco,
        bool includeInternalNotes = false);

    Task<byte[]> GeneratePlppVersionPdfAsync(
        PlppVersionSnapshot snapshot,
        int versionNumber,
        string schoolName,
        string? schoolAddress,
        string? schoolIco,
        bool includeInternalNotes = false);

    string GetPlppPdfFilename(string studentLastName, string schoolYear);
}
