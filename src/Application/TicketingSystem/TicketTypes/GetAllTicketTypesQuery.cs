using DbApp.Domain.Interfaces.TicketingSystem; // We'll create this interface next
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DbApp.Application.TicketingSystem.TicketTypes;

// The Query object - just carries data, no logic.
public class GetAllTicketTypesQuery : IRequest<List<TicketTypeSummaryDto>>
{
}

// The Handler - contains the logic.
public class GetAllTicketTypesQueryHandler : IRequestHandler<GetAllTicketTypesQuery, List<TicketTypeSummaryDto>>
{
    private readonly ITicketTypeRepository _ticketTypeRepository;

    public GetAllTicketTypesQueryHandler(ITicketTypeRepository ticketTypeRepository)
    {
        _ticketTypeRepository = ticketTypeRepository;
    }

    public async Task<List<TicketTypeSummaryDto>> Handle(GetAllTicketTypesQuery request, CancellationToken cancellationToken)
    {
        var ticketTypes = await _ticketTypeRepository.GetAllAsync();

        // Mapping from Domain Entity to DTO happens here, in the Application Layer.
        return ticketTypes.Select(t => new TicketTypeSummaryDto
        {
            Id = t.TicketTypeId,
            TypeName = t.TypeName,
            BasePrice = t.BasePrice
        }).ToList();
    }
}