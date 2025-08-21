namespace DbApp.Domain.Enums;

/// <summary>
/// 活动生命周期状态
/// </summary>
public enum EventStatus
{
    /// <summary>
    /// 筹备中
    /// </summary>
    Planning,

    /// <summary>
    /// 进行中
    /// </summary>
    Ongoing,

    /// <summary>
    /// 已结束
    /// </summary>
    Completed,

    /// <summary>
    /// 已取消
    /// </summary>
    Cancelled
}

