using FluentAssertions;
using SpecEdu.Application.Common.Models;
using SpecEdu.Domain.Enums;
using Xunit;

namespace SpecEdu.Application.Tests.Models;

public class StudentAccessResultTests
{
    #region NoAccess Factory Tests

    [Fact]
    public void NoAccess_WithDefaultMessage_ReturnsCorrectResult()
    {
        var result = StudentAccessResult.NoAccess();

        result.CanAccess.Should().BeFalse();
        result.CanEdit.Should().BeFalse();
        result.AccessReason.Should().Be(AccessReason.NoAccess);
        result.Message.Should().Be("Nemáte oprávnění k přístupu k tomuto žákovi.");
    }

    [Fact]
    public void NoAccess_WithCustomMessage_ReturnsCorrectResult()
    {
        var result = StudentAccessResult.NoAccess("Uživatel nenalezen.");

        result.CanAccess.Should().BeFalse();
        result.CanEdit.Should().BeFalse();
        result.AccessReason.Should().Be(AccessReason.NoAccess);
        result.Message.Should().Be("Uživatel nenalezen.");
    }

    #endregion

    #region FullAccess Factory Tests

    [Fact]
    public void FullAccess_AsAdmin_ReturnsCorrectResult()
    {
        var result = StudentAccessResult.FullAccess(AccessReason.Admin, "Admin access granted.");

        result.CanAccess.Should().BeTrue();
        result.CanEdit.Should().BeTrue();
        result.AccessReason.Should().Be(AccessReason.Admin);
        result.Message.Should().Be("Admin access granted.");
    }

    [Fact]
    public void FullAccess_AsSchoolAdmin_ReturnsCorrectResult()
    {
        var result = StudentAccessResult.FullAccess(AccessReason.SchoolAdmin, "SchoolAdmin access granted.");

        result.CanAccess.Should().BeTrue();
        result.CanEdit.Should().BeTrue();
        result.AccessReason.Should().Be(AccessReason.SchoolAdmin);
        result.Message.Should().Be("SchoolAdmin access granted.");
    }

    [Fact]
    public void FullAccess_WithoutMessage_ReturnsNullMessage()
    {
        var result = StudentAccessResult.FullAccess(AccessReason.Admin);

        result.CanAccess.Should().BeTrue();
        result.CanEdit.Should().BeTrue();
        result.Message.Should().BeNull();
    }

    #endregion

    #region ReadOnly Factory Tests

    [Fact]
    public void ReadOnly_AsGuardian_ReturnsCorrectResult()
    {
        var result = StudentAccessResult.ReadOnly(AccessReason.Guardian, "Parent access granted.");

        result.CanAccess.Should().BeTrue();
        result.CanEdit.Should().BeFalse();
        result.AccessReason.Should().Be(AccessReason.Guardian);
        result.Message.Should().Be("Parent access granted.");
    }

    [Fact]
    public void ReadOnly_WithoutMessage_ReturnsNullMessage()
    {
        var result = StudentAccessResult.ReadOnly(AccessReason.Guardian);

        result.CanAccess.Should().BeTrue();
        result.CanEdit.Should().BeFalse();
        result.Message.Should().BeNull();
    }

    #endregion

    #region FromAccessLevel Factory Tests

    [Fact]
    public void FromAccessLevel_WithReadLevel_ReturnsReadOnlyAccess()
    {
        var result = StudentAccessResult.FromAccessLevel(
            AccessLevel.Read,
            AccessReason.StaffLink,
            "Staff with read access.");

        result.CanAccess.Should().BeTrue();
        result.CanEdit.Should().BeFalse();
        result.AccessReason.Should().Be(AccessReason.StaffLink);
        result.Message.Should().Be("Staff with read access.");
    }

    [Fact]
    public void FromAccessLevel_WithEditLevel_ReturnsEditAccess()
    {
        var result = StudentAccessResult.FromAccessLevel(
            AccessLevel.Edit,
            AccessReason.StaffLink,
            "Staff with edit access.");

        result.CanAccess.Should().BeTrue();
        result.CanEdit.Should().BeTrue();
        result.AccessReason.Should().Be(AccessReason.StaffLink);
        result.Message.Should().Be("Staff with edit access.");
    }

    #endregion

    #region AccessReason Enum Tests

    [Fact]
    public void AccessReason_HasExpectedValues()
    {
        Enum.GetValues<AccessReason>().Should().HaveCount(6);
        Enum.IsDefined(AccessReason.NoAccess).Should().BeTrue();
        Enum.IsDefined(AccessReason.Admin).Should().BeTrue();
        Enum.IsDefined(AccessReason.SchoolAdmin).Should().BeTrue();
        Enum.IsDefined(AccessReason.Guardian).Should().BeTrue();
        Enum.IsDefined(AccessReason.StaffLink).Should().BeTrue();
        Enum.IsDefined(AccessReason.DifferentSchool).Should().BeTrue();
    }

    [Theory]
    [InlineData(AccessReason.NoAccess, 0)]
    [InlineData(AccessReason.Admin, 1)]
    [InlineData(AccessReason.SchoolAdmin, 2)]
    [InlineData(AccessReason.Guardian, 3)]
    [InlineData(AccessReason.StaffLink, 4)]
    [InlineData(AccessReason.DifferentSchool, 5)]
    public void AccessReason_HasCorrectUnderlyingValues(AccessReason reason, int expectedValue)
    {
        ((int)reason).Should().Be(expectedValue);
    }

    #endregion

    #region Access Control Scenarios

    [Fact]
    public void AdminAccess_HasHighestPrivileges()
    {
        var adminResult = StudentAccessResult.FullAccess(AccessReason.Admin);
        var guardianResult = StudentAccessResult.ReadOnly(AccessReason.Guardian);
        var noAccessResult = StudentAccessResult.NoAccess();

        adminResult.CanAccess.Should().BeTrue();
        adminResult.CanEdit.Should().BeTrue();

        guardianResult.CanAccess.Should().BeTrue();
        guardianResult.CanEdit.Should().BeFalse();

        noAccessResult.CanAccess.Should().BeFalse();
        noAccessResult.CanEdit.Should().BeFalse();
    }

    [Fact]
    public void DirectPropertyAssignment_Works()
    {
        var result = new StudentAccessResult
        {
            CanAccess = true,
            CanEdit = false,
            AccessReason = AccessReason.Guardian,
            Message = "Custom message"
        };

        result.CanAccess.Should().BeTrue();
        result.CanEdit.Should().BeFalse();
        result.AccessReason.Should().Be(AccessReason.Guardian);
        result.Message.Should().Be("Custom message");
    }

    #endregion
}
