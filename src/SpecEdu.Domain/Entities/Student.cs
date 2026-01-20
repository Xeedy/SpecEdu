using SpecEdu.Domain.Common;

namespace SpecEdu.Domain.Entities;

/// <summary>
/// Represents a student with special educational needs.
/// Czech: Žák se speciálními vzdělávacími potřebami (SVP)
/// </summary>
public class Student : AuditableEntity
{
    /// <summary>
    /// Foreign key to the school this student belongs to.
    /// Czech: Škola, které žák patří
    /// </summary>
    public Guid SchoolId { get; set; }

    /// <summary>
    /// Student's first name.
    /// Czech: Křestní jméno
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Student's last name.
    /// Czech: Příjmení
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Student's date of birth.
    /// Czech: Datum narození
    /// </summary>
    public DateTime? BirthDate { get; set; }

    /// <summary>
    /// Class/grade the student is in.
    /// Czech: Třída
    /// </summary>
    public string? Class { get; set; }

    /// <summary>
    /// Optional photo identifier for the student.
    /// Czech: ID fotografie
    /// </summary>
    public Guid? PhotoId { get; set; }

    /// <summary>
    /// Indicates whether the student is active.
    /// Inactive students are soft-deleted.
    /// Czech: Aktivní stav žáka
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Navigation property to the school.
    /// </summary>
    public School? School { get; set; }

    /// <summary>
    /// Navigation property to guardian relationships.
    /// </summary>
    public ICollection<StudentGuardian> Guardians { get; set; } = new List<StudentGuardian>();

    /// <summary>
    /// Navigation property to staff link relationships.
    /// </summary>
    public ICollection<StudentStaffLink> StaffLinks { get; set; } = new List<StudentStaffLink>();

    /// <summary>
    /// Navigation property to diary entries.
    /// </summary>
    public ICollection<DiaryEntry> DiaryEntries { get; set; } = new List<DiaryEntry>();

    /// <summary>
    /// Navigation property to pedagogical support plans.
    /// Czech: Plány pedagogické podpory
    /// </summary>
    public ICollection<Plpp> Plpps { get; set; } = new List<Plpp>();

    /// <summary>
    /// Gets the student's full name.
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();
}
