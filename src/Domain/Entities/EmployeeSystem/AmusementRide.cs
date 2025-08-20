namespace DbApp.Domain.Entities;

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
    public string RideStatus { get; set; } = null!;

    /// <summary>
    /// 容量
    /// </summary>
    public short Capacity { get; set; }

    /// <summary>
    /// 单趟时间(秒)
    /// </summary>
    public short Duration { get; set; }

    /// <summary>
    /// 最低身高限制(cm)
    /// </summary>
    public decimal? HeightLimitMin { get; set; }

    /// <summary>
    /// 最高身高限制(cm)
    /// </summary>
    public decimal? HeightLimitMax { get; set; }

    /// <summary>
    /// 启用日期
    /// </summary>
    public DateTime? OpenDate { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    // 导航属性
    public Employee? Manager { get; set; }

    // 集合导航属性
    public ICollection<InspectionRecord> InspectionRecords { get; set; } = new List<InspectionRecord>();
    public ICollection<MaintenanceRecord> MaintenanceRecords { get; set; } = new List<MaintenanceRecord>();
    public ICollection<RideTrafficStat> RideTrafficStats { get; set; } = new List<RideTrafficStat>();
}
