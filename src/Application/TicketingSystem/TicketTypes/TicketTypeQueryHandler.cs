using DbApp.Domain.Enums.TicketingSystem;
using DbApp.Domain.Interfaces.TicketingSystem;
using MediatR;

namespace DbApp.Application.TicketingSystem.TicketTypes;

public class TicketTypeQueryHandler(ITicketTypeRepository ticketTypeRepository) :
    IRequestHandler<GetAllTicketTypesQuery, List<TicketTypeDto>>,
    IRequestHandler<GetTicketTypeByIdQuery, TicketTypeDto?>
{
    public async Task<List<TicketTypeDto>> Handle(GetAllTicketTypesQuery request, CancellationToken cancellationToken)
    {
        var types = await ticketTypeRepository.GetAllAsync();
        return [.. types.Select(t => new TicketTypeDto
        {
            TicketTypeId = t.TicketTypeId,
            TypeName = t.TypeName,
            Description = t.Description,
            BasePrice = t.BasePrice,
            RulesText = t.RulesText,
            MaxSaleLimit = t.MaxSaleLimit,
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt,
            ApplicableCrowd = t.ApplicableCrowd
        })];
    }

    public async Task<TicketTypeDto?> Handle(GetTicketTypeByIdQuery request, CancellationToken cancellationToken)
    {
        var t = await ticketTypeRepository.GetByIdAsync(request.TicketTypeId);
        if (t == null) return null;

        return new TicketTypeDto
        {
            TicketTypeId = t.TicketTypeId,
            TypeName = t.TypeName,
            Description = t.Description,
            BasePrice = t.BasePrice,
            RulesText = t.RulesText,
            MaxSaleLimit = t.MaxSaleLimit,
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt,
            ApplicableCrowd = t.ApplicableCrowd
        };
    }
}
