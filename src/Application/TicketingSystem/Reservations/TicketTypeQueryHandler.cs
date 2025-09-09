using DbApp.Application.TicketingSystem.TicketTypes;
using DbApp.Domain.Interfaces.TicketingSystem;
using MediatR;

namespace DbApp.Application.TicketingSystem.Reservations;

public class TicketTypeQueryHandler(ITicketTypeRepository ticketTypeRepository) :
    IRequestHandler<GetAvailableTicketTypesQuery, List<TicketTypeDto>>,
    IRequestHandler<GetTicketTypeByIdQuery, TicketTypeDto?>
{
    private readonly ITicketTypeRepository _ticketTypeRepository = ticketTypeRepository;

    /// <summary>
    /// 获取所有可用票种
    /// </summary>
    public async Task<List<TicketTypeDto>> Handle(GetAvailableTicketTypesQuery request, CancellationToken cancellationToken)
    {
        var ticketTypes = await _ticketTypeRepository.GetActiveTicketTypesAsync();

        var ticketTypeDtos = ticketTypes.Select(tt => new TicketTypeDto
        {
            TicketTypeId = tt.TicketTypeId,
            TypeName = tt.TypeName,
            Description = tt.Description,
            BasePrice = tt.BasePrice,
            RulesText = tt.RulesText,
            MaxSaleLimit = tt.MaxSaleLimit,
            ApplicableCrowd = tt.ApplicableCrowd,
            IsAvailable = true,
            RemainingQuantity = tt.MaxSaleLimit ?? int.MaxValue
        }).ToList();

        return ticketTypeDtos;
    }

    /// <summary>
    /// 根据ID获取特定票种
    /// </summary>
    public async Task<TicketTypeDto?> Handle(GetTicketTypeByIdQuery request, CancellationToken cancellationToken)
    {
        var ticketType = await _ticketTypeRepository.GetByIdAsync(request.TicketTypeId);

        if (ticketType == null)
        {
            return null;
        }

        return new TicketTypeDto
        {
            TicketTypeId = ticketType.TicketTypeId,
            TypeName = ticketType.TypeName,
            Description = ticketType.Description,
            BasePrice = ticketType.BasePrice,
            RulesText = ticketType.RulesText,
            MaxSaleLimit = ticketType.MaxSaleLimit,
            ApplicableCrowd = ticketType.ApplicableCrowd,
            IsAvailable = true,
            RemainingQuantity = ticketType.MaxSaleLimit ?? int.MaxValue
        };
    }
}
