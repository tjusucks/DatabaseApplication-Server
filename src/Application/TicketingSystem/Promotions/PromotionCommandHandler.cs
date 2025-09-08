using DbApp.Domain.Entities.TicketingSystem;
using DbApp.Domain.Interfaces.TicketingSystem;
using MediatR;

namespace DbApp.Application.TicketingSystem.Promotions;

public class PromotionCommandHandler(
    IPromotionRepository promotionRepository
) :
    IRequestHandler<CreatePromotionCommand, int>,
    IRequestHandler<UpdatePromotionCommand, Unit>,
    IRequestHandler<DeletePromotionCommand, Unit>
{
    public async Task<int> Handle(CreatePromotionCommand request, CancellationToken cancellationToken)
    {
        var promotion = new Promotion
        {
            PromotionName = request.PromotionName,
            PromotionType = request.PromotionType,
            Description = request.Description,
            StartDatetime = request.StartDatetime,
            EndDatetime = request.EndDatetime,
            UsageLimitPerUser = request.UsageLimitPerUser,
            TotalUsageLimit = request.TotalUsageLimit,
            CurrentUsageCount = 0,
            DisplayPriority = request.DisplayPriority,
            AppliesToAllTickets = request.AppliesToAllTickets,
            IsActive = request.IsActive,
            IsCombinable = request.IsCombinable,
            EmployeeId = request.EmployeeId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return await promotionRepository.CreateAsync(promotion);
    }

    public async Task<Unit> Handle(UpdatePromotionCommand request, CancellationToken cancellationToken)
    {
        var promotion = await promotionRepository.GetByIdAsync(request.PromotionId);
        if (promotion == null)
            return Unit.Value;

        promotion.PromotionName = request.PromotionName;
        promotion.PromotionType = request.PromotionType;
        promotion.Description = request.Description;
        promotion.StartDatetime = request.StartDatetime;
        promotion.EndDatetime = request.EndDatetime;
        promotion.UsageLimitPerUser = request.UsageLimitPerUser;
        promotion.TotalUsageLimit = request.TotalUsageLimit;
        promotion.DisplayPriority = request.DisplayPriority;
        promotion.AppliesToAllTickets = request.AppliesToAllTickets;
        promotion.IsActive = request.IsActive;
        promotion.IsCombinable = request.IsCombinable;
        promotion.EmployeeId = request.EmployeeId;
        promotion.UpdatedAt = DateTime.UtcNow;

        await promotionRepository.UpdateAsync(promotion);
        return Unit.Value;
    }

    public async Task<Unit> Handle(DeletePromotionCommand request, CancellationToken cancellationToken)
    {
        var promotion = await promotionRepository.GetByIdAsync(request.PromotionId);
        if (promotion == null)
            return Unit.Value;

        await promotionRepository.DeleteAsync(promotion);
        return Unit.Value;
    }
}
