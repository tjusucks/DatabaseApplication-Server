namespace DbApp.Domain.Constants;

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
}
