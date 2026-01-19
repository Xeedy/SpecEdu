using Microsoft.EntityFrameworkCore;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Infrastructure.Data;

namespace SpecEdu.Infrastructure.Tests.TestFixtures;

public static class TestDbContextFactory
{
    public static ApplicationDbContext Create(string? databaseName = null, ICurrentUserService? currentUserService = null)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options, currentUserService);
    }

    public static ApplicationDbContext CreateWithSameDatabase(ApplicationDbContext existingContext, ICurrentUserService? currentUserService = null)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(existingContext.Database.GetConnectionString() ?? "shared-db")
            .Options;

        return new ApplicationDbContext(options, currentUserService);
    }
}
