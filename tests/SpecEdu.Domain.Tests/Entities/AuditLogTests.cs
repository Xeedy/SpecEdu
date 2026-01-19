using FluentAssertions;
using SpecEdu.Domain.Entities;
using Xunit;

namespace SpecEdu.Domain.Tests.Entities;

public class AuditLogTests
{
    [Fact]
    public void AuditLog_DefaultValues_AreCorrect()
    {
        var log = new AuditLog();

        log.UserId.Should().BeNull();
        log.UserName.Should().BeNull();
        log.Action.Should().BeEmpty();
        log.EntityType.Should().BeEmpty();
        log.EntityId.Should().BeNull();
        log.StudentId.Should().BeNull();
        log.SchoolId.Should().BeNull();
        log.IpAddress.Should().BeNull();
        log.UserAgent.Should().BeNull();
        log.Details.Should().BeNull();
    }

    [Fact]
    public void AuditLog_SetAllProperties_PropertiesAreSet()
    {
        var userId = Guid.NewGuid().ToString();
        var entityId = Guid.NewGuid().ToString();
        var studentId = Guid.NewGuid();
        var schoolId = Guid.NewGuid();
        var timestamp = DateTime.UtcNow;

        var log = new AuditLog
        {
            UserId = userId,
            UserName = "admin@school.cz",
            Action = "Create",
            EntityType = "Student",
            EntityId = entityId,
            StudentId = studentId,
            SchoolId = schoolId,
            Timestamp = timestamp,
            IpAddress = "192.168.1.1",
            UserAgent = "Mozilla/5.0",
            Details = "Created new student Jan Novák"
        };

        log.UserId.Should().Be(userId);
        log.UserName.Should().Be("admin@school.cz");
        log.Action.Should().Be("Create");
        log.EntityType.Should().Be("Student");
        log.EntityId.Should().Be(entityId);
        log.StudentId.Should().Be(studentId);
        log.SchoolId.Should().Be(schoolId);
        log.Timestamp.Should().Be(timestamp);
        log.IpAddress.Should().Be("192.168.1.1");
        log.UserAgent.Should().Be("Mozilla/5.0");
        log.Details.Should().Be("Created new student Jan Novák");
    }

    [Theory]
    [InlineData("Create")]
    [InlineData("Update")]
    [InlineData("Delete")]
    [InlineData("View")]
    [InlineData("Login")]
    [InlineData("Logout")]
    public void AuditLog_CommonActions_CanBeRecorded(string action)
    {
        var log = new AuditLog
        {
            Action = action
        };

        log.Action.Should().Be(action);
    }

    [Theory]
    [InlineData("Student")]
    [InlineData("School")]
    [InlineData("DiaryEntry")]
    [InlineData("DiaryAttachment")]
    [InlineData("StudentGuardian")]
    [InlineData("StudentStaffLink")]
    public void AuditLog_EntityTypes_CanBeRecorded(string entityType)
    {
        var log = new AuditLog
        {
            EntityType = entityType
        };

        log.EntityType.Should().Be(entityType);
    }

    [Fact]
    public void AuditLog_WithoutOptionalFields_CanBeCreated()
    {
        var log = new AuditLog
        {
            UserId = Guid.NewGuid().ToString(),
            Action = "Login",
            EntityType = "User",
            Timestamp = DateTime.UtcNow
        };

        log.UserId.Should().NotBeNullOrEmpty();
        log.Action.Should().Be("Login");
        log.EntityId.Should().BeNull();
        log.IpAddress.Should().BeNull();
    }

    [Fact]
    public void AuditLog_Timestamp_CanBeSetToUtc()
    {
        var utcNow = DateTime.UtcNow;

        var log = new AuditLog
        {
            Timestamp = utcNow
        };

        log.Timestamp.Should().BeCloseTo(utcNow, TimeSpan.FromSeconds(1));
        log.Timestamp.Kind.Should().Be(DateTimeKind.Utc);
    }
}
