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
    IRequestHandler<SearchRideTrafficStatsByRideQuery, RideTrafficStatResult>,  
    IRequestHandler<SearchRideTrafficStatsByCrowdedQuery, RideTrafficStatResult>,  
    IRequestHandler<GetRideTrafficStatsQuery, RideTrafficStatsDto>  
{  
    private readonly IRideTrafficStatRepository _rideTrafficStatRepository = rideTrafficStatRepository;  
    private readonly IMapper _mapper = mapper;  
  
    public async Task<RideTrafficStatSummaryDto?> Handle(  
        GetRideTrafficStatByIdQuery request,  
        CancellationToken cancellationToken)  
    {  
        var stat = await _rideTrafficStatRepository.GetByIdAsync(request.RideId, request.RecordTime);  
        return stat == null ? null : _mapper.Map<RideTrafficStatSummaryDto>(stat);  
    }  
  
    public async Task<RideTrafficStatResult> Handle(  
        SearchRideTrafficStatsQuery request,  
        CancellationToken cancellationToken)  
    {  
        var stats = await _rideTrafficStatRepository.SearchAsync(  
            request.SearchTerm,  
            request.Page,  
            request.PageSize);  
  
        var totalCount = await _rideTrafficStatRepository.CountAsync(request.SearchTerm);  
        var statDtos = _mapper.Map<List<RideTrafficStatSummaryDto>>(stats);  
  
        return new RideTrafficStatResult  
        {  
            RideTrafficStats = statDtos,  
            TotalCount = totalCount,  
            Page = request.Page,  
            PageSize = request.PageSize  
        };  
    }  
  
    public async Task<RideTrafficStatResult> Handle(  
        SearchRideTrafficStatsByRideQuery request,  
        CancellationToken cancellationToken)  
    {  
        var stats = await _rideTrafficStatRepository.SearchByRideAsync(  
            request.RideId,  
            request.Page,  
            request.PageSize);  
  
        var totalCount = await _rideTrafficStatRepository.CountByRideAsync(request.RideId);  
        var statDtos = _mapper.Map<List<RideTrafficStatSummaryDto>>(stats);  
  
        return new RideTrafficStatResult  
        {  
            RideTrafficStats = statDtos,  
            TotalCount = totalCount,  
            Page = request.Page,  
            PageSize = request.PageSize  
        };  
    }  
  
    public async Task<RideTrafficStatResult> Handle(  
        SearchRideTrafficStatsByCrowdedQuery request,  
        CancellationToken cancellationToken)  
    {  
        var stats = await _rideTrafficStatRepository.SearchByCrowdedAsync(  
            request.IsCrowded,  
            request.Page,  
            request.PageSize);  
  
        var totalCount = await _rideTrafficStatRepository.CountByCrowdedAsync(request.IsCrowded);  
        var statDtos = _mapper.Map<List<RideTrafficStatSummaryDto>>(stats);  
  
        return new RideTrafficStatResult  
        {  
            RideTrafficStats = statDtos,  
            TotalCount = totalCount,  
            Page = request.Page,  
            PageSize = request.PageSize  
        };  
    }  
  
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