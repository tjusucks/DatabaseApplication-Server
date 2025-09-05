using DbApp.Domain.Enums.UserSystem;
using DbApp.Application.UserSystem.Users;
using DbApp.Application.Common;

namespace DbApp.Application.UserSystem.Visitors;

/// <summary>
/// DTO for visitor query response, including user and visitor-specific fields.
/// </summary>
public class VisitorDto
{
    public UserDto User { get; set; } = null!;
    public int VisitorId { get; set; }
    public VisitorType VisitorType { get; set; }
    public int Points { get; set; }
    public string? MemberLevel { get; set; }
    public DateTime? MemberSince { get; set; }
    public bool IsBlacklisted { get; set; }
    public int Height { get; set; }
}

/// <summary>
/// DTO for visitor statistics response.
/// </summary>
public class VisitorStatsDto
{
    public int TotalVisitors { get; set; }
    public int TotalMembers { get; set; }
    public int RegularVisitors { get; set; }
    public int BlacklistedVisitors { get; set; }
    public int BronzeMembers { get; set; }
    public int SilverMembers { get; set; }
    public int GoldMembers { get; set; }
    public int PlatinumMembers { get; set; }
    public decimal MembershipRate { get; set; }
    public int TotalPointsIssued { get; set; }
    public double AveragePointsPerMember { get; set; }
}

public class SearchVisitorsResult : PaginatedResult<VisitorDto>
{
}

public class GroupedVisitorStatsDto : GroupedResult<VisitorStatsDto>
{
}
