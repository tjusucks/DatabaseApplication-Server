using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Interfaces.ResourceSystem;
using MediatR;

namespace DbApp.Application.ResourceSystem.RideTrafficStats;

public class GetRideTrafficStatByIdQueryHandler(IRideTrafficStatRepository repository)
    : IRequestHandler<GetRideTrafficStatByIdQuery, RideTrafficStat?>
{
    public async Task<RideTrafficStat?> Handle(GetRideTrafficStatByIdQuery request, CancellationToken cancellationToken)
    {
        return await repository.GetByIdAsync(request.RideId, request.RecordTime);
    }
}

public class GetRideTrafficStatsByRideQueryHandler(IRideTrafficStatRepository repository)
    : IRequestHandler<GetRideTrafficStatsByRideQuery, List<RideTrafficStat>>
{
    public async Task<List<RideTrafficStat>> Handle(GetRideTrafficStatsByRideQuery request, CancellationToken cancellationToken)
    {
        return await repository.GetByRideIdAsync(request.RideId);
    }
}

public class GetRideTrafficStatsByDateRangeQueryHandler(IRideTrafficStatRepository repository)
    : IRequestHandler<GetRideTrafficStatsByDateRangeQuery, List<RideTrafficStat>>
{
    public async Task<List<RideTrafficStat>> Handle(GetRideTrafficStatsByDateRangeQuery request, CancellationToken cancellationToken)
    {
        return await repository.GetByDateRangeAsync(request.RideId, request.StartDate, request.EndDate);
    }
}

public class GetCrowdedRidesQueryHandler(IRideTrafficStatRepository repository)
    : IRequestHandler<GetCrowdedRidesQuery, List<RideTrafficStat>>
{
    public async Task<List<RideTrafficStat>> Handle(GetCrowdedRidesQuery request, CancellationToken cancellationToken)
    {
        return await repository.GetCrowdedRidesAsync(request.Date);
    }
}

public class GetRidePopularityReportQueryHandler(IRideTrafficStatRepository repository)
    : IRequestHandler<GetRidePopularityReportQuery, List<RideTrafficStat>>
{
    public async Task<List<RideTrafficStat>> Handle(GetRidePopularityReportQuery request, CancellationToken cancellationToken)
    {
        return await repository.GetPopularityReportAsync(request.StartDate, request.EndDate);
    }
}

public class GetPeakHoursAnalysisQueryHandler(IRideTrafficStatRepository repository)
    : IRequestHandler<GetPeakHoursAnalysisQuery, List<RideTrafficStat>>
{
    public async Task<List<RideTrafficStat>> Handle(GetPeakHoursAnalysisQuery request, CancellationToken cancellationToken)
    {
        return await repository.GetPeakHoursAnalysisAsync(request.RideId, request.Date);
    }
}
