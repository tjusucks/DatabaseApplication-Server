using DbApp.Domain.Interfaces.TicketingSystem;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace DbApp.Application.TicketingSystem.TicketTypes;

public class GetTicketTypeByIdQuery : IRequest<TicketTypeSummaryDto?>
{
    public int TicketTypeId { get; }

    public GetTicketTypeByIdQuery(int ticketTypeId)
    {
        TicketTypeId = ticketTypeId;
    }
}

public class GetTicketTypeByIdQueryHandler : IRequestHandler<GetTicketTypeByIdQuery, TicketTypeSummaryDto?>
{
    private readonly ITicketTypeRepository _ticketTypeRepository;

    public GetTicketTypeByIdQueryHandler(ITicketTypeRepository ticketTypeRepository)
    {
        _ticketTypeRepository = ticketTypeRepository;
    }

    public async Task<TicketTypeSummaryDto?> Handle(GetTicketTypeByIdQuery request, CancellationToken cancellationToken)
    {
        var ticketType = await _ticketTypeRepository.GetByIdAsync(request.TicketTypeId);

        if (ticketType == null)
        {
            return null;
        }

        return new TicketTypeSummaryDto
        {
            Id = ticketType.TicketTypeId,
            TypeName = ticketType.TypeName,
            BasePrice = ticketType.BasePrice
        };
    }
}