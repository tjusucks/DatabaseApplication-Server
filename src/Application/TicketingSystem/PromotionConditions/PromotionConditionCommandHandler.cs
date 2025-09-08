using DbApp.Domain.Entities.TicketingSystem;
using DbApp.Domain.Interfaces.TicketingSystem;
using MediatR;
using static DbApp.Domain.Exceptions;

namespace DbApp.Application.TicketingSystem.PromotionConditions;

public class PromotionConditionCommandHandler(
    IPromotionRepository promotionRepository,
    IPromotionConditionRepository conditionRepository
) :
    IRequestHandler<CreatePromotionConditionCommand, int>,
    IRequestHandler<UpdatePromotionConditionCommand, Unit>,
    IRequestHandler<DeletePromotionConditionCommand, Unit>
{
    public async Task<int> Handle(CreatePromotionConditionCommand request, CancellationToken cancellationToken)
    {
        _ = await promotionRepository.GetByIdAsync(request.PromotionId)
            ?? throw new ValidationException($"Promotion with ID {request.PromotionId} does not exist.");

        var condition = new PromotionCondition
        {
            PromotionId = request.PromotionId,
            ConditionName = request.ConditionName,
            ConditionType = request.ConditionType,
            TicketTypeId = request.TicketTypeId,
            MinQuantity = request.MinQuantity,
            MinAmount = request.MinAmount,
            VisitorType = request.VisitorType,
            MemberLevel = request.MemberLevel,
            DateFrom = request.DateFrom,
            DateTo = request.DateTo,
            DayOfWeek = request.DayOfWeek,
            Priority = request.Priority,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return await conditionRepository.CreateAsync(condition);
    }

    public async Task<Unit> Handle(UpdatePromotionConditionCommand request, CancellationToken cancellationToken)
    {
        var condition = await conditionRepository.GetByIdAsync(request.ConditionId)
            ?? throw new NotFoundException($"Condition with ID {request.ConditionId} does not exist.");

        condition.PromotionId = request.PromotionId;
        condition.ConditionName = request.ConditionName;
        condition.ConditionType = request.ConditionType;
        condition.TicketTypeId = request.TicketTypeId;
        condition.MinQuantity = request.MinQuantity;
        condition.MinAmount = request.MinAmount;
        condition.VisitorType = request.VisitorType;
        condition.MemberLevel = request.MemberLevel;
        condition.DateFrom = request.DateFrom;
        condition.DateTo = request.DateTo;
        condition.DayOfWeek = request.DayOfWeek;
        condition.Priority = request.Priority;
        condition.UpdatedAt = DateTime.UtcNow;

        await conditionRepository.UpdateAsync(condition);
        return Unit.Value;
    }

    public async Task<Unit> Handle(DeletePromotionConditionCommand request, CancellationToken cancellationToken)
    {
        var condition = await conditionRepository.GetByIdAsync(request.ConditionId)
            ?? throw new NotFoundException($"Condition with ID {request.ConditionId} does not exist.");

        await conditionRepository.DeleteAsync(condition);
        return Unit.Value;
    }
}
