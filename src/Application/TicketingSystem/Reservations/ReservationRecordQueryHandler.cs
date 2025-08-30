using AutoMapper;
using DbApp.Domain.Interfaces.TicketingSystem;
using MediatR;

namespace DbApp.Application.TicketingSystem.Reservations;

/// <summary>
/// Combined handler for all reservation search and statistics queries.
/// </summary>
public class ReservationRecordQueryHandler(
    IReservationRepository reservationRepository,
    IMapper mapper) :
    IRequestHandler<SearchReservationRecordByVisitorQuery, ReservationRecordResult>,
    IRequestHandler<SearchReservationRecordQuery, ReservationRecordResult>,
    IRequestHandler<GetVisitorReservationRecordStatsQuery, ReservationRecordStatsDto>
{
    private readonly IReservationRepository _reservationRepository = reservationRepository;
    private readonly IMapper _mapper = mapper;

    /// <summary>
    /// Handle searching reservations by visitor ID with filtering options.
    /// </summary>
    public async Task<ReservationRecordResult> Handle(
        SearchReservationRecordByVisitorQuery request,
        CancellationToken cancellationToken)
    {
        var reservations = await _reservationRepository.SearchAsync(
            request.VisitorId,
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
            request.VisitorId,
            request.Keyword,
            request.StartDate,
            request.EndDate,
            request.PaymentStatus,
            request.Status,
            request.MinAmount,
            request.MaxAmount,
            request.PromotionId);

        var summaryDtos = _mapper.Map<List<ReservationRecordSummaryDto>>(reservations);

        return new ReservationRecordResult
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
    public async Task<ReservationRecordResult> Handle(
        SearchReservationRecordQuery request,
        CancellationToken cancellationToken)
    {
        var reservations = await _reservationRepository.SearchAsync(
            request.VisitorId,
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
            request.VisitorId,
            request.Keyword,
            request.StartDate,
            request.EndDate,
            request.PaymentStatus,
            request.Status,
            request.MinAmount,
            request.MaxAmount,
            request.PromotionId);

        var summaryDtos = _mapper.Map<List<ReservationRecordSummaryDto>>(reservations);

        return new ReservationRecordResult
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
    public async Task<ReservationRecordStatsDto> Handle(
        GetVisitorReservationRecordStatsQuery request,
        CancellationToken cancellationToken)
    {
        var stats = await _reservationRepository.GetStatsByVisitorAsync(
            request.VisitorId,
            request.StartDate,
            request.EndDate);

        return _mapper.Map<ReservationRecordStatsDto>(stats);
    }
}
