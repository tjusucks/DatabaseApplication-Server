using DbApp.Domain.Interfaces;
using DbApp.Domain.Interfaces.TicketingSystem;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace DbApp.Application.TicketingSystem.PriceRules;

public class UpdatePriceRuleCommand : IRequest<PriceRuleDto?>
{
    public int RuleId { get; }
    public UpdatePriceRuleRequest Dto { get; }

    public UpdatePriceRuleCommand(int ruleId, UpdatePriceRuleRequest dto)
    {
        RuleId = ruleId;
        Dto = dto;
    }
}

public class UpdatePriceRuleCommandHandler : IRequestHandler<UpdatePriceRuleCommand, PriceRuleDto?>
{
    private readonly IPriceRuleRepository _priceRuleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePriceRuleCommandHandler(IPriceRuleRepository priceRuleRepository, IUnitOfWork unitOfWork)
    {
        _priceRuleRepository = priceRuleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PriceRuleDto?> Handle(UpdatePriceRuleCommand request, CancellationToken cancellationToken)
    {
        var rule = await _priceRuleRepository.GetByIdAsync(request.RuleId);
        if (rule == null || request.Dto.Price < 0 || request.Dto.EffectiveStartDate >= request.Dto.EffectiveEndDate)
        {
            return null;
        }

        rule.RuleName = request.Dto.RuleName;
        rule.Priority = request.Dto.Priority;
        rule.Price = request.Dto.Price;
        rule.EffectiveStartDate = request.Dto.EffectiveStartDate;
        rule.EffectiveEndDate = request.Dto.EffectiveEndDate;

        _priceRuleRepository.Update(rule);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new PriceRuleDto
        {
            Id = rule.PriceRuleId,
            RuleName = rule.RuleName,
            Priority = rule.Priority,
            Price = rule.Price,
            EffectiveStartDate = rule.EffectiveStartDate,
            EffectiveEndDate = rule.EffectiveEndDate
        };
    }
}