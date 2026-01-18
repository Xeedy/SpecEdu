namespace SpecEdu.Application.Common.Models;

/// <summary>
/// Data transfer object for Student entity.
/// Used to transfer student data without exposing internal implementation.
/// </summary>
public class StudentDto
{
    /// <summary>
    /// Unique identifier of the student.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// ID of the school this student belongs to.
    /// </summary>
    public Guid SchoolId { get; set; }

    /// <summary>
    /// Name of the school (populated when needed).
    /// </summary>
    public string? SchoolName { get; set; }

    /// <summary>
    /// Student's first name.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Student's last name.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Student's full name.
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();

    /// <summary>
    /// Student's date of birth.
    /// </summary>
    public DateTime? BirthDate { get; set; }

    /// <summary>
    /// Class/grade the student is in.
    /// </summary>
    public string? Class { get; set; }

    /// <summary>
    /// Optional photo identifier for the student.
    /// </summary>
    public Guid? PhotoId { get; set; }

    /// <summary>
    /// Indicates whether the student is active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// UTC timestamp when the student was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Number of guardians linked to this student (optional).
    /// </summary>
    public int? GuardianCount { get; set; }

    /// <summary>
    /// Number of staff links for this student (optional).
    /// </summary>
    public int? StaffLinkCount { get; set; }
}
