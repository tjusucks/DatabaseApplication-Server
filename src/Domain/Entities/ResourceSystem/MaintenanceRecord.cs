using System.ComponentModel.DataAnnotations;
using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Enums.ResourceSystem;

namespace DbApp.Domain.Entities.ResourceSystem;

/// <summary>
/// 设施维护记录
/// </summary>
public class MaintenanceRecord
{
    /// <summary>
    /// 维护ID
    /// </summary>
    public int MaintenanceId { get; set; }

    /// <summary>
    /// 设施ID
    /// </summary>
    public int RideId { get; set; }

    /// <summary>
    /// 维修组ID
    /// </summary>
    public int TeamId { get; set; }

    /// <summary>
    /// 负责管理员ID
    /// </summary>
    public int? ManagerId { get; set; }

    /// <summary>
    /// 维护类型
    /// </summary>
    public MaintenanceType MaintenanceType { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// 维护费用
    /// </summary>
    [Range(0.0, double.MaxValue)]
    public decimal Cost { get; set; }

    /// <summary>
    /// 更换部件
    /// </summary>
    public string? PartsReplaced { get; set; }

    /// <summary>
    /// 维护详情
    /// </summary>
    public string? MaintenanceDetails { get; set; }

    /// <summary>
    /// 是否完成
    /// </summary>
    public bool IsCompleted { get; set; }

    /// <summary>
    /// 是否验收通过
    /// </summary>
    public bool? IsAccepted { get; set; }

    /// <summary>
    /// 验收日期
    /// </summary>
    public DateTime? AcceptanceDate { get; set; }

    /// <summary>
    /// 验收意见
    /// </summary>
    public string? AcceptanceComments { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    // 导航属性
    public StaffTeam Team { get; set; } = null!;
    public Employee? Manager { get; set; }
    public AmusementRide Ride { get; set; } = null!;
}
