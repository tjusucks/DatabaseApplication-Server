using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Specifications.Common;
using DbApp.Domain.Specifications.UserSystem;
using DbApp.Domain.Statistics.UserSystem;

namespace DbApp.Domain.Interfaces.UserSystem;

/// <summary>
/// Repository interface for Visitor entity operations.
/// </summary>
public interface IVisitorRepository
{
    /// <summary>
    /// Creates a new visitor record.
    /// </summary>
    Task<int> CreateAsync(Visitor visitor);

    /// <summary>
    /// Gets a visitor by their ID.
    /// </summary>
    Task<Visitor?> GetByIdAsync(int visitorId);

    /// <summary>
    /// Gets all visitors.
    /// </summary>
    Task<List<Visitor>> GetAllAsync();

    /// <summary>
    /// Updates a visitor record.
    /// </summary>
    Task UpdateAsync(Visitor visitor);

    /// <summary>
    /// Deletes a visitor record.
    /// </summary>
    Task DeleteAsync(Visitor visitor);

    /// <summary>
    /// Searches visitors by multiple criteria. (admin use)
    /// </summary>
    Task<List<Visitor>> SearchAsync(PaginatedSpec<VisitorSpec> spec);

    /// <summary>
    /// Get overall visitor statistics with filtering. (admin use)
    /// </summary>
    Task<VisitorStats> GetStatsAsync(VisitorSpec spec);

    /// <summary>
    /// Get total count of visitors with filtering. (admin use)
    /// </summary>
    Task<int> CountAsync(VisitorSpec spec);

    /// <summary>
    /// Get grouped visitor statistics with filtering. (admin use)
    /// </summary>
    Task<List<GroupedVisitorStats>> GetGroupedStatsAsync(GroupedSpec<VisitorSpec> spec);
}
