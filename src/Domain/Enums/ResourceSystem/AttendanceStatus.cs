namespace DbApp.Domain.Enums.ResourceSystem;

/// <summary>
/// 员工考勤状态
/// </summary>
public enum AttendanceStatus
{
    /// <summary>
    /// 正常
    /// </summary>
    Present = 0,

    /// <summary>
    /// 迟到
    /// </summary>
    Late = 1,

    /// <summary>
    /// 缺勤
    /// </summary>
    Absent = 2,

    /// <summary>
    /// 请假
    /// </summary>
    Leave = 3
}
