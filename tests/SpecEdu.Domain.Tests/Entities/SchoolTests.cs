using FluentAssertions;
using SpecEdu.Domain.Entities;
using Xunit;

namespace SpecEdu.Domain.Tests.Entities;

/// <summary>
/// Tests for the School entity.
/// </summary>
public class SchoolTests
{
    [Fact]
    public void School_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var school = new School();

        // Assert
        school.Name.Should().BeEmpty();
        school.InstitutionType.Should().Be("Škola");
        school.IsActive.Should().BeTrue();
        school.Ico.Should().BeNull();
        school.Address.Should().BeNull();
        school.City.Should().BeNull();
        school.PostalCode.Should().BeNull();
        school.ContactEmail.Should().BeNull();
        school.ContactPhone.Should().BeNull();
        school.LicenseExpiresAt.Should().BeNull();
    }

    [Fact]
    public void School_SetAllProperties_PropertiesAreSet()
    {
        // Arrange
        var licenseExpiry = DateTime.UtcNow.AddYears(1);

        // Act
        var school = new School
        {
            Name = "ZŠ Zlín",
            Ico = "12345678",
            Address = "Školní 123",
            City = "Zlín",
            PostalCode = "76001",
            ContactEmail = "info@zszlin.cz",
            ContactPhone = "+420777123456",
            InstitutionType = "Škola",
            IsActive = true,
            LicenseExpiresAt = licenseExpiry
        };

        // Assert
        school.Name.Should().Be("ZŠ Zlín");
        school.Ico.Should().Be("12345678");
        school.Address.Should().Be("Školní 123");
        school.City.Should().Be("Zlín");
        school.PostalCode.Should().Be("76001");
        school.ContactEmail.Should().Be("info@zszlin.cz");
        school.ContactPhone.Should().Be("+420777123456");
        school.InstitutionType.Should().Be("Škola");
        school.IsActive.Should().BeTrue();
        school.LicenseExpiresAt.Should().Be(licenseExpiry);
    }

    [Theory]
    [InlineData("Škola")]
    [InlineData("PPP")]
    [InlineData("SPC")]
    [InlineData("ŠPP")]
    [InlineData("Jiné")]
    public void School_InstitutionType_AcceptsValidTypes(string institutionType)
    {
        // Arrange & Act
        var school = new School
        {
            Name = "Test Institution",
            InstitutionType = institutionType
        };

        // Assert
        school.InstitutionType.Should().Be(institutionType);
    }

    [Fact]
    public void School_Deactivate_SetsIsActiveToFalse()
    {
        // Arrange
        var school = new School
        {
            Name = "Test School",
            IsActive = true
        };

        // Act
        school.IsActive = false;

        // Assert
        school.IsActive.Should().BeFalse();
    }

    [Fact]
    public void School_WithExpiredLicense_LicenseExpiresAtIsInPast()
    {
        // Arrange
        var expiredDate = DateTime.UtcNow.AddDays(-30);

        // Act
        var school = new School
        {
            Name = "Expired School",
            LicenseExpiresAt = expiredDate
        };

        // Assert
        school.LicenseExpiresAt.Should().BeBefore(DateTime.UtcNow);
    }

    [Fact]
    public void School_WithActiveLicense_LicenseExpiresAtIsInFuture()
    {
        // Arrange
        var futureDate = DateTime.UtcNow.AddYears(1);

        // Act
        var school = new School
        {
            Name = "Active School",
            LicenseExpiresAt = futureDate
        };

        // Assert
        school.LicenseExpiresAt.Should().BeAfter(DateTime.UtcNow);
    }
}
