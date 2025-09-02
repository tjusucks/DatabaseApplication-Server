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
    [InlineData("Bronze", 1.0)]    // 无折扣 (100%)
    [InlineData("Silver", 0.9)]    // 9折 (90%)
    [InlineData("Gold", 0.8)]      // 8折 (80%)
    [InlineData("Platinum", 0.7)]  // 7折 (70%)
    [InlineData(null, 1.0)]
    [InlineData("Unknown", 1.0)]
    public void GetDiscountMultiplier_ShouldReturnCorrectMultiplier(string? memberLevel, decimal expectedMultiplier)
    {
        // Act
        var result = MembershipService.GetDiscountMultiplier(memberLevel);

        // Assert
        Assert.Equal(expectedMultiplier, result);
    }

    [Theory]
    [InlineData(100.0, "Bronze", 100.0)]    // 100 * 1.0 = 100 (无折扣)
    [InlineData(100.0, "Silver", 90.0)]     // 100 * 0.9 = 90 (9折)
    [InlineData(100.0, "Gold", 80.0)]       // 100 * 0.8 = 80 (8折)
    [InlineData(100.0, "Platinum", 70.0)]   // 100 * 0.7 = 70 (7折)
    [InlineData(50.0, "Gold", 40.0)]        // 50 * 0.8 = 40
    public void CalculateDiscountedPrice_ShouldReturnCorrectPrice(decimal originalPrice, string memberLevel, decimal expectedPrice)
    {
        // Act
        var result = MembershipService.CalculateDiscountedPrice(originalPrice, memberLevel);

        // Assert
        Assert.Equal(expectedPrice, result);
    }

    [Fact]
    public void UpdateMemberLevel_WhenLevelChanges_ShouldReturnTrueAndUpdateLevel()
    {
        // Arrange
        var visitor = new Visitor
        {
            Points = 1500,
            MemberLevel = MembershipConstants.LevelNames.Bronze
        };

        // Act
        var result = MembershipService.UpdateMemberLevel(visitor);

        // Assert
        Assert.True(result);
        Assert.Equal(MembershipConstants.LevelNames.Silver, visitor.MemberLevel);
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
    public void UpgradeToMember_WhenRegularVisitor_ShouldUpgradeToMember()
    {
        // Arrange
        var visitor = new Visitor
        {
            VisitorType = VisitorType.Regular,
            Points = 1200
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
