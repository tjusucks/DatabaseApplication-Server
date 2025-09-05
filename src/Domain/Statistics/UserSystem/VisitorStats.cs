namespace DbApp.Domain.Statistics.UserSystem;

/// <summary>
/// Comprehensive statistics for visitors and members, including member levels.
/// </summary>
public class VisitorStats
{
    /// <summary>
    /// Total number of visitors (including members).
    /// </summary>
    public int TotalVisitors { get; set; }

    /// <summary>
    /// Total number of members.
    /// </summary>
    public int TotalMembers { get; set; }

    /// <summary>
    /// Number of regular (non-member) visitors.
    /// </summary>
    public int RegularVisitors { get; set; }

    /// <summary>
    /// Number of blacklisted visitors.
    /// </summary>
    public int BlacklistedVisitors { get; set; }

    /// <summary>
    /// Number of members by level.
    /// </summary>
    public int BronzeMembers { get; set; }
    public int SilverMembers { get; set; }
    public int GoldMembers { get; set; }
    public int PlatinumMembers { get; set; }

    /// <summary>
    /// Membership rate (members / total visitors).
    /// </summary>
    public decimal MembershipRate { get; set; }

    /// <summary>
    /// Total points issued to all members.
    /// </summary>
    public int TotalPointsIssued { get; set; }

    /// <summary>
    /// Average points per member.
    /// </summary>
    public double AveragePointsPerMember { get; set; }
}
