
using DbApp.Domain.Entities.TicketingSystem;
using DbApp.Domain.Interfaces.TicketingSystem;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace DbApp.Application.TicketingSystem.Promotions;

// Refactored: This single class now handles all PromotionCondition commands
public class PromotionConditionCommandHandler :
    IRequestHandler<AddConditionToPromotionCommand, PromotionConditionDto?>,
    IRequestHandler<UpdatePromotionConditionCommand, PromotionConditionDto?>,
    IRequestHandler<DeletePromotionConditionCommand, bool>
{
    private readonly IPromotionRepository _promotionRepository;
    private readonly IPromotionConditionRepository _conditionRepository;
    public PromotionConditionCommandHandler(IPromotionRepository promotionRepository, IPromotionConditionRepository conditionRepository)
    {
        _promotionRepository = promotionRepository;
        _conditionRepository = conditionRepository;
    }

    // Handler for Adding a Condition
    public async Task<PromotionConditionDto?> Handle(AddConditionToPromotionCommand request, CancellationToken cancellationToken)
    {
        var promotion = await _promotionRepository.GetByIdAsync(request.PromotionId);
        if (promotion == null) return null;

        var condition = new PromotionCondition
        {
            PromotionId = request.PromotionId,
            ConditionName = request.Dto.ConditionName,
            ConditionType = request.Dto.ConditionType,
            TicketTypeId = request.Dto.TicketTypeId,
            MinQuantity = request.Dto.MinQuantity,
            MinAmount = request.Dto.MinAmount,
            Priority = request.Dto.Priority
        };

        var newCondition = await _conditionRepository.AddAsync(condition);

        return new PromotionConditionDto
        {
            ConditionId = newCondition.ConditionId,
            ConditionName = newCondition.ConditionName,
            ConditionType = newCondition.ConditionType,
            TicketTypeId = newCondition.TicketTypeId,
            MinQuantity = newCondition.MinQuantity,
            MinAmount = newCondition.MinAmount,
            Priority = newCondition.Priority
        };
    
    }

    // Handler for Updating a Condition
    // Inside the PromotionConditionCommandHandler class...

    // Handler for Updating a Condition

    public async Task<PromotionConditionDto?> Handle(UpdatePromotionConditionCommand request, CancellationToken cancellationToken)
    {
        var condition = await _conditionRepository.GetByIdAsync(request.ConditionId);
        if (condition == null) return null;

        condition.ConditionName = request.Dto.ConditionName;
        condition.ConditionType = request.Dto.ConditionType;
        condition.TicketTypeId = request.Dto.TicketTypeId;
        condition.MinQuantity = request.Dto.MinQuantity;
        condition.MinAmount = request.Dto.MinAmount;
        condition.Priority = request.Dto.Priority;

        await _conditionRepository.UpdateAsync(condition);

        return new PromotionConditionDto
        {
            ConditionId = condition.ConditionId,
            ConditionName = condition.ConditionName,
            ConditionType = condition.ConditionType,
            TicketTypeId = condition.TicketTypeId,
            MinQuantity = condition.MinQuantity,
            MinAmount = condition.MinAmount,
            Priority = condition.Priority
        };
    }

    // Handler for Deleting a Condition
    public async Task<bool> Handle(DeletePromotionConditionCommand request, CancellationToken cancellationToken)
    {
        var condition = await _conditionRepository.GetByIdAsync(request.ConditionId);
        if (condition == null) return false;

        await _conditionRepository.DeleteAsync(condition);

        return true;
    }
}