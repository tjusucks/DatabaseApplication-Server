using AutoMapper;  
using DbApp.Domain.Interfaces.ResourceSystem;  
using MediatR;  
  
namespace DbApp.Application.ResourceSystem.MaintenanceRecords;  
  
/// <summary>  
/// Combined handler for all maintenance record queries.  
/// </summary>  
public class MaintenanceRecordQueryHandler(  
    IMaintenanceRecordRepository maintenanceRecordRepository,  
    IMapper mapper) :  
    IRequestHandler<GetMaintenanceRecordByIdQuery, MaintenanceRecordSummaryDto?>,  
    IRequestHandler<SearchMaintenanceRecordsQuery, MaintenanceRecordResult>,  
    IRequestHandler<SearchMaintenanceRecordsByRideQuery, MaintenanceRecordResult>,  
    IRequestHandler<SearchMaintenanceRecordsByStatusQuery, MaintenanceRecordResult>,  
    IRequestHandler<GetMaintenanceRecordStatsQuery, MaintenanceRecordStatsDto>  
{  
    private readonly IMaintenanceRecordRepository _maintenanceRecordRepository = maintenanceRecordRepository;  
    private readonly IMapper _mapper = mapper;  
  
    public async Task<MaintenanceRecordSummaryDto?> Handle(  
        GetMaintenanceRecordByIdQuery request,  
        CancellationToken cancellationToken)  
    {  
        var record = await _maintenanceRecordRepository.GetByIdAsync(request.MaintenanceId);  
        return record == null ? null : _mapper.Map<MaintenanceRecordSummaryDto>(record);  
    }  
  
    public async Task<MaintenanceRecordResult> Handle(  
        SearchMaintenanceRecordsQuery request,  
        CancellationToken cancellationToken)  
    {  
        var records = await _maintenanceRecordRepository.SearchAsync(  
            request.SearchTerm,  
            request.Page,  
            request.PageSize);  
  
        var totalCount = await _maintenanceRecordRepository.CountAsync(request.SearchTerm);  
        var recordDtos = _mapper.Map<List<MaintenanceRecordSummaryDto>>(records);  
  
        return new MaintenanceRecordResult  
        {  
            MaintenanceRecords = recordDtos,  
            TotalCount = totalCount,  
            Page = request.Page,  
            PageSize = request.PageSize  
        };  
    }  
  
    public async Task<MaintenanceRecordResult> Handle(  
        SearchMaintenanceRecordsByRideQuery request,  
        CancellationToken cancellationToken)  
    {  
        var records = await _maintenanceRecordRepository.SearchByRideAsync(  
            request.RideId,  
            request.Page,  
            request.PageSize);  
  
        var totalCount = await _maintenanceRecordRepository.CountByRideAsync(request.RideId);  
        var recordDtos = _mapper.Map<List<MaintenanceRecordSummaryDto>>(records);  
  
        return new MaintenanceRecordResult  
        {  
            MaintenanceRecords = recordDtos,  
            TotalCount = totalCount,  
            Page = request.Page,  
            PageSize = request.PageSize  
        };  
    }  
  
    public async Task<MaintenanceRecordResult> Handle(  
        SearchMaintenanceRecordsByStatusQuery request,  
        CancellationToken cancellationToken)  
    {  
        var records = await _maintenanceRecordRepository.SearchByStatusAsync(  
            request.IsCompleted,  
            request.IsAccepted,  
            request.Page,  
            request.PageSize);  
  
        var totalCount = await _maintenanceRecordRepository.CountByStatusAsync(  
            request.IsCompleted,  
            request.IsAccepted);  
        var recordDtos = _mapper.Map<List<MaintenanceRecordSummaryDto>>(records);  
  
        return new MaintenanceRecordResult  
        {  
            MaintenanceRecords = recordDtos,  
            TotalCount = totalCount,  
            Page = request.Page,  
            PageSize = request.PageSize  
        };  
    }  
  
    public async Task<MaintenanceRecordStatsDto> Handle(  
        GetMaintenanceRecordStatsQuery request,  
        CancellationToken cancellationToken)  
    {  
        var stats = await _maintenanceRecordRepository.GetStatsAsync(  
            request.StartDate,  
            request.EndDate);  
  
        return _mapper.Map<MaintenanceRecordStatsDto>(stats);  
    }  
}