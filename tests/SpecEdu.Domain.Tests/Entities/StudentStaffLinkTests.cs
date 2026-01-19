using FluentAssertions;
using SpecEdu.Domain.Entities;
using SpecEdu.Domain.Enums;
using Xunit;

namespace SpecEdu.Domain.Tests.Entities;

public class StudentStaffLinkTests
{
    [Fact]
    public void StudentStaffLink_DefaultValues_AreCorrect()
    {
        var staffLink = new StudentStaffLink();

        staffLink.UserId.Should().BeEmpty();
        staffLink.IsActive.Should().BeTrue();
        staffLink.Student.Should().BeNull();
    }

    [Fact]
    public void StudentStaffLink_SetAllProperties_PropertiesAreSet()
    {
        var studentId = Guid.NewGuid();
        var userId = Guid.NewGuid().ToString();

        var staffLink = new StudentStaffLink
        {
            StudentId = studentId,
            UserId = userId,
            LinkType = StaffLinkType.Teacher,
            AccessLevel = AccessLevel.Edit,
            IsActive = true
        };

        staffLink.StudentId.Should().Be(studentId);
        staffLink.UserId.Should().Be(userId);
        staffLink.LinkType.Should().Be(StaffLinkType.Teacher);
        staffLink.AccessLevel.Should().Be(AccessLevel.Edit);
        staffLink.IsActive.Should().BeTrue();
    }

    [Theory]
    [InlineData(StaffLinkType.Teacher)]
    [InlineData(StaffLinkType.Assistant)]
    [InlineData(StaffLinkType.SPP)]
    [InlineData(StaffLinkType.PPP)]
    [InlineData(StaffLinkType.SPC)]
    public void StudentStaffLink_AllLinkTypes_CanBeSet(StaffLinkType linkType)
    {
        var staffLink = new StudentStaffLink
        {
            LinkType = linkType
        };

        staffLink.LinkType.Should().Be(linkType);
    }

    [Theory]
    [InlineData(AccessLevel.Read)]
    [InlineData(AccessLevel.Edit)]
    public void StudentStaffLink_AllAccessLevels_CanBeSet(AccessLevel accessLevel)
    {
        var staffLink = new StudentStaffLink
        {
            AccessLevel = accessLevel
        };

        staffLink.AccessLevel.Should().Be(accessLevel);
    }

    [Fact]
    public void StudentStaffLink_WithStudent_NavigationPropertyIsSet()
    {
        var student = new Student
        {
            FirstName = "Marie",
            LastName = "Nováková"
        };

        var staffLink = new StudentStaffLink
        {
            Student = student,
            LinkType = StaffLinkType.Teacher,
            AccessLevel = AccessLevel.Read
        };

        staffLink.Student.Should().Be(student);
    }

    [Fact]
    public void StudentStaffLink_TeacherWithEditAccess_HasEditPermission()
    {
        var staffLink = new StudentStaffLink
        {
            LinkType = StaffLinkType.Teacher,
            AccessLevel = AccessLevel.Edit
        };

        staffLink.AccessLevel.Should().Be(AccessLevel.Edit);
    }

    [Fact]
    public void StudentStaffLink_AssistantWithReadAccess_HasReadOnlyPermission()
    {
        var staffLink = new StudentStaffLink
        {
            LinkType = StaffLinkType.Assistant,
            AccessLevel = AccessLevel.Read
        };

        staffLink.AccessLevel.Should().Be(AccessLevel.Read);
    }

    [Fact]
    public void StudentStaffLink_Deactivate_SetsIsActiveToFalse()
    {
        var staffLink = new StudentStaffLink
        {
            UserId = Guid.NewGuid().ToString(),
            LinkType = StaffLinkType.Teacher,
            AccessLevel = AccessLevel.Edit,
            IsActive = true
        };

        staffLink.IsActive = false;

        staffLink.IsActive.Should().BeFalse();
    }
}
