using System.ComponentModel.DataAnnotations;
using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Enums.ResourceSystem;

namespace DbApp.Domain.Entities.ResourceSystem;

/// <summary>
/// 游乐设施表
/// </summary>
public class AmusementRide
{
    /// <summary>
    /// 设施ID
    /// </summary>
    public int RideId { get; set; }

    /// <summary>
    /// 设施名称
    /// </summary>
    public string RideName { get; set; } = null!;

    /// <summary>
    /// 管理员ID
    /// </summary>
    public int? ManagerId { get; set; }

    /// <summary>
    /// 位置
    /// </summary>
    public string Location { get; set; } = null!;

    /// <summary>
    /// 设施描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 设施状态
    /// </summary>
    public RideStatus RideStatus { get; set; }

    /// <summary>
    /// 容量
    /// </summary>
    [Range(1, int.MaxValue)]
    public int Capacity { get; set; }

    /// <summary>
    /// 单趟时间(秒)
    /// </summary>
    [Range(1, int.MaxValue)]
    public int Duration { get; set; }

    /// <summary>
    /// 最低身高限制(cm)
    /// </summary>
    [Range(50, 300)]
    public int HeightLimitMin { get; set; }

    /// <summary>
    /// 最高身高限制(cm)
    /// </summary>
    [Range(50, 300)]
    public int HeightLimitMax { get; set; }

    /// <summary>
    /// 启用日期
    /// </summary>
    public DateTime? OpenDate { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    // 导航属性
    public Employee? Manager { get; set; }
    public ICollection<InspectionRecord> InspectionRecords { get; set; } = [];
    public ICollection<MaintenanceRecord> MaintenanceRecords { get; set; } = [];
    public ICollection<RideTrafficStat> RideTrafficStats { get; set; } = [];
}
