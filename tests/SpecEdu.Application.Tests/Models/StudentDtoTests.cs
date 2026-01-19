using FluentAssertions;
using SpecEdu.Application.Common.Models;
using Xunit;

namespace SpecEdu.Application.Tests.Models;

public class StudentDtoTests
{
    [Fact]
    public void FullName_WithFirstAndLastName_ReturnsFormattedName()
    {
        var dto = new StudentDto
        {
            FirstName = "Jan",
            LastName = "Novák"
        };

        var fullName = dto.FullName;

        fullName.Should().Be("Jan Novák");
    }

    [Fact]
    public void FullName_WithOnlyFirstName_ReturnsTrimmedName()
    {
        var dto = new StudentDto
        {
            FirstName = "Jan",
            LastName = ""
        };

        var fullName = dto.FullName;

        fullName.Should().Be("Jan");
    }

    [Fact]
    public void FullName_WithOnlyLastName_ReturnsTrimmedName()
    {
        var dto = new StudentDto
        {
            FirstName = "",
            LastName = "Novák"
        };

        var fullName = dto.FullName;

        fullName.Should().Be("Novák");
    }

    [Fact]
    public void StudentDto_DefaultValues_AreCorrect()
    {
        var dto = new StudentDto();

        dto.Id.Should().Be(Guid.Empty);
        dto.SchoolId.Should().Be(Guid.Empty);
        dto.SchoolName.Should().BeNull();
        dto.FirstName.Should().BeEmpty();
        dto.LastName.Should().BeEmpty();
        dto.BirthDate.Should().BeNull();
        dto.Class.Should().BeNull();
        dto.PhotoId.Should().BeNull();
        dto.IsActive.Should().BeFalse();
        dto.GuardianCount.Should().BeNull();
        dto.StaffLinkCount.Should().BeNull();
    }

    [Fact]
    public void StudentDto_SetAllProperties_PropertiesAreSet()
    {
        var id = Guid.NewGuid();
        var schoolId = Guid.NewGuid();
        var photoId = Guid.NewGuid();
        var birthDate = new DateTime(2015, 5, 10);
        var createdAt = DateTime.UtcNow;

        var dto = new StudentDto
        {
            Id = id,
            SchoolId = schoolId,
            SchoolName = "ZŠ Test",
            FirstName = "Marie",
            LastName = "Nováková",
            BirthDate = birthDate,
            Class = "3.A",
            PhotoId = photoId,
            IsActive = true,
            CreatedAt = createdAt,
            GuardianCount = 2,
            StaffLinkCount = 3
        };

        dto.Id.Should().Be(id);
        dto.SchoolId.Should().Be(schoolId);
        dto.SchoolName.Should().Be("ZŠ Test");
        dto.FirstName.Should().Be("Marie");
        dto.LastName.Should().Be("Nováková");
        dto.BirthDate.Should().Be(birthDate);
        dto.Class.Should().Be("3.A");
        dto.PhotoId.Should().Be(photoId);
        dto.IsActive.Should().BeTrue();
        dto.CreatedAt.Should().Be(createdAt);
        dto.GuardianCount.Should().Be(2);
        dto.StaffLinkCount.Should().Be(3);
        dto.FullName.Should().Be("Marie Nováková");
    }
}
