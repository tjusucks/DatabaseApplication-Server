using MediatR;

namespace DbApp.Application.TicketingSystem.Promotions;

public record GetPromotionByIdQuery(int PromotionId) : IRequest<PromotionDto?>;

public record GetAllPromotionsQuery() : IRequest<List<PromotionDto>>;
