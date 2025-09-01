using MediatR;

namespace DbApp.Application.TicketingSystem.PriceRules;

public record GetPriceRuleByTicketTypeQuery(int TicketTypeId) : IRequest<List<PriceRuleDto>>;

public record GetPriceRuleByIdQuery(int TicketTypeId, int PriceRuleId) : IRequest<PriceRuleDto?>;
