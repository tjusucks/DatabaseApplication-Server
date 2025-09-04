using DbApp.Domain.Enums.TicketingSystem;
using MediatR;

namespace DbApp.Application.TicketingSystem.Tickets;

/// <summary>
/// 申请退票命令
/// </summary>
public record RequestRefundCommand : IRequest<RefundResultDto>
{
    public int TicketId { get; init; }
    public int RequestingVisitorId { get; init; }
    public string RefundReason { get; init; } = string.Empty;
    public bool IsAdminRequest { get; init; } = false;
    public int? ProcessorId { get; init; }
}

/// <summary>
/// 处理退票申请命令（管理员）
/// </summary>
public record ProcessRefundCommand : IRequest<RefundResultDto>
{
    public int RefundId { get; init; }
    public RefundStatus Decision { get; init; }
    public int ProcessorId { get; init; }
    public string ProcessingNotes { get; init; } = string.Empty;
}

/// <summary>
/// 批量退票命令（批量处理）
/// </summary>
public record BatchRefundCommand : IRequest<BatchRefundResultDto>
{
    public List<int> TicketIds { get; init; } = [];
    public string RefundReason { get; init; } = string.Empty;
    public int ProcessorId { get; init; }
}

/// <summary>
/// 退票结果DTO
/// </summary>
public class RefundResultDto
{
    public bool IsSuccess { get; set; }
    public int? RefundId { get; set; }
    public decimal RefundAmount { get; set; }
    public RefundStatus Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime? ProcessedAt { get; set; }
    public string? RefundReference { get; set; }
}

/// <summary>
/// 批量退票结果DTO
/// </summary>
public class BatchRefundResultDto
{
    public int TotalRequested { get; set; }
    public int SuccessfulRefunds { get; set; }
    public int FailedRefunds { get; set; }
    public decimal TotalRefundAmount { get; set; }
    public List<RefundResultDto> Results { get; set; } = [];
    public List<string> Errors { get; set; } = [];
}

/// <summary>
/// 退票详情DTO
/// </summary>
public class RefundDetailsDto
{
    public int RefundId { get; set; }
    public int TicketId { get; set; }
    public string TicketSerialNumber { get; set; } = string.Empty;
    public string TicketTypeName { get; set; } = string.Empty;
    public int VisitorId { get; set; }
    public string VisitorName { get; set; } = string.Empty;
    public decimal OriginalAmount { get; set; }
    public decimal RefundAmount { get; set; }
    public decimal RefundFee { get; set; }
    public DateTime RefundTime { get; set; }
    public string? RefundReason { get; set; }
    public RefundStatus RefundStatus { get; set; }
    public int? ProcessorId { get; set; }
    public string? ProcessorName { get; set; }
    public string? ProcessingNotes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
