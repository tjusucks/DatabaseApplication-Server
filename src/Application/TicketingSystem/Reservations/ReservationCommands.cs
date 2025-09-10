using DbApp.Domain.Enums.TicketingSystem;
using MediatR;

namespace DbApp.Application.TicketingSystem.Reservations;

/// <summary>
/// 创建预订命令
/// </summary>
public record CreateReservationCommand : IRequest<CreateReservationResponseDto>
{
    public int VisitorId { get; init; }
    public DateTime VisitDate { get; init; }
    public List<CreateReservationItemDto> Items { get; init; } = [];
    public int? PromotionId { get; init; }
    public string? SpecialRequests { get; init; }
    public string PaymentMethod { get; init; } = string.Empty;
}

/// <summary>
/// 更新预订状态命令
/// </summary>
public record UpdateReservationStatusCommand : IRequest<ReservationDto>
{
    public int ReservationId { get; init; }
    public ReservationStatus Status { get; init; }
    public string? Reason { get; init; }
}

/// <summary>
/// 处理支付命令
/// </summary>
public record ProcessPaymentCommand : IRequest<ReservationDto>
{
    public int ReservationId { get; init; }
    public string PaymentMethod { get; init; } = string.Empty;
}

/// <summary>
/// 取消预订命令
/// </summary>
public record CancelReservationCommand : IRequest<bool>
{
    public int ReservationId { get; init; }
    public string CancellationReason { get; init; } = string.Empty;
    public int? RequestingVisitorId { get; init; }
}

/// <summary>
/// Deletes a reservation.
/// </summary>
public record DeleteReservationCommand(int ReservationId) : IRequest<bool>;

/// <summary>
/// 处理支付响应DTO
/// </summary>
public class ProcessPaymentResponseDto
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public PaymentStatus PaymentStatus { get; set; }
    public ReservationStatus ReservationStatus { get; set; }
}

/// <summary>
/// 取消预订响应DTO
/// </summary>
public class CancelReservationResponseDto
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public decimal RefundAmount { get; set; }
}
