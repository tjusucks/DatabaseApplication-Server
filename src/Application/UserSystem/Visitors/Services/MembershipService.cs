using DbApp.Domain.Constants.UserSystem;
using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Enums.UserSystem;

namespace DbApp.Application.UserSystem.Visitors.Services;

/// <summary>
/// Service for membership-related business logic.
/// </summary>
public static class MembershipService
{
    /// <summary>
    /// Determines the member level based on points.
    /// </summary>
    /// <param name="points">The current points.</param>
    /// <returns>The appropriate member level string.</returns>
    public static string DetermineMemberLevel(int points)
    {
        return points switch
        {
            >= MembershipConstants.PointsThresholds.Platinum => MembershipConstants.LevelNames.Platinum,
            >= MembershipConstants.PointsThresholds.Gold => MembershipConstants.LevelNames.Gold,
            >= MembershipConstants.PointsThresholds.Silver => MembershipConstants.LevelNames.Silver,
            _ => MembershipConstants.LevelNames.Bronze
        };
    }

    /// <summary>
    /// Gets the discount multiplier for a visitor.
    /// Only members are eligible for discounts.
    /// </summary>
    /// <param name="visitor">The visitor to check.</param>
    /// <returns>The discount multiplier (e.g., 0.7 for 30% discount).</returns>
    public static decimal GetDiscountMultiplier(Visitor visitor)
    {
        // Only members get discounts
        if (visitor.VisitorType != VisitorType.Member)
        {
            return 1.0m; // No discount for regular visitors
        }

        return GetDiscountMultiplierByLevel(visitor.MemberLevel);
    }

    /// <summary>
    /// Gets the discount multiplier for a member level (internal helper method).
    /// </summary>
    /// <param name="memberLevel">The member level.</param>
    /// <returns>The discount multiplier (e.g., 0.7 for 30% discount).</returns>
    internal static decimal GetDiscountMultiplierByLevel(string? memberLevel)
    {
        return memberLevel switch
        {
            MembershipConstants.LevelNames.Platinum => MembershipConstants.DiscountMultipliers.Platinum,
            MembershipConstants.LevelNames.Gold => MembershipConstants.DiscountMultipliers.Gold,
            MembershipConstants.LevelNames.Silver => MembershipConstants.DiscountMultipliers.Silver,
            _ => MembershipConstants.DiscountMultipliers.Bronze
        };
    }

    /// <summary>
    /// Calculates the final price after applying member discount.
    /// Only members are eligible for discounts.
    /// </summary>
    /// <param name="originalPrice">The original price.</param>
    /// <param name="visitor">The visitor to calculate discount for.</param>
    /// <returns>The final price after discount.</returns>
    public static decimal CalculateDiscountedPrice(decimal originalPrice, Visitor visitor)
    {
        var discountMultiplier = GetDiscountMultiplier(visitor);
        return originalPrice * discountMultiplier;
    }

    /// <summary>
    /// Updates the visitor's member level based on their current points.
    /// Only members can have their levels updated.
    /// </summary>
    /// <param name="visitor">The visitor to update.</param>
    /// <returns>True if the level was changed, false otherwise.</returns>
    public static bool UpdateMemberLevel(Visitor visitor)
    {
        // Only members can have member levels
        if (visitor.VisitorType != VisitorType.Member)
        {
            return false;
        }

        var newLevel = DetermineMemberLevel(visitor.Points);
        var levelChanged = visitor.MemberLevel != newLevel;

        if (levelChanged)
        {
            visitor.MemberLevel = newLevel;
            visitor.UpdatedAt = DateTime.UtcNow;
        }

        return levelChanged;
    }

    /// <summary>
    /// Upgrades a visitor to member status.
    /// Requires either phone number or email to be registered.
    /// </summary>
    /// <param name="visitor">The visitor to upgrade.</param>
    /// <exception cref="InvalidOperationException">Thrown when visitor doesn't meet membership requirements.</exception>
    public static void UpgradeToMember(Visitor visitor)
    {
        if (visitor.VisitorType != VisitorType.Regular)
        {
            return; // Already a member
        }

        // Check membership requirements - either email or phone number is required
        if (!visitor.User.IsEligibleForMemberUpgrade())
        {
            throw new InvalidOperationException("Either email or phone number is required to become a member");
        }

        visitor.VisitorType = VisitorType.Member;
        visitor.MemberSince = DateTime.UtcNow;
        visitor.MemberLevel = DetermineMemberLevel(visitor.Points);
        visitor.UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Calculates points to award based on activity type.
    /// </summary>
    /// <param name="activityType">The type of activity.</param>
    /// <param name="baseAmount">Base amount for calculation (e.g., ticket price).</param>
    /// <returns>The points to award.</returns>
    public static int CalculatePointsForActivity(string activityType, decimal baseAmount = 0)
    {
        return activityType.ToLowerInvariant() switch
        {
            "ticket_purchase" => Math.Max(MembershipConstants.PointsEarning.TicketPurchase, (int)(baseAmount / 10)),
            "park_entry" => MembershipConstants.PointsEarning.ParkEntry,
            "ride_usage" => MembershipConstants.PointsEarning.RideUsage,
            "event_participation" => MembershipConstants.PointsEarning.EventParticipation,
            "birthday_bonus" => MembershipConstants.PointsEarning.BirthdayBonus,
            _ => 0
        };
    }

    /// <summary>
    /// Checks if it's the visitor's birthday and awards bonus points if applicable.
    /// </summary>
    /// <param name="visitor">The visitor to check.</param>
    /// <returns>The birthday bonus points awarded, or 0 if not applicable.</returns>
    public static int CheckAndAwardBirthdayBonus(Visitor visitor)
    {
        if (visitor.User?.BirthDate.HasValue == true)
        {
            var today = DateTime.Today;
            var birthDate = visitor.User.BirthDate.Value.Date;
            
            if (today.Month == birthDate.Month && today.Day == birthDate.Day)
            {
                return MembershipConstants.PointsEarning.BirthdayBonus;
            }
        }
        
        return 0;
    }
}
