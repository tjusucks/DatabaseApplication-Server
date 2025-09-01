using DbApp.Domain.Enums.TicketingSystem;
using MediatR;

namespace DbApp.Application.TicketingSystem.TicketTypes;

public record CreateTicketTypeCommand(
    string TypeName,
    string? Description,
    decimal BasePrice,
    string? RulesText,
    int? MaxSaleLimit,
    ApplicableCrowd ApplicableCrowd
) : IRequest<int>;

public record UpdateTicketTypeCommand(
    int TicketTypeId,
    string TypeName,
    string? Description,
    decimal BasePrice,
    string? RulesText,
    int? MaxSaleLimit,
    ApplicableCrowd ApplicableCrowd
) : IRequest<Unit>;

public record DeleteTicketTypeCommand(int TicketTypeId) : IRequest<Unit>;
