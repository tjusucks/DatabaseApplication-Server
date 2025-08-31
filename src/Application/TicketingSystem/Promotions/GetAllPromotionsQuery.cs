
using DbApp.Domain.Interfaces.TicketingSystem;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DbApp.Application.TicketingSystem.Promotions;

public class GetAllPromotionsQuery : IRequest<List<PromotionSummaryDto>>
{
}

public class GetAllPromotionsQueryHandler : IRequestHandler<GetAllPromotionsQuery, List<PromotionSummaryDto>>
{
    private readonly IPromotionRepository _promotionRepository;

    public GetAllPromotionsQueryHandler(IPromotionRepository promotionRepository)
    {
        _promotionRepository = promotionRepository;
    }

    public async Task<List<PromotionSummaryDto>> Handle(GetAllPromotionsQuery request, CancellationToken cancellationToken)
    {
        var promotions = await _promotionRepository.GetAllAsync();

        return promotions.Select(p => new PromotionSummaryDto
        {
            Id = p.PromotionId,
            Name = p.PromotionName,
            Type = p.PromotionType.ToString(),
            StartDate = p.StartDatetime,
            EndDate = p.EndDatetime,
            IsActive = p.IsActive
        }).ToList();
    }
}