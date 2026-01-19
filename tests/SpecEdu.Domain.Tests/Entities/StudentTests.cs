using FluentAssertions;
using SpecEdu.Domain.Entities;
using Xunit;

namespace SpecEdu.Domain.Tests.Entities;

public class StudentTests
{
    [Fact]
    public void FullName_WithFirstAndLastName_ReturnsFormattedName()
    {
        var student = new Student
        {
            FirstName = "Jan",
            LastName = "Novák"
        };

        var fullName = student.FullName;

        fullName.Should().Be("Jan Novák");
    }

    [Fact]
    public void FullName_WithOnlyFirstName_ReturnsTrimmedName()
    {
        var student = new Student
        {
            FirstName = "Jan",
            LastName = ""
        };

        var fullName = student.FullName;

        fullName.Should().Be("Jan");
    }

    [Fact]
    public void FullName_WithOnlyLastName_ReturnsTrimmedName()
    {
        var student = new Student
        {
            FirstName = "",
            LastName = "Novák"
        };

        var fullName = student.FullName;

        fullName.Should().Be("Novák");
    }

    [Fact]
    public void FullName_WithEmptyNames_ReturnsEmptyString()
    {
        var student = new Student
        {
            FirstName = "",
            LastName = ""
        };

        var fullName = student.FullName;

        fullName.Should().BeEmpty();
    }

    [Fact]
    public void Student_DefaultValues_AreCorrect()
    {
        var student = new Student();

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
        var schoolId = Guid.NewGuid();
        var birthDate = new DateTime(2015, 5, 10);
        var photoId = Guid.NewGuid();

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
        var student = new Student();
        var school = new School { Name = "Test School" };
        var guardian = new StudentGuardian();
        var staffLink = new StudentStaffLink();
        var diaryEntry = new DiaryEntry();

        student.School = school;
        student.Guardians.Add(guardian);
        student.StaffLinks.Add(staffLink);
        student.DiaryEntries.Add(diaryEntry);

        student.School.Should().Be(school);
        student.Guardians.Should().ContainSingle().Which.Should().Be(guardian);
        student.StaffLinks.Should().ContainSingle().Which.Should().Be(staffLink);
        student.DiaryEntries.Should().ContainSingle().Which.Should().Be(diaryEntry);
    }

    [Theory]
    [InlineData("Jan", "Novák", "Jan Novák")]
    [InlineData("Marie", "Svobodová", "Marie Svobodová")]
    [InlineData("Petr", "Dvořák", "Petr Dvořák")]
    [InlineData("  Jan  ", "  Novák  ", "  Jan     Novák  ")]
    public void FullName_WithVariousNames_ReturnsExpectedFullName(string firstName, string lastName, string expectedFullName)
    {
        var student = new Student
        {
            FirstName = firstName,
            LastName = lastName
        };

        var fullName = student.FullName;

        fullName.Should().Be(expectedFullName.Trim());
    }
}
