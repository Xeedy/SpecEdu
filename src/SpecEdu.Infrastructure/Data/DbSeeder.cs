using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SpecEdu.Domain.Constants;
using SpecEdu.Domain.Entities;
using SpecEdu.Domain.Enums;
using SpecEdu.Infrastructure.Identity;

namespace SpecEdu.Infrastructure.Data;

/// <summary>
/// Seeds the database with initial data.
/// Includes roles, admin user, and test data for development.
/// </summary>
public static class DbSeeder
{
    /// <summary>
    /// Seeds the database with required initial data.
    /// </summary>
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var environment = serviceProvider.GetRequiredService<IHostEnvironment>();
        var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

        await dbContext.Database.MigrateAsync();

        await SeedRolesAsync(roleManager, logger);

        await SeedAdminUserAsync(userManager, configuration, logger);

        if (environment.IsDevelopment())
        {
            await SeedTestDataAsync(dbContext, userManager, logger);
        }
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager, ILogger logger)
    {
        foreach (var roleName in Roles.All)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(roleName));
                if (result.Succeeded)
                {
                    logger.LogInformation("Created role: {RoleName}", roleName);
                }
                else
                {
                    logger.LogError("Failed to create role {RoleName}: {Errors}",
                        roleName, string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }

    private static async Task SeedAdminUserAsync(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        ILogger logger)
    {
        var adminEmail = configuration["AdminUser:Email"] ?? "admin@specedu.cz";
        var adminPassword = configuration["AdminUser:Password"] ?? "Admin123!";
        var adminFirstName = configuration["AdminUser:FirstName"] ?? "Systém";
        var adminLastName = configuration["AdminUser:LastName"] ?? "Administrátor";

        var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
        if (existingAdmin != null)
        {
            logger.LogInformation("Admin user already exists: {Email}", adminEmail);
            return;
        }

        var adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FirstName = adminFirstName,
            LastName = adminLastName,
            EmailConfirmed = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            SchoolId = null
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, Roles.Admin);
            logger.LogInformation("Created admin user: {Email}", adminEmail);
        }
        else
        {
            logger.LogError("Failed to create admin user: {Errors}",
                string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }

    private static async Task SeedTestDataAsync(
        ApplicationDbContext dbContext,
        UserManager<ApplicationUser> userManager,
        ILogger logger)
    {
        School? testSchool = null;

        if (!await dbContext.Schools.AnyAsync())
        {
            testSchool = new School
            {
                Name = "Testovací ZŠ",
                InstitutionType = "Škola",
                City = "Zlín",
                Address = "Testovací 123",
                PostalCode = "760 01",
                ContactEmail = "test@testskola.cz",
                IsActive = true
            };

            dbContext.Schools.Add(testSchool);
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Created test school: {SchoolName}", testSchool.Name);
        }
        else
        {
            testSchool = await dbContext.Schools.FirstAsync();
        }

        // Create School Admin
        var schoolAdminEmail = "spravce@testskola.cz";
        var existingSchoolAdmin = await userManager.FindByEmailAsync(schoolAdminEmail);

        if (existingSchoolAdmin == null)
        {
            var schoolAdmin = new ApplicationUser
            {
                UserName = schoolAdminEmail,
                Email = schoolAdminEmail,
                FirstName = "Jan",
                LastName = "Správce",
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                SchoolId = testSchool.Id
            };

            var result = await userManager.CreateAsync(schoolAdmin, "Spravce123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(schoolAdmin, Roles.SchoolAdmin);
                logger.LogInformation("Created test school admin: {Email}", schoolAdminEmail);
            }
        }

        // Create Teacher
        var teacherEmail = "ucitel@testskola.cz";
        var teacher = await userManager.FindByEmailAsync(teacherEmail);

        if (teacher == null)
        {
            teacher = new ApplicationUser
            {
                UserName = teacherEmail,
                Email = teacherEmail,
                FirstName = "Marie",
                LastName = "Učitelová",
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                SchoolId = testSchool.Id
            };

            var result = await userManager.CreateAsync(teacher, "Ucitel123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(teacher, Roles.Teacher);
                logger.LogInformation("Created test teacher: {Email}", teacherEmail);
            }
        }

        // Create Parent
        var parentEmail = "rodic@testskola.cz";
        var parent = await userManager.FindByEmailAsync(parentEmail);

        if (parent == null)
        {
            parent = new ApplicationUser
            {
                UserName = parentEmail,
                Email = parentEmail,
                FirstName = "Karel",
                LastName = "Novák",
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                SchoolId = null // Parents are not tied to a school directly
            };

            var result = await userManager.CreateAsync(parent, "Rodic123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(parent, Roles.Parent);
                logger.LogInformation("Created test parent: {Email}", parentEmail);
            }
        }

        // Create Test Students
        if (!await dbContext.Students.AnyAsync())
        {
            var student1 = new Student
            {
                SchoolId = testSchool.Id,
                FirstName = "Jan",
                LastName = "Novák",
                BirthDate = new DateTime(2015, 3, 15),
                Class = "5.A",
                IsActive = true
            };

            var student2 = new Student
            {
                SchoolId = testSchool.Id,
                FirstName = "Marie",
                LastName = "Nováková",
                BirthDate = new DateTime(2017, 7, 22),
                Class = "3.B",
                IsActive = true
            };

            var student3 = new Student
            {
                SchoolId = testSchool.Id,
                FirstName = "Petr",
                LastName = "Svoboda",
                BirthDate = new DateTime(2016, 11, 8),
                Class = "4.A",
                IsActive = true
            };

            dbContext.Students.AddRange(student1, student2, student3);
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Created test students: Jan Novák, Marie Nováková, Petr Svoboda");

            // Create Guardian relationships (parent -> students)
            parent = await userManager.FindByEmailAsync(parentEmail);
            if (parent != null)
            {
                var guardian1 = new StudentGuardian
                {
                    StudentId = student1.Id,
                    ParentUserId = parent.Id,
                    RelationshipType = RelationshipType.Father,
                    IsActive = true
                };

                var guardian2 = new StudentGuardian
                {
                    StudentId = student2.Id,
                    ParentUserId = parent.Id,
                    RelationshipType = RelationshipType.Father,
                    IsActive = true
                };

                dbContext.StudentGuardians.AddRange(guardian1, guardian2);
                await dbContext.SaveChangesAsync();
                logger.LogInformation("Created guardian relationships for parent {Email}", parentEmail);
            }

            // Create Staff links (teacher -> students)
            teacher = await userManager.FindByEmailAsync(teacherEmail);
            if (teacher != null)
            {
                var staffLink1 = new StudentStaffLink
                {
                    StudentId = student1.Id,
                    UserId = teacher.Id,
                    LinkType = StaffLinkType.Teacher,
                    AccessLevel = AccessLevel.Edit,
                    IsActive = true
                };

                var staffLink2 = new StudentStaffLink
                {
                    StudentId = student2.Id,
                    UserId = teacher.Id,
                    LinkType = StaffLinkType.Teacher,
                    AccessLevel = AccessLevel.Edit,
                    IsActive = true
                };

                var staffLink3 = new StudentStaffLink
                {
                    StudentId = student3.Id,
                    UserId = teacher.Id,
                    LinkType = StaffLinkType.Teacher,
                    AccessLevel = AccessLevel.Read,
                    IsActive = true
                };

                dbContext.StudentStaffLinks.AddRange(staffLink1, staffLink2, staffLink3);
                await dbContext.SaveChangesAsync();
                logger.LogInformation("Created staff links for teacher {Email}", teacherEmail);
            }
        }
    }
}
