using DbApp.Domain.Enums.ResourceSystem;

namespace DbApp.Domain.Entities.ResourceSystem;

/// <summary>
/// 季节性活动表
/// </summary>
public class SeasonalEvent
{
    /// <summary>
    /// 活动ID
    /// </summary>
    public int EventId { get; set; }

    /// <summary>
    /// 活动名称
    /// </summary>
    public string EventName { get; set; } = null!;

    /// <summary>
    /// 活动类型
    /// </summary>
    public EventType EventType { get; set; }

    /// <summary>
    /// 活动描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 开始日期
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// 结束日期
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// 活动地点
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// 预算
    /// </summary>
    public decimal? Budget { get; set; }

    /// <summary>
    /// 最大容量
    /// </summary>
    public int? MaxCapacity { get; set; }

    /// <summary>
    /// 票价
    /// </summary>
    public decimal? TicketPrice { get; set; }

    /// <summary>
    /// 活动状态
    /// </summary>
    public EventStatus Status { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
