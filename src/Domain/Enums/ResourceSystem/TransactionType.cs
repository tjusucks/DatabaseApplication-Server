namespace DbApp.Domain.Enums.ResourceSystem;

/// <summary>
/// 财务交易类型
/// </summary>
public enum TransactionType
{
    /// <summary>
    /// 收入
    /// </summary>
    Income,

    /// <summary>
    /// 支出
    /// </summary>
    Expense,

    /// <summary>
    /// 退款
    /// </summary>
    Refund,

    /// <summary>
    /// 转账
    /// </summary>
    Transfer
}
