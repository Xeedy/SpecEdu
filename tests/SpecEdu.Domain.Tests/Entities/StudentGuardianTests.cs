using FluentAssertions;
using SpecEdu.Domain.Entities;
using SpecEdu.Domain.Enums;
using Xunit;

namespace SpecEdu.Domain.Tests.Entities;

/// <summary>
/// Tests for the StudentGuardian entity.
/// </summary>
public class StudentGuardianTests
{
    [Fact]
    public void StudentGuardian_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var guardian = new StudentGuardian();

        // Assert
        guardian.ParentUserId.Should().BeEmpty();
        guardian.IsActive.Should().BeTrue();
        guardian.Student.Should().BeNull();
    }

    [Fact]
    public void StudentGuardian_SetAllProperties_PropertiesAreSet()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var parentUserId = Guid.NewGuid().ToString();

        // Act
        var guardian = new StudentGuardian
        {
            StudentId = studentId,
            ParentUserId = parentUserId,
            RelationshipType = RelationshipType.Mother,
            IsActive = true
        };

        // Assert
        guardian.StudentId.Should().Be(studentId);
        guardian.ParentUserId.Should().Be(parentUserId);
        guardian.RelationshipType.Should().Be(RelationshipType.Mother);
        guardian.IsActive.Should().BeTrue();
    }

    [Theory]
    [InlineData(RelationshipType.Mother)]
    [InlineData(RelationshipType.Father)]
    [InlineData(RelationshipType.LegalGuardian)]
    [InlineData(RelationshipType.Other)]
    public void StudentGuardian_AllRelationshipTypes_CanBeSet(RelationshipType relationshipType)
    {
        // Arrange & Act
        var guardian = new StudentGuardian
        {
            RelationshipType = relationshipType
        };

        // Assert
        guardian.RelationshipType.Should().Be(relationshipType);
    }

    [Fact]
    public void StudentGuardian_WithStudent_NavigationPropertyIsSet()
    {
        // Arrange
        var student = new Student
        {
            FirstName = "Jan",
            LastName = "Novák"
        };

        // Act
        var guardian = new StudentGuardian
        {
            Student = student,
            RelationshipType = RelationshipType.Mother
        };

        // Assert
        guardian.Student.Should().Be(student);
    }

    [Fact]
    public void StudentGuardian_Deactivate_SetsIsActiveToFalse()
    {
        // Arrange
        var guardian = new StudentGuardian
        {
            ParentUserId = Guid.NewGuid().ToString(),
            RelationshipType = RelationshipType.Father,
            IsActive = true
        };

        // Act
        guardian.IsActive = false;

        // Assert
        guardian.IsActive.Should().BeFalse();
    }
}
