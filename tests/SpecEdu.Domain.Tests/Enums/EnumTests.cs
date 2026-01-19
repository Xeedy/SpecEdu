using FluentAssertions;
using SpecEdu.Domain.Enums;
using Xunit;

namespace SpecEdu.Domain.Tests.Enums;

/// <summary>
/// Tests for domain enums to ensure correct values and coverage.
/// </summary>
public class EnumTests
{
    #region RelationshipType Tests

    [Fact]
    public void RelationshipType_HasExpectedValues()
    {
        // Assert
        Enum.GetValues<RelationshipType>().Should().HaveCount(4);
        Enum.IsDefined(RelationshipType.Mother).Should().BeTrue();
        Enum.IsDefined(RelationshipType.Father).Should().BeTrue();
        Enum.IsDefined(RelationshipType.LegalGuardian).Should().BeTrue();
        Enum.IsDefined(RelationshipType.Other).Should().BeTrue();
    }

    [Theory]
    [InlineData(RelationshipType.Mother, 1)]
    [InlineData(RelationshipType.Father, 2)]
    [InlineData(RelationshipType.LegalGuardian, 3)]
    [InlineData(RelationshipType.Other, 4)]
    public void RelationshipType_HasCorrectUnderlyingValues(RelationshipType type, int expectedValue)
    {
        // Assert
        ((int)type).Should().Be(expectedValue);
    }

    #endregion

    #region StaffLinkType Tests

    [Fact]
    public void StaffLinkType_HasExpectedValues()
    {
        // Assert
        Enum.GetValues<StaffLinkType>().Should().HaveCount(5);
        Enum.IsDefined(StaffLinkType.Teacher).Should().BeTrue();
        Enum.IsDefined(StaffLinkType.Assistant).Should().BeTrue();
        Enum.IsDefined(StaffLinkType.SPP).Should().BeTrue();
        Enum.IsDefined(StaffLinkType.PPP).Should().BeTrue();
        Enum.IsDefined(StaffLinkType.SPC).Should().BeTrue();
    }

    [Theory]
    [InlineData(StaffLinkType.Teacher, 1)]
    [InlineData(StaffLinkType.Assistant, 2)]
    [InlineData(StaffLinkType.SPP, 3)]
    [InlineData(StaffLinkType.PPP, 4)]
    [InlineData(StaffLinkType.SPC, 5)]
    public void StaffLinkType_HasCorrectUnderlyingValues(StaffLinkType type, int expectedValue)
    {
        // Assert
        ((int)type).Should().Be(expectedValue);
    }

    #endregion

    #region AccessLevel Tests

    [Fact]
    public void AccessLevel_HasExpectedValues()
    {
        // Assert
        Enum.GetValues<AccessLevel>().Should().HaveCount(2);
        Enum.IsDefined(AccessLevel.Read).Should().BeTrue();
        Enum.IsDefined(AccessLevel.Edit).Should().BeTrue();
    }

    [Theory]
    [InlineData(AccessLevel.Read, 1)]
    [InlineData(AccessLevel.Edit, 2)]
    public void AccessLevel_HasCorrectUnderlyingValues(AccessLevel level, int expectedValue)
    {
        // Assert
        ((int)level).Should().Be(expectedValue);
    }

    [Fact]
    public void AccessLevel_ReadIsLessThanEdit()
    {
        // This ensures that Read (0) < Edit (1) for logical comparisons
        ((int)AccessLevel.Read).Should().BeLessThan((int)AccessLevel.Edit);
    }

    #endregion

    #region DiaryEntryType Tests

    [Fact]
    public void DiaryEntryType_HasExpectedValues()
    {
        // Assert
        Enum.GetValues<DiaryEntryType>().Should().HaveCount(5);
        Enum.IsDefined(DiaryEntryType.Note).Should().BeTrue();
        Enum.IsDefined(DiaryEntryType.PhoneCall).Should().BeTrue();
        Enum.IsDefined(DiaryEntryType.Meeting).Should().BeTrue();
        Enum.IsDefined(DiaryEntryType.ParentCollaboration).Should().BeTrue();
        Enum.IsDefined(DiaryEntryType.PppSpcCollaboration).Should().BeTrue();
    }

    [Theory]
    [InlineData(DiaryEntryType.Note, 1)]
    [InlineData(DiaryEntryType.PhoneCall, 2)]
    [InlineData(DiaryEntryType.Meeting, 3)]
    [InlineData(DiaryEntryType.ParentCollaboration, 4)]
    [InlineData(DiaryEntryType.PppSpcCollaboration, 5)]
    public void DiaryEntryType_HasCorrectUnderlyingValues(DiaryEntryType type, int expectedValue)
    {
        // Assert
        ((int)type).Should().Be(expectedValue);
    }

    #endregion

    #region DiaryVisibility Tests

    [Fact]
    public void DiaryVisibility_HasExpectedValues()
    {
        // Assert
        Enum.GetValues<DiaryVisibility>().Should().HaveCount(2);
        Enum.IsDefined(DiaryVisibility.SchoolOnly).Should().BeTrue();
        Enum.IsDefined(DiaryVisibility.ParentVisible).Should().BeTrue();
    }

    [Theory]
    [InlineData(DiaryVisibility.SchoolOnly, 1)]
    [InlineData(DiaryVisibility.ParentVisible, 2)]
    public void DiaryVisibility_HasCorrectUnderlyingValues(DiaryVisibility visibility, int expectedValue)
    {
        // Assert
        ((int)visibility).Should().Be(expectedValue);
    }

    #endregion

    #region Enum Parsing Tests

    [Theory]
    [InlineData("Mother", RelationshipType.Mother)]
    [InlineData("Father", RelationshipType.Father)]
    [InlineData("LegalGuardian", RelationshipType.LegalGuardian)]
    [InlineData("Other", RelationshipType.Other)]
    public void RelationshipType_CanBeParsedFromString(string value, RelationshipType expected)
    {
        // Act
        var result = Enum.Parse<RelationshipType>(value);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("Teacher", StaffLinkType.Teacher)]
    [InlineData("Assistant", StaffLinkType.Assistant)]
    [InlineData("SPP", StaffLinkType.SPP)]
    [InlineData("PPP", StaffLinkType.PPP)]
    [InlineData("SPC", StaffLinkType.SPC)]
    public void StaffLinkType_CanBeParsedFromString(string value, StaffLinkType expected)
    {
        // Act
        var result = Enum.Parse<StaffLinkType>(value);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("Note", DiaryEntryType.Note)]
    [InlineData("PhoneCall", DiaryEntryType.PhoneCall)]
    [InlineData("Meeting", DiaryEntryType.Meeting)]
    [InlineData("ParentCollaboration", DiaryEntryType.ParentCollaboration)]
    [InlineData("PppSpcCollaboration", DiaryEntryType.PppSpcCollaboration)]
    public void DiaryEntryType_CanBeParsedFromString(string value, DiaryEntryType expected)
    {
        // Act
        var result = Enum.Parse<DiaryEntryType>(value);

        // Assert
        result.Should().Be(expected);
    }

    #endregion
}
