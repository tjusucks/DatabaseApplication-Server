namespace DbApp.Domain.Constants.UserSystem;

/// <summary>
/// Constants related to membership system.
/// </summary>
public static class MembershipConstants
{
    /// <summary>
    /// Points thresholds for different membership levels.
    /// </summary>
    public static class PointsThresholds
    {
        public const int Bronze = 0;
        public const int Silver = 1000;
        public const int Gold = 5000;
        public const int Platinum = 10000;
    }

    /// <summary>
    /// Points earned for different activities.
    /// </summary>
    public static class PointsEarning
    {
        public const int TicketPurchase = 10;
        public const int ParkEntry = 5;
        public const int RideUsage = 2;
        public const int EventParticipation = 15;
        public const int BirthdayBonus = 50;
    }

    /// <summary>
    /// Membership level names.
    /// </summary>
    public static class LevelNames
    {
        public const string Bronze = "Bronze";
        public const string Silver = "Silver";
        public const string Gold = "Gold";
        public const string Platinum = "Platinum";
    }

    /// <summary>
    /// Membership discount multipliers (final price = original price * multiplier).
    /// </summary>
    public static class DiscountMultipliers
    {
        public const decimal Bronze = 1.0m;    // 无折扣 (100%)
        public const decimal Silver = 0.9m;    // 9折 (90%)
        public const decimal Gold = 0.8m;      // 8折 (80%)
        public const decimal Platinum = 0.7m;  // 7折 (70%)
    }

    public static string GetLevelByPoints(int points)
    {
        if (points >= PointsThresholds.Platinum)
            return LevelNames.Platinum;
        if (points >= PointsThresholds.Gold)
            return LevelNames.Gold;
        if (points >= PointsThresholds.Silver)
            return LevelNames.Silver;
        return LevelNames.Bronze;
    }

    public static decimal GetDiscountMultiplier(string level)
    {
        return level switch
        {
            LevelNames.Platinum => DiscountMultipliers.Platinum,
            LevelNames.Gold => DiscountMultipliers.Gold,
            LevelNames.Silver => DiscountMultipliers.Silver,
            _ => DiscountMultipliers.Bronze,
        };
    }

    public static int GetPointsEarningForActivity(string activity)
    {
        return activity switch
        {
            "TicketPurchase" => PointsEarning.TicketPurchase,
            "ParkEntry" => PointsEarning.ParkEntry,
            "RideUsage" => PointsEarning.RideUsage,
            "EventParticipation" => PointsEarning.EventParticipation,
            "BirthdayBonus" => PointsEarning.BirthdayBonus,
            _ => 0,
        };
    }
}
