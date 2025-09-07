using AutoMapper;
using DbApp.Domain.Interfaces.UserSystem;
using DbApp.Domain.Specifications.Common;
using DbApp.Domain.Specifications.UserSystem;
using MediatR;
using static DbApp.Domain.Exceptions;

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
            ?? throw new NotFoundException($"Entry record with ID {request.EntryRecordId} not found.");
        return _mapper.Map<EntryRecordDto>(entryRecord);
    }

    public async Task<List<EntryRecordDto>> Handle(GetAllEntryRecordsQuery request, CancellationToken cancellationToken)
    {
        var entryRecords = await _entryRecordRepo.GetAllAsync();
        return _mapper.Map<List<EntryRecordDto>>(entryRecords);
    }

    public async Task<SearchEntryRecordsResult> Handle(SearchEntryRecordsQuery request, CancellationToken cancellationToken)
    {
        var searchSpec = _mapper.Map<EntryRecordSpec>(request)
            ?? throw new ValidationException("Invalid search parameters.");
        var paginatedSpec = new PaginatedSpec<EntryRecordSpec>
        {
            InnerSpec = searchSpec,
            Page = request.Page,
            PageSize = request.PageSize,
            OrderBy = request.OrderBy,
            Descending = request.Descending
        };

        var entryRecords = await _entryRecordRepo.SearchAsync(paginatedSpec);
        var totalCount = await _entryRecordRepo.CountAsync(paginatedSpec.InnerSpec);

        var entryRecordDtos = _mapper.Map<List<EntryRecordDto>>(entryRecords);

        return new SearchEntryRecordsResult
        {
            Items = entryRecordDtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize,
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
        var searchSpec = _mapper.Map<EntryRecordSpec>(request);
        var groupedSpec = new GroupedSpec<EntryRecordSpec>
        {
            InnerSpec = searchSpec,
            GroupBy = request.GroupBy,
            OrderBy = request.OrderBy,
            Descending = request.Descending
        };

        var groupedStats = await _entryRecordRepo.GetGroupedStatsAsync(groupedSpec);
        return _mapper.Map<List<GroupedEntryRecordStatsDto>>(groupedStats);
    }
}
