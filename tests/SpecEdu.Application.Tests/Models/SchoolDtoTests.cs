using FluentAssertions;
using SpecEdu.Application.Common.Models;
using Xunit;

namespace SpecEdu.Application.Tests.Models;

public class SchoolDtoTests
{
    [Fact]
    public void FullAddress_WithAllParts_ReturnsFormattedAddress()
    {
        var dto = new SchoolDto
        {
            Address = "Školní 123",
            City = "Zlín",
            PostalCode = "76001"
        };

        var fullAddress = dto.FullAddress;

        fullAddress.Should().Be("Školní 123, Zlín, 76001");
    }

    [Fact]
    public void FullAddress_WithOnlyAddress_ReturnsOnlyAddress()
    {
        var dto = new SchoolDto
        {
            Address = "Školní 123"
        };

        var fullAddress = dto.FullAddress;

        fullAddress.Should().Be("Školní 123");
    }

    [Fact]
    public void FullAddress_WithOnlyCityAndPostalCode_ReturnsPartialAddress()
    {
        var dto = new SchoolDto
        {
            City = "Zlín",
            PostalCode = "76001"
        };

        var fullAddress = dto.FullAddress;

        fullAddress.Should().Be("Zlín, 76001");
    }

    [Fact]
    public void FullAddress_WithNoParts_ReturnsEmptyString()
    {
        var dto = new SchoolDto();

        var fullAddress = dto.FullAddress;

        fullAddress.Should().BeEmpty();
    }

    [Fact]
    public void FullAddress_WithWhitespaceValues_IgnoresWhitespace()
    {
        var dto = new SchoolDto
        {
            Address = "   ",
            City = "Zlín",
            PostalCode = ""
        };

        var fullAddress = dto.FullAddress;

        fullAddress.Should().Be("Zlín");
    }

    [Fact]
    public void SchoolDto_DefaultValues_AreCorrect()
    {
        var dto = new SchoolDto();

        dto.Id.Should().Be(Guid.Empty);
        dto.Name.Should().BeEmpty();
        dto.InstitutionType.Should().Be("Škola");
        dto.Ico.Should().BeNull();
        dto.Address.Should().BeNull();
        dto.City.Should().BeNull();
        dto.PostalCode.Should().BeNull();
        dto.ContactEmail.Should().BeNull();
        dto.ContactPhone.Should().BeNull();
        dto.IsActive.Should().BeFalse();
        dto.LicenseExpiresAt.Should().BeNull();
        dto.UserCount.Should().BeNull();
    }

    [Fact]
    public void SchoolDto_SetAllProperties_PropertiesAreSet()
    {
        var id = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;
        var licenseExpiry = DateTime.UtcNow.AddYears(1);

        var dto = new SchoolDto
        {
            Id = id,
            Name = "ZŠ Zlín",
            Ico = "12345678",
            Address = "Školní 123",
            City = "Zlín",
            PostalCode = "76001",
            ContactEmail = "info@zszlin.cz",
            ContactPhone = "+420777123456",
            InstitutionType = "Škola",
            IsActive = true,
            LicenseExpiresAt = licenseExpiry,
            CreatedAt = createdAt,
            UserCount = 50
        };

        dto.Id.Should().Be(id);
        dto.Name.Should().Be("ZŠ Zlín");
        dto.Ico.Should().Be("12345678");
        dto.Address.Should().Be("Školní 123");
        dto.City.Should().Be("Zlín");
        dto.PostalCode.Should().Be("76001");
        dto.ContactEmail.Should().Be("info@zszlin.cz");
        dto.ContactPhone.Should().Be("+420777123456");
        dto.InstitutionType.Should().Be("Škola");
        dto.IsActive.Should().BeTrue();
        dto.LicenseExpiresAt.Should().Be(licenseExpiry);
        dto.CreatedAt.Should().Be(createdAt);
        dto.UserCount.Should().Be(50);
    }
}
