using MediatR;

namespace DbApp.Application.UserSystem.EntryRecords;

/// <summary>
/// Query to get an entry record by ID.
/// </summary>
public record GetEntryRecordByIdQuery(int EntryRecordId) : IRequest<EntryRecordDto?>;

/// <summary>
/// Query to get all entry records.
/// </summary>
public record GetAllEntryRecordsQuery() : IRequest<List<EntryRecordDto>>;

/// <summary>
/// Query for searching/filtering entry records with multiple criteria and pagination.
/// </summary>
public record SearchEntryRecordsQuery(
    string? Keyword,
    int? VisitorId,
    DateTime? Start,
    DateTime? End,
    DateTime? EntryTimeStart,
    DateTime? EntryTimeEnd,
    DateTime? ExitTimeStart,
    DateTime? ExitTimeEnd,
    string? EntryGate,
    string? ExitGate,
    int? TicketId,
    bool? IsActive,
    int Page = 1,
    int PageSize = 20,
    bool Descending = true,
    string OrderBy = "EntryTime"
) : IRequest<SearchEntryRecordsResult>;

/// <summary>
/// Query for getting entry record statistics with filtering.
/// </summary>
public record GetEntryRecordStatsQuery(
    string? Keyword,
    int? VisitorId,
    DateTime? Start,
    DateTime? End,
    DateTime? EntryTimeStart,
    DateTime? EntryTimeEnd,
    DateTime? ExitTimeStart,
    DateTime? ExitTimeEnd,
    string? EntryGate,
    string? ExitGate,
    int? TicketId,
    bool? IsActive
) : IRequest<EntryRecordStatsDto>;

/// <summary>
/// Query for getting grouped entry record statistics with filtering.
/// </summary>
public record GetGroupedEntryRecordStatsQuery(
    string? Keyword,
    int? VisitorId,
    DateTime? Start,
    DateTime? End,
    DateTime? EntryTimeStart,
    DateTime? EntryTimeEnd,
    DateTime? ExitTimeStart,
    DateTime? ExitTimeEnd,
    string? EntryGate,
    string? ExitGate,
    int? TicketId,
    bool? IsActive,
    bool Descending = true,
    string GroupBy = "Date",
    string OrderBy = "TotalEntries"
) : IRequest<List<GroupedEntryRecordStatsDto>>;
