namespace DbApp.Domain.Enums;

/// <summary>
/// 支付方式类型
/// </summary>
public enum PaymentMethod
{
    /// <summary>
    /// 现金
    /// </summary>
    Cash,

    /// <summary>
    /// 信用卡
    /// </summary>
    CreditCard,

    /// <summary>
    /// 移动支付
    /// </summary>
    MobilePay,

    /// <summary>
    /// 数字货币
    /// </summary>
    Crypto
}
