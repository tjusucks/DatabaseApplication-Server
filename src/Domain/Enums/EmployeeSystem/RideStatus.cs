namespace DbApp.Domain.Enums;

/// <summary>
/// 游乐设施运行状态
/// </summary>
public enum RideStatus
{
    /// <summary>
    /// 运行中
    /// </summary>
    Operating,

    /// <summary>
    /// 维护中
    /// </summary>
    Maintenance,

    /// <summary>
    /// 已关闭
    /// </summary>
    Closed,

    /// <summary>
    /// 测试中
    /// </summary>
    Testing
}
