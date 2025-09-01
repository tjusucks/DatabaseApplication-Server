using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Enums.UserSystem;
using MediatR;

namespace DbApp.Application.UserSystem.Visitors;

/// <summary>
/// Query to get all visitors.
/// </summary>
public record GetAllVisitorsQuery : IRequest<List<Visitor>>;

/// <summary>
/// Query to get a visitor by ID.
/// </summary>
/// <param name="VisitorId">The visitor ID.</param>
public record GetVisitorByIdQuery(int VisitorId) : IRequest<Visitor?>;

/// <summary>
/// Query to get a visitor by user ID.
/// </summary>
/// <param name="UserId">The user ID.</param>
public record GetVisitorByUserIdQuery(int UserId) : IRequest<Visitor?>;

/// <summary>
/// Query to get visitors by type.
/// </summary>
/// <param name="VisitorType">The visitor type to filter by.</param>
public record GetVisitorsByTypeQuery(VisitorType VisitorType) : IRequest<List<Visitor>>;

/// <summary>
/// Query to get visitors by member level.
/// </summary>
/// <param name="MemberLevel">The member level to filter by.</param>
public record GetVisitorsByMemberLevelQuery(string MemberLevel) : IRequest<List<Visitor>>;

/// <summary>
/// Query to get visitors by points range.
/// </summary>
/// <param name="MinPoints">Minimum points.</param>
/// <param name="MaxPoints">Maximum points.</param>
public record GetVisitorsByPointsRangeQuery(int MinPoints, int MaxPoints) : IRequest<List<Visitor>>;

/// <summary>
/// Query to get member statistics.
/// </summary>
public record GetMembershipStatisticsQuery : IRequest<MembershipStatistics>;

/// <summary>
/// Membership statistics data transfer object.
/// </summary>
public class MembershipStatistics
{
    public int TotalVisitors { get; set; }
    public int TotalMembers { get; set; }
    public int BronzeMembers { get; set; }
    public int SilverMembers { get; set; }
    public int GoldMembers { get; set; }
    public int PlatinumMembers { get; set; }
    public decimal MembershipRate { get; set; }
    public int TotalPointsIssued { get; set; }
    public double AveragePointsPerMember { get; set; }
}
