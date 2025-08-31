using DbApp.Domain.Entities.UserSystem;
using DbApp.Domain.Interfaces.UserSystem;
using MediatR;

namespace DbApp.Application.UserSystem.EntryRecords;

/// <summary>
/// Handler for getting all entry records.
/// </summary>
public class GetAllEntryRecordsQueryHandler(IEntryRecordRepository entryRecordRepository) : IRequestHandler<GetAllEntryRecordsQuery, List<EntryRecord>>
{
    private readonly IEntryRecordRepository _entryRecordRepository = entryRecordRepository;

    public async Task<List<EntryRecord>> Handle(GetAllEntryRecordsQuery request, CancellationToken cancellationToken)
    {
        return await _entryRecordRepository.GetAllAsync();
    }
}

/// <summary>
/// Handler for getting an entry record by ID.
/// </summary>
public class GetEntryRecordByIdQueryHandler(IEntryRecordRepository entryRecordRepository) : IRequestHandler<GetEntryRecordByIdQuery, EntryRecord?>
{
    private readonly IEntryRecordRepository _entryRecordRepository = entryRecordRepository;

    public async Task<EntryRecord?> Handle(GetEntryRecordByIdQuery request, CancellationToken cancellationToken)
    {
        return await _entryRecordRepository.GetByIdAsync(request.EntryRecordId);
    }
}

/// <summary>
/// Handler for getting entry records by visitor ID.
/// </summary>
public class GetEntryRecordsByVisitorIdQueryHandler(IEntryRecordRepository entryRecordRepository) : IRequestHandler<GetEntryRecordsByVisitorIdQuery, List<EntryRecord>>
{
    private readonly IEntryRecordRepository _entryRecordRepository = entryRecordRepository;

    public async Task<List<EntryRecord>> Handle(GetEntryRecordsByVisitorIdQuery request, CancellationToken cancellationToken)
    {
        return await _entryRecordRepository.GetByVisitorIdAsync(request.VisitorId);
    }
}

/// <summary>
/// Handler for getting current visitors in the park.
/// </summary>
public class GetCurrentVisitorsQueryHandler(IEntryRecordRepository entryRecordRepository) : IRequestHandler<GetCurrentVisitorsQuery, List<EntryRecord>>
{
    private readonly IEntryRecordRepository _entryRecordRepository = entryRecordRepository;

    public async Task<List<EntryRecord>> Handle(GetCurrentVisitorsQuery request, CancellationToken cancellationToken)
    {
        return await _entryRecordRepository.GetCurrentVisitorsAsync();
    }
}

/// <summary>
/// Handler for getting entry records by date range.
/// </summary>
public class GetEntryRecordsByDateRangeQueryHandler(IEntryRecordRepository entryRecordRepository) : IRequestHandler<GetEntryRecordsByDateRangeQuery, List<EntryRecord>>
{
    private readonly IEntryRecordRepository _entryRecordRepository = entryRecordRepository;

    public async Task<List<EntryRecord>> Handle(GetEntryRecordsByDateRangeQuery request, CancellationToken cancellationToken)
    {
        return await _entryRecordRepository.GetByDateRangeAsync(request.StartDate, request.EndDate);
    }
}

/// <summary>
/// Handler for getting current visitor count.
/// </summary>
public class GetCurrentVisitorCountQueryHandler(IEntryRecordRepository entryRecordRepository) : IRequestHandler<GetCurrentVisitorCountQuery, int>
{
    private readonly IEntryRecordRepository _entryRecordRepository = entryRecordRepository;

    public async Task<int> Handle(GetCurrentVisitorCountQuery request, CancellationToken cancellationToken)
    {
        return await _entryRecordRepository.GetCurrentVisitorCountAsync();
    }
}

/// <summary>
/// Handler for getting daily statistics.
/// </summary>
public class GetDailyStatisticsQueryHandler(IEntryRecordRepository entryRecordRepository) : IRequestHandler<GetDailyStatisticsQuery, DailyStatisticsDto>
{
    private readonly IEntryRecordRepository _entryRecordRepository = entryRecordRepository;

    public async Task<DailyStatisticsDto> Handle(GetDailyStatisticsQuery request, CancellationToken cancellationToken)
    {
        var (totalEntries, totalExits, currentCount) = await _entryRecordRepository.GetDailyStatisticsAsync(request.Date);

        return new DailyStatisticsDto(
            Date: request.Date.Date,
            TotalEntries: totalEntries,
            TotalExits: totalExits,
            CurrentCount: currentCount
        );
    }
}

/// <summary>
/// Handler for getting entry records by gate.
/// </summary>
public class GetEntryRecordsByGateQueryHandler(IEntryRecordRepository entryRecordRepository) : IRequestHandler<GetEntryRecordsByGateQuery, List<EntryRecord>>
{
    private readonly IEntryRecordRepository _entryRecordRepository = entryRecordRepository;

    public async Task<List<EntryRecord>> Handle(GetEntryRecordsByGateQuery request, CancellationToken cancellationToken)
    {
        return await _entryRecordRepository.GetByEntryGateAsync(request.EntryGate);
    }
}

/// <summary>
/// Handler for getting active entry for a visitor.
/// </summary>
public class GetActiveEntryForVisitorQueryHandler(IEntryRecordRepository entryRecordRepository) : IRequestHandler<GetActiveEntryForVisitorQuery, EntryRecord?>
{
    private readonly IEntryRecordRepository _entryRecordRepository = entryRecordRepository;

    public async Task<EntryRecord?> Handle(GetActiveEntryForVisitorQuery request, CancellationToken cancellationToken)
    {
        return await _entryRecordRepository.GetActiveEntryForVisitorAsync(request.VisitorId);
    }
}
