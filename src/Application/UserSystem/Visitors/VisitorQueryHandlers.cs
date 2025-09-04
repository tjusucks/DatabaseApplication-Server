using DbApp.Application.UserSystem.Visitors.DTOs;
using DbApp.Domain.Constants;
using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Enums.UserSystem;
using DbApp.Domain.Interfaces.UserSystem;
using MediatR;

namespace DbApp.Application.UserSystem.Visitors;

/// <summary>
/// Handler for getting all visitors.
/// </summary>
public class GetAllVisitorsQueryHandler(IVisitorRepository visitorRepository)
    : IRequestHandler<GetAllVisitorsQuery, List<Visitor>>
{
    private readonly IVisitorRepository _visitorRepository = visitorRepository;

    public async Task<List<Visitor>> Handle(GetAllVisitorsQuery request, CancellationToken cancellationToken)
    {
        return await _visitorRepository.GetAllAsync();
    }
}

/// <summary>
/// Handler for getting a visitor by ID.
/// </summary>
public class GetVisitorByIdQueryHandler(IVisitorRepository visitorRepository)
    : IRequestHandler<GetVisitorByIdQuery, Visitor?>
{
    private readonly IVisitorRepository _visitorRepository = visitorRepository;

    public async Task<Visitor?> Handle(GetVisitorByIdQuery request, CancellationToken cancellationToken)
    {
        return await _visitorRepository.GetByIdAsync(request.VisitorId);
    }
}

/// <summary>
/// Handler for getting a visitor by user ID.
/// </summary>
public class GetVisitorByUserIdQueryHandler(IVisitorRepository visitorRepository)
    : IRequestHandler<GetVisitorByUserIdQuery, Visitor?>
{
    private readonly IVisitorRepository _visitorRepository = visitorRepository;

    public async Task<Visitor?> Handle(GetVisitorByUserIdQuery request, CancellationToken cancellationToken)
    {
        return await _visitorRepository.GetByUserIdAsync(request.UserId);
    }
}

// === 队友的搜索和筛选功能 ===

/// <summary>
/// Handler for searching visitors by name.
/// </summary>
public class SearchVisitorsByNameQueryHandler(IVisitorRepository visitorRepository) : IRequestHandler<SearchVisitorsByNameQuery, List<Visitor>>
{
    private readonly IVisitorRepository _visitorRepository = visitorRepository;

    public async Task<List<Visitor>> Handle(SearchVisitorsByNameQuery request, CancellationToken cancellationToken)
    {
        return await _visitorRepository.SearchByNameAsync(request.Name);
    }
}

/// <summary>
/// Handler for searching visitors by phone number.
/// </summary>
public class SearchVisitorsByPhoneQueryHandler(IVisitorRepository visitorRepository) : IRequestHandler<SearchVisitorsByPhoneQuery, List<Visitor>>
{
    private readonly IVisitorRepository _visitorRepository = visitorRepository;

    public async Task<List<Visitor>> Handle(SearchVisitorsByPhoneQuery request, CancellationToken cancellationToken)
    {
        return await _visitorRepository.SearchByPhoneNumberAsync(request.PhoneNumber);
    }
}

/// <summary>
/// Handler for getting visitors by blacklist status.
/// </summary>
public class GetVisitorsByBlacklistStatusQueryHandler(IVisitorRepository visitorRepository) : IRequestHandler<GetVisitorsByBlacklistStatusQuery, List<Visitor>>
{
    private readonly IVisitorRepository _visitorRepository = visitorRepository;

    public async Task<List<Visitor>> Handle(GetVisitorsByBlacklistStatusQuery request, CancellationToken cancellationToken)
    {
        return await _visitorRepository.GetByBlacklistStatusAsync(request.IsBlacklisted);
    }
}

/// <summary>
/// Handler for getting visitors by visitor type.
/// </summary>
public class GetVisitorsByTypeQueryHandler(IVisitorRepository visitorRepository)
    : IRequestHandler<GetVisitorsByTypeQuery, List<Visitor>>
{
    private readonly IVisitorRepository _visitorRepository = visitorRepository;

    public async Task<List<Visitor>> Handle(GetVisitorsByTypeQuery request, CancellationToken cancellationToken)
    {
        return await _visitorRepository.GetByTypeAsync(request.VisitorType);
    }
}

/// <summary>
/// Handler for getting visitors by registration date range.
/// </summary>
public class GetVisitorsByRegistrationDateRangeQueryHandler(IVisitorRepository visitorRepository) : IRequestHandler<GetVisitorsByRegistrationDateRangeQuery, List<Visitor>>
{
    private readonly IVisitorRepository _visitorRepository = visitorRepository;

    public async Task<List<Visitor>> Handle(GetVisitorsByRegistrationDateRangeQuery request, CancellationToken cancellationToken)
    {
        return await _visitorRepository.GetByRegistrationDateRangeAsync(request.StartDate, request.EndDate);
    }
}

// === 您的会员和积分查询功能 ===

/// <summary>
/// Handler for getting visitors by member level.
/// </summary>
public class GetVisitorsByMemberLevelQueryHandler(IVisitorRepository visitorRepository)
    : IRequestHandler<GetVisitorsByMemberLevelQuery, List<Visitor>>
{
    private readonly IVisitorRepository _visitorRepository = visitorRepository;

    public async Task<List<Visitor>> Handle(GetVisitorsByMemberLevelQuery request, CancellationToken cancellationToken)
    {
        return await _visitorRepository.GetByMemberLevelAsync(request.MemberLevel);
    }
}

/// <summary>
/// Handler for unified RESTful visitor search with pagination.
/// </summary>
public class SearchVisitorsQueryHandler(IVisitorRepository visitorRepository) : IRequestHandler<SearchVisitorsQuery, SearchVisitorsResult>
{
    private readonly IVisitorRepository _visitorRepository = visitorRepository;

    public async Task<SearchVisitorsResult> Handle(SearchVisitorsQuery request, CancellationToken cancellationToken)
    {
        // Get total count for pagination
        var totalCount = await _visitorRepository.GetSearchCountAsync(
            request.Keyword,
            request.VisitorType,
            request.MemberLevel,
            request.IsBlacklisted,
            request.MinPoints,
            request.MaxPoints,
            request.StartDate,
            request.EndDate);

        // Get paginated results
        var visitors = await _visitorRepository.SearchWithPaginationAsync(
            request.Keyword,
            request.VisitorType,
            request.MemberLevel,
            request.IsBlacklisted,
            request.MinPoints,
            request.MaxPoints,
            request.StartDate,
            request.EndDate,
            request.Page,
            request.PageSize);

        // Convert to DTOs
        var visitorDtos = visitors.Select(v => new VisitorResponseDto
        {
            VisitorId = v.VisitorId,
            VisitorType = v.VisitorType,
            Points = v.Points,
            MemberLevel = v.MemberLevel,
            IsBlacklisted = v.IsBlacklisted,
            Height = v.Height,
            CreatedAt = v.CreatedAt,
            UpdatedAt = v.UpdatedAt
        }).ToList();

        return new SearchVisitorsResult
        {
            Visitors = visitorDtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize,
            AppliedFilters = new SearchFilters
            {
                Keyword = request.Keyword,
                VisitorType = request.VisitorType,
                MemberLevel = request.MemberLevel,
                IsBlacklisted = request.IsBlacklisted,
                MinPoints = request.MinPoints,
                MaxPoints = request.MaxPoints,
                StartDate = request.StartDate,
                EndDate = request.EndDate
            }
        };
    }
}

/// <summary>
/// Handler for getting visitors by points range.
/// </summary>
public class GetVisitorsByPointsRangeQueryHandler(IVisitorRepository visitorRepository)
    : IRequestHandler<GetVisitorsByPointsRangeQuery, List<Visitor>>
{
    private readonly IVisitorRepository _visitorRepository = visitorRepository;

    public async Task<List<Visitor>> Handle(GetVisitorsByPointsRangeQuery request, CancellationToken cancellationToken)
    {
        return await _visitorRepository.GetByPointsRangeAsync(request.MinPoints, request.MaxPoints);
    }
}

/// <summary>
/// Handler for getting visitor history information including entry records.
/// </summary>
public class GetVisitorHistoryQueryHandler(
    IVisitorRepository visitorRepository,
    IEntryRecordRepository entryRecordRepository) : IRequestHandler<GetVisitorHistoryQuery, VisitorHistoryDto?>
{
    private readonly IVisitorRepository _visitorRepository = visitorRepository;
    private readonly IEntryRecordRepository _entryRecordRepository = entryRecordRepository;

    public async Task<VisitorHistoryDto?> Handle(GetVisitorHistoryQuery request, CancellationToken cancellationToken)
    {
        var visitor = await _visitorRepository.GetByIdAsync(request.VisitorId);
        if (visitor == null)
            return null;

        // Get all entry records for this visitor
        var entryRecords = await _entryRecordRepository.GetByVisitorIdAsync(request.VisitorId);

        // Calculate statistics
        var totalVisits = entryRecords.Count;
        var lastVisitDate = entryRecords.OrderByDescending(er => er.EntryTime).FirstOrDefault()?.EntryTime;

        // Get recent entry records (last 10)
        var recentEntryRecords = entryRecords
            .OrderByDescending(er => er.EntryTime)
            .Take(10)
            .Select(er => new EntryRecordSummaryDto(
                er.EntryRecordId,
                er.EntryTime,
                er.ExitTime,
                er.EntryGate,
                er.ExitGate,
                er.ExitTime.HasValue ? er.ExitTime.Value - er.EntryTime : null
            ))
            .ToList();

        // Calculate age
        int? age = null;
        if (visitor.User.BirthDate.HasValue)
        {
            var today = DateTime.Today;
            age = today.Year - visitor.User.BirthDate.Value.Year;
            if (visitor.User.BirthDate.Value.Date > today.AddYears(-age.Value))
                age--;
        }

        return new VisitorHistoryDto(
            visitor.VisitorId,
            visitor.User.Username,
            visitor.User.DisplayName,
            visitor.User.Email,
            visitor.User.PhoneNumber,
            visitor.User.BirthDate,
            age,
            visitor.User.Gender,
            visitor.User.RegisterTime,
            visitor.VisitorType,
            visitor.Points,
            visitor.MemberLevel,
            visitor.MemberSince,
            visitor.IsBlacklisted,
            visitor.Height,
            totalVisits,
            lastVisitDate,
            recentEntryRecords
        );
    }
}

/// <summary>
/// Handler for getting membership statistics.
/// </summary>
public class GetMembershipStatisticsQueryHandler(IVisitorRepository visitorRepository)
    : IRequestHandler<GetMembershipStatisticsQuery, MembershipStatistics>
{
    private readonly IVisitorRepository _visitorRepository = visitorRepository;

    public async Task<MembershipStatistics> Handle(GetMembershipStatisticsQuery request, CancellationToken cancellationToken)
    {
        var allVisitors = await _visitorRepository.GetAllAsync();
        var members = allVisitors.Where(v => v.VisitorType == VisitorType.Member).ToList();

        var bronzeMembers = members.Count(m => m.MemberLevel == MembershipConstants.LevelNames.Bronze);
        var silverMembers = members.Count(m => m.MemberLevel == MembershipConstants.LevelNames.Silver);
        var goldMembers = members.Count(m => m.MemberLevel == MembershipConstants.LevelNames.Gold);
        var platinumMembers = members.Count(m => m.MemberLevel == MembershipConstants.LevelNames.Platinum);

        var totalPoints = members.Sum(m => m.Points);
        var averagePoints = members.Count > 0 ? (double)totalPoints / members.Count : 0;

        return new MembershipStatistics
        {
            TotalVisitors = allVisitors.Count,
            TotalMembers = members.Count,
            BronzeMembers = bronzeMembers,
            SilverMembers = silverMembers,
            GoldMembers = goldMembers,
            PlatinumMembers = platinumMembers,
            MembershipRate = allVisitors.Count > 0 ? (decimal)members.Count / allVisitors.Count * 100 : 0,
            TotalPointsIssued = totalPoints,
            AveragePointsPerMember = averagePoints
        };
    }
}
