namespace DbApp.Domain.Interfaces.TicketingSystem;

/// <summary>
/// 事务管理接口
/// </summary>
public interface ITransactionManagerService
{
    /// <summary>
    /// 执行事务操作
    /// </summary>
    Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default);

    /// <summary>
    /// 执行事务操作（无返回值）
    /// </summary>
    Task ExecuteInTransactionAsync(Func<Task> operation, CancellationToken cancellationToken = default);
}
