using DbApp.Domain.Entities.TicketingSystem;
using DbApp.Domain.Interfaces.TicketingSystem;
using MediatR;

namespace DbApp.Application.TicketingSystem.PromotionActions;

public class PromotionActionCommandHandler(
    IPromotionRepository promotionRepository, IPromotionActionRepository actionRepository) :
    IRequestHandler<CreatePromotionActionCommand, int>,
    IRequestHandler<UpdatePromotionActionCommand, Unit>,
    IRequestHandler<DeletePromotionActionCommand, Unit>
{
    private readonly IPromotionRepository _promotionRepository = promotionRepository;
    private readonly IPromotionActionRepository _actionRepository = actionRepository;

    // Handler for Creating an Action
    public async Task<int> Handle(CreatePromotionActionCommand request, CancellationToken cancellationToken)
    {
        var promotion = await _promotionRepository.GetByIdAsync(request.PromotionId);
        if (promotion == null) return 0;

        var action = new PromotionAction
        {
            PromotionId = request.PromotionId,
            ActionName = request.ActionName,
            ActionType = request.ActionType,
            TargetTicketTypeId = request.TargetTicketTypeId,
            DiscountPercentage = request.DiscountPercentage,
            DiscountAmount = request.DiscountAmount,
            FixedPrice = request.FixedPrice,
            PointsAwarded = request.PointsAwarded,
            FreeTicketTypeId = request.FreeTicketTypeId,
            FreeTicketQuantity = request.FreeTicketQuantity,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return await _actionRepository.CreateAsync(action);
    }

    // Handler for Updating an Action
    public async Task<Unit> Handle(UpdatePromotionActionCommand request, CancellationToken cancellationToken)
    {
        var action = await _actionRepository.GetByIdAsync(request.ActionId);
        if (action == null) return Unit.Value;

        action.PromotionId = request.PromotionId;
        action.ActionName = request.ActionName;
        action.ActionType = request.ActionType;
        action.TargetTicketTypeId = request.TargetTicketTypeId;
        action.DiscountPercentage = request.DiscountPercentage;
        action.DiscountAmount = request.DiscountAmount;
        action.FixedPrice = request.FixedPrice;
        action.PointsAwarded = request.PointsAwarded;
        action.FreeTicketTypeId = request.FreeTicketTypeId;
        action.FreeTicketQuantity = request.FreeTicketQuantity;
        action.UpdatedAt = DateTime.UtcNow;

        await _actionRepository.UpdateAsync(action);

        return Unit.Value;
    }

    // Handler for Deleting an Action
    public async Task<Unit> Handle(DeletePromotionActionCommand request, CancellationToken cancellationToken)
    {
        var action = await _actionRepository.GetByIdAsync(request.ActionId);
        if (action == null) return Unit.Value;

        await _actionRepository.DeleteAsync(action);

        return Unit.Value;
    }
}
