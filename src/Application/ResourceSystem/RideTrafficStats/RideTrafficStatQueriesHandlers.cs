using AutoMapper;
using DbApp.Domain.Interfaces.ResourceSystem;
using MediatR;

namespace DbApp.Application.ResourceSystem.RideTrafficStats;

/// <summary>  
/// Combined handler for all ride traffic stat queries.  
/// </summary>  
public class RideTrafficStatQueryHandler(
    IRideTrafficStatRepository rideTrafficStatRepository,
    IMapper mapper) :
    IRequestHandler<GetRideTrafficStatByIdQuery, RideTrafficStatSummaryDto?>,
    IRequestHandler<SearchRideTrafficStatsQuery, RideTrafficStatResult>,
    IRequestHandler<GetRideTrafficStatsQuery, RideTrafficStatsDto>
{
    private readonly IRideTrafficStatRepository _rideTrafficStatRepository = rideTrafficStatRepository;
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
            request.SearchTerm,
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
            request.SearchTerm,
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
}
