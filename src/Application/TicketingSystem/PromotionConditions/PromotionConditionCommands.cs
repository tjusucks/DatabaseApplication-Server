using DbApp.Domain.Enums.TicketingSystem;
using MediatR;

namespace DbApp.Application.TicketingSystem.PromotionConditions;

public record CreatePromotionConditionCommand(
    int PromotionId,
    string ConditionName,
    ConditionType ConditionType,
    int? TicketTypeId,
    int? MinQuantity,
    decimal? MinAmount,
    string? VisitorType,
    string? MemberLevel,
    DateTime? DateFrom,
    DateTime? DateTo,
    int? DayOfWeek,
    int Priority
) : IRequest<int>;

public record UpdatePromotionConditionCommand(
    int ConditionId,
    int PromotionId,
    string ConditionName,
    ConditionType ConditionType,
    int? TicketTypeId,
    int? MinQuantity,
    decimal? MinAmount,
    string? VisitorType,
    string? MemberLevel,
    DateTime? DateFrom,
    DateTime? DateTo,
    int? DayOfWeek,
    int Priority
) : IRequest<Unit>;

public record DeletePromotionConditionCommand(int ConditionId) : IRequest<Unit>;
