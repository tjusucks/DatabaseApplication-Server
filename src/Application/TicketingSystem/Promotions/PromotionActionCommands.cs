
using MediatR;

namespace DbApp.Application.TicketingSystem.Promotions;

// Refactored: Command to add an action to a specific promotion
public record AddActionToPromotionCommand(
    int PromotionId,
    CreateActionRequest Dto
) : IRequest<PromotionActionDto?>;

// Refactored: Command to update an existing action by its own ID
public record UpdatePromotionActionCommand(
    int ActionId,
    CreateActionRequest Dto
) : IRequest<PromotionActionDto?>;

// Refactored: Command to delete an existing action by its own ID
public record DeletePromotionActionCommand(int ActionId) : IRequest<bool>;