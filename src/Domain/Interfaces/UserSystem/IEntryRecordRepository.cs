using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Specifications.Common;
using DbApp.Domain.Specifications.UserSystem;
using DbApp.Domain.Statistics.UserSystem;

namespace DbApp.Domain.Interfaces.UserSystem;

/// <summary>
/// Repository interface for EntryRecord entity operations.
/// Provides methods for visitor entry/exit management and statistics.
/// </summary>
public interface IEntryRecordRepository
{
    /// <summary>
    /// Creates a new entry record for visitor entry.
    /// </summary>
    Task<int> CreateAsync(EntryRecord entryRecord);

    /// <summary>
    /// Gets an entry record by its ID.
    /// </summary>
    Task<EntryRecord?> GetByIdAsync(int entryRecordId);

    /// <summary>
    /// Gets all entry records.
    /// </summary>
    /// <returns>List of all entry records.</returns>
    Task<List<EntryRecord>> GetAllAsync();

    /// <summary>
    /// Updates an entry record (typically for exit registration).
    /// </summary>
    Task UpdateAsync(EntryRecord entryRecord);

    /// <summary>
    /// Deletes an entry record.
    /// </summary>
    Task DeleteAsync(EntryRecord entryRecord);

    /// <summary>
    /// Gets the active (not exited) entry record for a given visitor.
    /// </summary>
    Task<EntryRecord?> GetActiveEntryByVisitorIdAsync(int visitorId);

    /// <summary>
    /// Searches entry records by multiple criteria.
    /// </summary>
    Task<List<EntryRecord>> SearchAsync(PaginatedSpec<EntryRecordSpec> spec);

    /// <summary>
    /// Get overall entry record statistics with filtering.
    /// </summary>
    Task<EntryRecordStats> GetStatsAsync(EntryRecordSpec spec);

    /// <summary>
    /// Get total count of entry records with filtering.
    /// </summary>
    Task<int> CountAsync(EntryRecordSpec spec);

    /// <summary>
    /// Get grouped entry record statistics with filtering.
    /// </summary>
    Task<List<GroupedEntryRecordStats>> GetGroupedStatsAsync(GroupedSpec<EntryRecordSpec> spec);
}
