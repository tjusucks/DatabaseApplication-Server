using DbApp.Domain.Enums.TicketingSystem;
using MediatR;

namespace DbApp.Application.TicketingSystem.Refunds;

/// <summary>
/// 查询退票记录
/// </summary>
public record SearchRefundRecordsQuery : IRequest<RefundSearchResultDto>
{
    public string? Keyword { get; init; } = null; // 搜索关键词：票号、访客姓名等
    public DateTime? StartDate { get; init; } = null;
    public DateTime? EndDate { get; init; } = null;
    public int? VisitorId { get; init; } = null;
    public int? TicketTypeId { get; init; } = null;
    public RefundStatus? Status { get; init; } = null;
    public decimal? MinAmount { get; init; } = null;
    public decimal? MaxAmount { get; init; } = null;
    public string? SortBy { get; init; } = "RefundTime"; // 排序字段
    public bool Descending { get; init; } = true;
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

/// <summary>
/// 根据ID查询退票记录详情
/// </summary>
public record GetRefundByIdQuery(int RefundId) : IRequest<RefundDetailsDto?>;

/// <summary>
/// 根据票ID查询退票记录
/// </summary>
public record GetRefundByTicketIdQuery(int TicketId) : IRequest<RefundDetailsDto?>;

/// <summary>
/// 获取退票统计信息
/// </summary>
public record GetRefundStatsQuery : IRequest<RefundStatsDto>
{
    public DateTime? StartDate { get; init; } = null;
    public DateTime? EndDate { get; init; } = null;
    public int? TicketTypeId { get; init; } = null;
    public RefundStatus? Status { get; init; } = null;
}

/// <summary>
/// 退票搜索结果DTO
/// </summary>
public class RefundSearchResultDto
{
    public List<RefundDetailsDto> Refunds { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}

/// <summary>
/// 退票统计DTO
/// </summary>
public class RefundStatsDto
{
    public int TotalRefunds { get; set; }
    public decimal TotalRefundAmount { get; set; }
    public decimal TotalRefundFees { get; set; }
    public decimal NetRefundAmount { get; set; }
    public decimal AverageRefundAmount { get; set; }
    public int PendingRefunds { get; set; }
    public int ApprovedRefunds { get; set; }
    public int RejectedRefunds { get; set; }
    public int CompletedRefunds { get; set; }
    public decimal RefundRate { get; set; } // 退票率
    public DateTime? FirstRefund { get; set; }
    public DateTime? LastRefund { get; set; }
}
