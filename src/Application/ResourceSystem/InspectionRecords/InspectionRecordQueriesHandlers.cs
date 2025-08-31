using AutoMapper;  
using DbApp.Domain.Interfaces.ResourceSystem;  
using MediatR;  
  
namespace DbApp.Application.ResourceSystem.InspectionRecords;  
  
/// <summary>  
/// Combined handler for all inspection record queries.  
/// </summary>  
public class InspectionRecordQueryHandler(  
    IInspectionRecordRepository inspectionRecordRepository,  
    IMapper mapper) :  
    IRequestHandler<GetInspectionRecordByIdQuery, InspectionRecordSummaryDto?>,  
    IRequestHandler<SearchInspectionRecordsQuery, InspectionRecordResult>,  
    IRequestHandler<SearchInspectionRecordsByRideQuery, InspectionRecordResult>,  
    IRequestHandler<GetInspectionRecordStatsQuery, InspectionRecordStatsDto>  
{  
    private readonly IInspectionRecordRepository _inspectionRecordRepository = inspectionRecordRepository;  
    private readonly IMapper _mapper = mapper;  
  
    public async Task<InspectionRecordSummaryDto?> Handle(  
        GetInspectionRecordByIdQuery request,  
        CancellationToken cancellationToken)  
    {  
        var record = await _inspectionRecordRepository.GetByIdAsync(request.InspectionId);  
        return record == null ? null : _mapper.Map<InspectionRecordSummaryDto>(record);  
    }  
  
    public async Task<InspectionRecordResult> Handle(  
        SearchInspectionRecordsQuery request,  
        CancellationToken cancellationToken)  
    {  
        var records = await _inspectionRecordRepository.SearchAsync(  
            request.SearchTerm,  
            request.Page,  
            request.PageSize);  
  
        var totalCount = await _inspectionRecordRepository.CountAsync(request.SearchTerm);  
        var recordDtos = _mapper.Map<List<InspectionRecordSummaryDto>>(records);  
  
        return new InspectionRecordResult  
        {  
            InspectionRecords = recordDtos,  
            TotalCount = totalCount,  
            Page = request.Page,  
            PageSize = request.PageSize  
        };  
    }  
  
    public async Task<InspectionRecordResult> Handle(  
        SearchInspectionRecordsByRideQuery request,  
        CancellationToken cancellationToken)  
    {  
        var records = await _inspectionRecordRepository.SearchByRideAsync(  
            request.RideId,  
            request.Page,  
            request.PageSize);  
  
        var totalCount = await _inspectionRecordRepository.CountByRideAsync(request.RideId);  
        var recordDtos = _mapper.Map<List<InspectionRecordSummaryDto>>(records);  
  
        return new InspectionRecordResult  
        {  
            InspectionRecords = recordDtos,  
            TotalCount = totalCount,  
            Page = request.Page,  
            PageSize = request.PageSize  
        };  
    }  
  
    public async Task<InspectionRecordStatsDto> Handle(  
        GetInspectionRecordStatsQuery request,  
        CancellationToken cancellationToken)  
    {  
        var stats = await _inspectionRecordRepository.GetStatsAsync(  
            request.StartDate,  
            request.EndDate);  
  
        return _mapper.Map<InspectionRecordStatsDto>(stats);  
    }  
}