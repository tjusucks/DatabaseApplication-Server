namespace DbApp.Domain.Entities;

/// <summary>
/// 员工考勤记录
/// </summary>
public class Attendance
{
    /// <summary>
    /// 考勤ID
    /// </summary>
    public int AttendanceId { get; set; }

    /// <summary>
    /// 员工ID
    /// </summary>
    public int EmployeeId { get; set; }

    /// <summary>
    /// 考勤日期
    /// </summary>
    public DateTime AttendanceDate { get; set; }

    /// <summary>
    /// 签到时间
    /// </summary>
    public DateTime CheckInTime { get; set; }

    /// <summary>
    /// 签退时间
    /// </summary>
    public DateTime? CheckOutTime { get; set; }

    /// <summary>
    /// 考勤状态
    /// </summary>
    public string AttendanceStatus { get; set; } = null!;

    /// <summary>
    /// 请假类型
    /// </summary>
    public string? LeaveType { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    // 导航属性
    public Employee Employee { get; set; } = null!;
}