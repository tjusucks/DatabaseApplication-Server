using AutoMapper;
using DbApp.Domain.Interfaces.TicketingSystem;
using MediatR;

namespace DbApp.Application.TicketingSystem.Tickets;

public class TicketSaleQueryHandler(
    ITicketRepository ticketRepository,
    IMapper mapper) :
    IRequestHandler<SearchTicketSaleQuery, TicketSaleResult>,
    IRequestHandler<GetTicketSaleStatsQuery, TicketSaleStatsDto>,
    IRequestHandler<GetGroupedTicketSaleStatsQuery, List<GroupedTicketSaleStatsDto>>
{
    private readonly ITicketRepository _ticketRepository = ticketRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<TicketSaleResult> Handle(SearchTicketSaleQuery request, CancellationToken cancellationToken)
    {
        var tickets = await _ticketRepository.SearchAsync(
            request.Keyword,
            request.StartDate,
            request.EndDate,
            request.TicketTypeId,
            request.PromotionId,
            request.PaymentStatus,
            request.SortBy,
            request.Descending,
            request.Page,
            request.PageSize);

        var totalCount = await _ticketRepository.CountAsync(
            request.Keyword,
            request.StartDate,
            request.EndDate,
            request.TicketTypeId,
            request.PromotionId,
            request.PaymentStatus);

        var summaryDtos = _mapper.Map<List<TicketSaleSummaryDto>>(tickets);

        return new TicketSaleResult
        {
            TicketSales = summaryDtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    public async Task<TicketSaleStatsDto> Handle(GetTicketSaleStatsQuery request, CancellationToken cancellationToken)
    {
        var stats = await _ticketRepository.GetStatsAsync(
            request.Keyword,
            request.StartDate,
            request.EndDate,
            request.TicketTypeId,
            request.PromotionId,
            request.PaymentStatus);

        return _mapper.Map<TicketSaleStatsDto>(stats);
    }

    public async Task<List<GroupedTicketSaleStatsDto>> Handle(GetGroupedTicketSaleStatsQuery request, CancellationToken cancellationToken)
    {
        var groupedStats = await _ticketRepository.GetGroupedStatsAsync(
            request.Keyword,
            request.StartDate,
            request.EndDate,
            request.TicketTypeId,
            request.PromotionId,
            request.PaymentStatus,
            request.GroupBy,
            request.SortBy,
            request.Descending);

        return _mapper.Map<List<GroupedTicketSaleStatsDto>>(groupedStats);
    }
}
