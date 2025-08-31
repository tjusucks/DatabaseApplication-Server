using System.Threading;
using System.Threading.Tasks;

namespace DbApp.Domain.Interfaces;

/// <summary>
/// Represents the Unit of Work pattern for managing transactions and saving changes to the database.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Saves all changes made in this context to the database as a single transaction.
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}