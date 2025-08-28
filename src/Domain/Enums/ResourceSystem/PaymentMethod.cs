namespace DbApp.Domain.Enums.ResourceSystem;

/// <summary>
/// 支付方式类型
/// </summary>
public enum PaymentMethod
{
    /// <summary>
    /// 现金
    /// </summary>
    Cash = 0,

    /// <summary>
    /// 信用卡
    /// </summary>
    CreditCard = 1,

    /// <summary>
    /// 移动支付
    /// </summary>
    MobilePay = 2,

    /// <summary>
    /// 数字货币
    /// </summary>
    Crypto = 3
}
