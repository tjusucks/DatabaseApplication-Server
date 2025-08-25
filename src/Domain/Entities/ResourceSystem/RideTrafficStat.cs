namespace DbApp.Domain.Entities.ResourceSystem;

/// <summary>
/// 游乐设施流量统计表（复合主键：设施ID + 记录时间）
/// </summary>
public class RideTrafficStat
{
    /// <summary>
    /// 设施ID
    /// </summary>
    public int RideId { get; set; }

    /// <summary>
    /// 记录时间
    /// </summary>
    public DateTime RecordTime { get; set; }

    /// <summary>
    /// 当前人流量
    /// </summary>
    public int VisitorCount { get; set; }

    /// <summary>
    /// 排队长度
    /// </summary>
    public int QueueLength { get; set; }

    /// <summary>
    /// 预计等待时间(分钟)
    /// </summary>
    public int WaitingTime { get; set; }

    /// <summary>
    /// 是否拥挤
    /// </summary>
    public bool? IsCrowded { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    // 导航属性
    public AmusementRide Ride { get; set; } = null!;
}
