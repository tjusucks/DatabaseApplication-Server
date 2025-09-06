using DbApp.Domain.Enums.UserSystem;
using MediatR;

namespace DbApp.Application.UserSystem.Visitors;

/// <summary>
/// Query for getting a visitor by ID.
/// </summary>
public record GetVisitorByIdQuery(int VisitorId) : IRequest<VisitorDto?>;

/// <summary>
/// Query for getting all visitors.
/// </summary>
public record GetAllVisitorsQuery() : IRequest<List<VisitorDto>>;

/// <summary>
/// Query for searching/filtering visitors with user and visitor criteria and pagination.
/// </summary>
public record SearchVisitorsQuery(
    string? Keyword,
    int? UserId,
    DateTime? BirthDateStart,
    DateTime? BirthDateEnd,
    Gender? Gender,
    DateTime? RegisterTimeStart,
    DateTime? RegisterTimeEnd,
    int? PermissionLevelMin,
    int? PermissionLevelMax,
    int? RoleId,
    DateTime? CreatedAtStart,
    DateTime? CreatedAtEnd,
    DateTime? UpdatedAtStart,
    DateTime? UpdatedAtEnd,
    int? VisitorType,
    int? PointsMin,
    int? PointsMax,
    string? MemberLevel,
    DateTime? MemberSinceStart,
    DateTime? MemberSinceEnd,
    bool? IsBlacklisted,
    int? HeightMin,
    int? HeightMax,
    int Page = 1,
    int PageSize = 20,
    bool Descending = true,
    string OrderBy = "RegisterTime"
) : IRequest<SearchVisitorsResult>;

/// <summary>
/// Query for getting visitor statistics with filtering.
/// </summary>
public record GetVisitorStatsQuery(
    string? Keyword,
    int? UserId,
    DateTime? BirthDateStart,
    DateTime? BirthDateEnd,
    Gender? Gender,
    DateTime? RegisterTimeStart,
    DateTime? RegisterTimeEnd,
    int? PermissionLevelMin,
    int? PermissionLevelMax,
    int? RoleId,
    DateTime? CreatedAtStart,
    DateTime? CreatedAtEnd,
    DateTime? UpdatedAtStart,
    DateTime? UpdatedAtEnd,
    int? VisitorType,
    int? PointsMin,
    int? PointsMax,
    string? MemberLevel,
    DateTime? MemberSinceStart,
    DateTime? MemberSinceEnd,
    bool? IsBlacklisted,
    int? HeightMin,
    int? HeightMax
) : IRequest<VisitorStatsDto>;

/// <summary>
/// Query for getting grouped visitor statistics with filtering.
/// </summary>
public record GetGroupedVisitorStatsQuery(
    string? Keyword,
    int? UserId,
    DateTime? BirthDateStart,
    DateTime? BirthDateEnd,
    Gender? Gender,
    DateTime? RegisterTimeStart,
    DateTime? RegisterTimeEnd,
    int? PermissionLevelMin,
    int? PermissionLevelMax,
    int? RoleId,
    DateTime? CreatedAtStart,
    DateTime? CreatedAtEnd,
    DateTime? UpdatedAtStart,
    DateTime? UpdatedAtEnd,
    int? VisitorType,
    int? PointsMin,
    int? PointsMax,
    string? MemberLevel,
    DateTime? MemberSinceStart,
    DateTime? MemberSinceEnd,
    bool? IsBlacklisted,
    int? HeightMin,
    int? HeightMax,
    bool Descending = true,
    string GroupBy = "VisitorType",
    string OrderBy = "Count"
) : IRequest<List<GroupedVisitorStatsDto>>;
