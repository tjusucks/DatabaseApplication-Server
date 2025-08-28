namespace DbApp.Domain.Enums.ResourceSystem;

/// <summary>
/// 活动生命周期状态
/// </summary>
public enum EventStatus
{
    /// <summary>
    /// 筹备中
    /// </summary>
    Planning = 0,

    /// <summary>
    /// 进行中
    /// </summary>
    Ongoing = 1,

    /// <summary>
    /// 已结束
    /// </summary>
    Completed = 2,

    /// <summary>
    /// 已取消
    /// </summary>
    Cancelled = 3
}

