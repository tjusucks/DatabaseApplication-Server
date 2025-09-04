using DbApp.Domain.Entities.TicketingSystem;
using DbApp.Domain.Enums.TicketingSystem;

namespace DbApp.Domain.Interfaces.TicketingSystem;

public interface IRefundRepository
{
    /// <summary>
    /// 添加退款记录
    /// </summary>
    Task<RefundRecord> AddAsync(RefundRecord refundRecord);

    /// <summary>
    /// 更新退款记录
    /// </summary>
    Task<RefundRecord> UpdateAsync(RefundRecord refundRecord);

    /// <summary>
    /// 根据ID获取退款记录
    /// </summary>
    Task<RefundRecord?> GetByIdAsync(int refundId);

    /// <summary>
    /// 根据票ID获取退款记录
    /// </summary>
    Task<RefundRecord?> GetByTicketIdAsync(int ticketId);

    /// <summary>
    /// 删除退款记录
    /// </summary>
    Task<bool> DeleteAsync(int refundId);

    /// <summary>
    /// 搜索退款记录（带分页）
    /// </summary>
    Task<List<RefundRecord>> SearchAsync(
        string? keyword = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int? visitorId = null,
        int? ticketTypeId = null,
        RefundStatus? status = null,
        decimal? minAmount = null,
        decimal? maxAmount = null,
        string? sortBy = "RefundTime",
        bool descending = true,
        int page = 1,
        int pageSize = 20);

    /// <summary>
    /// 统计退款记录数量
    /// </summary>
    Task<int> CountAsync(
        string? keyword = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int? visitorId = null,
        int? ticketTypeId = null,
        RefundStatus? status = null,
        decimal? minAmount = null,
        decimal? maxAmount = null);

    /// <summary>
    /// 获取退款统计信息
    /// </summary>
    Task<RefundStatistics> GetStatisticsAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        int? ticketTypeId = null,
        RefundStatus? status = null);

    /// <summary>
    /// 检查票是否已退款
    /// </summary>
    Task<bool> IsTicketRefundedAsync(int ticketId);

    /// <summary>
    /// 获取访客的退款记录
    /// </summary>
    Task<List<RefundRecord>> GetByVisitorIdAsync(int visitorId, int page = 1, int pageSize = 20);
}

/// <summary>
/// 退款统计信息
/// </summary>
public class RefundStatistics
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
    public DateTime? FirstRefund { get; set; }
    public DateTime? LastRefund { get; set; }
}
