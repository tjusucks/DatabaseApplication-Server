using DbApp.Domain.Interfaces.TicketingSystem;
using MediatR;

namespace DbApp.Application.TicketingSystem.PromotionActions;

public class PromotionActionQueryHandler(IPromotionActionRepository actionRepository) :
    IRequestHandler<GetPromotionActionByIdQuery, PromotionActionDto?>,
    IRequestHandler<GetPromotionActionsByPromotionIdQuery, List<PromotionActionDto>>
{
    public async Task<PromotionActionDto?> Handle(GetPromotionActionByIdQuery request, CancellationToken cancellationToken)
    {
        var action = await actionRepository.GetByIdAsync(request.ActionId);
        if (action == null) return null;

        return new PromotionActionDto
        {
            ActionId = action.ActionId,
            PromotionId = action.PromotionId,
            ActionName = action.ActionName,
            ActionType = action.ActionType,
            TargetTicketTypeId = action.TargetTicketTypeId,
            DiscountPercentage = action.DiscountPercentage,
            DiscountAmount = action.DiscountAmount,
            FixedPrice = action.FixedPrice,
            PointsAwarded = action.PointsAwarded,
            FreeTicketTypeId = action.FreeTicketTypeId,
            FreeTicketQuantity = action.FreeTicketQuantity,
            CreatedAt = action.CreatedAt,
            UpdatedAt = action.UpdatedAt
        };
    }

    public async Task<List<PromotionActionDto>> Handle(GetPromotionActionsByPromotionIdQuery request, CancellationToken cancellationToken)
    {
        var actions = await actionRepository.GetByPromotionIdAsync(request.PromotionId);
        return [.. actions.Select(action => new PromotionActionDto
        {
            ActionId = action.ActionId,
            PromotionId = action.PromotionId,
            ActionName = action.ActionName,
            ActionType = action.ActionType,
            TargetTicketTypeId = action.TargetTicketTypeId,
            DiscountPercentage = action.DiscountPercentage,
            DiscountAmount = action.DiscountAmount,
            FixedPrice = action.FixedPrice,
            PointsAwarded = action.PointsAwarded,
            FreeTicketTypeId = action.FreeTicketTypeId,
            FreeTicketQuantity = action.FreeTicketQuantity,
            CreatedAt = action.CreatedAt,
            UpdatedAt = action.UpdatedAt
        })];
    }
}
