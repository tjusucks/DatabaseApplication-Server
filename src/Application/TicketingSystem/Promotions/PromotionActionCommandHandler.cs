using DbApp.Domain.Entities.TicketingSystem;
using DbApp.Domain.Interfaces.TicketingSystem; // Repository interfaces
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace DbApp.Application.TicketingSystem.Promotions;

// Refactored: This single class now handles all PromotionAction commands
public class PromotionActionCommandHandler :
    IRequestHandler<AddActionToPromotionCommand, PromotionActionDto?>,
    IRequestHandler<UpdatePromotionActionCommand, PromotionActionDto?>,
    IRequestHandler<DeletePromotionActionCommand, bool>
{
    private readonly IPromotionRepository _promotionRepository;
    private readonly IPromotionActionRepository _actionRepository;
    public PromotionActionCommandHandler(IPromotionRepository promotionRepository, IPromotionActionRepository actionRepository)
    {
        _promotionRepository = promotionRepository;
        _actionRepository = actionRepository;
    }

    // Handler for Adding an Action
    public async Task<PromotionActionDto?> Handle(AddActionToPromotionCommand request, CancellationToken cancellationToken)
    {
        var promotion = await _promotionRepository.GetByIdAsync(request.PromotionId);
        if (promotion == null) return null;

        var action = new PromotionAction
        {
            PromotionId = request.PromotionId,
            ActionName = request.Dto.ActionName,
            ActionType = request.Dto.ActionType,
            DiscountPercentage = request.Dto.DiscountPercentage,
            DiscountAmount = request.Dto.DiscountAmount
        };

        var newAction = await _actionRepository.AddAsync(action);

        return new PromotionActionDto
        {
            ActionId = newAction.ActionId,
            ActionName = newAction.ActionName,
            ActionType = newAction.ActionType,
            DiscountPercentage = newAction.DiscountPercentage,
            DiscountAmount = newAction.DiscountAmount
        };
    }

    // Handler for Updating an Action
    public async Task<PromotionActionDto?> Handle(UpdatePromotionActionCommand request, CancellationToken cancellationToken)
    {
        var action = await _actionRepository.GetByIdAsync(request.ActionId);
        if (action == null) return null;

        action.ActionName = request.Dto.ActionName;
        action.ActionType = request.Dto.ActionType;
        action.DiscountPercentage = request.Dto.DiscountPercentage;
        action.DiscountAmount = request.Dto.DiscountAmount;

        await _actionRepository.UpdateAsync(action);

        return new PromotionActionDto
        {
            ActionId = action.ActionId,
            ActionName = action.ActionName,
            ActionType = action.ActionType,
            DiscountPercentage = action.DiscountPercentage,
            DiscountAmount = action.DiscountAmount
        };
    
    }

    // Handler for Deleting an Action
    public async Task<bool> Handle(DeletePromotionActionCommand request, CancellationToken cancellationToken)
    {
        var action = await _actionRepository.GetByIdAsync(request.ActionId);
        if (action == null) return false;

        await _actionRepository.DeleteAsync(action);

        return true;
    }
}