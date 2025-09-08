using DbApp.Domain.Interfaces.TicketingSystem;
using MediatR;
using static DbApp.Domain.Exceptions;

namespace DbApp.Application.TicketingSystem.PriceRules;

public class PriceRuleQueryHandlers(IPriceRuleRepository priceRuleRepository) :
    IRequestHandler<GetPriceRuleByTicketTypeQuery, List<PriceRuleDto>>,
    IRequestHandler<GetPriceRuleByIdQuery, PriceRuleDto?>
{
    public async Task<List<PriceRuleDto>> Handle(GetPriceRuleByTicketTypeQuery request, CancellationToken cancellationToken)
    {
        var rules = await priceRuleRepository.GetByTicketTypeIdAsync(request.TicketTypeId);

        return [.. rules.Select(r => new PriceRuleDto
        {
            PriceRuleId = r.PriceRuleId,
            TicketTypeId = r.TicketTypeId,
            RuleName = r.RuleName,
            Priority = r.Priority,
            Price = r.Price,
            EffectiveStartDate = r.EffectiveStartDate,
            EffectiveEndDate = r.EffectiveEndDate,
            MinQuantity = r.MinQuantity,
            MaxQuantity = r.MaxQuantity,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt,
            CreatedById = r.CreatedById
        })];
    }

    public async Task<PriceRuleDto?> Handle(GetPriceRuleByIdQuery request, CancellationToken cancellationToken)
    {
        var rule = await priceRuleRepository.GetByIdAsync(request.PriceRuleId);
        if (rule == null || rule.TicketTypeId != request.TicketTypeId)
        {
            throw new NotFoundException($"无法找到 ID 为 {request.PriceRuleId} 的价格规则。");
        }

        return new PriceRuleDto
        {
            PriceRuleId = rule.PriceRuleId,
            TicketTypeId = rule.TicketTypeId,
            RuleName = rule.RuleName,
            Priority = rule.Priority,
            Price = rule.Price,
            EffectiveStartDate = rule.EffectiveStartDate,
            EffectiveEndDate = rule.EffectiveEndDate,
            MinQuantity = rule.MinQuantity,
            MaxQuantity = rule.MaxQuantity,
            CreatedAt = rule.CreatedAt,
            UpdatedAt = rule.UpdatedAt,
            CreatedById = rule.CreatedById
        };
    }
}
