using DbApp.Domain.Entities.ResourceSystem;
using MediatR;

namespace DbApp.Application.ResourceSystem.RideTrafficStats;

public record GetRideTrafficStatByIdQuery(int RideId, DateTime RecordTime) : IRequest<RideTrafficStat?>;

public record GetRideTrafficStatsByRideQuery(int RideId) : IRequest<List<RideTrafficStat>>;

public record GetRideTrafficStatsByDateRangeQuery(int RideId, DateTime StartDate, DateTime EndDate) : IRequest<List<RideTrafficStat>>;

public record GetCrowdedRidesQuery(DateTime? Date = null) : IRequest<List<RideTrafficStat>>;

public record GetRidePopularityReportQuery(DateTime StartDate, DateTime EndDate) : IRequest<List<RideTrafficStat>>;

public record GetPeakHoursAnalysisQuery(int RideId, DateTime Date) : IRequest<List<RideTrafficStat>>;
