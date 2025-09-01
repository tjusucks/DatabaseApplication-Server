using MediatR;

namespace DbApp.Application.TicketingSystem.PriceRules;

public record CreatePriceRuleCommand(
    int TicketTypeId,
    string RuleName,
    int Priority,
    decimal Price,
    DateTime EffectiveStartDate,
    DateTime EffectiveEndDate,
    int? MinQuantity,
    int? MaxQuantity,
    int? CreatedById
) : IRequest<int>;

public record UpdatePriceRuleCommand(
    int RuleId,
    string RuleName,
    int Priority,
    decimal Price,
    DateTime EffectiveStartDate,
    DateTime EffectiveEndDate,
    int? MinQuantity,
    int? MaxQuantity
) : IRequest<Unit>;

public record DeletePriceRuleCommand(int RuleId) : IRequest<Unit>;
