using AutoMapper;
using DbApp.Domain.Interfaces.ResourceSystem;
using MediatR;
using static DbApp.Domain.Exceptions;

namespace DbApp.Application.ResourceSystem.InspectionRecords;

/// <summary>
/// Combined handler for all inspection record search and statistics queries.
/// </summary>
public class InspectionRecordQueryHandlers(
    IInspectionRecordRepository inspectionRecordRepository,
    IMapper mapper) :
    IRequestHandler<GetInspectionRecordByIdQuery, InspectionRecordSummaryDto?>,
    IRequestHandler<SearchInspectionRecordsQuery, InspectionRecordResult>,
    IRequestHandler<GetInspectionRecordStatsQuery, InspectionRecordStatsDto>,
    IRequestHandler<CreateInspectionRecordCommand, int>,
    IRequestHandler<UpdateInspectionRecordCommand>,
    IRequestHandler<DeleteInspectionRecordCommand, bool>
{
    private readonly IInspectionRecordRepository _inspectionRecordRepository = inspectionRecordRepository;
    private readonly IMapper _mapper = mapper;

    /// <summary>
    /// Handle getting inspection record by ID.
    /// </summary>
    public async Task<InspectionRecordSummaryDto?> Handle(
        GetInspectionRecordByIdQuery request,
        CancellationToken cancellationToken)
    {
        var record = await _inspectionRecordRepository.GetByIdAsync(request.InspectionId);
        return record == null ? null : _mapper.Map<InspectionRecordSummaryDto>(record);
    }

    /// <summary>
    /// Handle searching inspection records with comprehensive filtering options.
    /// </summary>
    public async Task<InspectionRecordResult> Handle(
        SearchInspectionRecordsQuery request,
        CancellationToken cancellationToken)
    {
        var records = await _inspectionRecordRepository.SearchAsync(
            request.Keyword,
            request.RideId,
            request.TeamId,
            request.CheckType,
            request.IsPassed,
            request.CheckDateFrom,
            request.CheckDateTo,
            request.Page,
            request.PageSize);

        var totalCount = await _inspectionRecordRepository.CountAsync(
            request.Keyword,
            request.RideId,
            request.TeamId,
            request.CheckType,
            request.IsPassed,
            request.CheckDateFrom,
            request.CheckDateTo);

        var recordDtos = _mapper.Map<List<InspectionRecordSummaryDto>>(records);

        return new InspectionRecordResult
        {
            InspectionRecords = recordDtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    /// <summary>
    /// Handle getting inspection record statistics.
    /// </summary>
    public async Task<InspectionRecordStatsDto> Handle(
        GetInspectionRecordStatsQuery request,
        CancellationToken cancellationToken)
    {
        var stats = await _inspectionRecordRepository.GetStatsAsync(request.StartDate, request.EndDate);
        return _mapper.Map<InspectionRecordStatsDto>(stats);
    }

    /// <summary>
    /// Handle creating a new inspection record.
    /// </summary>
    public async Task<int> Handle(
        CreateInspectionRecordCommand request,
        CancellationToken cancellationToken)
    {
        var record = new Domain.Entities.ResourceSystem.InspectionRecord
        {
            RideId = request.RideId,
            TeamId = request.TeamId,
            CheckDate = request.CheckDate,
            CheckType = request.CheckType,
            IsPassed = request.IsPassed,
            IssuesFound = request.IssuesFound,
            Recommendations = request.Recommendations,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _inspectionRecordRepository.AddAsync(record);
        return record.InspectionId;
    }

    /// <summary>
    /// Handle updating an existing inspection record.
    /// </summary>
    public async Task Handle(
        UpdateInspectionRecordCommand request,
        CancellationToken cancellationToken)
    {
        var record = await _inspectionRecordRepository.GetByIdAsync(request.InspectionId)
            ?? throw new NotFoundException("Inspection record not found");
        record.RideId = request.RideId;
        record.TeamId = request.TeamId;
        record.CheckDate = request.CheckDate;
        record.CheckType = request.CheckType;
        record.IsPassed = request.IsPassed;
        record.IssuesFound = request.IssuesFound;
        record.Recommendations = request.Recommendations;
        record.UpdatedAt = DateTime.UtcNow;

        await _inspectionRecordRepository.UpdateAsync(record);
    }

    /// <summary>
    /// Handle deleting an inspection record.
    /// </summary>
    public async Task<bool> Handle(
        DeleteInspectionRecordCommand request,
        CancellationToken cancellationToken)
    {
        var record = await _inspectionRecordRepository.GetByIdAsync(request.InspectionId);

        if (record == null)
        {
            return false;
        }

        await _inspectionRecordRepository.DeleteAsync(record);
        return true;
    }
}
