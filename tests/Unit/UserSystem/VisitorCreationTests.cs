using DbApp.Application.UserSystem.Visitors;
using DbApp.Application.UserSystem.Visitors.Services;
using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Enums.UserSystem;
using Xunit;

namespace DbApp.Tests.Unit.UserSystem;

/// <summary>
/// Tests for visitor creation and membership upgrade logic.
/// </summary>
public class VisitorCreationTests
{
    [Fact]
    public void CreateVisitorCommand_WithoutEmailAndPhone_ShouldBeValid()
    {
        // Arrange & Act
        var command = new CreateVisitorCommand(
            Username: "testuser",
            PasswordHash: "hashedpassword",
            Email: null,
            DisplayName: "Test User",
            PhoneNumber: null,
            BirthDate: null,
            Gender: null,
            VisitorType: VisitorType.Regular,
            Height: 170
        );

        // Assert
        Assert.NotNull(command);
        Assert.Null(command.Email);
        Assert.Null(command.PhoneNumber);
        Assert.Equal(VisitorType.Regular, command.VisitorType);
    }

    [Fact]
    public void CreateVisitorCommand_WithOnlyEmail_ShouldBeValid()
    {
        // Arrange & Act
        var command = new CreateVisitorCommand(
            Username: "testuser",
            PasswordHash: "hashedpassword",
            Email: "test@example.com",
            DisplayName: "Test User",
            PhoneNumber: null,
            BirthDate: null,
            Gender: null,
            VisitorType: VisitorType.Regular,
            Height: 170
        );

        // Assert
        Assert.NotNull(command);
        Assert.Equal("test@example.com", command.Email);
        Assert.Null(command.PhoneNumber);
    }

    [Fact]
    public void CreateVisitorCommand_WithOnlyPhone_ShouldBeValid()
    {
        // Arrange & Act
        var command = new CreateVisitorCommand(
            Username: "testuser",
            PasswordHash: "hashedpassword",
            Email: null,
            DisplayName: "Test User",
            PhoneNumber: "1234567890",
            BirthDate: null,
            Gender: null,
            VisitorType: VisitorType.Regular,
            Height: 170
        );

        // Assert
        Assert.NotNull(command);
        Assert.Null(command.Email);
        Assert.Equal("1234567890", command.PhoneNumber);
    }

    [Fact]
    public void User_HasContactInformation_WithEmail_ShouldReturnTrue()
    {
        // Arrange
        var user = new User
        {
            Email = "test@example.com",
            PhoneNumber = null
        };

        // Act & Assert
        Assert.True(user.HasContactInformation());
    }

    [Fact]
    public void User_HasContactInformation_WithPhone_ShouldReturnTrue()
    {
        // Arrange
        var user = new User
        {
            Email = null,
            PhoneNumber = "1234567890"
        };

        // Act & Assert
        Assert.True(user.HasContactInformation());
    }

    [Fact]
    public void User_HasContactInformation_WithBoth_ShouldReturnTrue()
    {
        // Arrange
        var user = new User
        {
            Email = "test@example.com",
            PhoneNumber = "1234567890"
        };

        // Act & Assert
        Assert.True(user.HasContactInformation());
    }

    [Fact]
    public void User_HasContactInformation_WithNeither_ShouldReturnFalse()
    {
        // Arrange
        var user = new User
        {
            Email = null,
            PhoneNumber = null
        };

        // Act & Assert
        Assert.False(user.HasContactInformation());
    }

    [Fact]
    public void User_HasContactInformation_WithEmptyStrings_ShouldReturnFalse()
    {
        // Arrange
        var user = new User
        {
            Email = "",
            PhoneNumber = "   "
        };

        // Act & Assert
        Assert.False(user.HasContactInformation());
    }

    [Fact]
    public void User_IsEligibleForMemberUpgrade_WithContactInfo_ShouldReturnTrue()
    {
        // Arrange
        var user = new User
        {
            Email = "test@example.com",
            PhoneNumber = null
        };

        // Act & Assert
        Assert.True(user.IsEligibleForMemberUpgrade());
    }

    [Fact]
    public void User_IsEligibleForMemberUpgrade_WithoutContactInfo_ShouldReturnFalse()
    {
        // Arrange
        var user = new User
        {
            Email = null,
            PhoneNumber = null
        };

        // Act & Assert
        Assert.False(user.IsEligibleForMemberUpgrade());
    }

    [Fact]
    public void MembershipService_UpgradeToMember_WithContactInfo_ShouldSucceed()
    {
        // Arrange
        var visitor = new Visitor
        {
            VisitorType = VisitorType.Regular,
            Points = 100,
            User = new User
            {
                Email = "test@example.com",
                PhoneNumber = null
            }
        };

        // Act
        MembershipService.UpgradeToMember(visitor);

        // Assert
        Assert.Equal(VisitorType.Member, visitor.VisitorType);
        Assert.NotNull(visitor.MemberSince);
        Assert.NotNull(visitor.MemberLevel);
        Assert.NotNull(visitor.UpdatedAt);
    }

    [Fact]
    public void MembershipService_UpgradeToMember_WithoutContactInfo_ShouldThrow()
    {
        // Arrange
        var visitor = new Visitor
        {
            VisitorType = VisitorType.Regular,
            Points = 100,
            User = new User
            {
                Email = null,
                PhoneNumber = null
            }
        };

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            MembershipService.UpgradeToMember(visitor));
        Assert.Contains("Either email or phone number is required", exception.Message);
    }

    [Fact]
    public void MembershipService_GetDiscountMultiplier_ForRegularVisitor_ShouldReturnNoDiscount()
    {
        // Arrange
        var visitor = new Visitor
        {
            VisitorType = VisitorType.Regular,
            MemberLevel = null
        };

        // Act
        var multiplier = MembershipService.GetDiscountMultiplier(visitor);

        // Assert
        Assert.Equal(1.0m, multiplier); // No discount for regular visitors
    }

    [Fact]
    public void MembershipService_GetDiscountMultiplier_ForBronzeMember_ShouldReturnNoDiscount()
    {
        // Arrange
        var visitor = new Visitor
        {
            VisitorType = VisitorType.Member,
            MemberLevel = "Bronze"
        };

        // Act
        var multiplier = MembershipService.GetDiscountMultiplier(visitor);

        // Assert
        Assert.Equal(1.0m, multiplier); // Bronze members get no discount
    }

    [Fact]
    public void MembershipService_GetDiscountMultiplier_ForSilverMember_ShouldReturnDiscount()
    {
        // Arrange
        var visitor = new Visitor
        {
            VisitorType = VisitorType.Member,
            MemberLevel = "Silver"
        };

        // Act
        var multiplier = MembershipService.GetDiscountMultiplier(visitor);

        // Assert
        Assert.Equal(0.9m, multiplier); // Silver members get 10% discount
    }
}
