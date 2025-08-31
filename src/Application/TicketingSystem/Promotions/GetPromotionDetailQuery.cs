using DbApp.Domain.Interfaces.TicketingSystem;
using MediatR;
using DbApp.Application.TicketingSystem.TicketTypes;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DbApp.Application.TicketingSystem.Promotions;

public class GetPromotionDetailQuery : IRequest<PromotionDetailDto?>
{
    public int PromotionId { get; }

    public GetPromotionDetailQuery(int promotionId)
    {
        PromotionId = promotionId;
    }
}

public class GetPromotionDetailQueryHandler : IRequestHandler<GetPromotionDetailQuery, PromotionDetailDto?>
{
    private readonly IPromotionRepository _promotionRepository;

    public GetPromotionDetailQueryHandler(IPromotionRepository promotionRepository)
    {
        _promotionRepository = promotionRepository;
    }

    public async Task<PromotionDetailDto?> Handle(GetPromotionDetailQuery request, CancellationToken cancellationToken)
    {
        var promotion = await _promotionRepository.GetByIdWithDetailsAsync(request.PromotionId);
        if (promotion == null)
        {
            return null;
        }

        return new PromotionDetailDto
        {
            Id = promotion.PromotionId,
            Name = promotion.PromotionName,
            Type = promotion.PromotionType.ToString(),
            StartDate = promotion.StartDatetime,
            EndDate = promotion.EndDatetime,
            IsActive = promotion.IsActive,
            ApplicableTickets = promotion.PromotionTicketTypes
                .Select(pt => new TicketTypeSummaryDto
                {
                    Id = pt.TicketType.TicketTypeId,
                    TypeName = pt.TicketType.TypeName,
                    BasePrice = pt.TicketType.BasePrice
                }).ToList(),
            Conditions = promotion.PromotionConditions
                .Select(c => new PromotionConditionDto
                {
                    ConditionId = c.ConditionId,
                    ConditionName = c.ConditionName,
                    ConditionType = c.ConditionType,
                    TicketTypeId = c.TicketTypeId,
                    MinQuantity = c.MinQuantity,
                    MinAmount = c.MinAmount,
                    Priority = c.Priority
                }).ToList(),
            Actions = promotion.PromotionActions
                .Select(a => new PromotionActionDto
                {
                    ActionId = a.ActionId,
                    ActionName = a.ActionName,
                    ActionType = a.ActionType,
                    DiscountPercentage = a.DiscountPercentage,
                    DiscountAmount = a.DiscountAmount
                }).ToList()
        };
    }
}