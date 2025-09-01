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

/// <summary>
/// Handler for getting visitors by type.
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
