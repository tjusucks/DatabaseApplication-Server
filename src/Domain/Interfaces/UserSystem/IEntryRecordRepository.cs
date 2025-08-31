using DbApp.Domain.Entities.UserSystem;

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
    /// <param name="entryRecord">The entry record to create.</param>
    /// <returns>The ID of the created entry record.</returns>
    Task<int> CreateAsync(EntryRecord entryRecord);

    /// <summary>
    /// Gets an entry record by its ID.
    /// </summary>
    /// <param name="entryRecordId">The entry record ID.</param>
    /// <returns>The entry record if found, null otherwise.</returns>
    Task<EntryRecord?> GetByIdAsync(int entryRecordId);

    /// <summary>
    /// Gets all entry records.
    /// </summary>
    /// <returns>List of all entry records.</returns>
    Task<List<EntryRecord>> GetAllAsync();

    /// <summary>
    /// Gets entry records for a specific visitor.
    /// </summary>
    /// <param name="visitorId">The visitor ID.</param>
    /// <returns>List of entry records for the visitor.</returns>
    Task<List<EntryRecord>> GetByVisitorIdAsync(int visitorId);

    /// <summary>
    /// Gets current visitors in the park (entry without exit).
    /// </summary>
    /// <returns>List of entry records for visitors currently in the park.</returns>
    Task<List<EntryRecord>> GetCurrentVisitorsAsync();

    /// <summary>
    /// Gets entry records within a date range.
    /// </summary>
    /// <param name="startDate">Start date for the range.</param>
    /// <param name="endDate">End date for the range.</param>
    /// <returns>List of entry records within the date range.</returns>
    Task<List<EntryRecord>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Gets the count of current visitors in the park.
    /// </summary>
    /// <returns>Number of visitors currently in the park.</returns>
    Task<int> GetCurrentVisitorCountAsync();

    /// <summary>
    /// Gets visitor count statistics for a specific date.
    /// </summary>
    /// <param name="date">The date to get statistics for.</param>
    /// <returns>Tuple containing (total entries, total exits, current count).</returns>
    Task<(int TotalEntries, int TotalExits, int CurrentCount)> GetDailyStatisticsAsync(DateTime date);

    /// <summary>
    /// Updates an entry record (typically for exit registration).
    /// </summary>
    /// <param name="entryRecord">The entry record to update.</param>
    Task UpdateAsync(EntryRecord entryRecord);

    /// <summary>
    /// Deletes an entry record.
    /// </summary>
    /// <param name="entryRecord">The entry record to delete.</param>
    Task DeleteAsync(EntryRecord entryRecord);

    /// <summary>
    /// Gets entry records by entry gate.
    /// </summary>
    /// <param name="entryGate">The entry gate name.</param>
    /// <returns>List of entry records for the specified gate.</returns>
    Task<List<EntryRecord>> GetByEntryGateAsync(string entryGate);

    /// <summary>
    /// Gets the most recent entry record for a visitor (if still in park).
    /// </summary>
    /// <param name="visitorId">The visitor ID.</param>
    /// <returns>The most recent entry record without exit time, null if visitor is not in park.</returns>
    Task<EntryRecord?> GetActiveEntryForVisitorAsync(int visitorId);
}
