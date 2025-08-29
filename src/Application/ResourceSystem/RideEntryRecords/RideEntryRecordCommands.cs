using DbApp.Domain.Entities.ResourceSystem;  
using MediatR;  
  
namespace DbApp.Application.ResourceSystem.RideEntryRecords;  
  
public record CreateRideEntryRecordCommand(  
    int RideId,  
    int VisitorId,  
    DateTime EntryTime,  
    int? TicketId = null  
) : IRequest<RideEntryRecord>;  
  
public record UpdateRideEntryRecordCommand(  
    int RideEntryRecordId,  
    int RideId,  
    int VisitorId,  
    DateTime EntryTime,  
    DateTime? ExitTime,  
    int? TicketId  
) : IRequest;  
  
public record SetExitTimeCommand(  
    int RideEntryRecordId,  
    DateTime ExitTime  
) : IRequest;  
  
public record DeleteRideEntryRecordCommand(  
    int RideEntryRecordId  
) : IRequest;