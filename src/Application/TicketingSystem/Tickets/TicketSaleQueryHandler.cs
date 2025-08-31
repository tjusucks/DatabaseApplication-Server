using AutoMapper;
using DbApp.Domain.Interfaces.TicketingSystem;
using DbApp.Domain.Specifications.TicketingSystem;
using MediatR;

namespace DbApp.Application.TicketingSystem.Tickets;

/// <summary>
/// Handler for all ticket sale queries.
/// </summary>
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
        var searchSpec = _mapper.Map<TicketSaleSearchSpec>(request);
        var countSpec = _mapper.Map<TicketSaleCountSpec>(request);

        var tickets = await _ticketRepository.SearchAsync(searchSpec);
        var totalCount = await _ticketRepository.CountAsync(countSpec);

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
        var statsSpec = _mapper.Map<TicketSaleStatsSpec>(request);
        var stats = await _ticketRepository.GetStatsAsync(statsSpec);

        return _mapper.Map<TicketSaleStatsDto>(stats);
    }

    public async Task<List<GroupedTicketSaleStatsDto>> Handle(GetGroupedTicketSaleStatsQuery request, CancellationToken cancellationToken)
    {
        var groupedStatsSpec = _mapper.Map<TicketSaleGroupedStatsSpec>(request);
        var groupedStats = await _ticketRepository.GetGroupedStatsAsync(groupedStatsSpec);

        return _mapper.Map<List<GroupedTicketSaleStatsDto>>(groupedStats);
    }
}
