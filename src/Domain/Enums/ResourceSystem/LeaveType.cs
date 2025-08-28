namespace DbApp.Domain.Enums.ResourceSystem;

/// <summary>
/// 员工请假类型
/// </summary>
public enum LeaveType
{
    /// <summary>
    /// 事假
    /// </summary>
    Personal = 0,

    /// <summary>
    /// 病假
    /// </summary>
    Sick = 1,

    /// <summary>
    /// 年假
    /// </summary>
    Annual = 2,

    /// <summary>
    /// 调休
    /// </summary>
    Compensatory = 3
}
