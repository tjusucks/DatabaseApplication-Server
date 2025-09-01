using MediatR;

namespace DbApp.Application.TicketingSystem.PromotionActions;

public record GetPromotionActionByIdQuery(int PromotionId, int ActionId) : IRequest<PromotionActionDto?>;

public record GetPromotionActionsByPromotionIdQuery(int PromotionId) : IRequest<List<PromotionActionDto>>;
