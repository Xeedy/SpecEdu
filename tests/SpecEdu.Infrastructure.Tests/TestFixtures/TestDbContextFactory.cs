using Microsoft.EntityFrameworkCore;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Infrastructure.Data;

namespace SpecEdu.Infrastructure.Tests.TestFixtures;

/// <summary>
/// Factory for creating in-memory database contexts for testing.
/// </summary>
public static class TestDbContextFactory
{
    /// <summary>
    /// Creates a new ApplicationDbContext with a unique in-memory database.
    /// </summary>
    /// <param name="databaseName">Optional database name. If not provided, a unique name is generated.</param>
    /// <param name="currentUserService">Optional current user service for audit fields.</param>
    /// <returns>A new ApplicationDbContext instance.</returns>
    public static ApplicationDbContext Create(string? databaseName = null, ICurrentUserService? currentUserService = null)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options, currentUserService);
    }

    /// <summary>
    /// Creates a new ApplicationDbContext with the same database as the provided context.
    /// </summary>
    /// <param name="existingContext">Existing context to share the database with.</param>
    /// <param name="currentUserService">Optional current user service for audit fields.</param>
    /// <returns>A new ApplicationDbContext instance sharing the database.</returns>
    public static ApplicationDbContext CreateWithSameDatabase(ApplicationDbContext existingContext, ICurrentUserService? currentUserService = null)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(existingContext.Database.GetConnectionString() ?? "shared-db")
            .Options;

        return new ApplicationDbContext(options, currentUserService);
    }
}
