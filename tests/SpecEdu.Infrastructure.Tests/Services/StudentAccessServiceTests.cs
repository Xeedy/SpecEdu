using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using SpecEdu.Application.Common.Models;
using SpecEdu.Domain.Constants;
using SpecEdu.Domain.Entities;
using SpecEdu.Domain.Enums;
using SpecEdu.Infrastructure.Identity;
using SpecEdu.Infrastructure.Services;
using SpecEdu.Infrastructure.Tests.TestFixtures;
using Xunit;

namespace SpecEdu.Infrastructure.Tests.Services;

public class StudentAccessServiceTests : IDisposable
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;

    public StudentAccessServiceTests()
    {
        _userManagerMock = MockUserManagerFactory.Create();
    }

    public void Dispose()
    {
    }

    #region CanAccessStudentAsync - Admin Access Tests

    [Fact]
    public async Task CanAccessStudentAsync_AdminUser_ReturnsFullAccess()
    {
        using var dbContext = TestDbContextFactory.Create();
        var school = new School { Id = Guid.NewGuid(), Name = "Test School" };
        var student = new Student
        {
            Id = Guid.NewGuid(),
            FirstName = "Jan",
            LastName = "Novák",
            SchoolId = school.Id,
            School = school
        };
        dbContext.Schools.Add(school);
        dbContext.Students.Add(student);
        await dbContext.SaveChangesAsync();

        var adminUser = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "admin@test.cz",
            SchoolId = null
        };

        _userManagerMock.SetupFindByIdAsync(adminUser.Id, adminUser);
        _userManagerMock.SetupIsInRoleAsync(adminUser, Roles.Admin, true);

        var service = new StudentAccessService(dbContext, _userManagerMock.Object);

        var result = await service.CanAccessStudentAsync(adminUser.Id, student.Id);

        result.CanAccess.Should().BeTrue();
        result.CanEdit.Should().BeTrue();
        result.AccessReason.Should().Be(AccessReason.Admin);
    }

    #endregion

    #region CanAccessStudentAsync - SchoolAdmin Access Tests

    [Fact]
    public async Task CanAccessStudentAsync_SchoolAdminSameSchool_ReturnsFullAccess()
    {
        using var dbContext = TestDbContextFactory.Create();
        var schoolId = Guid.NewGuid();
        var school = new School { Id = schoolId, Name = "Test School" };
        var student = new Student
        {
            Id = Guid.NewGuid(),
            FirstName = "Jan",
            LastName = "Novák",
            SchoolId = schoolId
        };
        dbContext.Schools.Add(school);
        dbContext.Students.Add(student);
        await dbContext.SaveChangesAsync();

        var schoolAdminUser = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "schooladmin@test.cz",
            SchoolId = schoolId
        };

        _userManagerMock.SetupFindByIdAsync(schoolAdminUser.Id, schoolAdminUser);
        _userManagerMock.SetupIsInRoleAsync(schoolAdminUser, Roles.Admin, false);
        _userManagerMock.SetupIsInRoleAsync(schoolAdminUser, Roles.SchoolAdmin, true);

        var service = new StudentAccessService(dbContext, _userManagerMock.Object);

        var result = await service.CanAccessStudentAsync(schoolAdminUser.Id, student.Id);

        result.CanAccess.Should().BeTrue();
        result.CanEdit.Should().BeTrue();
        result.AccessReason.Should().Be(AccessReason.SchoolAdmin);
    }

    [Fact]
    public async Task CanAccessStudentAsync_SchoolAdminDifferentSchool_ReturnsNoAccess()
    {
        using var dbContext = TestDbContextFactory.Create();
        var school1 = new School { Id = Guid.NewGuid(), Name = "School 1" };
        var school2 = new School { Id = Guid.NewGuid(), Name = "School 2" };
        var student = new Student
        {
            Id = Guid.NewGuid(),
            FirstName = "Jan",
            LastName = "Novák",
            SchoolId = school1.Id
        };
        dbContext.Schools.AddRange(school1, school2);
        dbContext.Students.Add(student);
        await dbContext.SaveChangesAsync();

        var schoolAdminUser = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "schooladmin@test.cz",
            SchoolId = school2.Id
        };

        _userManagerMock.SetupFindByIdAsync(schoolAdminUser.Id, schoolAdminUser);
        _userManagerMock.SetupIsInRoleAsync(schoolAdminUser, Roles.Admin, false);

        var service = new StudentAccessService(dbContext, _userManagerMock.Object);

        var result = await service.CanAccessStudentAsync(schoolAdminUser.Id, student.Id);

        result.CanAccess.Should().BeFalse();
        result.CanEdit.Should().BeFalse();
        result.AccessReason.Should().Be(AccessReason.NoAccess);
    }

    #endregion

    #region CanAccessStudentAsync - Guardian Access Tests

    [Fact]
    public async Task CanAccessStudentAsync_ParentWithGuardianLink_ReturnsReadOnlyAccess()
    {
        using var dbContext = TestDbContextFactory.Create();
        var schoolId = Guid.NewGuid();
        var school = new School { Id = schoolId, Name = "Test School" };
        var student = new Student
        {
            Id = Guid.NewGuid(),
            FirstName = "Jan",
            LastName = "Novák",
            SchoolId = schoolId
        };
        dbContext.Schools.Add(school);
        dbContext.Students.Add(student);

        var parentUser = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "parent@test.cz",
            SchoolId = schoolId
        };

        var guardianLink = new StudentGuardian
        {
            Id = Guid.NewGuid(),
            StudentId = student.Id,
            ParentUserId = parentUser.Id,
            RelationshipType = RelationshipType.Mother,
            IsActive = true
        };
        dbContext.StudentGuardians.Add(guardianLink);
        await dbContext.SaveChangesAsync();

        _userManagerMock.SetupFindByIdAsync(parentUser.Id, parentUser);
        _userManagerMock.SetupIsInRoleAsync(parentUser, Roles.Admin, false);
        _userManagerMock.SetupIsInRoleAsync(parentUser, Roles.SchoolAdmin, false);

        var service = new StudentAccessService(dbContext, _userManagerMock.Object);

        var result = await service.CanAccessStudentAsync(parentUser.Id, student.Id);

        result.CanAccess.Should().BeTrue();
        result.CanEdit.Should().BeFalse();
        result.AccessReason.Should().Be(AccessReason.Guardian);
    }

    [Fact]
    public async Task CanAccessStudentAsync_ParentWithInactiveGuardianLink_ReturnsNoAccess()
    {
        using var dbContext = TestDbContextFactory.Create();
        var schoolId = Guid.NewGuid();
        var school = new School { Id = schoolId, Name = "Test School" };
        var student = new Student
        {
            Id = Guid.NewGuid(),
            FirstName = "Jan",
            LastName = "Novák",
            SchoolId = schoolId
        };
        dbContext.Schools.Add(school);
        dbContext.Students.Add(student);

        var parentUser = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "parent@test.cz",
            SchoolId = schoolId
        };

        var inactiveGuardianLink = new StudentGuardian
        {
            Id = Guid.NewGuid(),
            StudentId = student.Id,
            ParentUserId = parentUser.Id,
            RelationshipType = RelationshipType.Mother,
            IsActive = false
        };
        dbContext.StudentGuardians.Add(inactiveGuardianLink);
        await dbContext.SaveChangesAsync();

        _userManagerMock.SetupFindByIdAsync(parentUser.Id, parentUser);
        _userManagerMock.SetupIsInRoleAsync(parentUser, Roles.Admin, false);
        _userManagerMock.SetupIsInRoleAsync(parentUser, Roles.SchoolAdmin, false);

        var service = new StudentAccessService(dbContext, _userManagerMock.Object);

        var result = await service.CanAccessStudentAsync(parentUser.Id, student.Id);

        result.CanAccess.Should().BeFalse();
    }

    #endregion

    #region CanAccessStudentAsync - Staff Link Access Tests

    [Fact]
    public async Task CanAccessStudentAsync_TeacherWithEditStaffLink_ReturnsEditAccess()
    {
        using var dbContext = TestDbContextFactory.Create();
        var schoolId = Guid.NewGuid();
        var school = new School { Id = schoolId, Name = "Test School" };
        var student = new Student
        {
            Id = Guid.NewGuid(),
            FirstName = "Jan",
            LastName = "Novák",
            SchoolId = schoolId
        };
        dbContext.Schools.Add(school);
        dbContext.Students.Add(student);

        var teacherUser = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "teacher@test.cz",
            SchoolId = schoolId
        };

        var staffLink = new StudentStaffLink
        {
            Id = Guid.NewGuid(),
            StudentId = student.Id,
            UserId = teacherUser.Id,
            LinkType = StaffLinkType.Teacher,
            AccessLevel = AccessLevel.Edit,
            IsActive = true
        };
        dbContext.StudentStaffLinks.Add(staffLink);
        await dbContext.SaveChangesAsync();

        _userManagerMock.SetupFindByIdAsync(teacherUser.Id, teacherUser);
        _userManagerMock.SetupIsInRoleAsync(teacherUser, Roles.Admin, false);
        _userManagerMock.SetupIsInRoleAsync(teacherUser, Roles.SchoolAdmin, false);

        var service = new StudentAccessService(dbContext, _userManagerMock.Object);

        var result = await service.CanAccessStudentAsync(teacherUser.Id, student.Id);

        result.CanAccess.Should().BeTrue();
        result.CanEdit.Should().BeTrue();
        result.AccessReason.Should().Be(AccessReason.StaffLink);
    }

    [Fact]
    public async Task CanAccessStudentAsync_AssistantWithReadStaffLink_ReturnsReadOnlyAccess()
    {
        using var dbContext = TestDbContextFactory.Create();
        var schoolId = Guid.NewGuid();
        var school = new School { Id = schoolId, Name = "Test School" };
        var student = new Student
        {
            Id = Guid.NewGuid(),
            FirstName = "Jan",
            LastName = "Novák",
            SchoolId = schoolId
        };
        dbContext.Schools.Add(school);
        dbContext.Students.Add(student);

        var assistantUser = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "assistant@test.cz",
            SchoolId = schoolId
        };

        var staffLink = new StudentStaffLink
        {
            Id = Guid.NewGuid(),
            StudentId = student.Id,
            UserId = assistantUser.Id,
            LinkType = StaffLinkType.Assistant,
            AccessLevel = AccessLevel.Read,
            IsActive = true
        };
        dbContext.StudentStaffLinks.Add(staffLink);
        await dbContext.SaveChangesAsync();

        _userManagerMock.SetupFindByIdAsync(assistantUser.Id, assistantUser);
        _userManagerMock.SetupIsInRoleAsync(assistantUser, Roles.Admin, false);
        _userManagerMock.SetupIsInRoleAsync(assistantUser, Roles.SchoolAdmin, false);

        var service = new StudentAccessService(dbContext, _userManagerMock.Object);

        var result = await service.CanAccessStudentAsync(assistantUser.Id, student.Id);

        result.CanAccess.Should().BeTrue();
        result.CanEdit.Should().BeFalse();
        result.AccessReason.Should().Be(AccessReason.StaffLink);
    }

    #endregion

    #region CanAccessStudentAsync - Error Cases

    [Fact]
    public async Task CanAccessStudentAsync_UserNotFound_ReturnsNoAccess()
    {
        using var dbContext = TestDbContextFactory.Create();
        var student = new Student
        {
            Id = Guid.NewGuid(),
            FirstName = "Jan",
            LastName = "Novák",
            SchoolId = Guid.NewGuid()
        };
        dbContext.Students.Add(student);
        await dbContext.SaveChangesAsync();

        var nonExistentUserId = Guid.NewGuid().ToString();
        _userManagerMock.SetupFindByIdAsync(nonExistentUserId, null);

        var service = new StudentAccessService(dbContext, _userManagerMock.Object);

        var result = await service.CanAccessStudentAsync(nonExistentUserId, student.Id);

        result.CanAccess.Should().BeFalse();
        result.Message.Should().Contain("Uživatel nenalezen");
    }

    [Fact]
    public async Task CanAccessStudentAsync_StudentNotFound_ReturnsNoAccess()
    {
        using var dbContext = TestDbContextFactory.Create();

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "user@test.cz"
        };

        _userManagerMock.SetupFindByIdAsync(user.Id, user);

        var service = new StudentAccessService(dbContext, _userManagerMock.Object);
        var nonExistentStudentId = Guid.NewGuid();

        var result = await service.CanAccessStudentAsync(user.Id, nonExistentStudentId);

        result.CanAccess.Should().BeFalse();
        result.Message.Should().Contain("Žák nenalezen");
    }

    #endregion

    #region GetStudentsForParentAsync Tests

    [Fact]
    public async Task GetStudentsForParentAsync_ParentWithChildren_ReturnsChildrenList()
    {
        using var dbContext = TestDbContextFactory.Create();
        var schoolId = Guid.NewGuid();
        var school = new School { Id = schoolId, Name = "Test School" };
        var parentUserId = Guid.NewGuid().ToString();

        var student1 = new Student
        {
            Id = Guid.NewGuid(),
            FirstName = "Jan",
            LastName = "Novák",
            SchoolId = schoolId,
            School = school,
            IsActive = true
        };
        var student2 = new Student
        {
            Id = Guid.NewGuid(),
            FirstName = "Marie",
            LastName = "Nováková",
            SchoolId = schoolId,
            School = school,
            IsActive = true
        };

        dbContext.Schools.Add(school);
        dbContext.Students.AddRange(student1, student2);
        dbContext.StudentGuardians.AddRange(
            new StudentGuardian
            {
                Id = Guid.NewGuid(),
                StudentId = student1.Id,
                ParentUserId = parentUserId,
                RelationshipType = RelationshipType.Father,
                IsActive = true
            },
            new StudentGuardian
            {
                Id = Guid.NewGuid(),
                StudentId = student2.Id,
                ParentUserId = parentUserId,
                RelationshipType = RelationshipType.Father,
                IsActive = true
            });
        await dbContext.SaveChangesAsync();

        var service = new StudentAccessService(dbContext, _userManagerMock.Object);

        var result = await service.GetStudentsForParentAsync(parentUserId);

        result.Should().HaveCount(2);
        result.Select(s => s.FirstName).Should().Contain(new[] { "Jan", "Marie" });
    }

    [Fact]
    public async Task GetStudentsForParentAsync_ParentWithNoChildren_ReturnsEmptyList()
    {
        using var dbContext = TestDbContextFactory.Create();
        var parentUserId = Guid.NewGuid().ToString();

        var service = new StudentAccessService(dbContext, _userManagerMock.Object);

        var result = await service.GetStudentsForParentAsync(parentUserId);

        result.Should().BeEmpty();
    }

    #endregion

    #region GetStudentsForStaffAsync Tests

    [Fact]
    public async Task GetStudentsForStaffAsync_StaffWithLinks_ReturnsLinkedStudents()
    {
        using var dbContext = TestDbContextFactory.Create();
        var schoolId = Guid.NewGuid();
        var school = new School { Id = schoolId, Name = "Test School" };
        var staffUserId = Guid.NewGuid().ToString();

        var student1 = new Student
        {
            Id = Guid.NewGuid(),
            FirstName = "Jan",
            LastName = "Novák",
            SchoolId = schoolId,
            School = school,
            IsActive = true
        };
        var student2 = new Student
        {
            Id = Guid.NewGuid(),
            FirstName = "Marie",
            LastName = "Nováková",
            SchoolId = schoolId,
            School = school,
            IsActive = true
        };

        dbContext.Schools.Add(school);
        dbContext.Students.AddRange(student1, student2);
        dbContext.StudentStaffLinks.AddRange(
            new StudentStaffLink
            {
                Id = Guid.NewGuid(),
                StudentId = student1.Id,
                UserId = staffUserId,
                LinkType = StaffLinkType.Teacher,
                AccessLevel = AccessLevel.Edit,
                IsActive = true
            },
            new StudentStaffLink
            {
                Id = Guid.NewGuid(),
                StudentId = student2.Id,
                UserId = staffUserId,
                LinkType = StaffLinkType.Teacher,
                AccessLevel = AccessLevel.Read,
                IsActive = true
            });
        await dbContext.SaveChangesAsync();

        var service = new StudentAccessService(dbContext, _userManagerMock.Object);

        var result = await service.GetStudentsForStaffAsync(staffUserId);

        result.Should().HaveCount(2);
    }

    #endregion

    #region GetStudentsForSchoolAsync Tests

    [Fact]
    public async Task GetStudentsForSchoolAsync_SchoolWithStudents_ReturnsActiveStudents()
    {
        using var dbContext = TestDbContextFactory.Create();
        var schoolId = Guid.NewGuid();
        var school = new School { Id = schoolId, Name = "Test School" };

        var activeStudent = new Student
        {
            Id = Guid.NewGuid(),
            FirstName = "Jan",
            LastName = "Novák",
            SchoolId = schoolId,
            School = school,
            IsActive = true
        };
        var inactiveStudent = new Student
        {
            Id = Guid.NewGuid(),
            FirstName = "Marie",
            LastName = "Nováková",
            SchoolId = schoolId,
            School = school,
            IsActive = false
        };

        dbContext.Schools.Add(school);
        dbContext.Students.AddRange(activeStudent, inactiveStudent);
        await dbContext.SaveChangesAsync();

        var service = new StudentAccessService(dbContext, _userManagerMock.Object);

        var result = await service.GetStudentsForSchoolAsync(schoolId, includeInactive: false);

        result.Should().HaveCount(1);
        result.First().FirstName.Should().Be("Jan");
    }

    [Fact]
    public async Task GetStudentsForSchoolAsync_IncludeInactive_ReturnsAllStudents()
    {
        using var dbContext = TestDbContextFactory.Create();
        var schoolId = Guid.NewGuid();
        var school = new School { Id = schoolId, Name = "Test School" };

        var activeStudent = new Student
        {
            Id = Guid.NewGuid(),
            FirstName = "Jan",
            LastName = "Novák",
            SchoolId = schoolId,
            School = school,
            IsActive = true
        };
        var inactiveStudent = new Student
        {
            Id = Guid.NewGuid(),
            FirstName = "Marie",
            LastName = "Nováková",
            SchoolId = schoolId,
            School = school,
            IsActive = false
        };

        dbContext.Schools.Add(school);
        dbContext.Students.AddRange(activeStudent, inactiveStudent);
        await dbContext.SaveChangesAsync();

        var service = new StudentAccessService(dbContext, _userManagerMock.Object);

        var result = await service.GetStudentsForSchoolAsync(schoolId, includeInactive: true);

        result.Should().HaveCount(2);
    }

    #endregion
}
