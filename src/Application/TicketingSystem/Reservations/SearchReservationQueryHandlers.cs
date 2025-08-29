using AutoMapper;
using DbApp.Domain.Interfaces.TicketingSystem;
using MediatR;

namespace DbApp.Application.TicketingSystem.Reservations;

/// <summary>
/// Handler for searching reservations by visitor ID with filtering options.
/// </summary>
public class SearchReservationsByVisitorQueryHandler(
    IReservationRepository reservationRepository, IMapper mapper)
    : IRequestHandler<SearchReservationsByVisitorQuery, SearchReservationsResult>
{
    private readonly IReservationRepository _reservationRepository = reservationRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<SearchReservationsResult> Handle(
        SearchReservationsByVisitorQuery request, CancellationToken cancellationToken)
    {
        var reservations = await _reservationRepository.SearchByVisitorAsync(
            request.VisitorId,
            request.StartDate,
            request.EndDate,
            request.PaymentStatus,
            request.Status,
            request.SortBy,
            request.Descending,
            request.Page,
            request.PageSize);

        var totalCount = await _reservationRepository.CountByVisitorAsync(
            request.VisitorId,
            request.StartDate,
            request.EndDate,
            request.PaymentStatus,
            request.Status);

        var summaryDtos = _mapper.Map<List<ReservationSummaryDto>>(reservations);

        return new SearchReservationsResult
        {
            Reservations = summaryDtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}

/// <summary>
/// Handler for searching reservations by multiple criteria (admin use).
/// </summary>
public class SearchReservationsQueryHandler(
    IReservationRepository reservationRepository, IMapper mapper)
    : IRequestHandler<SearchReservationsQuery, SearchReservationsResult>
{
    private readonly IReservationRepository _reservationRepository = reservationRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<SearchReservationsResult> Handle(
        SearchReservationsQuery request, CancellationToken cancellationToken)
    {
        var reservations = await _reservationRepository.SearchAsync(
            request.Keyword,
            request.StartDate,
            request.EndDate,
            request.PaymentStatus,
            request.Status,
            request.MinAmount,
            request.MaxAmount,
            request.PromotionId,
            request.SortBy,
            request.Descending,
            request.Page,
            request.PageSize);

        var totalCount = await _reservationRepository.CountAsync(
            request.Keyword,
            request.StartDate,
            request.EndDate,
            request.PaymentStatus,
            request.Status,
            request.MinAmount,
            request.MaxAmount,
            request.PromotionId);

        var summaryDtos = _mapper.Map<List<ReservationSummaryDto>>(reservations);

        return new SearchReservationsResult
        {
            Reservations = summaryDtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}

/// <summary>
/// Handler for getting visitor reservation statistics.
/// </summary>
public class GetVisitorReservationStatsQueryHandler(
    IReservationRepository reservationRepository, IMapper mapper)
    : IRequestHandler<GetVisitorReservationStatsQuery, ReservationStatsDto>
{
    private readonly IReservationRepository _reservationRepository = reservationRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<ReservationStatsDto> Handle(
        GetVisitorReservationStatsQuery request, CancellationToken cancellationToken)
    {
        var stats = await _reservationRepository.GetStatsByVisitorAsync(
            request.VisitorId,
            request.StartDate,
            request.EndDate);

        return _mapper.Map<ReservationStatsDto>(stats);
    }
}
