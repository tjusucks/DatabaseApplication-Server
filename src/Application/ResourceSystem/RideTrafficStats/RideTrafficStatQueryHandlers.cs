using AutoMapper;
using DbApp.Domain.Interfaces.ResourceSystem;
using MediatR;
using static DbApp.Domain.Exceptions;

namespace DbApp.Application.ResourceSystem.RideTrafficStats;

/// <summary>
/// Combined handler for all ride traffic stat queries.
/// </summary>
public class RideTrafficStatQueryHandlers(
    IRideTrafficStatRepository rideTrafficStatRepository,
    IRideTrafficStatService rideTrafficStatService,
    IMapper mapper) :
    IRequestHandler<GetRideTrafficStatByIdQuery, RideTrafficStatSummaryDto?>,
    IRequestHandler<SearchRideTrafficStatsQuery, RideTrafficStatResult>,
    IRequestHandler<GetRideTrafficStatsQuery, RideTrafficStatsDto>,
    IRequestHandler<GetRealTimeRideTrafficStatQuery, RideTrafficStatSummaryDto>,
    IRequestHandler<GetAllRealTimeRideTrafficStatsQuery, List<RideTrafficStatSummaryDto>>
{
    private readonly IRideTrafficStatRepository _rideTrafficStatRepository = rideTrafficStatRepository;
    private readonly IRideTrafficStatService _rideTrafficStatService = rideTrafficStatService;
    private readonly IMapper _mapper = mapper;

    /// <summary>
    /// Handle getting ride traffic stat by composite key.
    /// </summary>
    public async Task<RideTrafficStatSummaryDto?> Handle(
        GetRideTrafficStatByIdQuery request,
        CancellationToken cancellationToken)
    {
        var stat = await _rideTrafficStatRepository.GetByIdAsync(request.RideId, request.RecordTime);
        return stat == null ? null : _mapper.Map<RideTrafficStatSummaryDto>(stat);
    }

    /// <summary>
    /// Handle searching ride traffic stats with comprehensive filtering options.
    /// </summary>
    public async Task<RideTrafficStatResult> Handle(
        SearchRideTrafficStatsQuery request,
        CancellationToken cancellationToken)
    {
        var stats = await _rideTrafficStatRepository.SearchAsync(
            request.Keyword,
            request.RideId,
            request.IsCrowded,
            request.MinVisitorCount,
            request.MaxVisitorCount,
            request.MinQueueLength,
            request.MaxQueueLength,
            request.MinWaitingTime,
            request.MaxWaitingTime,
            request.RecordTimeFrom,
            request.RecordTimeTo,
            request.Page,
            request.PageSize);

        var totalCount = await _rideTrafficStatRepository.CountAsync(
            request.Keyword,
            request.RideId,
            request.IsCrowded,
            request.MinVisitorCount,
            request.MaxVisitorCount,
            request.MinQueueLength,
            request.MaxQueueLength,
            request.MinWaitingTime,
            request.MaxWaitingTime,
            request.RecordTimeFrom,
            request.RecordTimeTo);

        var statDtos = _mapper.Map<List<RideTrafficStatSummaryDto>>(stats);

        return new RideTrafficStatResult
        {
            RideTrafficStats = statDtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    /// <summary>
    /// Handle getting ride traffic statistics.
    /// </summary>
    public async Task<RideTrafficStatsDto> Handle(
        GetRideTrafficStatsQuery request,
        CancellationToken cancellationToken)
    {
        var stats = await _rideTrafficStatRepository.GetStatsAsync(
            request.StartDate,
            request.EndDate);

        return _mapper.Map<RideTrafficStatsDto>(stats);
    }

    /// <summary>
    /// Handle getting real-time ride traffic statistics for a specific ride.
    /// </summary>
    public async Task<RideTrafficStatSummaryDto> Handle(
        GetRealTimeRideTrafficStatQuery request,
        CancellationToken cancellationToken)
    {
        var stat = await _rideTrafficStatService.GetRealTimeStatsAsync(request.RideId)
            ?? throw new NotFoundException($"Ride with ID {request.RideId} not found.");
        return _mapper.Map<RideTrafficStatSummaryDto>(stat);
    }

    /// <summary>
    /// Handle getting real-time ride traffic statistics for all rides.
    /// </summary>
    public async Task<List<RideTrafficStatSummaryDto>> Handle(
        GetAllRealTimeRideTrafficStatsQuery request,
        CancellationToken cancellationToken)
    {
        var stats = await _rideTrafficStatService.GetAllRealTimeStatsAsync();
        return _mapper.Map<List<RideTrafficStatSummaryDto>>(stats);
    }
}
