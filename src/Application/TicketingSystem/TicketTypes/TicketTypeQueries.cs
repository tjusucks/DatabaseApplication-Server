using MediatR;

namespace DbApp.Application.TicketingSystem.TicketTypes;

public record GetAllTicketTypesQuery() : IRequest<List<TicketTypeDto>>;

public record GetTicketTypeByIdQuery(int TicketTypeId) : IRequest<TicketTypeDto?>;
