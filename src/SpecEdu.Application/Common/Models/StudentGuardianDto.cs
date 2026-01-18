using SpecEdu.Domain.Enums;

namespace SpecEdu.Application.Common.Models;

public class StudentGuardianDto
{
    public Guid Id { get; set; }

    public Guid StudentId { get; set; }

    public string? StudentName { get; set; }

    public string ParentUserId { get; set; } = string.Empty;

    public string? ParentName { get; set; }

    public string? ParentEmail { get; set; }

    public RelationshipType RelationshipType { get; set; }

    public string RelationshipTypeName => RelationshipType switch
    {
        RelationshipType.Mother => "Matka",
        RelationshipType.Father => "Otec",
        RelationshipType.LegalGuardian => "Zákonný zástupce",
        RelationshipType.Other => "Jiný",
        _ => "Neznámý"
    };

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
}
