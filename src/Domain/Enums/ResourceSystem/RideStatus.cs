namespace DbApp.Domain.Enums.ResourceSystem;

/// <summary>
/// 游乐设施运行状态
/// </summary>
public enum RideStatus
{
    /// <summary>
    /// 运行中
    /// </summary>
<<<<<<< HEAD
<<<<<<< HEAD
    Operating = 0,
=======
    Operating = 1,
>>>>>>> 351eae1 (feat(resource): add resource system domain and API)
=======
    Operating = 0,
>>>>>>> 8be1347 (refactor(resource): implement CQRS pattern for ride traffic statistics)

    /// <summary>
    /// 维护中
    /// </summary>
<<<<<<< HEAD
<<<<<<< HEAD
    Maintenance = 1,
=======
    Maintenance = 2,
>>>>>>> 351eae1 (feat(resource): add resource system domain and API)
=======
    Maintenance = 1,
>>>>>>> 8be1347 (refactor(resource): implement CQRS pattern for ride traffic statistics)

    /// <summary>
    /// 已关闭
    /// </summary>
<<<<<<< HEAD
<<<<<<< HEAD
    Closed = 2,
=======
    Closed = 3,
>>>>>>> 351eae1 (feat(resource): add resource system domain and API)
=======
    Closed = 2,
>>>>>>> 8be1347 (refactor(resource): implement CQRS pattern for ride traffic statistics)

    /// <summary>
    /// 测试中
    /// </summary>
<<<<<<< HEAD
<<<<<<< HEAD
    Testing = 3
=======
    Testing = 4
>>>>>>> 351eae1 (feat(resource): add resource system domain and API)
=======
    Testing = 3
>>>>>>> 8be1347 (refactor(resource): implement CQRS pattern for ride traffic statistics)
}
