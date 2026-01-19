using FluentAssertions;
using SpecEdu.Domain.Entities;
using Xunit;

namespace SpecEdu.Domain.Tests.Entities;

/// <summary>
/// Tests for the Student entity.
/// </summary>
public class StudentTests
{
    [Fact]
    public void FullName_WithFirstAndLastName_ReturnsFormattedName()
    {
        // Arrange
        var student = new Student
        {
            FirstName = "Jan",
            LastName = "Novák"
        };

        // Act
        var fullName = student.FullName;

        // Assert
        fullName.Should().Be("Jan Novák");
    }

    [Fact]
    public void FullName_WithOnlyFirstName_ReturnsTrimmedName()
    {
        // Arrange
        var student = new Student
        {
            FirstName = "Jan",
            LastName = ""
        };

        // Act
        var fullName = student.FullName;

        // Assert
        fullName.Should().Be("Jan");
    }

    [Fact]
    public void FullName_WithOnlyLastName_ReturnsTrimmedName()
    {
        // Arrange
        var student = new Student
        {
            FirstName = "",
            LastName = "Novák"
        };

        // Act
        var fullName = student.FullName;

        // Assert
        fullName.Should().Be("Novák");
    }

    [Fact]
    public void FullName_WithEmptyNames_ReturnsEmptyString()
    {
        // Arrange
        var student = new Student
        {
            FirstName = "",
            LastName = ""
        };

        // Act
        var fullName = student.FullName;

        // Assert
        fullName.Should().BeEmpty();
    }

    [Fact]
    public void Student_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var student = new Student();

        // Assert
        student.FirstName.Should().BeEmpty();
        student.LastName.Should().BeEmpty();
        student.IsActive.Should().BeTrue();
        student.BirthDate.Should().BeNull();
        student.Class.Should().BeNull();
        student.PhotoId.Should().BeNull();
        student.Guardians.Should().NotBeNull().And.BeEmpty();
        student.StaffLinks.Should().NotBeNull().And.BeEmpty();
        student.DiaryEntries.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void Student_SetProperties_PropertiesAreSet()
    {
        // Arrange
        var schoolId = Guid.NewGuid();
        var birthDate = new DateTime(2015, 5, 10);
        var photoId = Guid.NewGuid();

        // Act
        var student = new Student
        {
            SchoolId = schoolId,
            FirstName = "Marie",
            LastName = "Nováková",
            BirthDate = birthDate,
            Class = "3.A",
            PhotoId = photoId,
            IsActive = false
        };

        // Assert
        student.SchoolId.Should().Be(schoolId);
        student.FirstName.Should().Be("Marie");
        student.LastName.Should().Be("Nováková");
        student.BirthDate.Should().Be(birthDate);
        student.Class.Should().Be("3.A");
        student.PhotoId.Should().Be(photoId);
        student.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Student_NavigationProperties_CanBeAssigned()
    {
        // Arrange
        var student = new Student();
        var school = new School { Name = "Test School" };
        var guardian = new StudentGuardian();
        var staffLink = new StudentStaffLink();
        var diaryEntry = new DiaryEntry();

        // Act
        student.School = school;
        student.Guardians.Add(guardian);
        student.StaffLinks.Add(staffLink);
        student.DiaryEntries.Add(diaryEntry);

        // Assert
        student.School.Should().Be(school);
        student.Guardians.Should().ContainSingle().Which.Should().Be(guardian);
        student.StaffLinks.Should().ContainSingle().Which.Should().Be(staffLink);
        student.DiaryEntries.Should().ContainSingle().Which.Should().Be(diaryEntry);
    }

    [Theory]
    [InlineData("Jan", "Novák", "Jan Novák")]
    [InlineData("Marie", "Svobodová", "Marie Svobodová")]
    [InlineData("Petr", "Dvořák", "Petr Dvořák")]
    [InlineData("  Jan  ", "  Novák  ", "  Jan     Novák  ")] // Note: Trim only removes leading/trailing spaces from the combined string
    public void FullName_WithVariousNames_ReturnsExpectedFullName(string firstName, string lastName, string expectedFullName)
    {
        // Arrange
        var student = new Student
        {
            FirstName = firstName,
            LastName = lastName
        };

        // Act
        var fullName = student.FullName;

        // Assert
        fullName.Should().Be(expectedFullName.Trim());
    }
}
