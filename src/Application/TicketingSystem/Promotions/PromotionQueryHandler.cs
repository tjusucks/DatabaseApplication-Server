using DbApp.Domain.Interfaces.TicketingSystem;
using MediatR;
using static DbApp.Domain.Exceptions;
namespace DbApp.Application.TicketingSystem.Promotions;

public class PromotionQueryHandler(IPromotionRepository promotionRepository) :
    IRequestHandler<GetPromotionByIdQuery, PromotionDto?>,
    IRequestHandler<GetAllPromotionsQuery, List<PromotionDto>>
{
    public async Task<PromotionDto?> Handle(GetPromotionByIdQuery request, CancellationToken cancellationToken)
    {
        var promotion = await promotionRepository.GetByIdAsync(request.PromotionId);
        if (promotion == null)
        {
            throw new NotFoundException($"{request.PromotionId}could not found");
        }

        return new PromotionDto
        {
            PromotionId = promotion.PromotionId,
            PromotionName = promotion.PromotionName,
            PromotionType = promotion.PromotionType,
            Description = promotion.Description,
            StartDatetime = promotion.StartDatetime,
            EndDatetime = promotion.EndDatetime,
            UsageLimitPerUser = promotion.UsageLimitPerUser,
            TotalUsageLimit = promotion.TotalUsageLimit,
            CurrentUsageCount = promotion.CurrentUsageCount,
            DisplayPriority = promotion.DisplayPriority,
            AppliesToAllTickets = promotion.AppliesToAllTickets,
            IsActive = promotion.IsActive,
            IsCombinable = promotion.IsCombinable,
            EmployeeId = promotion.EmployeeId,
            CreatedAt = promotion.CreatedAt,
            UpdatedAt = promotion.UpdatedAt
        };
    }

    public async Task<List<PromotionDto>> Handle(GetAllPromotionsQuery request, CancellationToken cancellationToken)
    {
        var promotions = await promotionRepository.GetAllAsync();
        return [.. promotions.Select(p => new PromotionDto
        {
            PromotionId = p.PromotionId,
            PromotionName = p.PromotionName,
            PromotionType = p.PromotionType,
            Description = p.Description,
            StartDatetime = p.StartDatetime,
            EndDatetime = p.EndDatetime,
            UsageLimitPerUser = p.UsageLimitPerUser,
            TotalUsageLimit = p.TotalUsageLimit,
            CurrentUsageCount = p.CurrentUsageCount,
            DisplayPriority = p.DisplayPriority,
            AppliesToAllTickets = p.AppliesToAllTickets,
            IsActive = p.IsActive,
            IsCombinable = p.IsCombinable,
            EmployeeId = p.EmployeeId,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        })];
    }
}
