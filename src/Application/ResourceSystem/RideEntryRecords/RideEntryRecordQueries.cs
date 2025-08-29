using DbApp.Domain.Entities.ResourceSystem;  
using MediatR;  
  
namespace DbApp.Application.ResourceSystem.RideEntryRecords;  
  
public record GetRideEntryRecordByIdQuery(int Id) : IRequest<RideEntryRecord?>;  
  
public record GetRideEntryRecordsQuery(  
    int? RideId = null,  
    int? VisitorId = null,  
    DateTime? StartDate = null,  
    DateTime? EndDate = null,  
    int Page = 1,  
    int PageSize = 50  
) : IRequest<List<RideEntryRecord>>;  
  
public record GetRideEntryRecordsByRideQuery(int RideId) : IRequest<List<RideEntryRecord>>;  
  
public record GetRideEntryRecordsByVisitorQuery(int VisitorId) : IRequest<List<RideEntryRecord>>;  
  
public record GetCurrentVisitorsInRidesQuery() : IRequest<List<RideEntryRecord>>;  
  
public record GetTrafficSummaryQuery(  
    DateTime StartDate,  
    DateTime EndDate,  
    int? RideId = null  
) : IRequest<object>;