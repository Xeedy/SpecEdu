using FluentAssertions;
using SpecEdu.Domain.Entities;
using SpecEdu.Infrastructure.Services;
using SpecEdu.Infrastructure.Tests.TestFixtures;
using Xunit;

namespace SpecEdu.Infrastructure.Tests.Services;

public class SchoolServiceTests : IDisposable
{
    public void Dispose()
    {
    }

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_ValidSchool_CreatesAndReturnsSchool()
    {
        using var dbContext = TestDbContextFactory.Create();
        var service = new SchoolService(dbContext);

        var result = await service.CreateAsync(
            name: "ZŠ Test",
            institutionType: "Škola",
            ico: "12345678",
            address: "Testovací 123",
            city: "Praha",
            postalCode: "10000",
            contactEmail: "info@zstest.cz",
            contactPhone: "+420123456789");

        result.Should().NotBeNull();
        result.Name.Should().Be("ZŠ Test");
        result.InstitutionType.Should().Be("Škola");
        result.Ico.Should().Be("12345678");
        result.Address.Should().Be("Testovací 123");
        result.City.Should().Be("Praha");
        result.PostalCode.Should().Be("10000");
        result.ContactEmail.Should().Be("info@zstest.cz");
        result.ContactPhone.Should().Be("+420123456789");
        result.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task CreateAsync_MinimalData_CreatesSchool()
    {
        using var dbContext = TestDbContextFactory.Create();
        var service = new SchoolService(dbContext);

        var result = await service.CreateAsync(
            name: "Minimal School",
            institutionType: "PPP");

        result.Should().NotBeNull();
        result.Name.Should().Be("Minimal School");
        result.InstitutionType.Should().Be("PPP");
        result.Ico.Should().BeNull();
        result.Address.Should().BeNull();
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_ExistingSchool_ReturnsSchool()
    {
        using var dbContext = TestDbContextFactory.Create();
        var school = new School
        {
            Id = Guid.NewGuid(),
            Name = "Test School",
            InstitutionType = "Škola",
            IsActive = true
        };
        dbContext.Schools.Add(school);
        await dbContext.SaveChangesAsync();

        var service = new SchoolService(dbContext);

        var result = await service.GetByIdAsync(school.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(school.Id);
        result.Name.Should().Be("Test School");
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingSchool_ReturnsNull()
    {
        using var dbContext = TestDbContextFactory.Create();
        var service = new SchoolService(dbContext);

        var result = await service.GetByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    #endregion

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_ExcludesInactiveByDefault()
    {
        using var dbContext = TestDbContextFactory.Create();
        dbContext.Schools.AddRange(
            new School { Id = Guid.NewGuid(), Name = "Active School", IsActive = true },
            new School { Id = Guid.NewGuid(), Name = "Inactive School", IsActive = false }
        );
        await dbContext.SaveChangesAsync();

        var service = new SchoolService(dbContext);

        var result = await service.GetAllAsync();

        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Active School");
    }

    [Fact]
    public async Task GetAllAsync_IncludesInactiveWhenRequested()
    {
        using var dbContext = TestDbContextFactory.Create();
        dbContext.Schools.AddRange(
            new School { Id = Guid.NewGuid(), Name = "Active School", IsActive = true },
            new School { Id = Guid.NewGuid(), Name = "Inactive School", IsActive = false }
        );
        await dbContext.SaveChangesAsync();

        var service = new SchoolService(dbContext);

        var result = await service.GetAllAsync(includeInactive: true);

        result.Should().HaveCount(2);
    }

    #endregion

    #region GetPagedAsync Tests

    [Fact]
    public async Task GetPagedAsync_ReturnsPaginatedResults()
    {
        using var dbContext = TestDbContextFactory.Create();
        for (int i = 0; i < 25; i++)
        {
            dbContext.Schools.Add(new School
            {
                Id = Guid.NewGuid(),
                Name = $"School {i}",
                IsActive = true
            });
        }
        await dbContext.SaveChangesAsync();

        var service = new SchoolService(dbContext);

        var (schools, totalCount) = await service.GetPagedAsync(page: 2, pageSize: 10);

        schools.Should().HaveCount(10);
        totalCount.Should().Be(25);
    }

    [Fact]
    public async Task GetPagedAsync_WithSearchTerm_FiltersResults()
    {
        using var dbContext = TestDbContextFactory.Create();
        dbContext.Schools.AddRange(
            new School { Id = Guid.NewGuid(), Name = "ZŠ Praha", City = "Praha", IsActive = true },
            new School { Id = Guid.NewGuid(), Name = "ZŠ Brno", City = "Brno", IsActive = true },
            new School { Id = Guid.NewGuid(), Name = "ZŠ Zlín", City = "Zlín", IsActive = true }
        );
        await dbContext.SaveChangesAsync();

        var service = new SchoolService(dbContext);

        var (schools, totalCount) = await service.GetPagedAsync(
            page: 1, pageSize: 10, searchTerm: "Praha");

        schools.Should().HaveCount(1);
        schools.First().Name.Should().Contain("Praha");
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_ExistingSchool_UpdatesAndReturnsSchool()
    {
        using var dbContext = TestDbContextFactory.Create();
        var school = new School
        {
            Id = Guid.NewGuid(),
            Name = "Original Name",
            InstitutionType = "Škola",
            IsActive = true
        };
        dbContext.Schools.Add(school);
        await dbContext.SaveChangesAsync();

        var service = new SchoolService(dbContext);
        var licenseExpiry = DateTime.UtcNow.AddYears(1);

        var result = await service.UpdateAsync(
            school.Id,
            name: "Updated Name",
            institutionType: "PPP",
            ico: "87654321",
            address: "New Address",
            city: "Brno",
            postalCode: "60200",
            contactEmail: "new@email.cz",
            contactPhone: "+420987654321",
            isActive: true,
            licenseExpiresAt: licenseExpiry);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Updated Name");
        result.InstitutionType.Should().Be("PPP");
        result.Ico.Should().Be("87654321");
        result.LicenseExpiresAt.Should().BeCloseTo(licenseExpiry, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task UpdateAsync_NonExistingSchool_ReturnsNull()
    {
        using var dbContext = TestDbContextFactory.Create();
        var service = new SchoolService(dbContext);

        var result = await service.UpdateAsync(
            Guid.NewGuid(),
            name: "Name",
            institutionType: "Škola",
            ico: null, address: null, city: null, postalCode: null,
            contactEmail: null, contactPhone: null,
            isActive: true, licenseExpiresAt: null);

        result.Should().BeNull();
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_ExistingSchool_SoftDeletesSchool()
    {
        using var dbContext = TestDbContextFactory.Create();
        var school = new School
        {
            Id = Guid.NewGuid(),
            Name = "To Delete",
            IsActive = true
        };
        dbContext.Schools.Add(school);
        await dbContext.SaveChangesAsync();

        var service = new SchoolService(dbContext);

        var result = await service.DeleteAsync(school.Id);

        result.Should().BeTrue();

        var deleted = await dbContext.Schools.FindAsync(school.Id);
        deleted.Should().NotBeNull();
        deleted!.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_NonExistingSchool_ReturnsFalse()
    {
        using var dbContext = TestDbContextFactory.Create();
        var service = new SchoolService(dbContext);

        var result = await service.DeleteAsync(Guid.NewGuid());

        result.Should().BeFalse();
    }

    #endregion

    #region ExistsAsync Tests

    [Fact]
    public async Task ExistsAsync_ExistingSchool_ReturnsTrue()
    {
        using var dbContext = TestDbContextFactory.Create();
        var school = new School
        {
            Id = Guid.NewGuid(),
            Name = "Existing School",
            IsActive = true
        };
        dbContext.Schools.Add(school);
        await dbContext.SaveChangesAsync();

        var service = new SchoolService(dbContext);

        var result = await service.ExistsAsync(school.Id);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_NonExistingSchool_ReturnsFalse()
    {
        using var dbContext = TestDbContextFactory.Create();
        var service = new SchoolService(dbContext);

        var result = await service.ExistsAsync(Guid.NewGuid());

        result.Should().BeFalse();
    }

    #endregion
}
