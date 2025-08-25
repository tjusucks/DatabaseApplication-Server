namespace DbApp.Domain.Enums.ResourceSystem;

/// <summary>
/// 员工考勤状态
/// </summary>
public enum AttendanceStatus
{
    /// <summary>
    /// 正常
    /// </summary>
    Present,

    /// <summary>
    /// 迟到
    /// </summary>
    Late,

    /// <summary>
    /// 缺勤
    /// </summary>
    Absent,

    /// <summary>
    /// 请假
    /// </summary>
    Leave
}
