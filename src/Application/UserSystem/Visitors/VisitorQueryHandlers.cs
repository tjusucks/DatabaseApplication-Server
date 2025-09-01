<<<<<<< HEAD
=======
using DbApp.Domain.Constants;
>>>>>>> 1bba8b9 (feat: implement membership registration and points system)
using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Enums.UserSystem;
using DbApp.Domain.Interfaces.UserSystem;
using MediatR;

namespace DbApp.Application.UserSystem.Visitors;

/// <summary>
/// Handler for getting all visitors.
/// </summary>
<<<<<<< HEAD
public class GetAllVisitorsQueryHandler(IVisitorRepository visitorRepository) : IRequestHandler<GetAllVisitorsQuery, List<Visitor>>
=======
public class GetAllVisitorsQueryHandler(IVisitorRepository visitorRepository) 
    : IRequestHandler<GetAllVisitorsQuery, List<Visitor>>
>>>>>>> 1bba8b9 (feat: implement membership registration and points system)
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
<<<<<<< HEAD
public class GetVisitorByIdQueryHandler(IVisitorRepository visitorRepository) : IRequestHandler<GetVisitorByIdQuery, Visitor?>
=======
public class GetVisitorByIdQueryHandler(IVisitorRepository visitorRepository) 
    : IRequestHandler<GetVisitorByIdQuery, Visitor?>
>>>>>>> 1bba8b9 (feat: implement membership registration and points system)
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
<<<<<<< HEAD
public class GetVisitorByUserIdQueryHandler(IVisitorRepository visitorRepository) : IRequestHandler<GetVisitorByUserIdQuery, Visitor?>
=======
public class GetVisitorByUserIdQueryHandler(IVisitorRepository visitorRepository) 
    : IRequestHandler<GetVisitorByUserIdQuery, Visitor?>
>>>>>>> 1bba8b9 (feat: implement membership registration and points system)
{
    private readonly IVisitorRepository _visitorRepository = visitorRepository;

    public async Task<Visitor?> Handle(GetVisitorByUserIdQuery request, CancellationToken cancellationToken)
    {
        return await _visitorRepository.GetByUserIdAsync(request.UserId);
    }
}

/// <summary>
<<<<<<< HEAD
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
public class GetVisitorsByTypeQueryHandler(IVisitorRepository visitorRepository) : IRequestHandler<GetVisitorsByTypeQuery, List<Visitor>>
=======
/// Handler for getting visitors by type.
/// </summary>
public class GetVisitorsByTypeQueryHandler(IVisitorRepository visitorRepository) 
    : IRequestHandler<GetVisitorsByTypeQuery, List<Visitor>>
>>>>>>> 1bba8b9 (feat: implement membership registration and points system)
{
    private readonly IVisitorRepository _visitorRepository = visitorRepository;

    public async Task<List<Visitor>> Handle(GetVisitorsByTypeQuery request, CancellationToken cancellationToken)
    {
<<<<<<< HEAD
        return await _visitorRepository.GetByVisitorTypeAsync(request.VisitorType);
=======
        return await _visitorRepository.GetByTypeAsync(request.VisitorType);
>>>>>>> 1bba8b9 (feat: implement membership registration and points system)
    }
}

/// <summary>
<<<<<<< HEAD
/// Handler for getting visitors by registration date range.
/// </summary>
public class GetVisitorsByRegistrationDateRangeQueryHandler(IVisitorRepository visitorRepository) : IRequestHandler<GetVisitorsByRegistrationDateRangeQuery, List<Visitor>>
{
    private readonly IVisitorRepository _visitorRepository = visitorRepository;

    public async Task<List<Visitor>> Handle(GetVisitorsByRegistrationDateRangeQuery request, CancellationToken cancellationToken)
    {
        return await _visitorRepository.GetByRegistrationDateRangeAsync(request.StartDate, request.EndDate);
=======
/// Handler for getting visitors by member level.
/// </summary>
public class GetVisitorsByMemberLevelQueryHandler(IVisitorRepository visitorRepository) 
    : IRequestHandler<GetVisitorsByMemberLevelQuery, List<Visitor>>
{
    private readonly IVisitorRepository _visitorRepository = visitorRepository;

    public async Task<List<Visitor>> Handle(GetVisitorsByMemberLevelQuery request, CancellationToken cancellationToken)
    {
        return await _visitorRepository.GetByMemberLevelAsync(request.MemberLevel);
>>>>>>> 1bba8b9 (feat: implement membership registration and points system)
    }
}

/// <summary>
<<<<<<< HEAD
/// Handler for searching visitors with multiple criteria.
/// </summary>
public class SearchVisitorsQueryHandler(IVisitorRepository visitorRepository) : IRequestHandler<SearchVisitorsQuery, List<Visitor>>
{
    private readonly IVisitorRepository _visitorRepository = visitorRepository;

    public async Task<List<Visitor>> Handle(SearchVisitorsQuery request, CancellationToken cancellationToken)
    {
        return await _visitorRepository.SearchAsync(
            request.Name,
            request.PhoneNumber,
            request.IsBlacklisted,
            request.VisitorType,
            request.StartDate,
            request.EndDate);
=======
/// Handler for getting visitors by points range.
/// </summary>
public class GetVisitorsByPointsRangeQueryHandler(IVisitorRepository visitorRepository) 
    : IRequestHandler<GetVisitorsByPointsRangeQuery, List<Visitor>>
{
    private readonly IVisitorRepository _visitorRepository = visitorRepository;

    public async Task<List<Visitor>> Handle(GetVisitorsByPointsRangeQuery request, CancellationToken cancellationToken)
    {
        return await _visitorRepository.GetByPointsRangeAsync(request.MinPoints, request.MaxPoints);
>>>>>>> 1bba8b9 (feat: implement membership registration and points system)
    }
}

/// <summary>
<<<<<<< HEAD
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
=======
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
>>>>>>> 1bba8b9 (feat: implement membership registration and points system)
    }
}
