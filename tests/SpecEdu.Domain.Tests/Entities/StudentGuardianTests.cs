using FluentAssertions;
using SpecEdu.Domain.Entities;
using SpecEdu.Domain.Enums;
using Xunit;

namespace SpecEdu.Domain.Tests.Entities;

public class StudentGuardianTests
{
    [Fact]
    public void StudentGuardian_DefaultValues_AreCorrect()
    {
        var guardian = new StudentGuardian();

        guardian.ParentUserId.Should().BeEmpty();
        guardian.IsActive.Should().BeTrue();
        guardian.Student.Should().BeNull();
    }

    [Fact]
    public void StudentGuardian_SetAllProperties_PropertiesAreSet()
    {
        var studentId = Guid.NewGuid();
        var parentUserId = Guid.NewGuid().ToString();

        var guardian = new StudentGuardian
        {
            StudentId = studentId,
            ParentUserId = parentUserId,
            RelationshipType = RelationshipType.Mother,
            IsActive = true
        };

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
        var guardian = new StudentGuardian
        {
            RelationshipType = relationshipType
        };

        guardian.RelationshipType.Should().Be(relationshipType);
    }

    [Fact]
    public void StudentGuardian_WithStudent_NavigationPropertyIsSet()
    {
        var student = new Student
        {
            FirstName = "Jan",
            LastName = "Novák"
        };

        var guardian = new StudentGuardian
        {
            Student = student,
            RelationshipType = RelationshipType.Mother
        };

        guardian.Student.Should().Be(student);
    }

    [Fact]
    public void StudentGuardian_Deactivate_SetsIsActiveToFalse()
    {
        var guardian = new StudentGuardian
        {
            ParentUserId = Guid.NewGuid().ToString(),
            RelationshipType = RelationshipType.Father,
            IsActive = true
        };

        guardian.IsActive = false;

        guardian.IsActive.Should().BeFalse();
    }
}
