using MediatR;

namespace DbApp.Application.TicketingSystem.Reservations;

/// <summary>
/// 查询所有可用票种
/// </summary>
public record GetAvailableTicketTypesQuery : IRequest<List<TicketTypeDto>>
{
    public DateTime? ForDate { get; init; } = null; 
}

/// <summary>
/// 根据ID查询特定票种
/// </summary>
public record GetTicketTypeByIdQuery(int TicketTypeId) : IRequest<TicketTypeDto?>;

/// <summary>
/// 计算预订价格
/// </summary>
public record CalculateReservationPriceQuery : IRequest<ReservationPriceCalculationDto>
{
    public List<ReservationItemRequestDto> Items { get; init; } = [];
    public int? PromotionId { get; init; }
    public int VisitorId { get; init; }
    public DateTime? VisitDate { get; init; }
}
