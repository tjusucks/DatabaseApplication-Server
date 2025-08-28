namespace DbApp.Domain.Enums.ResourceSystem;

/// <summary>
/// 设施维护类型
/// </summary>
public enum MaintenanceType
{
    /// <summary>
    /// 预防性维护
    /// </summary>
    Preventive = 0,

    /// <summary>
    /// 紧急维修
    /// </summary>
    Emergency = 1,

    /// <summary>
    /// 部件更换
    /// </summary>
    Replacement = 2,

    /// <summary>
    /// 软件升级
    /// </summary>
    SoftwareUpdate = 3
}
