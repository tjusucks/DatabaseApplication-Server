using DbApp.Application.UserSystem.Visitors.Services;
using DbApp.Domain.Constants;
using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Enums.UserSystem;
using Xunit;

namespace Tests.UserSystem.Visitors;

/// <summary>
/// Unit tests for MembershipService.
/// </summary>
public class MembershipServiceTests
{
    [Theory]
    [InlineData(0, MembershipConstants.LevelNames.Bronze)]
    [InlineData(500, MembershipConstants.LevelNames.Bronze)]
    [InlineData(999, MembershipConstants.LevelNames.Bronze)]
    [InlineData(1000, MembershipConstants.LevelNames.Silver)]
    [InlineData(2500, MembershipConstants.LevelNames.Silver)]
    [InlineData(4999, MembershipConstants.LevelNames.Silver)]
    [InlineData(5000, MembershipConstants.LevelNames.Gold)]
    [InlineData(7500, MembershipConstants.LevelNames.Gold)]
    [InlineData(9999, MembershipConstants.LevelNames.Gold)]
    [InlineData(10000, MembershipConstants.LevelNames.Platinum)]
    [InlineData(15000, MembershipConstants.LevelNames.Platinum)]
    public void DetermineMemberLevel_ShouldReturnCorrectLevel(int points, string expectedLevel)
    {
        // Act
        var result = MembershipService.DetermineMemberLevel(points);

        // Assert
        Assert.Equal(expectedLevel, result);
    }

    [Theory]
    [InlineData(VisitorType.Member, "Bronze", 1.0)]    // 会员Bronze无折扣 (100%)
    [InlineData(VisitorType.Member, "Silver", 0.9)]    // 会员Silver 9折 (90%)
    [InlineData(VisitorType.Member, "Gold", 0.8)]      // 会员Gold 8折 (80%)
    [InlineData(VisitorType.Member, "Platinum", 0.7)]  // 会员Platinum 7折 (70%)
    [InlineData(VisitorType.Regular, "Silver", 1.0)]   // 普通访客无折扣 (100%)
    [InlineData(VisitorType.Regular, "Gold", 1.0)]     // 普通访客无折扣 (100%)
    public void GetDiscountMultiplier_ShouldReturnCorrectMultiplier(VisitorType visitorType, string memberLevel, decimal expectedMultiplier)
    {
        // Arrange
        var visitor = new Visitor
        {
            VisitorType = visitorType,
            MemberLevel = memberLevel,
            User = new User { Email = "test@example.com", PhoneNumber = "1234567890" }
        };

        // Act
        var result = MembershipService.GetDiscountMultiplier(visitor);

        // Assert
        Assert.Equal(expectedMultiplier, result);
    }

    [Theory]
    [InlineData(100.0, VisitorType.Member, "Bronze", 100.0)]    // 会员Bronze: 100 * 1.0 = 100 (无折扣)
    [InlineData(100.0, VisitorType.Member, "Silver", 90.0)]     // 会员Silver: 100 * 0.9 = 90 (9折)
    [InlineData(100.0, VisitorType.Member, "Gold", 80.0)]       // 会员Gold: 100 * 0.8 = 80 (8折)
    [InlineData(100.0, VisitorType.Member, "Platinum", 70.0)]   // 会员Platinum: 100 * 0.7 = 70 (7折)
    [InlineData(50.0, VisitorType.Member, "Gold", 40.0)]        // 会员Gold: 50 * 0.8 = 40
    [InlineData(100.0, VisitorType.Regular, "Gold", 100.0)]     // 普通访客: 100 * 1.0 = 100 (无折扣)
    public void CalculateDiscountedPrice_ShouldReturnCorrectPrice(decimal originalPrice, VisitorType visitorType, string memberLevel, decimal expectedPrice)
    {
        // Arrange
        var visitor = new Visitor
        {
            VisitorType = visitorType,
            MemberLevel = memberLevel,
            User = new User { Email = "test@example.com", PhoneNumber = "1234567890" }
        };

        // Act
        var result = MembershipService.CalculateDiscountedPrice(originalPrice, visitor);

        // Assert
        Assert.Equal(expectedPrice, result);
    }

    [Fact]
    public void UpdateMemberLevel_WhenMemberLevelChanges_ShouldReturnTrueAndUpdateLevel()
    {
        // Arrange
        var visitor = new Visitor
        {
            VisitorType = VisitorType.Member, // Must be a member
            Points = 1500,
            MemberLevel = MembershipConstants.LevelNames.Bronze,
            User = new User { Email = "test@example.com", PhoneNumber = "1234567890" }
        };

        // Act
        var result = MembershipService.UpdateMemberLevel(visitor);

        // Assert
        Assert.True(result);
        Assert.Equal(MembershipConstants.LevelNames.Silver, visitor.MemberLevel);
        Assert.NotNull(visitor.UpdatedAt);
    }

    [Fact]
    public void UpdateMemberLevel_WhenRegularVisitor_ShouldSetBronzeLevel()
    {
        // Arrange
        var visitor = new Visitor
        {
            VisitorType = VisitorType.Regular, // Regular visitor
            Points = 1500, // Even with high points
            MemberLevel = MembershipConstants.LevelNames.Silver, // Currently Silver
            User = new User { Email = "test@example.com", PhoneNumber = "1234567890" }
        };

        // Act
        var result = MembershipService.UpdateMemberLevel(visitor);

        // Assert
        Assert.True(result); // Level should change from Silver to Bronze
        Assert.Equal(MembershipConstants.LevelNames.Bronze, visitor.MemberLevel);
        Assert.NotNull(visitor.UpdatedAt);
    }

    [Fact]
    public void UpdateMemberLevel_WhenLevelDoesNotChange_ShouldReturnFalse()
    {
        // Arrange
        var visitor = new Visitor
        {
            Points = 500,
            MemberLevel = MembershipConstants.LevelNames.Bronze
        };

        // Act
        var result = MembershipService.UpdateMemberLevel(visitor);

        // Assert
        Assert.False(result);
        Assert.Equal(MembershipConstants.LevelNames.Bronze, visitor.MemberLevel);
    }

    [Fact]
    public void UpgradeToMember_WhenRegularVisitorWithPhoneAndEmail_ShouldUpgradeToMember()
    {
        // Arrange
        var visitor = new Visitor
        {
            VisitorType = VisitorType.Regular,
            Points = 1200,
            User = new User
            {
                Email = "test@example.com",
                PhoneNumber = "1234567890"
            }
        };

        // Act
        MembershipService.UpgradeToMember(visitor);

        // Assert
        Assert.Equal(VisitorType.Member, visitor.VisitorType);
        Assert.NotNull(visitor.MemberSince);
        Assert.Equal(MembershipConstants.LevelNames.Silver, visitor.MemberLevel);
        Assert.NotNull(visitor.UpdatedAt);
    }

    [Fact]
    public void UpgradeToMember_WhenVisitorHasOnlyEmail_ShouldUpgradeSuccessfully()
    {
        // Arrange
        var visitor = new Visitor
        {
            VisitorType = VisitorType.Regular,
            Points = 1200,
            User = new User
            {
                Email = "test@example.com",
                PhoneNumber = null // Only email, no phone
            }
        };

        // Act
        MembershipService.UpgradeToMember(visitor);

        // Assert
        Assert.Equal(VisitorType.Member, visitor.VisitorType);
        Assert.NotNull(visitor.MemberSince);
        Assert.Equal(MembershipConstants.LevelNames.Silver, visitor.MemberLevel);
    }

    [Fact]
    public void UpgradeToMember_WhenVisitorHasOnlyPhone_ShouldUpgradeSuccessfully()
    {
        // Arrange
        var visitor = new Visitor
        {
            VisitorType = VisitorType.Regular,
            Points = 1200,
            User = new User
            {
                Email = null, // No email
                PhoneNumber = "1234567890"
            }
        };

        // Act
        MembershipService.UpgradeToMember(visitor);

        // Assert
        Assert.Equal(VisitorType.Member, visitor.VisitorType);
        Assert.NotNull(visitor.MemberSince);
        Assert.Equal(MembershipConstants.LevelNames.Silver, visitor.MemberLevel);
    }

    [Fact]
    public void UpgradeToMember_WhenVisitorMissingBothEmailAndPhone_ShouldThrowException()
    {
        // Arrange
        var visitor = new Visitor
        {
            VisitorType = VisitorType.Regular,
            Points = 1200,
            User = new User
            {
                Email = null, // Missing email
                PhoneNumber = null // Missing phone
            }
        };

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            MembershipService.UpgradeToMember(visitor));
        Assert.Contains("Either email or phone number is required", exception.Message);
    }

    [Fact]
    public void UpgradeToMember_WhenAlreadyMember_ShouldNotChange()
    {
        // Arrange
        var originalMemberSince = DateTime.UtcNow.AddDays(-30);
        var visitor = new Visitor
        {
            VisitorType = VisitorType.Member,
            Points = 1200,
            MemberSince = originalMemberSince,
            MemberLevel = MembershipConstants.LevelNames.Silver
        };

        // Act
        MembershipService.UpgradeToMember(visitor);

        // Assert
        Assert.Equal(VisitorType.Member, visitor.VisitorType);
        Assert.Equal(originalMemberSince, visitor.MemberSince);
        Assert.Equal(MembershipConstants.LevelNames.Silver, visitor.MemberLevel);
    }

    [Theory]
    [InlineData("ticket_purchase", 0, MembershipConstants.PointsEarning.TicketPurchase)]
    [InlineData("ticket_purchase", 150, 15)] // 150/10 = 15, which is > 10
    [InlineData("park_entry", 0, MembershipConstants.PointsEarning.ParkEntry)]
    [InlineData("ride_usage", 0, MembershipConstants.PointsEarning.RideUsage)]
    [InlineData("event_participation", 0, MembershipConstants.PointsEarning.EventParticipation)]
    [InlineData("birthday_bonus", 0, MembershipConstants.PointsEarning.BirthdayBonus)]
    [InlineData("unknown_activity", 0, 0)]
    public void CalculatePointsForActivity_ShouldReturnCorrectPoints(string activityType, decimal baseAmount, int expectedPoints)
    {
        // Act
        var result = MembershipService.CalculatePointsForActivity(activityType, baseAmount);

        // Assert
        Assert.Equal(expectedPoints, result);
    }

    [Fact]
    public void CheckAndAwardBirthdayBonus_WhenTodayIsBirthday_ShouldReturnBonusPoints()
    {
        // Arrange
        var today = DateTime.Today;
        var visitor = new Visitor
        {
            User = new User
            {
                BirthDate = today.AddYears(-25) // Same month and day, different year
            }
        };

        // Act
        var result = MembershipService.CheckAndAwardBirthdayBonus(visitor);

        // Assert
        Assert.Equal(MembershipConstants.PointsEarning.BirthdayBonus, result);
    }

    [Fact]
    public void CheckAndAwardBirthdayBonus_WhenNotBirthday_ShouldReturnZero()
    {
        // Arrange
        var visitor = new Visitor
        {
            User = new User
            {
                BirthDate = DateTime.Today.AddDays(-1).AddYears(-25) // Different day
            }
        };

        // Act
        var result = MembershipService.CheckAndAwardBirthdayBonus(visitor);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void CheckAndAwardBirthdayBonus_WhenNoBirthDate_ShouldReturnZero()
    {
        // Arrange
        var visitor = new Visitor
        {
            User = new User
            {
                BirthDate = null
            }
        };

        // Act
        var result = MembershipService.CheckAndAwardBirthdayBonus(visitor);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void CheckAndAwardBirthdayBonus_WhenNoUser_ShouldReturnZero()
    {
        // Arrange
        var visitor = new Visitor
        {
            User = null!
        };

        // Act
        var result = MembershipService.CheckAndAwardBirthdayBonus(visitor);

        // Assert
        Assert.Equal(0, result);
    }
}
