using DbApp.Domain.Enums.ResourceSystem;

namespace DbApp.Domain.Entities.ResourceSystem;

/// <summary>
/// 游乐设施检查记录
/// </summary>
public class InspectionRecord
{
    /// <summary>
    /// 检查ID
    /// </summary>
    public int InspectionId { get; set; }

    /// <summary>
    /// 设施ID
    /// </summary>
    public int RideId { get; set; }

    /// <summary>
    /// 检查组ID
    /// </summary>
    public int TeamId { get; set; }

    /// <summary>
    /// 检查日期
    /// </summary>
    public DateTime CheckDate { get; set; }

    /// <summary>
    /// 检查类型
    /// </summary>
    public CheckType CheckType { get; set; }

    /// <summary>
    /// 是否通过
    /// </summary>
    public bool IsPassed { get; set; }

    /// <summary>
    /// 发现问题
    /// </summary>
    public string? IssuesFound { get; set; }

    /// <summary>
    /// 建议措施
    /// </summary>
    public string? Recommendations { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    // 导航属性
    public AmusementRide Ride { get; set; } = null!;
    //public StaffTeam Team { get; set; } = null!;
}
