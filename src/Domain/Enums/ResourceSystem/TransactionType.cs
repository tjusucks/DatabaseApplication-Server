namespace DbApp.Domain.Enums.ResourceSystem;

/// <summary>
/// 财务交易类型
/// </summary>
public enum TransactionType
{
    /// <summary>
    /// 收入
    /// </summary>
    Income = 0,

    /// <summary>
    /// 支出
    /// </summary>
    Expense = 1,

    /// <summary>
    /// 退款
    /// </summary>
    Refund = 2,

    /// <summary>
    /// 转账
    /// </summary>
    Transfer = 3
}
