namespace DbApp.Domain.Enums.ResourceSystem;

/// <summary>
/// 设施检查类型
/// </summary>
public enum CheckType
{
    /// <summary>
    /// 日常检查
    /// </summary>
    Daily = 0,

    /// <summary>
    /// 月度检查
    /// </summary>
    Monthly = 1,

    /// <summary>
    /// 年度检查
    /// </summary>
    Annual = 2,

    /// <summary>
    /// 特殊检查
    /// </summary>
    Special = 3
}
