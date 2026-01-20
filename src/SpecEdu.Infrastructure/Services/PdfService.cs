using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Application.Common.Models;
using SpecEdu.Domain.Enums;

namespace SpecEdu.Infrastructure.Services;

/// <summary>
/// Implementation of IPdfService using QuestPDF.
/// Generates PDF documents for PLPP.
/// Czech: Implementace služby pro generování PDF pomocí QuestPDF
/// </summary>
public class PdfService : IPdfService
{
    static PdfService()
    {
        // Configure QuestPDF license (Community license for open source)
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public Task<byte[]> GeneratePlppPdfAsync(
        PlppDto plpp,
        string schoolName,
        string? schoolAddress,
        string? schoolIco,
        bool includeInternalNotes = false)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                page.Header().Element(c => ComposeHeader(c, schoolName, schoolAddress, schoolIco, plpp.SchoolYear));

                page.Content().Element(c => ComposeContent(c, plpp, includeInternalNotes));

                page.Footer().Element(ComposeFooter);
            });
        });

        var bytes = document.GeneratePdf();
        return Task.FromResult(bytes);
    }

    public Task<byte[]> GeneratePlppVersionPdfAsync(
        PlppVersionSnapshot snapshot,
        int versionNumber,
        string schoolName,
        string? schoolAddress,
        string? schoolIco,
        bool includeInternalNotes = false)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                page.Header().Element(c => ComposeHeaderWithVersion(c, schoolName, schoolAddress, schoolIco, snapshot.SchoolYear, versionNumber));

                page.Content().Element(c => ComposeContentFromSnapshot(c, snapshot, includeInternalNotes));

                page.Footer().Element(ComposeFooter);
            });
        });

        var bytes = document.GeneratePdf();
        return Task.FromResult(bytes);
    }

    public string GetPlppPdfFilename(string studentLastName, string schoolYear)
    {
        var safeLastName = string.Join("_", studentLastName.Split(Path.GetInvalidFileNameChars()));
        var safeSchoolYear = schoolYear.Replace("/", "-");
        var date = DateTime.UtcNow.ToString("yyyyMMdd");
        return $"PLPP_{safeLastName}_{safeSchoolYear}_{date}.pdf";
    }

    private static void ComposeHeader(
        IContainer container,
        string schoolName,
        string? schoolAddress,
        string? schoolIco,
        string schoolYear)
    {
        container.Column(column =>
        {
            column.Item().Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text(schoolName).Bold().FontSize(12);
                    if (!string.IsNullOrEmpty(schoolAddress))
                    {
                        c.Item().Text(schoolAddress).FontSize(9);
                    }
                    if (!string.IsNullOrEmpty(schoolIco))
                    {
                        c.Item().Text($"ICO: {schoolIco}").FontSize(9);
                    }
                });

                row.RelativeItem().AlignRight().Column(c =>
                {
                    c.Item().Text($"Skolni rok: {schoolYear}").FontSize(9);
                });
            });

            column.Item().PaddingTop(15).AlignCenter()
                .Text("PLAN PEDAGOGICKE PODPORY")
                .Bold().FontSize(16);

            column.Item().PaddingBottom(10).AlignCenter()
                .Text("(PLPP)")
                .FontSize(12);

            column.Item().LineHorizontal(1).LineColor(Colors.Grey.Medium);
        });
    }

    private static void ComposeHeaderWithVersion(
        IContainer container,
        string schoolName,
        string? schoolAddress,
        string? schoolIco,
        string schoolYear,
        int versionNumber)
    {
        container.Column(column =>
        {
            column.Item().Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text(schoolName).Bold().FontSize(12);
                    if (!string.IsNullOrEmpty(schoolAddress))
                    {
                        c.Item().Text(schoolAddress).FontSize(9);
                    }
                    if (!string.IsNullOrEmpty(schoolIco))
                    {
                        c.Item().Text($"ICO: {schoolIco}").FontSize(9);
                    }
                });

                row.RelativeItem().AlignRight().Column(c =>
                {
                    c.Item().Text($"Skolni rok: {schoolYear}").FontSize(9);
                    c.Item().Text($"Verze: {versionNumber}").FontSize(9).Italic();
                });
            });

            column.Item().PaddingTop(15).AlignCenter()
                .Text("PLAN PEDAGOGICKE PODPORY")
                .Bold().FontSize(16);

            column.Item().PaddingBottom(10).AlignCenter()
                .Text("(PLPP)")
                .FontSize(12);

            column.Item().LineHorizontal(1).LineColor(Colors.Grey.Medium);
        });
    }

    private static void ComposeContent(IContainer container, PlppDto plpp, bool includeInternalNotes)
    {
        container.PaddingTop(10).Column(column =>
        {
            // Basic info table
            column.Item().Element(c => ComposeBasicInfoTable(c, plpp.StudentName ?? "N/A", plpp.SupportLevel, plpp.ValidFrom, plpp.ValidTo, plpp.Status));

            // Strengths
            if (!string.IsNullOrEmpty(plpp.Strengths))
            {
                column.Item().Element(c => ComposeSection(c, "Silne stranky zaka", plpp.Strengths));
            }

            // Areas needing support
            if (!string.IsNullOrEmpty(plpp.AreasNeedingSupport))
            {
                column.Item().Element(c => ComposeSection(c, "Oblasti vyzadujici podporu", plpp.AreasNeedingSupport));
            }

            // Recommended methods
            if (!string.IsNullOrEmpty(plpp.RecommendedMethods))
            {
                column.Item().Element(c => ComposeSection(c, "Doporucene metody a pristupy", plpp.RecommendedMethods));
            }

            // Organizational adjustments
            if (!string.IsNullOrEmpty(plpp.OrganizationalAdjustments))
            {
                column.Item().Element(c => ComposeSection(c, "Organizacni upravy", plpp.OrganizationalAdjustments));
            }

            // Content adjustments
            if (!string.IsNullOrEmpty(plpp.ContentAdjustments))
            {
                column.Item().Element(c => ComposeSection(c, "Obsahove upravy", plpp.ContentAdjustments));
            }

            // Assessment methods
            if (!string.IsNullOrEmpty(plpp.AssessmentMethods))
            {
                column.Item().Element(c => ComposeSection(c, "Zpusob hodnoceni", plpp.AssessmentMethods));
            }

            // Parent collaboration
            if (!string.IsNullOrEmpty(plpp.ParentCollaboration))
            {
                column.Item().Element(c => ComposeSection(c, "Spoluprace s rodici", plpp.ParentCollaboration));
            }

            // Goals table
            if (plpp.Goals.Count > 0)
            {
                column.Item().Element(c => ComposeGoalsTable(c, plpp.Goals.ToList()));
            }

            // Evaluations
            if (plpp.Evaluations.Count > 0)
            {
                column.Item().Element(c => ComposeEvaluations(c, plpp.Evaluations.ToList()));
            }

            // Internal notes (staff only)
            if (includeInternalNotes && !string.IsNullOrEmpty(plpp.InternalNotes))
            {
                column.Item().Element(c => ComposeSection(c, "Interni poznamky (pouze pro personál)", plpp.InternalNotes));
            }

            // Signature lines
            column.Item().Element(ComposeSignatures);
        });
    }

    private static void ComposeContentFromSnapshot(IContainer container, PlppVersionSnapshot snapshot, bool includeInternalNotes)
    {
        container.PaddingTop(10).Column(column =>
        {
            // Basic info table
            column.Item().Element(c => ComposeBasicInfoTable(c, snapshot.StudentName ?? "N/A", snapshot.SupportLevel, snapshot.ValidFrom, snapshot.ValidTo, snapshot.Status));

            // Strengths
            if (!string.IsNullOrEmpty(snapshot.Strengths))
            {
                column.Item().Element(c => ComposeSection(c, "Silne stranky zaka", snapshot.Strengths));
            }

            // Areas needing support
            if (!string.IsNullOrEmpty(snapshot.AreasNeedingSupport))
            {
                column.Item().Element(c => ComposeSection(c, "Oblasti vyzadujici podporu", snapshot.AreasNeedingSupport));
            }

            // Recommended methods
            if (!string.IsNullOrEmpty(snapshot.RecommendedMethods))
            {
                column.Item().Element(c => ComposeSection(c, "Doporucene metody a pristupy", snapshot.RecommendedMethods));
            }

            // Organizational adjustments
            if (!string.IsNullOrEmpty(snapshot.OrganizationalAdjustments))
            {
                column.Item().Element(c => ComposeSection(c, "Organizacni upravy", snapshot.OrganizationalAdjustments));
            }

            // Content adjustments
            if (!string.IsNullOrEmpty(snapshot.ContentAdjustments))
            {
                column.Item().Element(c => ComposeSection(c, "Obsahove upravy", snapshot.ContentAdjustments));
            }

            // Assessment methods
            if (!string.IsNullOrEmpty(snapshot.AssessmentMethods))
            {
                column.Item().Element(c => ComposeSection(c, "Zpusob hodnoceni", snapshot.AssessmentMethods));
            }

            // Parent collaboration
            if (!string.IsNullOrEmpty(snapshot.ParentCollaboration))
            {
                column.Item().Element(c => ComposeSection(c, "Spoluprace s rodici", snapshot.ParentCollaboration));
            }

            // Goals table
            if (snapshot.Goals.Count > 0)
            {
                column.Item().Element(c => ComposeGoalsTableFromSnapshot(c, snapshot.Goals.ToList()));
            }

            // Internal notes (staff only)
            if (includeInternalNotes && !string.IsNullOrEmpty(snapshot.InternalNotes))
            {
                column.Item().Element(c => ComposeSection(c, "Interni poznamky (pouze pro personál)", snapshot.InternalNotes));
            }

            // Signature lines
            column.Item().Element(ComposeSignatures);
        });
    }

    private static void ComposeBasicInfoTable(
        IContainer container,
        string studentName,
        SupportMeasureLevel supportLevel,
        DateTime validFrom,
        DateTime validTo,
        PlppStatus status)
    {
        container.PaddingVertical(10).Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(1);
                columns.RelativeColumn(2);
                columns.RelativeColumn(1);
                columns.RelativeColumn(2);
            });

            // Row 1
            table.Cell().Border(1).Padding(5).Background(Colors.Grey.Lighten3)
                .Text("Jmeno zaka:").Bold().FontSize(9);
            table.Cell().Border(1).Padding(5)
                .Text(studentName).FontSize(9);
            table.Cell().Border(1).Padding(5).Background(Colors.Grey.Lighten3)
                .Text("Stav:").Bold().FontSize(9);
            table.Cell().Border(1).Padding(5)
                .Text(GetStatusText(status)).FontSize(9);

            // Row 2
            table.Cell().Border(1).Padding(5).Background(Colors.Grey.Lighten3)
                .Text("Stupen podpory:").Bold().FontSize(9);
            table.Cell().Border(1).Padding(5)
                .Text(GetSupportLevelText(supportLevel)).FontSize(9);
            table.Cell().Border(1).Padding(5).Background(Colors.Grey.Lighten3)
                .Text("Platnost:").Bold().FontSize(9);
            table.Cell().Border(1).Padding(5)
                .Text($"{validFrom:dd.MM.yyyy} - {validTo:dd.MM.yyyy}").FontSize(9);
        });
    }

    private static void ComposeSection(IContainer container, string title, string content)
    {
        container.PaddingVertical(5).Column(column =>
        {
            column.Item().Text(title).Bold().FontSize(11);
            column.Item().PaddingTop(3).PaddingLeft(10).Text(content).FontSize(10);
        });
    }

    private static void ComposeGoalsTable(IContainer container, List<PlppGoalDto> goals)
    {
        container.PaddingVertical(10).Column(column =>
        {
            column.Item().Text("Cile planu").Bold().FontSize(11);

            column.Item().PaddingTop(5).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(30);
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                });

                // Header
                table.Header(header =>
                {
                    header.Cell().Border(1).Padding(3).Background(Colors.Grey.Lighten3)
                        .Text("#").Bold().FontSize(9);
                    header.Cell().Border(1).Padding(3).Background(Colors.Grey.Lighten3)
                        .Text("Popis cile").Bold().FontSize(9);
                    header.Cell().Border(1).Padding(3).Background(Colors.Grey.Lighten3)
                        .Text("Predmet").Bold().FontSize(9);
                    header.Cell().Border(1).Padding(3).Background(Colors.Grey.Lighten3)
                        .Text("Stav").Bold().FontSize(9);
                });

                var index = 1;
                foreach (var goal in goals.OrderBy(g => g.Order))
                {
                    table.Cell().Border(1).Padding(3).Text(index.ToString()).FontSize(9);
                    table.Cell().Border(1).Padding(3).Text(goal.Description).FontSize(9);
                    table.Cell().Border(1).Padding(3).Text(goal.Subject ?? "-").FontSize(9);
                    table.Cell().Border(1).Padding(3).Text(GetGoalStatusText(goal.Status)).FontSize(9);
                    index++;
                }
            });
        });
    }

    private static void ComposeGoalsTableFromSnapshot(IContainer container, List<PlppGoalSnapshot> goals)
    {
        container.PaddingVertical(10).Column(column =>
        {
            column.Item().Text("Cile planu").Bold().FontSize(11);

            column.Item().PaddingTop(5).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(30);
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                });

                // Header
                table.Header(header =>
                {
                    header.Cell().Border(1).Padding(3).Background(Colors.Grey.Lighten3)
                        .Text("#").Bold().FontSize(9);
                    header.Cell().Border(1).Padding(3).Background(Colors.Grey.Lighten3)
                        .Text("Popis cile").Bold().FontSize(9);
                    header.Cell().Border(1).Padding(3).Background(Colors.Grey.Lighten3)
                        .Text("Predmet").Bold().FontSize(9);
                    header.Cell().Border(1).Padding(3).Background(Colors.Grey.Lighten3)
                        .Text("Stav").Bold().FontSize(9);
                });

                var index = 1;
                foreach (var goal in goals.OrderBy(g => g.Order))
                {
                    table.Cell().Border(1).Padding(3).Text(index.ToString()).FontSize(9);
                    table.Cell().Border(1).Padding(3).Text(goal.Description).FontSize(9);
                    table.Cell().Border(1).Padding(3).Text(goal.Subject ?? "-").FontSize(9);
                    table.Cell().Border(1).Padding(3).Text(GetGoalStatusText(goal.Status)).FontSize(9);
                    index++;
                }
            });
        });
    }

    private static void ComposeEvaluations(IContainer container, List<PlppEvaluationDto> evaluations)
    {
        container.PaddingVertical(10).Column(column =>
        {
            column.Item().Text("Mesicni hodnoceni").Bold().FontSize(11);

            foreach (var eval in evaluations.OrderByDescending(e => e.EvaluationMonth).Take(3))
            {
                column.Item().PaddingTop(5).Border(1).Padding(5).Column(evalColumn =>
                {
                    evalColumn.Item().Text($"Mesic: {eval.EvaluationMonth:MM/yyyy}")
                        .Bold().FontSize(10);

                    if (!string.IsNullOrEmpty(eval.WhatStudentManages))
                    {
                        evalColumn.Item().PaddingTop(3).Text($"Co zak zvlada: {eval.WhatStudentManages}").FontSize(9);
                    }
                    if (!string.IsNullOrEmpty(eval.WhatNeedsImprovement))
                    {
                        evalColumn.Item().PaddingTop(2).Text($"Co potrebuje zlepsit: {eval.WhatNeedsImprovement}").FontSize(9);
                    }
                    if (eval.ProgressRating.HasValue)
                    {
                        evalColumn.Item().PaddingTop(2).Text($"Hodnoceni: {eval.ProgressRating}/5").FontSize(9);
                    }
                });
            }
        });
    }

    private static void ComposeSignatures(IContainer container)
    {
        container.PaddingTop(30).Row(row =>
        {
            row.RelativeItem().Column(c =>
            {
                c.Item().LineHorizontal(1);
                c.Item().PaddingTop(3).AlignCenter().Text("Trídní ucitel / Specpedagog").FontSize(9);
            });

            row.ConstantItem(50);

            row.RelativeItem().Column(c =>
            {
                c.Item().LineHorizontal(1);
                c.Item().PaddingTop(3).AlignCenter().Text("Zakonny zastupce").FontSize(9);
            });
        });
    }

    private static void ComposeFooter(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().AlignLeft().Text(text =>
            {
                text.Span("Vygenerovano: ").FontSize(8);
                text.Span(DateTime.UtcNow.ToString("dd.MM.yyyy HH:mm")).FontSize(8);
            });

            row.RelativeItem().AlignRight().Text(text =>
            {
                text.Span("Strana ").FontSize(8);
                text.CurrentPageNumber().FontSize(8);
                text.Span(" z ").FontSize(8);
                text.TotalPages().FontSize(8);
            });
        });
    }

    private static string GetStatusText(PlppStatus status)
    {
        return status switch
        {
            PlppStatus.Draft => "Koncept",
            PlppStatus.Active => "Aktivni",
            PlppStatus.UnderReview => "K revizi",
            PlppStatus.Archived => "Archivovano",
            _ => status.ToString()
        };
    }

    private static string GetSupportLevelText(SupportMeasureLevel level)
    {
        return level switch
        {
            SupportMeasureLevel.Level1 => "1. stupen",
            SupportMeasureLevel.Level2 => "2. stupen",
            SupportMeasureLevel.Level3 => "3. stupen",
            SupportMeasureLevel.Level4 => "4. stupen",
            SupportMeasureLevel.Level5 => "5. stupen",
            _ => level.ToString()
        };
    }

    private static string GetGoalStatusText(GoalStatus status)
    {
        return status switch
        {
            GoalStatus.NotStarted => "Nezahajeno",
            GoalStatus.InProgress => "Probiha",
            GoalStatus.Achieved => "Splneno",
            GoalStatus.PartiallyAchieved => "Castecne",
            GoalStatus.NotAchieved => "Nesplneno",
            _ => status.ToString()
        };
    }
}
