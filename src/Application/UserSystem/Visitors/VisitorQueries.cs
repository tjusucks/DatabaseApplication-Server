using DbApp.Application.UserSystem.Visitors.DTOs;
using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Enums.UserSystem;
using MediatR;

namespace DbApp.Application.UserSystem.Visitors;

// === 队友的访客历史和信息DTOs ===

/// <summary>
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

// === 基础查询功能 ===

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
/// </summary>
/// <param name="VisitorType">The visitor type to filter by.</param>
public record GetVisitorsByTypeQuery(VisitorType VisitorType) : IRequest<List<Visitor>>;

/// <summary>
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
/// Unified RESTful search API for visitors.
/// Supports keyword search and multiple filters with pagination.
/// Based on resource (visitors) rather than search parameters.
/// </summary>
/// <param name="Keyword">General keyword search (name, email, phone).</param>
/// <param name="VisitorType">Filter by visitor type.</param>
/// <param name="MemberLevel">Filter by member level.</param>
/// <param name="IsBlacklisted">Filter by blacklist status.</param>
/// <param name="MinPoints">Minimum points filter.</param>
/// <param name="MaxPoints">Maximum points filter.</param>
/// <param name="StartDate">Registration start date filter.</param>
/// <param name="EndDate">Registration end date filter.</param>
/// <param name="Page">Page number for pagination (1-based).</param>
/// <param name="PageSize">Page size for pagination (max 100).</param>
public record SearchVisitorsQuery(
    string? Keyword = null,
    VisitorType? VisitorType = null,
    string? MemberLevel = null,
    bool? IsBlacklisted = null,
    int? MinPoints = null,
    int? MaxPoints = null,
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<SearchVisitorsResult>;

/// <summary>
/// RESTful search result with pagination metadata.
/// </summary>
public class SearchVisitorsResult
{
    public List<VisitorResponseDto> Visitors { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;

    /// <summary>
    /// Applied filters for transparency.
    /// </summary>
    public SearchFilters AppliedFilters { get; set; } = new();
}

/// <summary>
/// Applied search filters for API transparency.
/// </summary>
public class SearchFilters
{
    public string? Keyword { get; set; }
    public VisitorType? VisitorType { get; set; }
    public string? MemberLevel { get; set; }
    public bool? IsBlacklisted { get; set; }
    public int? MinPoints { get; set; }
    public int? MaxPoints { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

// === 您的会员和积分查询功能 ===

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
