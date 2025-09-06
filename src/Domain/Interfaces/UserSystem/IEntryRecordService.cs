namespace DbApp.Domain.Interfaces.UserSystem;

/// <summary>
/// Repository interface for EntryRecord entity operations.
/// Provides methods for visitor entry/exit management and statistics.
/// </summary>
public interface IEntryRecordService
{
    Task<int> CreateEntryAsync(int visitorId, string gateName, int? ticketId);
    Task<int> CreateExitAsync(int visitorId, string gateName);
}
