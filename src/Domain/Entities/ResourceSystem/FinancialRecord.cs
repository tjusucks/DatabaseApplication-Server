using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Enums.ResourceSystem;

namespace DbApp.Domain.Entities.ResourceSystem;

/// <summary>
/// 财务记录表
/// </summary>
public class FinancialRecord
{
    /// <summary>
    /// 记录ID
    /// </summary>
    public int RecordId { get; set; }

    /// <summary>
    /// 交易日期
    /// </summary>
    public DateTime TransactionDate { get; set; }

    /// <summary>
    /// 金额
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// 交易类型
    /// </summary>
    public TransactionType TransactionType { get; set; }

    /// <summary>
    /// 支付方式
    /// </summary>
    public PaymentMethod? PaymentMethod { get; set; }

    /// <summary>
    /// 负责员工ID
    /// </summary>
    public int? ResponsibleEmployeeId { get; set; }

    /// <summary>
    /// 审批人ID
    /// </summary>
    public int? ApprovedById { get; set; }

    /// <summary>
    /// 交易描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    // 导航属性
    public Employee? ApprovedBy { get; set; }
    public Employee? ResponsibleEmployee { get; set; }
}
