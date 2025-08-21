namespace DbApp.Domain.Enums;

/// <summary>
/// 设施维护类型
/// </summary>
public enum MaintenanceType
{
    /// <summary>
    /// 预防性维护
    /// </summary>
    Preventive,

    /// <summary>
    /// 紧急维修
    /// </summary>
    Emergency,

    /// <summary>
    /// 部件更换
    /// </summary>
    Replacement,

    /// <summary>
    /// 软件升级
    /// </summary>
    SoftwareUpdate
}
