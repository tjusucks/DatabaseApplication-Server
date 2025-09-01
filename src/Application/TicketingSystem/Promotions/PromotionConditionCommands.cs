
using MediatR;

namespace DbApp.Application.TicketingSystem.Promotions;

// Refactored: Command to add a condition to a specific promotion
public record AddConditionToPromotionCommand(
    int PromotionId,
    CreateConditionRequest Dto
) : IRequest<PromotionConditionDto?>;

// Refactored: Command to update an existing condition by its own ID
public record UpdatePromotionConditionCommand(
    int ConditionId,
    CreateConditionRequest Dto
) : IRequest<PromotionConditionDto?>;

// Refactored: Command to delete an existing condition by its own ID
public record DeletePromotionConditionCommand(int ConditionId) : IRequest<bool>;