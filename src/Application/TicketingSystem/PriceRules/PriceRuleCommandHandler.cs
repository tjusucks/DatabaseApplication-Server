using DbApp.Domain.Entities.TicketingSystem;
using DbApp.Domain.Interfaces.TicketingSystem;
using MediatR;

namespace DbApp.Application.TicketingSystem.PriceRules;

public class PriceRuleCommandHandler(
    IPriceRuleRepository priceRuleRepository,
    ITicketTypeRepository ticketTypeRepository
) :
    IRequestHandler<CreatePriceRuleCommand, int>,
    IRequestHandler<UpdatePriceRuleCommand, Unit>,
    IRequestHandler<DeletePriceRuleCommand, Unit>
{
    public async Task<int> Handle(CreatePriceRuleCommand request, CancellationToken cancellationToken)
    {
        var ticketType = await ticketTypeRepository.GetByIdAsync(request.TicketTypeId);
        if (ticketType == null || request.Price < 0 || request.EffectiveStartDate >= request.EffectiveEndDate)
            return 0;

        var priceRule = new PriceRule
        {
            TicketTypeId = request.TicketTypeId,
            RuleName = request.RuleName,
            Priority = request.Priority,
            Price = request.Price,
            EffectiveStartDate = request.EffectiveStartDate,
            EffectiveEndDate = request.EffectiveEndDate,
            MinQuantity = request.MinQuantity,
            MaxQuantity = request.MaxQuantity,
            CreatedById = request.CreatedById,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return await priceRuleRepository.CreateAsync(priceRule);
    }

    public async Task<Unit> Handle(UpdatePriceRuleCommand request, CancellationToken cancellationToken)
    {
        var rule = await priceRuleRepository.GetByIdAsync(request.RuleId);
        if (rule == null || request.Price < 0 || request.EffectiveStartDate >= request.EffectiveEndDate)
            return Unit.Value;

        rule.RuleName = request.RuleName;
        rule.Priority = request.Priority;
        rule.Price = request.Price;
        rule.EffectiveStartDate = request.EffectiveStartDate;
        rule.EffectiveEndDate = request.EffectiveEndDate;
        rule.MinQuantity = request.MinQuantity;
        rule.MaxQuantity = request.MaxQuantity;
        rule.UpdatedAt = DateTime.UtcNow;

        await priceRuleRepository.UpdateAsync(rule);
        return Unit.Value;
    }

    public async Task<Unit> Handle(DeletePriceRuleCommand request, CancellationToken cancellationToken)
    {
        var rule = await priceRuleRepository.GetByIdAsync(request.RuleId);
        if (rule == null)
            return Unit.Value;

        await priceRuleRepository.DeleteAsync(rule);
        return Unit.Value;
    }
}
