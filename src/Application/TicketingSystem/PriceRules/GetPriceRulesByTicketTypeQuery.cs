
using DbApp.Domain.Interfaces.TicketingSystem;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DbApp.Application.TicketingSystem.PriceRules;

public class GetPriceRulesByTicketTypeQuery : IRequest<List<PriceRuleDto>>
{
    public int TicketTypeId { get; }

    public GetPriceRulesByTicketTypeQuery(int ticketTypeId)
    {
        TicketTypeId = ticketTypeId;
    }
}

public class GetPriceRulesByTicketTypeQueryHandler : IRequestHandler<GetPriceRulesByTicketTypeQuery, List<PriceRuleDto>>
{
    private readonly IPriceRuleRepository _priceRuleRepository;

    public GetPriceRulesByTicketTypeQueryHandler(IPriceRuleRepository priceRuleRepository)
    {
        _priceRuleRepository = priceRuleRepository;
    }

    public async Task<List<PriceRuleDto>> Handle(GetPriceRulesByTicketTypeQuery request, CancellationToken cancellationToken)
    {
        var rules = await _priceRuleRepository.GetByTicketTypeIdAsync(request.TicketTypeId);

        return rules.Select(r => new PriceRuleDto
        {
            Id = r.PriceRuleId,
            RuleName = r.RuleName,
            Priority = r.Priority,
            Price = r.Price,
            EffectiveStartDate = r.EffectiveStartDate,
            EffectiveEndDate = r.EffectiveEndDate
        }).ToList();
    }
}