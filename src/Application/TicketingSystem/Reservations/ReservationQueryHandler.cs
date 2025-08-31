using AutoMapper;
using DbApp.Domain.Interfaces.TicketingSystem;
using DbApp.Domain.Specifications.TicketingSystem;
using MediatR;

namespace DbApp.Application.TicketingSystem.Reservations;

/// <summary>
/// Combined handler for all reservation search and statistics queries.
/// </summary>
public class ReservationQueryHandler(
    IReservationRepository reservationRepository,
    IMapper mapper) :
    IRequestHandler<SearchReservationByVisitorQuery, ReservationSearchResult>,
    IRequestHandler<SearchReservationQuery, ReservationSearchResult>,
    IRequestHandler<GetVisitorReservationStatsQuery, ReservationStatsDto>
{
    private readonly IReservationRepository _reservationRepository = reservationRepository;
    private readonly IMapper _mapper = mapper;

    /// <summary>
    /// Handle searching reservations by visitor ID with filtering options.
    /// </summary>
    public async Task<ReservationSearchResult> Handle(
        SearchReservationByVisitorQuery request,
        CancellationToken cancellationToken)
    {
        var searchSpec = _mapper.Map<ReservationSearchByVisitorSpec>(request);
        var countSpec = _mapper.Map<ReservationCountByVisitorSpec>(request);

        var reservations = await _reservationRepository.SearchByVisitorAsync(searchSpec);
        var totalCount = await _reservationRepository.CountByVisitorAsync(countSpec);

        var summaryDtos = _mapper.Map<List<ReservationSummaryDto>>(reservations);

        return new ReservationSearchResult
        {
            Reservations = summaryDtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    /// <summary>
    /// Handle searching reservations by multiple criteria (admin use).
    /// </summary>
    public async Task<ReservationSearchResult> Handle(
        SearchReservationQuery request,
        CancellationToken cancellationToken)
    {
        var searchSpec = _mapper.Map<ReservationSearchSpec>(request);
        var countSpec = _mapper.Map<ReservationCountSpec>(request);

        var reservations = await _reservationRepository.SearchAsync(searchSpec);
        var totalCount = await _reservationRepository.CountAsync(countSpec);

        var summaryDtos = _mapper.Map<List<ReservationSummaryDto>>(reservations);

        return new ReservationSearchResult
        {
            Reservations = summaryDtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    /// <summary>
    /// Handle getting visitor reservation statistics.
    /// </summary>
    public async Task<ReservationStatsDto> Handle(
        GetVisitorReservationStatsQuery request,
        CancellationToken cancellationToken)
    {
        var statsSpec = _mapper.Map<ReservationStatsByVisitorSpec>(request);
        var stats = await _reservationRepository.GetStatsByVisitorAsync(statsSpec);

        return _mapper.Map<ReservationStatsDto>(stats);
    }
}
