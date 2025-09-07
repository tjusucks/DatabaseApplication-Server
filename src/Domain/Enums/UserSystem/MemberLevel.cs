namespace DbApp.Domain.Enums.UserSystem;

/// <summary>
/// Enum representing different membership levels.
/// </summary>
public enum MemberLevel
{
    /// <summary>
    /// Bronze membership level (0-999 points).
    /// </summary>
    Bronze = 0,

    /// <summary>
    /// Silver membership level (1000-4999 points).
    /// </summary>
    Silver = 1,

    /// <summary>
    /// Gold membership level (5000-9999 points).
    /// </summary>
    Gold = 2,

    /// <summary>
    /// Platinum membership level (10000+ points).
    /// </summary>
    Platinum = 3
}
