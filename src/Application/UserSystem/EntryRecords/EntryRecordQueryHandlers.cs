using AutoMapper;
using MediatR;
using DbApp.Domain.Interfaces.UserSystem;
using DbApp.Domain.Specifications.Common;
using DbApp.Domain.Specifications.UserSystem;

namespace DbApp.Application.UserSystem.EntryRecords;

/// <summary>
/// Centralized handler for all entry record queries.
/// </summary>
public class EntryRecordQueryHandlers(IEntryRecordRepository entryRecordRepo, IMapper mapper) :
    IRequestHandler<GetEntryRecordByIdQuery, EntryRecordDto?>,
    IRequestHandler<GetAllEntryRecordsQuery, List<EntryRecordDto>>,
    IRequestHandler<SearchEntryRecordsQuery, SearchEntryRecordsResult>,
    IRequestHandler<GetEntryRecordStatsQuery, EntryRecordStatsDto>,
    IRequestHandler<GetGroupedEntryRecordStatsQuery, List<GroupedEntryRecordStatsDto>>
{
    private readonly IEntryRecordRepository _entryRecordRepo = entryRecordRepo;
    private readonly IMapper _mapper = mapper;

    public async Task<EntryRecordDto?> Handle(GetEntryRecordByIdQuery request, CancellationToken cancellationToken)
    {
        var entryRecord = await _entryRecordRepo.GetByIdAsync(request.EntryRecordId)
            ?? throw new KeyNotFoundException($"Entry record with ID {request.EntryRecordId} not found.");
        return _mapper.Map<EntryRecordDto>(entryRecord);
    }

    public async Task<List<EntryRecordDto>> Handle(GetAllEntryRecordsQuery request, CancellationToken cancellationToken)
    {
        var entryRecords = await _entryRecordRepo.GetAllAsync();
        return _mapper.Map<List<EntryRecordDto>>(entryRecords);
    }

    public async Task<SearchEntryRecordsResult> Handle(SearchEntryRecordsQuery request, CancellationToken cancellationToken)
    {
        var searchSpec = _mapper.Map<PaginatedSpec<EntryRecordSpec>>(request);
        var countSpec = searchSpec.InnerSpec;

        var entryRecords = await _entryRecordRepo.SearchAsync(searchSpec);
        var totalCount = await _entryRecordRepo.CountAsync(countSpec);

        var entryRecordDtos = _mapper.Map<List<EntryRecordDto>>(entryRecords);

        return new SearchEntryRecordsResult
        {
            Items = entryRecordDtos,
            TotalCount = totalCount,
            Page = searchSpec.Page,
            PageSize = searchSpec.PageSize,
        };
    }

    public async Task<EntryRecordStatsDto> Handle(GetEntryRecordStatsQuery request, CancellationToken cancellationToken)
    {
        var statSpec = _mapper.Map<EntryRecordSpec>(request);
        var stats = await _entryRecordRepo.GetStatsAsync(statSpec);
        return _mapper.Map<EntryRecordStatsDto>(stats);
    }

    public async Task<List<GroupedEntryRecordStatsDto>> Handle(GetGroupedEntryRecordStatsQuery request, CancellationToken cancellationToken)
    {
        var groupedSpec = _mapper.Map<GroupedSpec<EntryRecordSpec>>(request);
        var groupedStats = await _entryRecordRepo.GetGroupedStatsAsync(groupedSpec);
        return _mapper.Map<List<GroupedEntryRecordStatsDto>>(groupedStats);
    }
}
