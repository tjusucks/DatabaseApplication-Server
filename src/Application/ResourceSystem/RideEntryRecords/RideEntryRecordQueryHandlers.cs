using DbApp.Domain.Entities.ResourceSystem;
using DbApp.Domain.Interfaces.ResourceSystem;
using MediatR;  
  
namespace DbApp.Application.ResourceSystem.RideEntryRecords;  
  
public class GetRideEntryRecordByIdQueryHandler(IRideEntryRecordRepository repository)   
    : IRequestHandler<GetRideEntryRecordByIdQuery, RideEntryRecord?>  
{  
    private readonly IRideEntryRecordRepository _repository = repository;  
  
    public async Task<RideEntryRecord?> Handle(GetRideEntryRecordByIdQuery request, CancellationToken cancellationToken)  
    {  
        return await _repository.GetByIdAsync(request.Id);  
    }  
}  
  
public class GetRideEntryRecordsQueryHandler(IRideEntryRecordRepository repository)   
    : IRequestHandler<GetRideEntryRecordsQuery, List<RideEntryRecord>>  
{  
    private readonly IRideEntryRecordRepository _repository = repository;  
  
    public async Task<List<RideEntryRecord>> Handle(GetRideEntryRecordsQuery request, CancellationToken cancellationToken)  
    {  
        return await _repository.GetFilteredAsync(  
            request.RideId,  
            request.VisitorId,  
            request.StartDate,  
            request.EndDate,  
            request.Page,  
            request.PageSize);  
    }  
}  
  
public class GetRideEntryRecordsByRideQueryHandler(IRideEntryRecordRepository repository)   
    : IRequestHandler<GetRideEntryRecordsByRideQuery, List<RideEntryRecord>>  
{  
    private readonly IRideEntryRecordRepository _repository = repository;  
  
    public async Task<List<RideEntryRecord>> Handle(GetRideEntryRecordsByRideQuery request, CancellationToken cancellationToken)  
    {  
        return await _repository.GetByRideIdAsync(request.RideId);  
    }  
}  
  
public class GetRideEntryRecordsByVisitorQueryHandler(IRideEntryRecordRepository repository)   
    : IRequestHandler<GetRideEntryRecordsByVisitorQuery, List<RideEntryRecord>>  
{  
    private readonly IRideEntryRecordRepository _repository = repository;  
  
    public async Task<List<RideEntryRecord>> Handle(GetRideEntryRecordsByVisitorQuery request, CancellationToken cancellationToken)  
    {  
        return await _repository.GetByVisitorIdAsync(request.VisitorId);  
    }  
}  
  
public class GetCurrentVisitorsInRidesQueryHandler(IRideEntryRecordRepository repository)   
    : IRequestHandler<GetCurrentVisitorsInRidesQuery, List<RideEntryRecord>>  
{  
    private readonly IRideEntryRecordRepository _repository = repository;  
  
    public async Task<List<RideEntryRecord>> Handle(GetCurrentVisitorsInRidesQuery request, CancellationToken cancellationToken)  
    {  
        return await _repository.GetCurrentVisitorsAsync();  
    }  
}  
  
public class GetTrafficSummaryQueryHandler(IRideEntryRecordRepository repository)   
    : IRequestHandler<GetTrafficSummaryQuery, object>  
{  
    private readonly IRideEntryRecordRepository _repository = repository;  
  
    public async Task<object> Handle(GetTrafficSummaryQuery request, CancellationToken cancellationToken)  
    {  
        return await _repository.GetTrafficSummaryAsync(request.StartDate, request.EndDate, request.RideId);  
    }  
}