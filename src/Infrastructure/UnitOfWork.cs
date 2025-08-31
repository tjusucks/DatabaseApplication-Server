using DbApp.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace DbApp.Infrastructure;

/// <summary>
/// Implements the Unit of Work pattern using the application's DbContext.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;

    public UnitOfWork(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Commits the transaction by saving changes in the DbContext.
    /// </summary>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}