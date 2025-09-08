using DbApp.Domain.Entities.TicketingSystem;
using DbApp.Domain.Interfaces.TicketingSystem;
using MediatR;
using static DbApp.Domain.Exceptions;

namespace DbApp.Application.TicketingSystem.TicketTypes;

public class TicketTypeCommandHandler(
    ITicketTypeRepository ticketTypeRepository
) :
    IRequestHandler<CreateTicketTypeCommand, int>,
    IRequestHandler<UpdateTicketTypeCommand, Unit>,
    IRequestHandler<DeleteTicketTypeCommand, Unit>
{
    public async Task<int> Handle(CreateTicketTypeCommand request, CancellationToken cancellationToken)
    {
        var ticketType = new TicketType
        {
            TypeName = request.TypeName,
            Description = request.Description,
            BasePrice = request.BasePrice,
            RulesText = request.RulesText,
            MaxSaleLimit = request.MaxSaleLimit,
            ApplicableCrowd = request.ApplicableCrowd,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return await ticketTypeRepository.CreateAsync(ticketType);
    }

    public async Task<Unit> Handle(UpdateTicketTypeCommand request, CancellationToken cancellationToken)
    {
        var ticketType = await ticketTypeRepository.GetByIdAsync(request.TicketTypeId)
            ?? throw new NotFoundException($"TicketType with ID {request.TicketTypeId} does not exist.");

        ticketType.TypeName = request.TypeName;
        ticketType.Description = request.Description;
        ticketType.BasePrice = request.BasePrice;
        ticketType.RulesText = request.RulesText;
        ticketType.MaxSaleLimit = request.MaxSaleLimit;
        ticketType.ApplicableCrowd = request.ApplicableCrowd;
        ticketType.UpdatedAt = DateTime.UtcNow;

        await ticketTypeRepository.UpdateAsync(ticketType);

        return Unit.Value;
    }

    public async Task<Unit> Handle(DeleteTicketTypeCommand request, CancellationToken cancellationToken)
    {
        var ticketType = await ticketTypeRepository.GetByIdAsync(request.TicketTypeId)
            ?? throw new NotFoundException($"TicketType with ID {request.TicketTypeId} does not exist.");

        await ticketTypeRepository.DeleteAsync(ticketType);

        return Unit.Value;
    }
}
