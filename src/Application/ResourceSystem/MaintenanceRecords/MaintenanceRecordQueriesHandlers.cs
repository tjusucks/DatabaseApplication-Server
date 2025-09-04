using AutoMapper;
using DbApp.Domain.Interfaces.ResourceSystem;
using MediatR;

namespace DbApp.Application.ResourceSystem.MaintenanceRecords;

/// <summary>  
/// Combined handler for all maintenance record search and statistics queries.  
/// </summary>  
public class MaintenanceRecordQueryHandler(
    IMaintenanceRecordRepository maintenanceRecordRepository,
    IMapper mapper) :
    IRequestHandler<GetMaintenanceRecordByIdQuery, MaintenanceRecordSummaryDto?>,
    IRequestHandler<SearchMaintenanceRecordsQuery, MaintenanceRecordResult>,
    IRequestHandler<GetMaintenanceRecordStatsQuery, MaintenanceRecordStatsDto>,
    IRequestHandler<CreateMaintenanceRecordCommand, int>,
    IRequestHandler<UpdateMaintenanceRecordCommand>,
    IRequestHandler<DeleteMaintenanceRecordCommand, bool>
{
    private readonly IMaintenanceRecordRepository _maintenanceRecordRepository = maintenanceRecordRepository;
    private readonly IMapper _mapper = mapper;

    /// <summary>  
    /// Handle getting maintenance record by ID.  
    /// </summary>  
    public async Task<MaintenanceRecordSummaryDto?> Handle(
        GetMaintenanceRecordByIdQuery request,
        CancellationToken cancellationToken)
    {
        var record = await _maintenanceRecordRepository.GetByIdAsync(request.MaintenanceId);
        return record == null ? null : _mapper.Map<MaintenanceRecordSummaryDto>(record);
    }

    /// <summary>  
    /// Handle searching maintenance records with comprehensive filtering options.  
    /// </summary>  
    public async Task<MaintenanceRecordResult> Handle(
        SearchMaintenanceRecordsQuery request,
        CancellationToken cancellationToken)
    {
        var records = await _maintenanceRecordRepository.SearchAsync(
            request.SearchTerm,
            request.RideId,
            request.TeamId,
            request.ManagerId,
            request.MaintenanceType,
            request.IsCompleted,
            request.IsAccepted,
            request.StartTimeFrom,
            request.StartTimeTo,
            request.EndTimeFrom,
            request.EndTimeTo,
            request.MinCost,
            request.MaxCost,
            request.Page,
            request.PageSize);

        var totalCount = await _maintenanceRecordRepository.CountAsync(
            request.SearchTerm,
            request.RideId,
            request.TeamId,
            request.ManagerId,
            request.MaintenanceType,
            request.IsCompleted,
            request.IsAccepted,
            request.StartTimeFrom,
            request.StartTimeTo,
            request.EndTimeFrom,
            request.EndTimeTo,
            request.MinCost,
            request.MaxCost);

        var recordDtos = _mapper.Map<List<MaintenanceRecordSummaryDto>>(records);

        return new MaintenanceRecordResult
        {
            MaintenanceRecords = recordDtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    /// <summary>  
    /// Handle getting maintenance record statistics.  
    /// </summary>  
    public async Task<MaintenanceRecordStatsDto> Handle(
        GetMaintenanceRecordStatsQuery request,
        CancellationToken cancellationToken)
    {
        var stats = await _maintenanceRecordRepository.GetStatsAsync(request.StartDate, request.EndDate);
        return _mapper.Map<MaintenanceRecordStatsDto>(stats);
    }

    /// <summary>  
    /// Handle creating a new maintenance record.  
    /// </summary>  
    public async Task<int> Handle(
        CreateMaintenanceRecordCommand request,
        CancellationToken cancellationToken)
    {
        var record = new Domain.Entities.ResourceSystem.MaintenanceRecord
        {
            RideId = request.RideId,
            TeamId = request.TeamId,
            ManagerId = request.ManagerId,
            MaintenanceType = request.MaintenanceType,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            Cost = request.Cost,
            PartsReplaced = request.PartsReplaced,
            MaintenanceDetails = request.MaintenanceDetails,
            IsCompleted = request.IsCompleted,
            IsAccepted = request.IsAccepted,
            AcceptanceDate = request.AcceptanceDate,
            AcceptanceComments = request.AcceptanceComments,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _maintenanceRecordRepository.AddAsync(record);
        return record.MaintenanceId;
    }

    /// <summary>  
    /// Handle updating an existing maintenance record.  
    /// </summary>  
    public async Task Handle(
        UpdateMaintenanceRecordCommand request,
        CancellationToken cancellationToken)
    {
        var record = await _maintenanceRecordRepository.GetByIdAsync(request.MaintenanceId);

        if (record == null)
        {
            throw new InvalidOperationException("Maintenance record not found");
        }

        record.RideId = request.RideId;
        record.TeamId = request.TeamId;
        record.ManagerId = request.ManagerId;
        record.MaintenanceType = request.MaintenanceType;
        record.StartTime = request.StartTime;
        record.EndTime = request.EndTime;
        record.Cost = request.Cost;
        record.PartsReplaced = request.PartsReplaced;
        record.MaintenanceDetails = request.MaintenanceDetails;
        record.IsCompleted = request.IsCompleted;
        record.IsAccepted = request.IsAccepted;
        record.AcceptanceDate = request.AcceptanceDate;
        record.AcceptanceComments = request.AcceptanceComments;
        record.UpdatedAt = DateTime.UtcNow;

        await _maintenanceRecordRepository.UpdateAsync(record);
    }

    /// <summary>  
    /// Handle deleting a maintenance record.  
    /// </summary>  
    public async Task<bool> Handle(
        DeleteMaintenanceRecordCommand request,
        CancellationToken cancellationToken)
    {
        var record = await _maintenanceRecordRepository.GetByIdAsync(request.MaintenanceId);

        if (record == null)
        {
            return false;
        }

        await _maintenanceRecordRepository.DeleteAsync(record);
        return true;
    }
}
