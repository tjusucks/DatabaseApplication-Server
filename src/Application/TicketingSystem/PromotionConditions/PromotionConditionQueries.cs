using MediatR;

namespace DbApp.Application.TicketingSystem.PromotionConditions;

public record GetPromotionConditionByIdQuery(int PromotionId, int ConditionId) : IRequest<PromotionConditionDto?>;

public record GetPromotionConditionsByPromotionIdQuery(int PromotionId) : IRequest<List<PromotionConditionDto>>;
