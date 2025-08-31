using DbApp.Domain.Entities.UserSystem;
using MediatR;

namespace DbApp.Application.UserSystem.EntryRecords;

/// <summary>
/// Query to get all entry records.
/// </summary>
public record GetAllEntryRecordsQuery : IRequest<List<EntryRecord>>;

/// <summary>
/// Query to get an entry record by its ID.
/// </summary>
/// <param name="EntryRecordId">The entry record ID.</param>
public record GetEntryRecordByIdQuery(int EntryRecordId) : IRequest<EntryRecord?>;

/// <summary>
/// Query to get entry records for a specific visitor.
/// </summary>
/// <param name="VisitorId">The visitor ID.</param>
public record GetEntryRecordsByVisitorIdQuery(int VisitorId) : IRequest<List<EntryRecord>>;

/// <summary>
/// Query to get current visitors in the park.
/// </summary>
public record GetCurrentVisitorsQuery : IRequest<List<EntryRecord>>;

/// <summary>
/// Query to get entry records within a date range.
/// </summary>
/// <param name="StartDate">Start date for the range.</param>
/// <param name="EndDate">End date for the range.</param>
public record GetEntryRecordsByDateRangeQuery(
    DateTime StartDate,
    DateTime EndDate
) : IRequest<List<EntryRecord>>;

/// <summary>
/// Query to get the current visitor count in the park.
/// </summary>
public record GetCurrentVisitorCountQuery : IRequest<int>;

/// <summary>
/// Query to get daily visitor statistics.
/// </summary>
/// <param name="Date">The date to get statistics for.</param>
public record GetDailyStatisticsQuery(DateTime Date) : IRequest<DailyStatisticsDto>;

/// <summary>
/// Query to get entry records by entry gate.
/// </summary>
/// <param name="EntryGate">The entry gate name.</param>
public record GetEntryRecordsByGateQuery(string EntryGate) : IRequest<List<EntryRecord>>;

/// <summary>
/// Query to get the active entry record for a visitor (if still in park).
/// </summary>
/// <param name="VisitorId">The visitor ID.</param>
public record GetActiveEntryForVisitorQuery(int VisitorId) : IRequest<EntryRecord?>;

/// <summary>
/// DTO for daily statistics response.
/// </summary>
/// <param name="Date">The date of the statistics.</param>
/// <param name="TotalEntries">Total number of entries for the day.</param>
/// <param name="TotalExits">Total number of exits for the day.</param>
/// <param name="CurrentCount">Current number of visitors in the park.</param>
/// <param name="PeakCount">Peak visitor count during the day (if available).</param>
public record DailyStatisticsDto(
    DateTime Date,
    int TotalEntries,
    int TotalExits,
    int CurrentCount,
    int? PeakCount = null
);
