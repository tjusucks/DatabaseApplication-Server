using DbApp.Domain.Enums.TicketingSystem;
using MediatR;

namespace DbApp.Application.TicketingSystem.Promotions;

public record CreatePromotionCommand(
    string PromotionName,
    PromotionType PromotionType,
    string? Description,
    DateTime StartDatetime,
    DateTime EndDatetime,
    int? UsageLimitPerUser,
    int? TotalUsageLimit,
    int DisplayPriority,
    bool AppliesToAllTickets,
    bool IsActive,
    bool IsCombinable,
    int EmployeeId
) : IRequest<int>;

public record UpdatePromotionCommand(
    int PromotionId,
    string PromotionName,
    PromotionType PromotionType,
    string? Description,
    DateTime StartDatetime,
    DateTime EndDatetime,
    int? UsageLimitPerUser,
    int? TotalUsageLimit,
    int DisplayPriority,
    bool AppliesToAllTickets,
    bool IsActive,
    bool IsCombinable,
    int EmployeeId
) : IRequest<Unit>;

public record DeletePromotionCommand(int PromotionId) : IRequest<Unit>;
