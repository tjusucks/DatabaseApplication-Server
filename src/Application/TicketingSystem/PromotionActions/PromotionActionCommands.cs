using DbApp.Domain.Enums.TicketingSystem;
using MediatR;

namespace DbApp.Application.TicketingSystem.PromotionActions;

public record CreatePromotionActionCommand(
    int PromotionId,
    string ActionName,
    PromotionActionType ActionType,
    int? TargetTicketTypeId,
    decimal? DiscountPercentage,
    decimal? DiscountAmount,
    decimal? FixedPrice,
    int? PointsAwarded,
    int? FreeTicketTypeId,
    int? FreeTicketQuantity
) : IRequest<int>;

public record UpdatePromotionActionCommand(
    int ActionId,
    int PromotionId,
    string ActionName,
    PromotionActionType ActionType,
    int? TargetTicketTypeId,
    decimal? DiscountPercentage,
    decimal? DiscountAmount,
    decimal? FixedPrice,
    int? PointsAwarded,
    int? FreeTicketTypeId,
    int? FreeTicketQuantity
) : IRequest<Unit>;

public record DeletePromotionActionCommand(int ActionId) : IRequest<Unit>;
