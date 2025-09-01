using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Enums.UserSystem;
using MediatR;

namespace DbApp.Application.UserSystem.Visitors;

/// <summary>
<<<<<<< HEAD
/// DTO for visitor history information including user details and entry records.
/// </summary>
/// <param name="VisitorId">The visitor ID.</param>
/// <param name="Username">The username.</param>
/// <param name="DisplayName">The display name.</param>
/// <param name="Email">The email address.</param>
/// <param name="PhoneNumber">The phone number.</param>
/// <param name="BirthDate">The birth date.</param>
/// <param name="Age">Calculated age based on birth date.</param>
/// <param name="Gender">The gender.</param>
/// <param name="RegisterTime">The registration time.</param>
/// <param name="VisitorType">The visitor type.</param>
/// <param name="Points">Membership points.</param>
/// <param name="MemberLevel">Member level.</param>
/// <param name="MemberSince">Member since date.</param>
/// <param name="IsBlacklisted">Whether the visitor is blacklisted.</param>
/// <param name="Height">Height in centimeters.</param>
/// <param name="TotalVisits">Total number of visits.</param>
/// <param name="LastVisitDate">Date of last visit.</param>
/// <param name="RecentEntryRecords">Recent entry records (last 10).</param>
public record VisitorHistoryDto(
    int VisitorId,
    string Username,
    string DisplayName,
    string Email,
    string? PhoneNumber,
    DateTime? BirthDate,
    int? Age,
    Gender? Gender,
    DateTime RegisterTime,
    VisitorType VisitorType,
    int Points,
    string? MemberLevel,
    DateTime? MemberSince,
    bool IsBlacklisted,
    int Height,
    int TotalVisits,
    DateTime? LastVisitDate,
    List<EntryRecordSummaryDto> RecentEntryRecords
);

/// <summary>
/// DTO for entry record summary in visitor history.
/// </summary>
/// <param name="EntryRecordId">The entry record ID.</param>
/// <param name="EntryTime">Entry time.</param>
/// <param name="ExitTime">Exit time (if exited).</param>
/// <param name="EntryGate">Entry gate name.</param>
/// <param name="ExitGate">Exit gate name.</param>
/// <param name="Duration">Duration of visit (if exited).</param>
public record EntryRecordSummaryDto(
    int EntryRecordId,
    DateTime EntryTime,
    DateTime? ExitTime,
    string EntryGate,
    string? ExitGate,
    TimeSpan? Duration
);

/// <summary>
/// DTO for visitor basic information.
/// </summary>
/// <param name="VisitorId">The visitor ID.</param>
/// <param name="Username">The username.</param>
/// <param name="DisplayName">The display name.</param>
/// <param name="Email">The email address.</param>
/// <param name="PhoneNumber">The phone number.</param>
/// <param name="Age">Calculated age.</param>
/// <param name="RegisterTime">Registration time.</param>
/// <param name="IsBlacklisted">Whether blacklisted.</param>
/// <param name="VisitorType">Visitor type.</param>
/// <param name="Points">Membership points.</param>
public record VisitorBasicInfoDto(
    int VisitorId,
    string Username,
    string DisplayName,
    string Email,
    string? PhoneNumber,
    int? Age,
    DateTime RegisterTime,
    bool IsBlacklisted,
    VisitorType VisitorType,
    int Points
);

/// <summary>
=======
>>>>>>> 1bba8b9 (feat: implement membership registration and points system)
/// Query to get all visitors.
/// </summary>
public record GetAllVisitorsQuery : IRequest<List<Visitor>>;

/// <summary>
/// Query to get a visitor by ID.
/// </summary>
/// <param name="VisitorId">The visitor ID.</param>
public record GetVisitorByIdQuery(int VisitorId) : IRequest<Visitor?>;

/// <summary>
<<<<<<< HEAD
/// Query to get visitor by user ID.
=======
/// Query to get a visitor by user ID.
>>>>>>> 1bba8b9 (feat: implement membership registration and points system)
/// </summary>
/// <param name="UserId">The user ID.</param>
public record GetVisitorByUserIdQuery(int UserId) : IRequest<Visitor?>;

/// <summary>
<<<<<<< HEAD
/// Query to search visitors by name (display name or username).
/// </summary>
/// <param name="Name">The name to search for (partial match).</param>
public record SearchVisitorsByNameQuery(string Name) : IRequest<List<Visitor>>;

/// <summary>
/// Query to search visitors by phone number.
/// </summary>
/// <param name="PhoneNumber">The phone number to search for.</param>
public record SearchVisitorsByPhoneQuery(string PhoneNumber) : IRequest<List<Visitor>>;

/// <summary>
/// Query to get visitors by blacklist status.
/// </summary>
/// <param name="IsBlacklisted">Whether to get blacklisted or non-blacklisted visitors.</param>
public record GetVisitorsByBlacklistStatusQuery(bool IsBlacklisted) : IRequest<List<Visitor>>;

/// <summary>
/// Query to get visitors by visitor type.
=======
/// Query to get visitors by type.
>>>>>>> 1bba8b9 (feat: implement membership registration and points system)
/// </summary>
/// <param name="VisitorType">The visitor type to filter by.</param>
public record GetVisitorsByTypeQuery(VisitorType VisitorType) : IRequest<List<Visitor>>;

/// <summary>
<<<<<<< HEAD
/// Query to get visitors registered within a date range.
/// </summary>
/// <param name="StartDate">Start date for registration.</param>
/// <param name="EndDate">End date for registration.</param>
public record GetVisitorsByRegistrationDateRangeQuery(
    DateTime StartDate,
    DateTime EndDate
) : IRequest<List<Visitor>>;

/// <summary>
/// Query to get visitor history information including entry records.
/// </summary>
/// <param name="VisitorId">The visitor ID.</param>
public record GetVisitorHistoryQuery(int VisitorId) : IRequest<VisitorHistoryDto?>;

/// <summary>
/// Query to search visitors with multiple criteria.
/// </summary>
/// <param name="Name">Optional name filter.</param>
/// <param name="PhoneNumber">Optional phone number filter.</param>
/// <param name="IsBlacklisted">Optional blacklist status filter.</param>
/// <param name="VisitorType">Optional visitor type filter.</param>
/// <param name="StartDate">Optional start date for registration.</param>
/// <param name="EndDate">Optional end date for registration.</param>
public record SearchVisitorsQuery(
    string? Name = null,
    string? PhoneNumber = null,
    bool? IsBlacklisted = null,
    VisitorType? VisitorType = null,
    DateTime? StartDate = null,
    DateTime? EndDate = null
) : IRequest<List<Visitor>>;
=======
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
>>>>>>> 1bba8b9 (feat: implement membership registration and points system)
